using Rest.Enums;
using RestSharp;

namespace Rest.Services
{
    public class BaseService
    {
        protected static RestResponse CreateAndExecuteRequest(string resource, Method method, Scope scope, object? model = null)
        {
            var request = new RestRequest(resource, method);

            if (model != null)
            {
                request.AddJsonBody(model);
            }

            RestTrainingClient.InitializeClient(scope);
            var response = RestTrainingClient.ExecuteRequest(request);
            
            return response;
        }
    }
}
