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
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using System.Security.Claims;

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
      var responseAsJsonDocument = JsonConvert.DeserializeObject<SuccessRes>(await response.Content.ReadAsStringAsync());
      if (responseAsJsonDocument.Access_token != null)
      {
        var stream = responseAsJsonDocument.Access_token;
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(stream);
        var tokenS = handler.ReadToken(stream) as JwtSecurityToken;
        var unique_name = tokenS.Claims.First(claim => claim.Type == "unique_name").Value;
        var name = tokenS.Claims.First(claim => claim.Type == "name").Value;
        var claims = new List<Claim>
        {
            new Claim("preferred_username", unique_name),
            new Claim("name",name),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationType);

        var authProperties = new AuthenticationProperties
        {
          //AllowRefresh = <bool>,
          // Refreshing the authentication session should be allowed.

          //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
          // The time at which the authentication ticket expires. A 
          // value set here overrides the ExpireTimeSpan option of 
          // CookieAuthenticationOptions set with AddCookie.

          //IsPersistent = true,
          // Whether the authentication session is persisted across 
          // multiple requests. When used with cookies, controls
          // whether the cookie's lifetime is absolute (matching the
          // lifetime of the authentication ticket) or session-based.

          //IssuedUtc = <DateTimeOffset>,
          // The time at which the authentication ticket was issued.

          //RedirectUri = <string>
          // The full path or absolute URI to be used as an http 
          // redirect response value.
        };
        IAuthenticationManager authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
        authenticationManager.SignIn(claimsIdentity);
      }
      return RedirectToAction("Index", "Home");
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
    public ActionResult SignOut()
    {
      IAuthenticationManager authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
      authenticationManager.SignOut(CookieAuthenticationDefaults.AuthenticationType);
      return RedirectToAction("Index", "Home");
    }
  }
}