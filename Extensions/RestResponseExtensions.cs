using Newtonsoft.Json;
using RestSharp;

namespace Rest.Extensions
{
    public static class RestResponseExtensions
    {
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
                Console.WriteLine($"Error deserializing of {response.ResponseUri} response content: {ex.Message}");
                throw;
            }
        }
    }
}
