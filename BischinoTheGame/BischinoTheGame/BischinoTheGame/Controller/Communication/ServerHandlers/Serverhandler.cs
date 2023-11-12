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
    /// <summary>
    /// This class is the bottleneck for communicating with the server through REST calls.
    /// </summary>
    public abstract class ServerHandler
    {
        /// <summary>
        /// Timeout in milliseconds of each http operation. 
        /// </summary>
        protected static int Timeout = 23 * 1000;



        /// <summary>
        /// This is the only instance <see cref="HttpClient"/> used in the application to communicate with the server.
        /// </summary>
        protected static readonly HttpClient Client = GetClient();



        /// <summary>
        /// Base URI for rest calls. 
        /// </summary>
        protected abstract string BaseUri { get; }  



        /// <summary>
        /// Converts a string to a relative uri
        /// </summary>
        /// <param name="uri">string version of a relative uri</param>
        /// <returns>Uri version of a relative uri</returns>
        protected static Uri ToRelativeUri(string uri) => new Uri(uri, UriKind.Relative);



        /// <summary>
        /// Remote Uri of the server.
        /// </summary>
        private static Uri RestServerUri { get; }



        /// <summary>
        /// Initializes <see cref="RestServerUri"/> with the actual uri of the server
        /// </summary>
        static ServerHandler()
        {
            //RestServerUri = new UriBuilder(@"http://89.34.16.77/plesk-site-preview/bischinocardgame.com/").Uri;
            RestServerUri = new UriBuilder(@"http://eqweqw.somee.com/").Uri;
            RestServerUri = new UriBuilder(@"http://10.0.2.2:5000").Uri;
            // RestServerUri = new UriBuilder(@"http://192.168.1.67").Uri;
        }
            


        /// <summary>
        /// Build an instance of <see cref="HttpClient"/> that will be used as singleton in <see cref="Client"/>
        /// </summary>
        /// <returns></returns>
        private static HttpClient GetClient()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
            };
            var client = new HttpClient(handler)
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", @"eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjVlM2VkYWUzMjIyNTk1MmQ5NDRkYWJhYSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE1ODEyNTI5MTYsImlzcyI6Imlzc3VlciIsImF1ZCI6ImF1ZGllbmNlIn0.X6aqo_1wj10GEzcOFbywEnKkfk1anRfdzFt6eFTF0Xk")
                }
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }



        /// <summary>
        /// Make a post request to the server, sending a .Net object as json object
        /// </summary>
        /// <param name="relativeUri">Relative uri of the rest call, which is added to <see cref="RestServerUri"/> and <see cref="BaseUri"/> </param>
        /// <param name="toSend">.Net object to send as json object</param>
        /// <param name="token">Cancellation token to cancel the async operation</param>
        protected static async Task PostWithUri(Uri relativeUri, object toSend, CancellationToken token = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            var uri = new Uri(RestServerUri.ToString() + relativeUri.ToString());
            var resp = await Client.PostAsJsonAsync(uri, toSend, linkedTokenSource.Token).TimeoutAfter(TimeSpan.FromMilliseconds(Timeout), linkedTokenSource);

            if (!resp.IsSuccessStatusCode)
                await ThrowAnException(resp);
        }



        /// <summary>
        /// Make a Post request to the server to obtain an object in the body.
        /// </summary>
        /// <typeparam name="T">Type of the expected object to be sent back from the server</typeparam>
        /// <param name="relativeUri">Relative uri of the rest call, which is added to <see cref="RestServerUri"/> and <see cref="BaseUri"/> </param>
        /// <param name="body">.Net object to be sent as json object in the body of the request</param>
        /// <param name="token">Cancellation token to cancel the async operation</param>
        /// <returns>A generic object returned from the server</returns>
        protected static async Task<T> GetWithUri<T>(Uri relativeUri, object body, CancellationToken token = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

            var uri = new Uri(RestServerUri.ToString() + relativeUri.ToString());
            var resp = await Client.PostAsJsonAsync(uri, body, linkedTokenSource.Token).TimeoutAfter(TimeSpan.FromMilliseconds(Timeout), linkedTokenSource);

            if (!resp.IsSuccessStatusCode)
                await ThrowAnException(resp);

            var correctResponse = await resp.Content.ReadAsAsync<ResponseHeader<T>>(token);
            return correctResponse.Value;
        }



        /// <summary>
        /// Make a Post request to the server to obtain an object in the body.
        /// </summary>
        /// <typeparam name="T">Type of the expected object to be sent back from the server</typeparam>
        /// <param name="relativeUri">Relative uri of the rest call, which is added to <see cref="RestServerUri"/> and <see cref="BaseUri"/> </param>
        /// <param name="token">Cancellation token to cancel the async operation</param>
        /// <returns>A generic object returned from the server</returns>
        protected static async Task<T> GetWithUri<T>(Uri relativeUri, CancellationToken token = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);

            var uri = new Uri(RestServerUri.ToString() + relativeUri.ToString());
            var resp = await Client.GetAsync(uri, linkedTokenSource.Token).TimeoutAfter(TimeSpan.FromMilliseconds(Timeout), linkedTokenSource);

            if (!resp.IsSuccessStatusCode)
                await ThrowAnException(resp);

            var correctResponse = await resp.Content.ReadAsAsync<ResponseHeader<T>>(token);
            return correctResponse.Value;
        }



        /// <summary>
        /// Make a post request to the server, sending a .Net object as json object
        /// </summary>
        /// <param name="toSend">.Net object to be sent as json object</param>
        /// <param name="token">Cancellation token to cancel the async operation</param>
        /// <param name="methodName">The caller method name. This string will is then converted into the relative uri of the post call</param>
        protected async Task Post(object toSend, CancellationToken token = default, [CallerMemberName] string methodName = "")
        {
            var uri = MethodNameToUri(methodName);
            await PostWithUri(uri, toSend, token);
        }



        /// <summary>
        /// Make a Post request to the server to obtain an object in the body.
        /// </summary>
        /// <typeparam name="T">Type of the expected object to be sent back from the server</typeparam>
        /// <param name="toSend">.Net object to be sent as json object</param>
        /// <param name="token">Cancellation token to cancel the async operation</param>
        /// <param name="methodName">The caller method name. This string will is then converted into the relative uri of the post call</param>
        /// <returns>A generic object returned from the server</returns>
        protected async Task<T> Get<T>(object toSend, CancellationToken token = default, [CallerMemberName] string methodName = "")
        {
            var uri = MethodNameToUri(methodName);
            var ret = await GetWithUri<T>(uri, toSend, token);
            return ret;
        }



        /// <summary>
        /// Make a Post request to the server to obtain an object in the body.
        /// </summary>
        /// <typeparam name="T">Type of the expected object to be sent back from the server</typeparam>
        /// <param name="methodName">The caller method name. This string will is then converted into the relative uri of the post call</param>
        /// <returns>A generic object returned from the server</returns>
        protected async Task<T> GetPure<T>([CallerMemberName] string methodName = "")
        {
            var uri = MethodNameToUri(methodName);
            var ret = await GetWithUri<T>(uri);
            return ret;
        }



        /// <summary>
        /// Throw a <see cref="HttpRequestException"/> if the error has meaning for the end-user, else throws an <see cref="Exception"/>
        /// </summary>
        /// <param name="response">Response sent back from the server</param>
        private static async Task ThrowAnException(HttpResponseMessage response)
        {
            var errorResponse = await response.Content.ReadAsAsync<ResponseHeader>();
            throw errorResponse == null ? new Exception($"Server error, status code {response.StatusCode}") : new HttpServerException(response.StatusCode, errorResponse.Message);
        }



        protected async Task<T> TryGet<T>(Func<Task<T>> func, int attempts = 2)
        {
            for (var i = 0; i < attempts; i++)
                try
                {
                    return await func.Invoke();
                }
                catch (Exception e) when (!(e is ServerException) && i < attempts - 1) { }

            throw new Exception("Impossible go here");
        }

        protected async Task TryPost(Func<Task> func, int attempts = 2)
        {
            for (var i = 0; i < attempts; i++)
                try
                {
                    await func.Invoke();
                    return;
                }
                catch (Exception e) when (!(e is ServerException) && i < attempts - 1) { }

            throw new Exception("Impossible go here");
        }





        /// <summary>
        /// Converts a method name to the full uri. This is used to make a rest call.
        /// </summary>
        /// <param name="name">The method name</param>
        /// <returns></returns>
        protected Uri MethodNameToUri(string name) => ToRelativeUri(BaseUri + name);
    }
}
