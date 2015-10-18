using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AnalyzeThis
{
    /// <summary>
    /// Summary description for LinkUsers
    /// </summary>
    public class LinkUsers : IHttpHandler
    {
        private readonly Auth0.Client client = new Auth0.Client(
                                ConfigurationManager.AppSettings["auth0:ClientId"],
                                ConfigurationManager.AppSettings["auth0:ClientSecret"],
                                ConfigurationManager.AppSettings["auth0:Domain"]);

        public void ProcessRequest(HttpContext context)
        {
            var token = client.ExchangeAuthorizationCodePerAccessToken(context.Request.QueryString["code"], context.Request.Url.ToString());
            var profile = client.GetUserInfo(token);
            
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}