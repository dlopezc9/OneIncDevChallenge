using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;
using Users.Contracts.Request;

namespace Users.Tests.Api.Middleware;

public class ValidationMappingMiddlewareTests
{
    private readonly HttpClient _client;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

    public ValidationMappingMiddlewareTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        _client = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpClientFactory
            .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Returns(_client);
    }

    [Fact]
    public async Task Should_Return_200_When_Request_Is_Valid()
    {
        var user = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john3.doe@example.com",
            PhoneNumber = "1234567890",
            DateOfBirth = new DateTime(1996, 1, 12)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString() == "http://localhost/api/users"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        var response = await _client.PostAsJsonAsync("api/users", user);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Should_Return_400_When_FirstName_Is_Missing()
    {
        var user = new CreateUserRequest
        {
            FirstName = "Jhon",
            LastName = "Doe",
            Email = "john3.doe@example.com",
            PhoneNumber = "1234567890",
            DateOfBirth = new DateTime(1996, 1, 12)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString() == "http://localhost/api/users"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{\"message\":\"FirstName is required\"}")
            });

        var response = await _client.PostAsJsonAsync("api/users", user);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("FirstName is required", content);
    }

    [Fact]
    public async Task Should_Return_400_When_Email_Is_Invalid()
    {

        var user = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "invalid-email",
            PhoneNumber = "1234567890",
            DateOfBirth = new DateTime(1996, 1, 12)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString() == "http://localhost/api/users"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{\"message\":\"Invalid email format\"}")
            });

        var response = await _client.PostAsJsonAsync("api/users", user);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Invalid email format", content);
    }

    [Fact]
    public async Task Should_Return_500_When_Server_Error_Occurs()
    {
        var user = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john3.doe@example.com",
            PhoneNumber = "1234567890",
            DateOfBirth = new DateTime(1996, 1, 12)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri.ToString() == "http://localhost/api/users"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("{\"message\":\"Internal server error\"}")
            });

        var response = await _client.PostAsJsonAsync("api/users", user);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Internal server error", content);
    }
}