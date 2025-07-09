using NLog;
using NUnit.Framework;
using Rest.Core;
using Rest.Enums;
using Rest.Extensions;
using RestSharp;
using System.Net;

namespace Rest.Services
{
    public class BaseService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected static RestResponse CreateAndExecuteRequest(string resource, Method method, Scope scope, object? model = null, 
            string? fileName = null)
        {
            var request = new RestRequest(resource, method);

            if (model != null)
            {
                request.AddJsonBody(model);
            }

            if (fileName != null)
            {
                Logger.Trace($"Add file: {fileName}");
                request.AddFile(name: "file", path: Path.Combine(TestContext.CurrentContext.TestDirectory, fileName), contentType: "multipart/form-data");
            }

            request.AttachRequestContentToReport();

            RestTrainingClient.InitializeClient(scope);

            Logger.Trace($"Execute {method} request '{resource}'");
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
                Logger.Error(message);
            }
            else
            {
                Logger.Info($"{method} request '{resource}' is executed successfully");
            }

            response.AttachResponseContentToReport();
        }
    }
}
