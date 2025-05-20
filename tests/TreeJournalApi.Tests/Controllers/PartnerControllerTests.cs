using System.Net;
using TreeJournalApi.Tests.Common;

namespace TreeJournalApi.Tests.Controllers
{
    [Collection("CustomWebApplicationFactory collection")]
    public class PartnerControllerTests : DatabaseTestBase
    {
        private readonly HttpClient _client;

        public PartnerControllerTests(CustomWebApplicationFactory<Program> factory)
            : base(factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RememberMe_ReturnsCorrectMessage()
        {
            var response = await _client.PostAsync("/api.user.partner/rememberMe?code=partner123", null);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Contains("Partner code received: partner123", responseBody);
        }

        [Fact]
        public async Task RememberMe_ReturnsBadRequest_WhenCodeIsMissing()
        {
            var response = await _client.PostAsync("/api.user.partner/rememberMe", null);
            Assert.Equal((int)HttpStatusCode.BadRequest, (int)response.StatusCode);
        }
    }
}
