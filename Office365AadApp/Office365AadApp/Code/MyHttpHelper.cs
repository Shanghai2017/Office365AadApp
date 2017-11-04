using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;


namespace Office365AadApp
{
    public class MyHttpHelper
    {
        public static JObject HttpPostRequest(string uri, IEnumerable<KeyValuePair<string, string>> postData)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                HttpRequestMessage requestMessage = new HttpRequestMessage();
                requestMessage.RequestUri = new Uri(uri);
                requestMessage.Headers.Accept.Clear();
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage responseMessage = null;
                HttpContent content = new FormUrlEncodedContent(postData);
                requestMessage.Method = HttpMethod.Post;
                requestMessage.Content = content;
                responseMessage = client.SendAsync(requestMessage).Result;
                string rspConent = responseMessage.Content.ReadAsStringAsync().Result;
                return JObject.Parse(rspConent);
            }
        }

        public static JObject HttpRestGet(string apiUrl, string accessToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                string rspConent = response.Content?.ReadAsStringAsync().Result;
                return JObject.Parse(rspConent);
            }
        }
    }
}