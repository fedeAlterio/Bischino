using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BischinoTheGame.Controller.Communication.Exceptions;
using BischinoTheGame.Extensions;
using BischinoTheGame.ViewModel.PageViewModels;
using Newtonsoft.Json;

namespace BischinoTheGame.Controller.Communication.ServerHandlers
{
    public abstract class ServerHandler
    {
        protected static int Timeout = 10 * 1000;


        protected static readonly HttpClient Client = GetClient();
        protected abstract string BaseUrl { get; }  
        protected static Uri ToRelativeUri(string uri) => new Uri(uri, UriKind.Relative);
        private static Uri RestServerUri { get; }


        static ServerHandler()
        {
            RestServerUri = new UriBuilder(@"https://bischino20200324045818.azurewebsites.net").Uri;
            //RestServerUri = new UriBuilder(@"https://10.0.2.2:5001").Uri;
            //RestServerUri = new UriBuilder(@"https://192.168.1.67:5001").Uri;
        }
            



        private static HttpClient GetClient()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(20),
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", @"eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjVlM2VkYWUzMjIyNTk1MmQ5NDRkYWJhYSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE1ODEyNTI5MTYsImlzcyI6Imlzc3VlciIsImF1ZCI6ImF1ZGllbmNlIn0.X6aqo_1wj10GEzcOFbywEnKkfk1anRfdzFt6eFTF0Xk")
                }
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }




        protected static async Task PostWithUri(Uri relativeUri, object toSend, CancellationToken token)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            var uri = new Uri(RestServerUri, relativeUri);
            var resp = await Client.PostAsJsonAsync(uri, toSend, linkedTokenSource.Token).TimeoutAfter(TimeSpan.FromMilliseconds(Timeout), linkedTokenSource);

            if (!resp.IsSuccessStatusCode)
                await ThrowAnException(resp);
        }



        protected static async Task<T> GetWithUri<T>(Uri relativeUri, object body, CancellationToken token)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

            var uri = new Uri(RestServerUri, relativeUri);
            var resp = await Client.PostAsJsonAsync(uri, body, linkedTokenSource.Token).TimeoutAfter(TimeSpan.FromMilliseconds(Timeout), linkedTokenSource);

            if (!resp.IsSuccessStatusCode)
                await ThrowAnException(resp);

            var correctResponse = await resp.Content.ReadAsAsync<ResponseHeader<T>>(token);
            return correctResponse.Value;
        }



        protected static async Task<T> GetWithUri<T>(Uri relativeUri)
        {

            var uri = new Uri(RestServerUri, relativeUri);
            var resp = await Client.GetAsync(uri);

            if (!resp.IsSuccessStatusCode)
                await ThrowAnException(resp);

            var correctResponse = await resp.Content.ReadAsAsync<ResponseHeader<T>>();
            return correctResponse.Value;
        }



        protected static Task PostWithUri(Uri relativeUri, object toSend)
            => PostWithUri(relativeUri, toSend, CancellationToken.None);



        private static async Task ThrowAnException(HttpResponseMessage response)
        {
            var errorResponse = await response.Content.ReadAsAsync<ResponseHeader>();
            throw errorResponse == null ? new Exception($"Server error, status code {response.StatusCode}") : new HttpServerException(response.StatusCode, errorResponse.Message);
        }

       

        protected async Task Post(object toSend, CancellationToken token, [CallerMemberName] string methodName = "")
        {
            var uri = MethodNameToUri(methodName);
            await PostWithUri(uri, toSend, token);
        }



        protected Task Post(object toSend, [CallerMemberName] string methodName = "")
            => Post(toSend, CancellationToken.None, methodName);



        protected async Task<T> Get<T>(object toSend, CancellationToken token = default, [CallerMemberName] string methodName = "")
        {
            var uri = MethodNameToUri(methodName);
            var ret = await GetWithUri<T>(uri, toSend, token);
            return ret;
        }



        protected async Task<T> GetPure<T>([CallerMemberName] string methodName = "")
        {
            var uri = MethodNameToUri(methodName);
            var ret = await GetWithUri<T>(uri);
            return ret;
        }


        protected Uri MethodNameToUri(string name) => ToRelativeUri(BaseUrl + name);
    }
}
