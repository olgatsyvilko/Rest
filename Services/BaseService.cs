using Rest.Core;
using Rest.Enums;
using RestSharp;
using System.Net;

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

            ValidateResponse(response, resource, method);

            return response;
        }

        private static void ValidateResponse(RestResponse response, string resource, Method method)
        {
            var successfulCodes = new List<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Accepted, HttpStatusCode.NoContent };
            
            if (!successfulCodes.Contains(response.StatusCode))
            {
                var message = $"{method} request '{resource}' is failed with status: {response.StatusCode} and message: {response.ErrorMessage}";
                Console.WriteLine(message);
            }
            else
            {
                Console.WriteLine($"{method} request '{resource}' is executed successfully");
            }
        }
    }
}
