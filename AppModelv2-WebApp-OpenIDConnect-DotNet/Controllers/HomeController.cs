using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Text;
using Newtonsoft.Json;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet.Controllers
{
  public class HomeController : Controller
  {

    string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];

    // RedirectUri is the URL where the user will be redirected to after they sign in.
    string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
    string clientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"];

    // Tenant is the tenant ID (e.g. contoso.onmicrosoft.com, or 'common' for multi-tenant)
    static string tenant = System.Configuration.ConfigurationManager.AppSettings["Tenant"];

    // Authority is the URL for authority, composed by Microsoft identity platform endpoint and the tenant name (e.g. https://login.microsoftonline.com/contoso.onmicrosoft.com/v2.0)
    string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, System.Configuration.ConfigurationManager.AppSettings["Authority"], tenant);
    // GET: Home
    public ActionResult Index()
    {
      return View();
    }
    /// <summary>
    /// Send an OpenID Connect sign-in request.
    /// Alternatively, you can just decorate the SignIn method with the [Authorize] attribute
    /// </summary>



    [HttpGet]
    public async Task<ActionResult> SignInByAzure(AuthReq req)
    {

      var httpClient = new HttpClient();
      var response = await httpClient.PostAsync(
        $"{authority}/token",
        new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/user.read"),
            new KeyValuePair<string, string>("code", req.Code),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("client_secret", clientSecret)

        })
      );
      var responseAsJsonDocument = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()).;

      authman

      return View();
    }

    public RedirectResult SignIn()
    {

      //if (!Request.IsAuthenticated)
      //{
      //  HttpContext.GetOwinContext().Authentication.Challenge(
      //      new AuthenticationProperties { RedirectUri = "/" },
      //      OpenIdConnectAuthenticationDefaults.AuthenticationType);
      //}
      return Redirect(authority +
                    $@"/authorize?client_id={clientId}" +
                    "&response_type=code" +
      $@"&redirect_uri={HttpUtility.UrlEncode(redirectUri)}" +
      "&response_mode=query" +
      "&scope=openid%20offline_access%20https%3A%2F%2Fgraph.microsoft.com%2Fuser.read" +
      "&state=12345");
    }
    /// <summary>
    /// Send an OpenID Connect sign-out request.
    /// </summary>
    public void SignOut()
    {
      HttpContext.GetOwinContext().Authentication.SignOut(
              OpenIdConnectAuthenticationDefaults.AuthenticationType,
              CookieAuthenticationDefaults.AuthenticationType);
    }
  }
}