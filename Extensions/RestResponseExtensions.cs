using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using Newtonsoft.Json;
using NLog;
using NUnit.Framework;
using RestSharp;

namespace Rest.Extensions
{
    public static class RestResponseExtensions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static T? DeserializeResponseContent<T>(this RestResponse response)
        {
            if (string.IsNullOrEmpty(response.Content))
            {
                throw new NullReferenceException($"The {response.ResponseUri} response content is null or empty");
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch (JsonException ex)
            {
                Logger.Error($"Error deserializing of {response.ResponseUri} response content: {ex.Message}");
                throw;
            }
        }

        [AllureStep("Attach Response Payload to Report")]
        public static void AttachResponseContentToReport(this RestResponse response)
        {
            var fileName = "responseContent.json";

            if (!string.IsNullOrEmpty(response.Content))
            {
                File.WriteAllText(fileName, response.Content);
                AllureApi.AddAttachment($"Response Payload from {response.ResponseUri}", "application/json",
                    Path.Combine(TestContext.CurrentContext.TestDirectory, fileName));
            }
        }
    }
}
