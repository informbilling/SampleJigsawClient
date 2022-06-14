using IdentityModel.Client;
using InformBilling_JigsawClient_Example.Exceptions;
using InformBilling_JigsawClient_Example.Formatters;
using InformBilling_JigsawClient_Example.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace InformBilling_JigsawClient_Example.APIClient
{
    public class InformHttpClientBase : IInformHttpClientBase
    {
        private System.Net.Http.HttpClient informClient;
        private TokenResponse tokenResponse;
        private DateTime tokenCreation;

        private IdentityServerSettingsDto _identityServerSettingsDto;
        private HttpClientBaseSettings _apiCommonSettingsDto;

        public string ClientName { get; set; }

        /// <summary>
        /// DO NOT USE - This is nessesary for DI to work when resolving this class.
        /// </summary>
        public InformHttpClientBase()
        {
        }

        public void InitialiseFromDIResolver(IOptions<IdentityServerSettingsDto> identityServiceSettingsOptions, HttpClientBaseSettings apiCommonSettingsOptions)
        {
            _identityServerSettingsDto = identityServiceSettingsOptions.Value;
            _apiCommonSettingsDto = apiCommonSettingsOptions;
            ClientName = string.Empty;
            Initialise(_apiCommonSettingsDto.BaseAddress);
        }

        public void Initialise(string endPointBaseAddress)
        {
            informClient = new System.Net.Http.HttpClient { BaseAddress = new Uri(endPointBaseAddress) };
            informClient.DefaultRequestHeaders.Accept.Clear();
            informClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AddExtraHeaders(_apiCommonSettingsDto.ExtraHeaderValues);
            informClient.Timeout = new TimeSpan(0, 5, 0);
        }

        private void AddExtraHeaders(Dictionary<string, string> extraHeaderValues = null)
        {
            if (extraHeaderValues != null)
            {
                foreach (var item in extraHeaderValues.Keys)
                    informClient.DefaultRequestHeaders.Add(item, extraHeaderValues[item]);
            }
        }

        public async Task<T> GetAsync<T>(string relativeEndpoint)
        {
            return await SendAsync<T>(HttpMethod.Get, relativeEndpoint, model: null);
        }

        public async Task<object> GetAsyncObject<T>(string relativeEndpoint)
        {
            return await SendAsyncObject<T>(HttpMethod.Get, relativeEndpoint, model: null);
        }

        #region Generic Functionality
        private async Task<T> SendAsync<T>(HttpMethod httpMethod, string relativeEndPoint, object model = null)
        {
            string endPoint = _apiCommonSettingsDto.BaseAddress + relativeEndPoint;

            HttpRequestMessage httpRequest = await BuildRequestMessage(httpMethod, endPoint, model);
            HttpResponseMessage responseMessage = await informClient.SendAsync(httpRequest);

            await CheckForValidationResponse(responseMessage);

            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsAsync<T>();
            return result;
        }

        private static List<HttpStatusCode> _successCodes = new List<HttpStatusCode>()
        {
            HttpStatusCode.OK,
            HttpStatusCode.NoContent,
            HttpStatusCode.Created,
            HttpStatusCode.Accepted
        };

        private async Task<object> SendAsyncObject<T>(HttpMethod httpMethod, string relativeEndPoint, object model = null)
        {
            string endPoint = _apiCommonSettingsDto.BaseAddress + relativeEndPoint;

            HttpRequestMessage httpRequest = await BuildRequestMessage(httpMethod, endPoint, model);

            HttpResponseMessage responseMessage = await informClient.SendAsync(httpRequest);

            await CheckForValidationResponse(responseMessage);

            if (!_successCodes.Contains(responseMessage.StatusCode))
                throw new InformHttpException(responseMessage.StatusCode, await responseMessage.Content.ReadAsAsync<object>());

            var result = await responseMessage.Content.ReadAsAsync<T>();
            return result;
        }

        internal async Task<HttpRequestMessage> BuildRequestMessage(HttpMethod httpMethod, string endPoint, object model = null)
        {
            string token = await GetAccessToken().ConfigureAwait(false);

            HttpRequestMessage httpRequest = new HttpRequestMessage(httpMethod, endPoint);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (model != null)
            {
                httpRequest.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            }
            return httpRequest;
        }

        internal async Task<string> GetAccessToken(bool forceRefresh = false)
        {
            string token = "";

            if (!forceRefresh && IsTokenValid())
                token = tokenResponse.AccessToken;

            if (string.IsNullOrEmpty(token))
            {
                await RequestToken().ConfigureAwait(false);
                token = tokenResponse.AccessToken;

                if (!IsTokenValid())
                    throw new InvalidOperationException("An unexpected token validation error has occured during a token request.");
            }

            return token;
        }

        private async Task RequestToken()
        {
            tokenResponse = await informClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _identityServerSettingsDto.TokenAccessEndPoint,
                ClientId = _identityServerSettingsDto.ClientId,
                ClientSecret = _identityServerSettingsDto.APIKey,
                Scope = _identityServerSettingsDto.Scope
            });
            tokenCreation = DateTime.UtcNow;
        }

        private bool IsTokenValid()
        {
            if (tokenCreation.Equals(DateTime.MinValue))
                tokenCreation = DateTime.UtcNow;

            return tokenResponse != null &&
                !tokenResponse.IsError &&
                !string.IsNullOrWhiteSpace(tokenResponse.AccessToken) &&
                (tokenCreation.AddSeconds(tokenResponse.ExpiresIn) > DateTime.UtcNow);
        }

        internal async Task CheckForValidationResponse(HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode.Equals(HttpStatusCode.BadRequest))
            {
                BadRequestModelDto responseObject = null;

                try
                {
                    List<System.Net.Http.Formatting.MediaTypeFormatter> formatters = new List<System.Net.Http.Formatting.MediaTypeFormatter>();
                    formatters.Add(new System.Net.Http.Formatting.JsonMediaTypeFormatter());
                    formatters.Add(new ProblemJsonMediaTypeFormatter());
                    responseObject = await responseMessage.Content.ReadAsAsync<BadRequestModelDto>(formatters);
                }
                catch
                {
                    //If it's anything else, we don't care and want it to continue
                }

                if (responseMessage != null)
                {
                    throw new InformHttpException(HttpStatusCode.BadRequest, responseObject);
                }
            }
        }

        #endregion
    }
}
