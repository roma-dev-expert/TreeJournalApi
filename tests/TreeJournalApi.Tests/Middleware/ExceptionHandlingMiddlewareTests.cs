using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using TreeJournalApi.Exceptions;
using TreeJournalApi.Middleware;
using TreeJournalApi.Tests.Common;

namespace TreeJournalApi.Tests.Middleware
{
    [Collection("CustomWebApplicationFactory collection")]
    public class ExceptionHandlingMiddlewareTests
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public ExceptionHandlingMiddlewareTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private async Task<HttpContext> InvokeMiddlewareWithException(Exception exception)
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.RequestServices = _factory.Services;

            RequestDelegate next = _ => throw exception;
            var middleware = new ExceptionHandlingMiddleware(next);

            await middleware.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            return context;
        }

        [Fact]
        public async Task ExceptionHandlingMiddleware_ReturnsCorrectResponse_ForSecureException()
        {
            var context = await InvokeMiddlewareWithException(new SecureException("Access Denied"));

            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);

            using var reader = new StreamReader(context.Response.Body);
            string responseBody = await reader.ReadToEndAsync();
            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            Assert.Equal("Secure", responseJson.GetProperty("type").GetString());
            Assert.Equal("Access Denied", responseJson.GetProperty("data").GetProperty("message").GetString());
        }

        [Fact]
        public async Task ExceptionHandlingMiddleware_ReturnsCorrectResponse_ForStandardException()
        {
            var context = await InvokeMiddlewareWithException(new Exception("Unexpected Error"));

            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);

            using var reader = new StreamReader(context.Response.Body);
            string responseBody = await reader.ReadToEndAsync();
            var responseJson = JsonDocument.Parse(responseBody).RootElement;

            Assert.Equal("Exception", responseJson.GetProperty("type").GetString());
            Assert.Contains("Internal server error ID =", responseJson.GetProperty("data").GetProperty("message").GetString());
        }
    }
}
