using NUnit.Framework;
using Rest.Extensions;
using Rest.Helpers;
using Rest.Models;
using Rest.Services;
using System.Net;

namespace Rest.Tests
{
    [TestFixture]
    public class UpdateUserServiceTests
    {
        private readonly UserService userService = new();
        private readonly ZipCodeService zipCodeService = new();

        [Test]
        public void UpdateUser_UserNewValuesAreCorrect_ResponseStatusCodeIsOK_UserIsUpdated()
        {
            var availableZipCodes = zipCodeService.GetZipCodes().DeserializeResponseContent<string[]>();

            var userNewValues = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = RandomHelper.CreateRandomString(),
                Sex = RandomHelper.GetRandomSex().ToString(),
                ZipCode = availableZipCodes![RandomHelper.CreateRandomInt() % availableZipCodes.Length]
            };

            var updateUserDto = CreateUpdateUserDto(userNewValues);
            var response = userService.UpdateUser(updateUserDto);
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.OK), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Equals(userNewValues)), Is.True, "User is NOT updated and new user values are NOT applied");
                Assert.That(allUsers!.Any(u => u.Equals(updateUserDto.UserToChange)), Is.False, "User with old values exists");
            });
        }

        [Test]
        public void UpdateUser_NewZipCodeIsIncorrect_ResponseStatusCodeIsFailedDependency_UserIsNotUpdated()
        {
            var userNewValues = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = RandomHelper.CreateRandomString(),
                Sex = RandomHelper.GetRandomSex().ToString(),
                ZipCode = RandomHelper.CreateRandomInt().ToString()
            };

            var updateUserDto = CreateUpdateUserDto(userNewValues);
            var response = userService.UpdateUser(updateUserDto);
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.FailedDependency), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Equals(userNewValues)), Is.False, "User is updated and new user values are applied");
                Assert.That(allUsers!.Any(u => u.Equals(updateUserDto.UserToChange)), Is.True, "User with old user values does NOT exist");
            });
        }

        // Bug title: Existing User is deleted when trying to update it with invalid new values
        // Steps to reproduce: create User model with invalid values -> call /users service to update User with invalid values
        // Expected result: Existing User is not updated and not deleted
        // Actual result: Existing User is deleted

        [Test]
        public void UpdateUser_RequiredFieldIsMissed_ResponseStatusCodeIsConflict_UserIsNotUpdated()
        {
            var availableZipCodes = zipCodeService.GetZipCodes().DeserializeResponseContent<string[]>();

            var userNewValues = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = RandomHelper.CreateRandomString(),
                ZipCode = availableZipCodes![RandomHelper.CreateRandomInt() % availableZipCodes.Length]
            };

            var updateUserDto = CreateUpdateUserDto(userNewValues);
            var response = userService.UpdateUser(updateUserDto);
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.Conflict), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Equals(userNewValues)), Is.False, "User is updated and new user values are applied");
                Assert.That(allUsers!.Any(u => u.Equals(updateUserDto.UserToChange)), Is.True, "User with old user values does NOT exist");
            });
        }

        // Bug title: Existing User is deleted when trying to update it with valid new value but missed required field
        // Steps to reproduce: create User model with valid value and miss required filed -> call /users service to update User with new value
        // Expected result: Existing User is not updated and not deleted
        // Actual result: Existing User is deleted

        [Test]
        public void UpdateUser_UserToChangeDoesNotExist_ResponseStatusCodeIsBadRequest()
        {
            var userNewValues = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = RandomHelper.CreateRandomString(),
                Sex = RandomHelper.GetRandomSex().ToString(),
            };

            var userToChange = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = RandomHelper.CreateRandomString(),
                Sex = RandomHelper.GetRandomSex().ToString(),
            };

            var updateUserDto = new UpdateUserDto()
            {
                UserNewValues = userNewValues,
                UserToChange = userToChange
            };

            var response = userService.UpdateUser(updateUserDto);
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.BadRequest), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Equals(userNewValues)), Is.False, "User is added");
            });
        }

        [Test]
        public void UpdateUser_UserToChangeIsNull_ResponseStatusCodeIsBadRequest()
        {
            var userNewValues = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = RandomHelper.CreateRandomString(),
                Sex = RandomHelper.GetRandomSex().ToString(),
            };

            var userToChange = null as UserDto;

            var updateUserDto = new UpdateUserDto()
            {
                UserNewValues = userNewValues,
                UserToChange = userToChange
            };

            var response = userService.UpdateUser(updateUserDto);
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.BadRequest), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Equals(userNewValues)), Is.False, "User is added");
            });
        }

        [Test]
        public void UpdateUser_UserNewValuesAreNull_ResponseStatusCodeIsBadRequest()
        {
            var userNewValues = null as UserDto;

            var updateUserDto = CreateUpdateUserDto(userNewValues);
            var response = userService.UpdateUser(updateUserDto);
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.BadRequest), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Equals(updateUserDto.UserToChange)), Is.True, "User with old user values does NOT exist");
            });
        }

        [Test]
        public void PartiallyUpdateUser_UserNewValueIsCorrect_ResponseStatusCodeIsOK_UserIsUpdated()
        {
            var userToChange = GetRandomUser();

            var userNewValues = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = userToChange.Name,
                Sex = userToChange.Sex
            };

            var updateUserDto = new UpdateUserDto()
            {
                UserNewValues = userNewValues,
                UserToChange = userToChange
            };

            var response = userService.PartiallyUpdateUser(updateUserDto);
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.OK), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Name == userToChange.Name && u.Age == userNewValues.Age), 
                    Is.True, "Existing User is NOT updated with new user value");
                Assert.That(allUsers!.Any(u => u.Name == userToChange.Name && u.ZipCode == userToChange.ZipCode),
                    Is.True, "Existing User is updated with default value for Zip Code and equals null");
            });
        }

        // Bug title: Existing User is updated with default value for Zip Code when trying to update it without Zip Code in new values
        // Steps to reproduce: create User model without Zip Code -> call /users service to update User with Age value
        // Expected result: Zip Code for Existing User is not updated
        // Actual result: Zip Code for Existing User is updated and equals null

        [Test]
        public void PartiallyUpdateUser_UserNewValueIsCorrect_RequiredFiledIsMissed_ResponseStatusCodeIsConflict_UserIsNotUpdated()
        {
            var userToChange = GetRandomUser();

            var userNewValues = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = userToChange.Name,
                ZipCode = userToChange.ZipCode
            };

            var updateUserDto = new UpdateUserDto()
            {
                UserNewValues = userNewValues,
                UserToChange = userToChange
            };

            var response = userService.PartiallyUpdateUser(updateUserDto);
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.Conflict), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Equals(userToChange)), Is.True, "Existing User is NOT updated with new user value");
            });
        }

        // Bug title: Existing User is deleted when trying to update it with valid new value but missed required field
        // Steps to reproduce: create User model with valid value and miss required filed -> call /users service to update User with new value
        // Expected result: Existing User is not updated and not deleted
        // Actual result: Existing User is deleted

        [Test]
        public void PartiallyUpdateUser_NewZipCodeIsIncorrect_ResponseStatusCodeIsFailedDependency_UserIsNotUpdated()
        {
            var userToChange = GetRandomUser();

            var userNewValues = new UserDto()
            {
                Age = userToChange.Age,
                Name = userToChange.Name,
                Sex = userToChange.Sex,
                ZipCode = RandomHelper.CreateRandomInt().ToString()
            };

            var updateUserDto = new UpdateUserDto()
            {
                UserNewValues = userNewValues,
                UserToChange = userToChange
            };

            var response = userService.PartiallyUpdateUser(updateUserDto);
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.FailedDependency), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Equals(userToChange)), Is.True, "Existing User is NOT updated with new user value");
            });
        }

        // Bug title: Existing User is deleted when trying to update it with invalid new value
        // Steps to reproduce: create User model with invalid new value -> call /users service to update User with new value
        // Expected result: Existing User is not updated and not deleted
        // Actual result: Existing User is deleted

        private UpdateUserDto CreateUpdateUserDto(UserDto userNewValues)
        {
            var updateUserDto = new UpdateUserDto()
            {
                UserNewValues = userNewValues,
                UserToChange = GetRandomUser()
            };

            return updateUserDto;
        }

        private UserDto GetRandomUser()
        {
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();
            return allUsers![RandomHelper.CreateRandomInt() % allUsers.Length];
        }
    }
}
