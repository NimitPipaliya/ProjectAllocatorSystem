using Newtonsoft.Json;
using ProjectAllocatorSystemMVC.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;

namespace ProjectAllocatorSystemMVC.Implementation
{
    [ExcludeFromCodeCoverage]
    public class HttpClientService: IHttpClientService
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient _httpClient;
        private HttpResponseMessage response = new HttpResponseMessage();

        public HttpClientService(IConfiguration configuration, HttpClient httpClient)
        {
            this.configuration = configuration;
            this._httpClient = httpClient;
        }

        public HttpResponseMessage GetHttpResponseMessage<TService>(string url, HttpRequest? httpRequest = null) where TService : class
        {
            var uri = new Uri($"{url}");

            return FetchResponseForGetAndPostMethods(uri, HttpMethod.Get, null, httpRequest);
        }

        /// <inheritdoc/>
        public HttpResponseMessage GetHttpResponseMessage<TService>(string url, object id, HttpRequest? httpRequest = null)
           where TService : class
        {
            var uri = new Uri($"{url}/{id}");

            return FetchResponseForGetAndPostMethods(uri, HttpMethod.Get, null, httpRequest);
        }

        /// <inheritdoc/>
        public HttpResponseMessage PostHttpResponseMessage<TService>(string url, TService entityDto, HttpRequest? httpRequest = null)
         where TService : class
        {
            var uri = new Uri($"{url}");

            return FetchResponseForGetAndPostMethods(uri, HttpMethod.Post, entityDto, httpRequest);
        }

        /// <inheritdoc/>
        public HttpResponseMessage PostHttpResponseMessage<TService>(string url, HttpRequest? httpRequest = null)
            where TService : class
        {
            var uri = new Uri($"{url}");

            return FetchResponseForGetAndPostMethods(uri, HttpMethod.Post, null, httpRequest);
        }

        /// <inheritdoc/>
        public HttpResponseMessage PutHttpResponseMessage<TService>(string url, TService entityDto, HttpRequest? httpRequest = null)
         where TService : class
        {
            var uri = new Uri($"{url}");

            return FetchResponseForGetAndPostMethods(uri, HttpMethod.Put, entityDto, httpRequest);
        }

        /// <inheritdoc/>
        public T ExecuteApiRequest<T>(string url, HttpMethod method, HttpRequest httpRequest, object? parameters = null, int? TimeOutInSecond = 60) where T : class
        {
            GetAndSetAccessToken(httpRequest);
            var request = new HttpRequestMessage(method, url);
            request.Content = parameters == null ? null : new StringContent(GetJson(parameters), Encoding.UTF8, "application/json");
            var response = _httpClient.Send(request);
            //response.EnsureSuccessStatusCode();
            var responseContent = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            else
            {
                responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        /// <summary>
        /// Method to get and set the access token
        /// </summary>
        /// <returns></returns>
        private void GetAndSetAccessToken(HttpRequest httpRequest)
        {
            // Retrieve the JWT token from the authentication cookie
            //Accessing the Cookie Data inside a Method
            var authCookie = httpRequest.Cookies["jwtToken"];

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authCookie);
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Common Method to fetch the response for get and post methods
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="httpMethod"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        private HttpResponseMessage FetchResponseForGetAndPostMethods(Uri uri, HttpMethod httpMethod, dynamic postData, HttpRequest? httpRequest = null)
        {
            GetAndSetAccessToken(httpRequest);
            HttpRequestMessage request = new HttpRequestMessage(httpMethod, uri);

            if (postData != null)
                request.Content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

            response = _httpClient.Send(request);
            return response;
        }

        private string GetJson(Object o)
        {
            var json = JsonConvert.SerializeObject(o);
            return json;
        }
    }
}
