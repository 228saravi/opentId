using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppModelv2_WebApp_OpenIDConnect_DotNet
{
  public class AuthReq
  {
    public string Code { get; set; }
    public string State { get; set; }
    public string Session_state { get; set; }
  }
}