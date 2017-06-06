
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Oauth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Plus.v1;
using Google.Apis.Plus.v1.Data;
using Google.Apis.Fitness.v1;
using Google.Apis.Services;
using Google.Apis.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.SessionState;
using System.Data.SqlClient;
using Newtonsoft.Json;


namespace GPlusQuickstartCsharp
{
    /// <summary>
    /// Summary description for addUserHandler
    /// </summary>
    public class addUserHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        static public string APP_NAME = "Fitness Leaderboard";

        public void ProcessRequest(HttpContext context)
        {

            if (context.Request.Path.EndsWith("/"))
            {

                context.Response.RedirectPermanent("/signin.ashx");
            }
            if (context.Request.Path.EndsWith("/addUserHandler.ashx"))
            {
                TokenResponse x = (TokenResponse)context.Session["authState"];
                if (x == null)
                {
                    Console.Write("Not Logged In");
                    context.Response.Redirect("/signin.ashx");
                    return;

                }
                else
                {
                    Console.Write("Logged In");
                }
                String accountStatus = (String)context.Session["accountStatus"];
                if (accountStatus == "ADMIN")
                {
                    Console.Write("Active");
                    // Render the templated HTML.
                    String templatedHTML = File.ReadAllText(
                         context.Server.MapPath("addUserForm.html"));
                    templatedHTML = Regex.Replace(templatedHTML,
                        "[{]{2}\\s*APPLICATION_NAME\\s*[}]{2}", APP_NAME);

                    context.Response.ContentType = "text/html";
                    context.Response.Write(templatedHTML);
                }
                
                return;
            }

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