using NUnit.Framework;
using Rest.Extensions;
using Rest.Helpers;
using Rest.Models;
using Rest.Services;
using System.Net;

namespace Rest.Tests
{
    [TestFixture]
    public class DeleteUserServiceTests
    {
        private readonly UserService userService = new();
        private readonly ZipCodeService zipCodeService = new();

        [Test]
        public void DeleteUser_AllFieldsAreFilledIn_ResponseStatusCodeIsNoContent_UserIsDeleted_UsedZipCodeIsReturnedBack()
        {
            var userToDelete = TestDataHelper.GetRandomUser();

            var response = userService.DeleteUser(userToDelete);

            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();
            var availableZipCodes = zipCodeService.GetZipCodes().DeserializeResponseContent<string[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.NoContent), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Equals(userToDelete)), Is.False, "User is NOT deleted");
                Assert.That(availableZipCodes!.Contains(userToDelete.ZipCode), Is.True, "Available Zip Codes list contains Zip Code of the deleted User");
            });
        }

        [Test]
        public void DeleteUser_RequiredFieldsAreFilledIn_ResponseStatusCodeIsNoContent_UserIsDeleted_UsedZipCodeIsReturnedBack()
        {
            var randomUser = TestDataHelper.GetRandomUser();
            var userToDelete = new UserDto()
            {
                Age = randomUser.Age,
                Name = randomUser.Name,
                Sex = randomUser.Sex
            };

            var response = userService.DeleteUser(userToDelete);

            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();
            var availableZipCodes = zipCodeService.GetZipCodes().DeserializeResponseContent<string[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.NoContent), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Equals(randomUser)), Is.True, "User is NOT deleted");
                Assert.That(availableZipCodes!.Contains(randomUser.ZipCode), Is.False, "Available Zip Codes list contains Zip Code of the deleted User");
            });
        }

        [Test]
        public void DeleteUser_RequiredFieldIsMissed_ResponseStatusCodeIsConflict_UserIsNotDeleted_UsedZipCodeIsNotReturnedBack()
        {
            var randomUser = TestDataHelper.GetRandomUser();
            var userToDelete = new UserDto()
            {
                Age = randomUser.Age,
                Name = randomUser.Name,
                ZipCode = randomUser.ZipCode
            };

            var response = userService.DeleteUser(userToDelete);

            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();
            var availableZipCodes = zipCodeService.GetZipCodes().DeserializeResponseContent<string[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.Conflict), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Equals(randomUser)), Is.True, "User is deleted");
                Assert.That(availableZipCodes!.Contains(randomUser.ZipCode), Is.False, $"'{randomUser.ZipCode}' Zip Code exists in the list");
            });
        }
    }
}
