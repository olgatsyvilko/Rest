using Allure.NUnit.Attributes;
using NUnit.Framework;
using Rest.Extensions;
using Rest.Helpers;
using Rest.Models;
using Rest.Services;
using System.Net;

namespace Rest.Tests
{
    [TestFixture]
    [AllureSubSuite("Upload Users Service")]
    public class UploadUsersServiceTests : BaseTest
    {
        private readonly UserService userService = new();

        [Test]
        public void UploadUsers_AllFieldsAreFilledIn_ResponseStatusCodeIsCreated_UsersAreUploaded()
        {
            var fileName = "users.json";
            var users = TestDataHelper.GenerateJsonFileWithUsers(userCount: 3, fileName);

            var response = userService.UploadUsers(fileName: fileName);
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.Created), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.SequenceEqual(users), Is.True, "Users are NOT uploaded from file");
            });
        }

        [Test]
        [AllureIssue("Response status code is not 'Failed Dependency'")]
        public void UploadUsers_UserHasInvalidZipCode_ResponseStatusCodeIsFailedDependency_UsersAreNotUploaded()
        {
            var fileName = "usersHasInvalidZipCode.json";

            var userDtoWithInvalidZipCode = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = RandomHelper.CreateRandomString(),
                Sex = RandomHelper.GetRandomSex().ToString(),
                ZipCode = RandomHelper.CreateRandomInt().ToString()
            };

            var users = TestDataHelper.GenerateJsonFileWithUsersWithInvalidData(userCount: 3, userDtoWithInvalidZipCode, fileName);

            var response = userService.UploadUsers(fileName: fileName);
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.FailedDependency), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.SequenceEqual(users), Is.False, "Users are uploaded from file");
            });
        }

        // Bug title: Response status code is not 'Failed Dependency'
        // Steps to reproduce: create json file with at least one user with invalid Zip Code ->
        // call /users/upload service and check response status code
        // Expected result: Response status code is 'Failed Dependency'
        // Actual result: Response status code is 'Internal Server Error'

        [Test]
        [AllureIssue("Response status code is not 'Failed Dependency'")]
        public void UploadUsers_UserHasMissedRequiredField_ResponseStatusCodeIsConflict_UsersAreNotUploaded()
        {
            var fileName = "usersHasMissedRequiredField.json";

            var userDtoWithMissedRequiredField = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = RandomHelper.CreateRandomString(),
            };

            var users = TestDataHelper.GenerateJsonFileWithUsersWithInvalidData(userCount: 3, userDtoWithMissedRequiredField, fileName);

            var response = userService.UploadUsers(fileName: fileName);
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.Conflict), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.SequenceEqual(users), Is.False, "Users are uploaded from file");
            });
        }

        // Bug title: Response status code is not 'Failed Dependency'
        // Steps to reproduce: create json file with at least one user with missed required field ->
        // call /users/upload service and check response status code
        // Expected result: Response status code is 'Failed Dependency'
        // Actual result: Response status code is 'Internal Server Error'
    }
}
