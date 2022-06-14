using InformBilling_JigsawClient_Example.Models;
using Microsoft.Extensions.Options;

namespace InformBilling_JigsawClient_Example.APIClient
{
    public interface IInformHttpClientBase
    {

        /// <summary>
        /// The name of the HttpClient
        /// </summary>
        string ClientName { get; set; }

        void InitialiseFromDIResolver(IOptions<IdentityServerSettingsDto> identityServiceSettingsOptions, HttpClientBaseSettings apiCommonSettingsOptions);

        /// <summary>
        /// Retrieve model via Web API EndPoint
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string relativeEndpoint);

        /// <summary>
        /// Retrieve model via Web API EndPoint.
        /// </summary>
        /// <param name="relativeEndpoint"></param>
        /// <returns></returns>
        Task<object> GetAsyncObject<T>(string relativeEndpoint);
    }
}
