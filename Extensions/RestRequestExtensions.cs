using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Rest.Extensions
{
    public static class RestRequestExtensions
    {
        [AllureStep("Attach Request Payload to Report")]
        public static void AttachRequestContentToReport(this RestRequest request)
        {
            var fileName = "requestContent.json";

            var requestParameters = request.Parameters
                .Where(p => p.Type == ParameterType.RequestBody || p.Type == ParameterType.GetOrPost)
                .Select(p => p.Value)
                .FirstOrDefault();

            if (requestParameters != null)
            {
                File.WriteAllText(fileName, JsonConvert.SerializeObject(requestParameters));
                AllureApi.AddAttachment($"Request Payload to {request.Resource}", "application/json",
                    Path.Combine(TestContext.CurrentContext.TestDirectory, fileName));
            }       
        }
    }
}
