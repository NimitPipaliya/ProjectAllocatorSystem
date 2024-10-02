namespace ProjectAllocatorSystemMVC.Infrastructure
{
    public interface IHttpClientService
    {

        /// <summary>
        /// To get response for method type Get
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        HttpResponseMessage GetHttpResponseMessage<TService>(string url, HttpRequest? httpRequest = null)
         where TService : class;

        /// <summary>
        /// To get response for method type Get with id object
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="url"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        HttpResponseMessage GetHttpResponseMessage<TService>(string url, object id, HttpRequest? httpRequest = null)
        where TService : class;

        /// <summary>
        /// To fetch response for method type Post with data
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="url"></param>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        HttpResponseMessage PostHttpResponseMessage<TService>(string url, TService entityDto, HttpRequest? httpRequest = null)
        where TService : class;

        /// <summary>
        /// To fetch response for method type Post
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        HttpResponseMessage PostHttpResponseMessage<TService>(string url, HttpRequest? httpRequest = null)
        where TService : class;

        /// <summary>
        /// To fetch response for method type put
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        HttpResponseMessage PutHttpResponseMessage<TService>(string url, TService entityDto, HttpRequest? httpRequest = null)
         where TService : class;

        /// <summary>
        /// Method to get the deserialized response data from api response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="TimeOutInSecond"></param>
        /// <returns></returns>
        T ExecuteApiRequest<T>(string url, HttpMethod method, HttpRequest httpRequest, object? parameters = null, int? TimeOutInSecond = 60)
        where T : class;
    }
}
