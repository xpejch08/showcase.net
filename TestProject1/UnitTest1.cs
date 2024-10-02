using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;

namespace TestProject1
{
    public class UserControllerTests
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _client = new HttpClient { BaseAddress = new System.Uri("http://localhost:5050") };
        }

        [Test]
        public async Task GetUser_Should_Return_User_When_User_Exists()
        {
            // Arrange: Send a valid user object
            var content = new StringContent(
                "{\"name\":\"admin\",\"password\":\"admin\"}", 
                System.Text.Encoding.UTF8, 
                "application/json");

            // Act: Make a POST request
            var response = await _client.PostAsync("/api/UserManipulation/GetUser", content);

            // Assert: Expect OK status code (200)
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }


        
        [TearDown]
        public void TearDown()
        {
            // Dispose of HttpClient to prevent resource leakage
            _client?.Dispose();
        }
    }
}