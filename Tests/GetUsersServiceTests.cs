using Allure.NUnit.Attributes;
using NUnit.Framework;
using Rest.Core;
using Rest.Enums;
using Rest.Extensions;
using Rest.Helpers;
using Rest.Models;
using Rest.Services;
using System.Net;

namespace Rest.Tests
{
    [TestFixture]
    [AllureSubSuite("Get User Service")]
    public class GetUsersServiceTests : BaseTest
    {
        private readonly UserService userService = new();

        [Test]
        public void GetUsers_ResponseStatusCode_IsOK_UsersAreReturned()
        {
            var response = userService.GetUsers();
            var availableUsers = response.DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.OK), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(availableUsers!.Length > 0, "Available Users list is empty");
            });
        }

        [Test]
        public void GetUsers_OlderThanFilterApplied_FilteredUsersAreReturned()
        {
            var ageValue = RandomHelper.CreateRandomInt();
            var expectedUsers = PrepareExpectedUsersListFilteredByAge(ageValue, older: true);

            var response = userService.GetFilteredUsers(Filters.OlderThan, ageValue);
            var filteredUsers = response.DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.OK), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(filteredUsers!.SequenceEqual(expectedUsers), Is.True, "Filtered Users list does NOT match expected Users list");
            });
        }

        [Test]
        public void GetUsers_YoungerThanFilterApplied_FilteredUsersAreReturned()
        {
            var ageValue = RandomHelper.CreateRandomInt();
            var expectedUsers = PrepareExpectedUsersListFilteredByAge(ageValue, older: false);

            var response = userService.GetFilteredUsers(Filters.YoungerThan, ageValue);
            var filteredUsers = response.DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.OK), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(filteredUsers!.SequenceEqual(expectedUsers), Is.True, "Filtered Users list does NOT match expected Users list");
            });
        }

        [Test]
        public void GetUsers_FilteredByFemaleSex_FilteredUsersAreReturned()
        {
            var expectedUsers = PrepareExpectedUsersListFilteredBySex(Sex.FEMALE);

            var response = userService.GetFilteredUsers(Filters.Sex, Sex.FEMALE);
            var filteredUsers = response.DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.OK), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(filteredUsers!.SequenceEqual(expectedUsers), Is.True, "Filtered Users list does NOT match expected Users list");
            });
        }

        [Test]
        public void GetUsers_FilteredByMaleSex_FilteredUsersAreReturned()
        {
            var expectedUsers = PrepareExpectedUsersListFilteredBySex(Sex.MALE);

            var response = userService.GetFilteredUsers(Filters.Sex, Sex.MALE);
            var filteredUsers = response.DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.OK), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(filteredUsers!.SequenceEqual(expectedUsers), Is.True, "Filtered Users list does NOT match expected Users list");
            });
        }

        [Test]
        public void GetUsers_FilteredByAgeAndMaleSex_FilteredUsersAreReturned()
        {
            var ageValue = RandomHelper.CreateRandomInt();
            var expectedUsers = PrepareExpectedUsersListFilteredByAgeAndSex(ageValue, older: false, Sex.MALE);

            var response = userService.GetFilteredUsersByTwoFilters(Filters.YoungerThan, ageValue, Filters.Sex, Sex.MALE);
            var filteredUsers = response.DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.OK), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(filteredUsers!.SequenceEqual(expectedUsers), Is.True, "Filtered Users list does NOT match expected Users list");
            });
        }

        [Test]
        public void GetUsers_FilteredByAgeAndFemaleSex_FilteredUsersAreReturned()
        {
            var ageValue = RandomHelper.CreateRandomInt();
            var expectedUsers = PrepareExpectedUsersListFilteredByAgeAndSex(ageValue, older: true, Sex.FEMALE);

            var response = userService.GetFilteredUsersByTwoFilters(Filters.OlderThan, ageValue, Filters.Sex, Sex.FEMALE);
            var filteredUsers = response.DeserializeResponseContent<UserDto[]>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode.Equals(HttpStatusCode.OK), $"Response Status Code is '{response.StatusCode}'");
                Assert.That(filteredUsers!.SequenceEqual(expectedUsers), Is.True, "Filtered Users list does NOT match expected Users list");
            });
        }

        [Test]
        public void GetUsers_FilteredByOlderAndYoungerAge_ResponseStatusCode_IsConflict()
        {
            var response = userService.GetFilteredUsersByTwoFilters
                (Filters.OlderThan, RandomHelper.CreateRandomInt(), Filters.YoungerThan, RandomHelper.CreateRandomInt());

            Assert.That(response.StatusCode.Equals(HttpStatusCode.Conflict), $"Response Status Code is '{response.StatusCode}'");
        }

        [AllureStep("Prepare Expected Users List Filtered By Age")]
        private UserDto[] PrepareExpectedUsersListFilteredByAge(int ageValue, bool older)
        {
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();
            return [.. allUsers!.Where(user => older ? user.Age > ageValue : user.Age < ageValue)];
        }

        [AllureStep("Prepare Expected Users List Filtered By Sex")]
        private UserDto[] PrepareExpectedUsersListFilteredBySex(Sex sexValue)
        {
            var allUsers = userService.GetUsers().DeserializeResponseContent<UserDto[]>();
            return [.. allUsers!.Where(user => user.Sex == sexValue.ToString())];
        }

        [AllureStep("Prepare Expected Users List Filtered By Age And Sex")]
        private UserDto[] PrepareExpectedUsersListFilteredByAgeAndSex(int ageValue, bool older, Sex sexValue)
        {
            return [.. PrepareExpectedUsersListFilteredByAge(ageValue, older).Where(user => user.Sex == sexValue.ToString())];
        }
    }
}
