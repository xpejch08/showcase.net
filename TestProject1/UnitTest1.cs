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
        
        /// <summary>
        /// Get User test
        /// </summary>
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
        
        
        /// <summary>
        /// user login if credentials missing
        /// </summary>
        [Test]
        public async Task Login_Should_Return_BadRequest_When_Missing_Inputs()
        {
            // Arrange
            var content = new StringContent(
                "{\"name\":\"\",\"password\":\"\"}",
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        /// <summary>
        /// User doesn't exist test
        /// </summary>
        [Test]
        public async Task Login_Should_Return_BadRequest_When_User_Not_Found()
        {
            // Arrange
            var content = new StringContent(
                "{\"name\":\"nonExistentUser\",\"password\":\"wrongPassword\"}",
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        /// <summary>
        /// admin logging in test
        /// </summary>
        [Test]
        public async Task Login_Should_Redirect_When_Admin_User_Logs_In()
        {
            // Arrange
            var content = new StringContent(
                "{\"name\":\"admin\",\"password\":\"admin\"}",
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent.Should().Contain("\"redirectUrl\":\"/views/admin/adminview.html\"");
        }

        [Test]
        public async Task Logout_should_Redirect_when_user_logs_out()
        {
            //first log user in
            var content = new StringContent(
                "{\"name\":\"admin\",\"password\":\"admin\"}",
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var response = await _client.PostAsync("/api/auth/login", content);
            
            //then logout
            
            content = new StringContent(
                "",
                System.Text.Encoding.UTF8,
                "application/json"
            );
            var logoutResponse = await _client.PostAsync("/api/auth/logout",content);
            
            logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            
        }

        [Test]
        public async Task Non_Authenticated_User_cannot_access_adminView()
        {
            var response = await _client.GetAsync("/views/admin/adminview.html");
            
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
        
        //test for adding a user to the database
        [Test]
        public async Task Add_User_To_Db_Test()
        {
            // Arrange: Send a valid user object
            var content = new StringContent(
                "{\"name\":\"newuser\",\"password\":\"newuser\"}", 
                System.Text.Encoding.UTF8, 
                "application/json");
            
            var response = await _client.PostAsync("/api/UserManipulation/DeleteUser", content);
            // Act: Make a POST request
            response = await _client.PostAsync("/api/UserManipulation/AddUser", content);

            // Assert: Expect OK status code (200)
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Delete_User_From_Db()
        {
            
            // create new user
            var content = new StringContent(
                "{\"name\":\"newuserTest\",\"password\":\"newuser\"}", 
                System.Text.Encoding.UTF8, 
                "application/json");

            // Act: Make a POST request
            var response = await _client.PostAsync("/api/UserManipulation/AddUser", content);
            
            response = await _client.PostAsync("/api/UserManipulation/DeleteUser", content);
            
            
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