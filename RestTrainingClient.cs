using Rest.Enums;
using RestSharp;

namespace Rest
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
            var restOptions = new RestClientOptions("http://localhost:3355")
            {
                Authenticator = new RestTrainingAuthenticator(scope)
            };
            _restClient = new RestClient(restOptions);
        }

        public void Dispose()
        {
            _restClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
