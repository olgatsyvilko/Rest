using System.Text.Json.Serialization;

namespace Rest.Models
{
    public class UpdateUserDto
    {
        [JsonPropertyName("userNewValues")]
        public UserDto UserNewValues { get; init; }

        [JsonPropertyName("userToChange")]
        public UserDto UserToChange { get; init; }
    }
}
