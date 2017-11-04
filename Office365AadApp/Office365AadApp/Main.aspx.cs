using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Office365AadApp
{
    public partial class Main : System.Web.UI.Page
    {
        string accessToken;
        string clientId = System.Configuration.ConfigurationManager.AppSettings["AadClientId"];
        string clientSecret = System.Configuration.ConfigurationManager.AppSettings["AadClientSecret"];
        O365InstnaceType o365Type;

        protected string msgToShow = "";

        protected void Page_Init(object sender, EventArgs e)
        {
            Enum.TryParse(System.Configuration.ConfigurationManager.AppSettings["O365Instance"], out o365Type);

            string code = Request["code"];
            if (string.IsNullOrEmpty(code))
            {
                string authorizeUrl = MakeSignInurl();
                Response.Redirect(authorizeUrl, true);
            }
            else
            {
                GetAccessToken(code);
            }
        }

        private void GetAccessToken(string authCode)
        {
            string redirectUrl = GetRedirectPageUrl();
            var appInfo = new MsOAuthAppInfo(o365Type, clientId, clientSecret, redirectUrl);

            string resourceUrl = appInfo.GetResourceUrl(o365Type);

            var postData = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "redirect_uri", redirectUrl },
                { "code",  authCode},
                { "grant_type", "authorization_code" },
                { "resource", resourceUrl }
            };

            JObject tokenObj = MyHttpHelper.HttpPostRequest(appInfo.GetTokenUrl(), postData);
            accessToken = tokenObj["access_token"].ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string graphCallPath = "https://microsoftgraph.chinacloudapi.cn/v1.0/me/messages";
            JObject msgsReturnObj = MyHttpHelper.HttpRestGet(graphCallPath, accessToken);
            int count = msgsReturnObj["value"].Count();
            for (int i = 0; i < count; i++)
            {
                JToken msg = msgsReturnObj["value"][i];
                msgToShow += string.Format("标题：{0} <a href=\"{1}\">点击阅读</a>{2}<br /><br />", msg["subject"].Value<string>(), msg["webLink"].Value<string>(), System.Environment.NewLine);
            }
        }

        private string MakeSignInurl()
        {
            var appInfo = new MsOAuthAppInfo(o365Type, clientId, clientSecret, GetRedirectPageUrl());
            return appInfo.GetAuthorizeUrl();
        }

        private string GetRedirectPageUrl()
        {
            return "http://localhost:61304/Main.aspx";
        }
    }
}