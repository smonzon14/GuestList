using App3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace App3.Data
{
    public class RestService
    {
        HttpClient client;
        string grant_type = "password";
        public RestService()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        }
        
        public async Task<Token> Register(User user)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("grant_type", grant_type));
            postData.Add(new KeyValuePair<string, string>("username", user.username));
            postData.Add(new KeyValuePair<string, string>("email", user.username));
            postData.Add(new KeyValuePair<string, string>("password", user.password));
            var content = new FormUrlEncodedContent(postData);
            var response = await PostResponseLogin<Token>(constants.LoginUrl + "api/auth/register", content);
            DateTime dt = new DateTime();
            dt = DateTime.Today;
            response.expire_date = dt.AddSeconds(response.expire_in);
            return response;
        }

        public async Task<Token> Login(User user)
        {
            var postData = new List<KeyValuePair<string, string>>();
            postData.Add(new KeyValuePair<string, string>("grant_type", grant_type));
            postData.Add(new KeyValuePair<string, string>("username", user.username));
            postData.Add(new KeyValuePair<string, string>("password", user.password));
            var content = new FormUrlEncodedContent(postData);
            var response = await PostResponseLogin<Token>(constants.LoginUrl + "api/auth/login", content);
            DateTime dt = new DateTime();
            dt = DateTime.Today;
            response.expire_date = dt.AddSeconds(response.expire_in);
            return response;
        }
        public async Task<T> PostResponseLogin<T>(string weburl, FormUrlEncodedContent content) where T : class
        {
            var response = await client.PostAsync(weburl, content);
            var jsonResult = response.Content.ReadAsStringAsync().Result;
            var responseObject = JsonConvert.DeserializeObject<T>(jsonResult);
            return responseObject;
        }
        public async Task<T> PostResponse<T>(string weburl, string jsonstring) where T : class
        {
            var token = App.TokenDatabase.GetToken();
            string contentType = "application/json";
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.access_token);
            try
            {
                var result = await client.PostAsync(weburl, new StringContent(jsonstring, Encoding.UTF8, contentType));
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonresult = result.Content.ReadAsStringAsync().Result;
                    var contentResponse = JsonConvert.DeserializeObject<T>(jsonresult);
                    return contentResponse;
                }
                else { return null; }
            }
            catch { return null; }
        }
        public async Task<T> GetResponse<T>(string weburl) where T : class
        {
            var token = App.TokenDatabase.GetToken();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.access_token);
            try
            {
                var response = await client.GetAsync(weburl);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonresult = response.Content.ReadAsStringAsync().Result;
                    Debug.WriteLine("JsonResult: " + jsonresult);
                    var contentResponse = JsonConvert.DeserializeObject<T>(jsonresult);
                    return contentResponse;
                }
                else return null;
            }
            catch
            {
                return null;
            }
            
        }
    }
}
