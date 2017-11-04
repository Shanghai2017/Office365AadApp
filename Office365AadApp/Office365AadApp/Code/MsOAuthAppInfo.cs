using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Office365AadApp
{
    public class MsOAuthAppInfo
    {
        public string ClientID { get; set; }

        public string ClientSecret { get; set; }

        public O365InstnaceType O365Instance { get; set; }

        public bool IsMultiTenant { get; set; }

        public string ReplyUrl { get; set; }

        public string State { get; set; }

        public MsOAuthAppInfo( O365InstnaceType intanceType, string clientId, string clientSecret, string replyUrl, string state = "")
        {
            O365Instance = intanceType;
            ClientID = clientId;
            ClientSecret = clientSecret;
            IsMultiTenant = false;
            ReplyUrl = replyUrl;
            State = state;
        }

        public string GetAuthorizeUrl()
        {
            string redirect = HttpUtility.UrlEncode(ReplyUrl);
            string authUrl =  GetAuthEntryUrl(O365Instance) + $"/common/oauth2/authorize?response_type=code&redirect_uri={redirect}&client_id={ClientID}";

            if (!string.IsNullOrEmpty(State))
            {
                authUrl += $"&state={State}";
            }

            authUrl += "&resource=" + HttpUtility.UrlEncode(GetResourceUrl(O365Instance));

            return authUrl;
        }

        public string GetTokenUrl()
        {
            return GetAuthEntryUrl(O365Instance) + $"/common/oauth2/token";
        }

        public string GetResourceUrl(O365InstnaceType type)
        {
            string resourceUrl = "https://microsoftgraph.chinacloudapi.cn";

            if (type == O365InstnaceType.Global)
            { 
                resourceUrl = "https://graph.microsoft.com";
            }
            return resourceUrl;
        }

        private string GetAuthEntryUrl(O365InstnaceType type)
        {
            string authEntry = "https://login.partner.microsoftonline.cn";
            if (type == O365InstnaceType.Global)
            {
                authEntry = "https://login.microsoftonline.com";
            }
            return authEntry;
        }
    }

    public enum O365InstnaceType
    {
        China = 1,
        Global = 2
    }
}