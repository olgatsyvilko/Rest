using Newtonsoft.Json;

namespace Rest.Models
{
    public class UserDto
    {
        [JsonProperty(PropertyName = "age")]
        public int Age { get; init; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; init; }

        [JsonProperty(PropertyName = "sex")]
        public string Sex { get; init; }

        [JsonProperty(PropertyName = "zipCode")]
        public string ZipCode { get; init; }

        public override bool Equals(object? obj)
        {
            if (obj is UserDto other)
            {
                return Name == other.Name && Age == other.Age && Sex == other.Sex && ZipCode == other.ZipCode;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Age, Sex, ZipCode);
        }
    }
}
