using System.Text.Json.Serialization;

namespace Rest.Models
{
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; init; }

        [JsonPropertyName("token_type")]
        public required string TokenType { get; init; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }

        [JsonPropertyName("scope")]
        public required string Scope { get; init; }
    }
}