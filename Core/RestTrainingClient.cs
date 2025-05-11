using Rest.Enums;
using RestSharp;

namespace Rest.Core
{
    public class RestTrainingClient : IDisposable
    {
        private static RestTrainingClient _instance;
        private static RestClient _restClient;

        private RestTrainingClient()
        {
        }

        public static RestTrainingClient Instance
        {
            get
            {
                _instance ??= new RestTrainingClient();

                return _instance;
            }
        }

        public static void InitializeClient(Scope scope)
        {
            var restOptions = new RestClientOptions(Configuration.BaseUrl)
            {
                Authenticator = new RestTrainingAuthenticator(scope)
            };
            _restClient = new RestClient(restOptions);
        }

        public static RestResponse ExecuteRequest(RestRequest request)
        {
            return _restClient.ExecuteAsync(request).Result;
        }

        public void Dispose()
        {
            _restClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
