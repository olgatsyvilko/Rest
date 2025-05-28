using Newtonsoft.Json;
using NUnit.Framework;
using Rest.Helpers;
using Rest.Models;
using Rest.Services;
using System.Net;

namespace Rest.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private readonly UserService userService = new();
        private readonly ZipCodeService zipCodeService = new();

        [Test]
        public void CreateUser_AllFieldsAreFilledIn_ResponseStatusCodeIsCreated_UserIsAdded_UsedZipCodeIsRemoved()
        {
            var availableZipCodes = JsonConvert.DeserializeObject<string[]>(zipCodeService.GetZipCodes().Content!);

            var userDto = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = RandomHelper.CreateRandomString(),
                Sex = RandomHelper.GetRandomSex().ToString(),
                ZipCode = availableZipCodes![RandomHelper.CreateRandomInt() % availableZipCodes.Length]
            };

            var response = userService.CreateUser(userDto);
            var allUsers = JsonConvert.DeserializeObject<UserDto[]>(userService.GetUsers().Content!);
            availableZipCodes = JsonConvert.DeserializeObject<string[]>(zipCodeService.GetZipCodes().Content!);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.Created), $"Response Status Code is '{response.StatusCode}'");
                
                Assert.That(allUsers!.Any(u => u.Name.Equals(userDto.Name) 
                && u.Sex.Equals(userDto.Sex) 
                && u.Age.Equals(userDto.Age) 
                && u.ZipCode.Equals(userDto.ZipCode)), Is.True, "User is NOT created");
                
                Assert.That(availableZipCodes!.Contains(userDto.ZipCode), Is.False, 
                    "Available Zip Codes list contains Zip Code used for User creation");
            });
        }

        [Test]
        public void CreateUser_RequiredFieldsAreFilledIn_ResponseStatusCodeIsCreated_UserIsAdded()
        {
            var userDto = new UserDto()
            {
                Name = RandomHelper.CreateRandomString(),
                Sex = RandomHelper.GetRandomSex().ToString(),
            };

            var response = userService.CreateUser(userDto);
            var allUsers = JsonConvert.DeserializeObject<UserDto[]>(userService.GetUsers().Content!);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.Created), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Name.Equals(userDto.Name) && u.Sex.Equals(userDto.Sex)), Is.True, "User is NOT created");
            });
        }

        [Test]
        public void CreateUser_ZipCodeIsIncorrect_ResponseStatusCodeIsFailedDependency_UserIsNotAdded()
        {
            var userDto = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = RandomHelper.CreateRandomString(),
                Sex = RandomHelper.GetRandomSex().ToString(),
                ZipCode = RandomHelper.CreateRandomInt().ToString()
            };

            var response = userService.CreateUser(userDto);
            var allUsers = JsonConvert.DeserializeObject<UserDto[]>(userService.GetUsers().Content!);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.FailedDependency), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Name.Equals(userDto.Name)), Is.False, "User is created");
            });
        }

        [Test]
        public void CreateUser_NameAndSexExist_ResponseStatusCodeIsBadRequest_UserIsNotAdded()
        {
            var availableUsers = JsonConvert.DeserializeObject<UserDto[]>(userService.GetUsers().Content!);

            var userDto = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = availableUsers![0].Name,
                Sex = availableUsers![0].Sex,
            };

            var response = userService.CreateUser(userDto);
            var allUsers = JsonConvert.DeserializeObject<UserDto[]>(userService.GetUsers().Content!);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.BadRequest), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Name.Equals(userDto.Name) && u.Sex.Equals(userDto.Sex) && u.Age.Equals(userDto.Age)),
                    Is.False, "User is created");
            });
        }

        // Bug title: User with the same Name and Sex as existing user is added
        // Steps to reproduce: create User model with existing Name + Sex -> call /users service
        // Expected result: Response status code is 'Bad Request', User is not added
        // Actual result: Response status code is 'Created', User is added

        [Test]
        public void CreateUser_SexFieldIsMissed_ResponseStatusCodeIsConflict_UserIsNotAdded()
        {
            var userDto = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Name = RandomHelper.CreateRandomString(),
            };

            var response = userService.CreateUser(userDto);
            var allUsers = JsonConvert.DeserializeObject<UserDto[]>(userService.GetUsers().Content!);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.Conflict), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Name.Equals(userDto.Name) && u.Age.Equals(userDto.Age)), Is.False, "User is created");
            });
        }

        [Test]
        public void CreateUser_NameFieldIsMissed_ResponseStatusCodeIsConflict_UserIsNotAdded()
        {
            var userDto = new UserDto()
            {
                Age = RandomHelper.CreateRandomInt(),
                Sex = RandomHelper.GetRandomSex().ToString(),
            };

            var response = userService.CreateUser(userDto);
            var allUsers = JsonConvert.DeserializeObject<UserDto[]>(userService.GetUsers().Content!);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.Conflict), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(allUsers!.Any(u => u.Age.Equals(userDto.Age) && u.Sex.Equals(userDto.Sex)), Is.False, "User is created");
            });
        }
    }
}
