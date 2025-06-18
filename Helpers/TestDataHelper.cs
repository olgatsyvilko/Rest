using Rest.Extensions;
using Rest.Models;
using Rest.Services;

namespace Rest.Helpers
{
    public static class TestDataHelper
    {
        private static readonly UserService userService = new();

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
    }
}
