using Newtonsoft.Json;
using Rest.Extensions;
using Rest.Models;
using Rest.Services;

namespace Rest.Helpers
{
    public static class TestDataHelper
    {
        private static readonly UserService userService = new();
        private static readonly ZipCodeService zipCodeService = new();

        public static UpdateUserDto CreateUpdateUserDto(UserDto userNewValues)
        {
            var updateUserDto = new UpdateUserDto()
            {
                UserNewValues = userNewValues,
                UserToChange = GetRandomUser()
            };

            return updateUserDto;
        }

        public static UserDto GetRandomUser()
        {
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();
            return allUsers![RandomHelper.CreateRandomInt() % allUsers.Length];
        }

        public static UserDto[] GenerateJsonFileWithUsers(int userCount, string fileName)
        {
            var users = CreateValidUsers(userCount);
            File.WriteAllText(fileName, JsonConvert.SerializeObject(users, Formatting.Indented));

            return users;
        }

        public static UserDto[] GenerateJsonFileWithUsersWithInvalidData(int userCount, UserDto invalidUser, string fileName)
        {
            var array = CreateValidUsers(userCount);

            Array.Resize(ref array, array.Length + 1);
            array[^1] = invalidUser;

            File.WriteAllText(fileName, JsonConvert.SerializeObject(array, Formatting.Indented));

            return array;
        }

        private static UserDto[] CreateValidUsers(int userCount)
        {
            var array = new UserDto[userCount];

            var availableZipCodes = zipCodeService.GetZipCodes().DeserializeResponseContent<string[]>();

            for (int i = 0; i < array.Length; i++)
            {
                var userDto = new UserDto()
                {
                    Age = RandomHelper.CreateRandomInt(),
                    Name = RandomHelper.CreateRandomString(),
                    Sex = RandomHelper.GetRandomSex().ToString(),
                    ZipCode = availableZipCodes![RandomHelper.CreateRandomInt() % availableZipCodes.Length]
                };

                array[i] = userDto;
            }
            return array;
        }
    }
}
