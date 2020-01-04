using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet
{
  public class SuccessRes
  {
    public string Access_token { get; set; }
    public string Scope { get; set; }
    public string Id_token { get; set; }
    public string Expires_in { get; set; }
    public string Refresh_token { get; set; }
  }
}