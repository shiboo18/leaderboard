/*
 * Copyright 2013-2014 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
    ///  This is a minimal implementation of Google+ Sign-In that
    ///  demonstrates:
    ///  - Using the Google+ Sign-In button to get an OAuth 2.0 refresh token.
    ///  - Exchanging the refresh token for an access token.
    ///  - Making Google+ API requests with the access token, including
    ///    getting a list of people that the user has circled.
    ///  - Disconnecting the app from the user's Google account and revoking
    ///    tokens.
    /// </summary>
    /// @author class@google.com (Gus Class)
    public class Signin : IHttpHandler, IRequiresSessionState, IRouteHandler
    {
        // These come from the APIs console:
        //   https://code.google.com/apis/console
        public static ClientSecrets secrets = new ClientSecrets()
        {
            ClientId = "1063463960063-r26vkrbeiosrtjdom40odjjmotv7ed1u.apps.googleusercontent.com",
            ClientSecret = "Z4GD6QGMKy5nEe-5ZlUDepdF"
        };

        // Configuration that you probably don't need to change.
        static public string APP_NAME = "Fitness Leaderboard";

        static public string[] SCOPES = { PlusService.Scope.PlusLogin, FitnessService.Scope.FitnessActivityRead };
        // Uncomment to retrieve email.
        //static public string[] SCOPES = { PlusService.Scope.PlusLogin, PlusService.Scope.UserinfoEmail };

        // Stores token response info such as the access token and refresh token.
        private TokenResponse token;

        // Used to peform API calls against Google+.
        //private PlusService ps = null;
        private FitnessService fs = null;
        private PlusService ps = null;
        private String emailid {get;set;}
        private String nickname;
        private String userAccountStatus;

        /// <summary>
        /// Processes the request based on the path.
        /// </summary>
        /// <param name="context">Contains the request and response.</param>
        public void ProcessRequest(HttpContext context)
        {
            // Redirect base path to signin.
            if (context.Request.Path.EndsWith("/"))
            {
                context.Response.RedirectPermanent("/signin.ashx");
            }

            // This is reached when the root document is passed. Return HTML
            // using index.html as a template.
            if (context.Request.Path.EndsWith("/signin.ashx"))
            {
                String state = (String)context.Session["state"];

                // Store a random string in the session for verifying
                // the responses in our OAuth2 flow.
                if (state == null)
                {
                    Random random = new Random((int)DateTime.Now.Ticks);
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < 13; i++)
                    {
                        builder.Append(Convert.ToChar(
                                Convert.ToInt32(Math.Floor(
                                        26 * random.NextDouble() + 65))));
                    }
                    state = builder.ToString();
                    context.Session["state"] = state;
                }

                // Render the templated HTML.
                String templatedHTML = File.ReadAllText(
                     context.Server.MapPath("index.html"));
                templatedHTML = Regex.Replace(templatedHTML,
                    "[{]{2}\\s*APPLICATION_NAME\\s*[}]{2}", APP_NAME);
                templatedHTML = Regex.Replace(templatedHTML,
                    "[{]{2}\\s*CLIENT_ID\\s*[}]{2}", secrets.ClientId);
                templatedHTML = Regex.Replace(templatedHTML,
                    "[{]{2}\\s*STATE\\s*[}]{2}", state);

                context.Response.ContentType = "text/html";
                context.Response.Write(templatedHTML);
                return;
            }


            if (context.Session["authState"] == null)
            {
                // The connect action exchanges a code from the sign-in button,
                // verifies it, and creates OAuth2 credentials.
                if (context.Request.Path.Contains("/connect"))
                {
                    // Get the code from the request POST body.
                    StreamReader sr = new StreamReader(
                        context.Request.InputStream);
                    string code = sr.ReadToEnd();

                    string state = context.Request["state"];

                    // Test that the request state matches the session state.
                    if (!state.Equals(context.Session["state"]))
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }

                    // Use the code exchange flow to get an access and refresh token.
                    IAuthorizationCodeFlow flow =
                        new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                        {
                            ClientSecrets = secrets,
                            Scopes = SCOPES
                        });

                    token = flow.ExchangeCodeForTokenAsync("", code, "postmessage",
                            CancellationToken.None).Result;

                    // Create an authorization state from the returned token.
                    context.Session["authState"] = token;

                    // Get tokeninfo for the access token if you want to verify.
                    Oauth2Service service = new Oauth2Service(
                        new Google.Apis.Services.BaseClientService.Initializer());
                    Oauth2Service.TokeninfoRequest request = service.Tokeninfo();
                    request.AccessToken = token.AccessToken;

                    Tokeninfo info = request.Execute();

                    string gplus_id = info.UserId;

                }
                else
                {
                    // No cached state and we are not connecting.
                    context.Response.StatusCode = 400;
                    return;
                }
            }
            else if (context.Request.Path.Contains("/connect"))
            {
                // The user is already connected and credentials are cached.
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 200;
                context.Response.Write(JsonConvert.SerializeObject("Current user is already connected."));

                return;
            }
            else
            {
                // Register the authenticator and construct the Plus service
                // for performing API calls on behalf of the user.
                token = (TokenResponse)context.Session["authState"];
                IAuthorizationCodeFlow flow =
                    new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = secrets,
                        Scopes = SCOPES
                    });

                UserCredential credential = new UserCredential(flow, "me", token);

                bool success = credential.RefreshTokenAsync(CancellationToken.None).Result;

                token = credential.Token;

                fs = new FitnessService(
                     new Google.Apis.Services.BaseClientService.Initializer()
                     {
                         ApplicationName = "Fitness App",
                         HttpClientInitializer = credential
                     });
                ps = new PlusService(
                    new Google.Apis.Services.BaseClientService.Initializer()
                    {
                        ApplicationName = ".NET Quickstart",
                        HttpClientInitializer = credential
                    });
                var me = ps.People.Get("me").Execute();
                var useremail = me.Emails.FirstOrDefault().Value;
                emailid = useremail;
                connection_obj conn = new connection_obj();
                nickname = conn.readusername(emailid);
                userAccountStatus = conn.getUserAccountStatus(emailid).Trim();
                context.Session["nickname"] = nickname;
                context.Session["accountStatus"] = userAccountStatus;
            }

            if (context.Request.Path.Contains("/getGfitFinalLeaderboard"))
            {
               
                connection_obj conn = new connection_obj();

                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime now = DateTime.UtcNow;
                DateTime xTimeConverted = TimeZoneInfo.ConvertTimeFromUtc(now, timeZone);
                DateTime x = new DateTime(xTimeConverted.Year, xTimeConverted.Month, xTimeConverted.Day, 0, 0, 0);

                string json = conn.readGstepsByDay(x.AddDays(-1));

                context.Response.Write(json);

                return;
            }
            if (context.Request.Path.Contains("/getGfitCurrLeaderboard"))
            {

                connection_obj conn = new connection_obj();

                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime now = DateTime.UtcNow;
                DateTime xTimeConverted = TimeZoneInfo.ConvertTimeFromUtc(now, timeZone);
                DateTime x = new DateTime(xTimeConverted.Year, xTimeConverted.Month, xTimeConverted.Day, 0, 0, 0);

                string json = conn.readGstepsByDay(x);

                context.Response.Write(json);

                return;
            }
            if (context.Request.Path.Contains("/getGfitUserChart"))
            {
               
                connection_obj conn = new connection_obj();

                string json = conn.readGStepsByUser(emailid);
                context.Response.Write(json);

                return;
            }
            if (context.Request.Path.Contains("/getManualFinalLeaderboard"))
            {
                
                connection_obj conn = new connection_obj();

                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime now = DateTime.UtcNow;
                DateTime xTimeConverted = TimeZoneInfo.ConvertTimeFromUtc(now, timeZone);
                DateTime x = new DateTime(xTimeConverted.Year, xTimeConverted.Month, xTimeConverted.Day, 0, 0, 0);

                string json = conn.readManualStepsByDay(x.AddDays(-1));

                context.Response.Write(json);

                return;
            }
            if (context.Request.Path.Contains("/getManualCurrLeaderboard"))
            {
                
                connection_obj conn = new connection_obj();

                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime now = DateTime.UtcNow;
                DateTime xTimeConverted = TimeZoneInfo.ConvertTimeFromUtc(now, timeZone);
                DateTime x = new DateTime(xTimeConverted.Year, xTimeConverted.Month, xTimeConverted.Day, 0, 0, 0);

                string json = conn.readManualStepsByDay(x);

                context.Response.Write(json);

                return;
            }
            if (context.Request.Path.Contains("/getManualUserChart"))
            {
                
                connection_obj conn = new connection_obj();
                string json = conn.readManaulStepsByUser(emailid);
                context.Response.Write(json);

                return;
            }
            if (context.Request.Path.Contains("/getUserData"))
            {

                connection_obj conn = new connection_obj();
                string json = conn.readTempManaulSteps(emailid);
                context.Response.Write(json);

                return;
            }
            if (context.Request.Path.Contains("/getUserGoal"))
            {
                connection_obj conn = new connection_obj();
                String goal = conn.readusergoal(emailid);
                context.Response.Write(goal);
                return;
            }
            // Perform an authenticated API request to retrieve the list of
            // people that the user has made visible to the app.
            if (context.Request.Path.Contains("/update_user_logging"))
            {

                connection_obj conn = new connection_obj();
                conn.inseruserloggingactivity(emailid);

                return;
            }

            if (context.Request.Path.Contains("/updateUserGfitSteps"))
            {
                
                
                {
                    var query = new stepQuery(fs);
                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime now = DateTime.UtcNow;
                    DateTime x = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    var xTimeConverted = TimeZoneInfo.ConvertTimeToUtc(x, timeZone);
                    var list = query.QueryStepPerDay(xTimeConverted.AddDays(-2), now);
                    connection_obj conn = new connection_obj();
                    conn.insertGStepsdata(list, nickname, emailid);                    
                }
                return;
            }

            if (context.Request.Path.Contains("/updateGoal"))
            {
                string json = new StreamReader(context.Request.InputStream).ReadToEnd();
                string[] tokens = json.Split('=');
                int goal = Convert.ToInt32(tokens[1]);

                connection_obj conn = new connection_obj();
                conn.updateStepGoal(goal, nickname, emailid);
                return;
            }
            if (context.Request.Path.Contains("/addUser"))
            {
                string newNickname = context.Request.Params.Get(0);
                string newEmailId = context.Request.Params.Get(1);
                string newGoal= context.Request.Params.Get(2);
                string newAccountStatus = context.Request.Params.Get(3);
                
                connection_obj conn = new connection_obj();
                conn.addUser(newNickname, newEmailId, newGoal, newAccountStatus);
                return;
            }
            if (context.Request.Path.Contains("/updateUserAccountStatus"))
            {
                string newNickname = context.Request.Params.Get(0);                
                string newAccountStatus = context.Request.Params.Get(1);

                connection_obj conn = new connection_obj();
                conn.updateUserStatus(newNickname, newAccountStatus);
                return;
            }


            // Get data between certain range
            if (context.Request.Path.Contains("/getDataForRange"))
            {
                string json = new StreamReader(context.Request.InputStream).ReadToEnd();
                string[] tokens = json.Split(new char[] { '&', '=' });
                string startdate = tokens[1];
                string enddate = tokens[3];
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime startDate = DateTime.Parse(startdate);
                DateTime endDate = DateTime.Parse(enddate);
                var query2 = new stepQuery(fs);
                var list = query2.QueryStepPerDay(TimeZoneInfo.ConvertTimeToUtc(startDate, timeZone), TimeZoneInfo.ConvertTimeToUtc(endDate.AddDays(1), timeZone));

                connection_obj conn = new connection_obj();
                conn.insertGStepsdata(list, nickname, emailid);

                context.Response.Write("Success");
                return;
            }
            if (context.Request.Path.Contains("/updateUserSteps"))
            {
               
                string day = context.Request.Params.Get(0);
                string steps = context.Request.Params.Get(1);                
                DateTime userDate = DateTime.Now;
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime now = DateTime.UtcNow;
                DateTime xTimeConverted = TimeZoneInfo.ConvertTimeFromUtc(now, timeZone);
                DateTime x = new DateTime(xTimeConverted.Year, xTimeConverted.Month, xTimeConverted.Day, 0, 0, 0);

                if (day == "Today")
                {
                    userDate = x;
                }
                else if (day == "Yesterday")
                {
                    userDate = x.AddDays(-1);
                }
                connection_obj conn = new connection_obj();
                conn.insertUserStepsData(userDate, steps, nickname, emailid);

                context.Response.Write("Success");
                return;
            }
            if (context.Request.Path.Contains("/getRanking"))
            {

                string typeOfRanking = context.Request.Params.Get(0);
                string dataSource = context.Request.Params.Get(1);
                DateTime startDay = new DateTime(2017, 4, 23, 0, 0, 0);
                DateTime endDay = new DateTime(2017,5, 6, 0, 0, 0);

                if (typeOfRanking == "Cumulative")
                {
                    userDate = x;
                }
                else if (typeOfRanking == "Week")
                {
                    userDate = x.AddDays(-1);
                }
                connection_obj conn = new connection_obj();
                conn.insertUserStepsData(userDate, steps, nickname, emailid);

                context.Response.Write("Success");
                return;
            }

            // Disconnect the user from the application by revoking the tokens
            // and removing all locally stored data associated with the user.
            if (context.Request.Path.Contains("/disconnect"))
            {
                // Perform a get request to the token endpoint to revoke the
                // refresh token.
                token = (TokenResponse)context.Session["authState"];
                string tokenToRevoke = (token.RefreshToken != null) ?
                    token.RefreshToken : token.AccessToken;

                WebRequest request = WebRequest.Create(
                    "https://accounts.google.com/o/oauth2/revoke?token=" +
                    tokenToRevoke);

                WebResponse response = request.GetResponse();

                // Remove the cached credentials.
                context.Session["authState"] = null;

                // You could reset the state in the session but you must also
                // reset the state on the client.
                context.Session["state"] = null;
                context.Response.Write(
                    response.GetResponseStream().ToString().ToCharArray());

                return;
            }
        }

        /// <summary>
        /// Implements IRouteHandler interface for mapping routes to this
        /// IHttpHandler.
        /// </summary>
        /// <param name="requestContext">Information about the request.
        /// </param>
        /// <returns></returns>
        public IHttpHandler GetHttpHandler(RequestContext
            requestContext)
        {
            var page = BuildManager.CreateInstanceFromVirtualPath
                 ("~/signin.ashx", typeof(IHttpHandler)) as IHttpHandler;
            return page;
        }

        public bool IsReusable { get { return false; } }
    }
}