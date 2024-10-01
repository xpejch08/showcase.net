using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace showcase.tests;

public class Test
{
    public class UserManipulationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UserManipulationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }
    }
}   