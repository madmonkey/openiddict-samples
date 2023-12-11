using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using static OpenIddict.Client.Owin.OpenIddictClientOwinConstants;

namespace Mortis.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
            => _httpClientFactory = httpClientFactory;

        [HttpGet, Route("~/")]
        public ActionResult Index() => View();

        [Authorize, HttpPost, Route("~/")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            var context = HttpContext.GetOwinContext();

            var result =
                await context.Authentication.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationType);
            var token = result.Properties.Dictionary[Tokens.BackchannelAccessToken];

            using var client = _httpClientFactory.CreateClient();

            using var request = new HttpRequestMessage(HttpMethod.Post,
                $"{WebConfigurationManager.AppSettings["OpenIddictServer"]}api/message");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return View(model: await response.Content.ReadAsStringAsync());
        }

        [Authorize, HttpPost, Route("~/dashboard")]
        [ValidateAntiForgeryToken]
        public ActionResult Dashboard(CancellationToken cancellationToken)
        {
            //var context = HttpContext.GetOwinContext();

            //var result =
            //    await context.Authentication.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationType);
            //var token = result.Properties.Dictionary[Tokens.BackchannelAccessToken];

            return Redirect($"{WebConfigurationManager.AppSettings["OpenIddictServer"]}home/dashboard");
            //using var client = _httpClientFactory.CreateClient();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //client.BaseAddress = new Uri($"{WebConfigurationManager.AppSettings["OpenIddictServer"]}home/dashboard");
            //return await client.GetAsync($"{WebConfigurationManager.AppSettings["OpenIddictServer"]}home/dashboard",
            //    cancellationToken);
            //using var request = new HttpRequestMessage(HttpMethod.Get,
            //    $"{WebConfigurationManager.AppSettings["OpenIddictServer"]}home/dashboard");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //return (await client.GetAsync($"{WebConfigurationManager.AppSettings["OpenIddictServer"]}home/dashboard"))
            //    .Result;
            //return client.GetAsync("",request);
            //using var response = await client.SendAsync(request, cancellationToken);
            //response.EnsureSuccessStatusCode();

            //return View(model: await response.Content.ReadAsStringAsync());
        }
    }
}