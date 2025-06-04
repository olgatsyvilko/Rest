using NUnit.Framework;
using Rest.Extensions;
using Rest.Helpers;
using Rest.Services;
using System.Net;

namespace Rest.Tests
{
    [TestFixture]
    public class ZipCodeServiceTests
    {
        private readonly ZipCodeService zipCodeService = new();

        [Test]
        public void GetZipCodes_ResponseStatusCode_IsOK_ZipCodesAreReturned()
        {
            var response = zipCodeService.GetZipCodes();
            var availableZipCodes = response.DeserializeResponseContent<string[]>();

            Assert.That(response.StatusCode.Equals(HttpStatusCode.OK), $"Response Status Code is '{response.StatusCode}'");
            Assert.That(availableZipCodes!.Length > 0, "Available Zip Codes list is empty");
        }

        // Bug title: Response status code is not 'OK'
        // Steps to reproduce: call /zip-codes service and check response status code
        // Expected result: Response status code is 'OK'
        // Actual result: Response status code is 'Created'

        [Test]
        public void CallExpandZipCodes_ResponseStatusCode_IsCreated()
        {
            var newZipCodes = RandomHelper.CreateRandomArray(3);

            var response = zipCodeService.AddZipCodes(newZipCodes);

            Assert.That(response.StatusCode.Equals(HttpStatusCode.Created), $"Response Status Code is '{response.StatusCode}'");
        }

        [Test]
        public void ExpandZipCodes_NewZipCodes_AreAddedToAvailable()
        {
            var newZipCodes = RandomHelper.CreateRandomArray(3);

            var zipCodesFromResponse = zipCodeService.AddZipCodes(newZipCodes).DeserializeResponseContent<string[]>();
            var availableZipCodes = zipCodeService.GetZipCodes().DeserializeResponseContent<string[]>();

            Assert.That(availableZipCodes!.SequenceEqual(zipCodesFromResponse!), Is.True, "Available Zip Codes list does not contain new codes");
        }

        [Test]
        public void CallExpandZipCodes_RequestBodyIsNull_ResponseStatusCodeIsCorrect()
        {
            var response = zipCodeService.AddZipCodes(null);

            Assert.That(response.StatusCode.Equals(HttpStatusCode.UnsupportedMediaType), $"Response Status Code is '{response.StatusCode}'");
        }

        [Test]
        public void ExpandZipCodes_NewZipCodesHaveDublicates_DublicatesAreNotAdded()
        {
            // Create random array with dublicates
            var newZipCodes = RandomHelper.CreateRandomArray(3);
            var newZipCodesWithDublicates = newZipCodes.Concat(newZipCodes.Clone() as string[]).ToArray();

            // Get available zip codes list before adding new zip codes
            var availableZipCodes = zipCodeService.GetZipCodes().DeserializeResponseContent<string[]>();

            // Prepare expected available zip codes list without dublicates
            var expectedAvailableZipCodes = availableZipCodes!.Concat(newZipCodes).Distinct();

            // Add new zip codes with dublicates
            zipCodeService.AddZipCodes(newZipCodesWithDublicates);

            availableZipCodes = zipCodeService.GetZipCodes().DeserializeResponseContent<string[]>();

            Assert.That(availableZipCodes!.SequenceEqual(expectedAvailableZipCodes), Is.True, "Available Zip Codes list does not contain dublicates");
        }

        // Bug title: Available Zip Codes list contains duplicates
        // Steps to reproduce: create request body with dublicates -> call /zip-codes/expand service -> call /zip-codes service and check response content
        // Expected result: Response content does not contain duplicates
        // Actual result: Response content contains duplicates
    }
}
