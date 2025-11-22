using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Identity.Presentation.Api.Dtos.V1.Users;
using Identity.Presentation.Api.Dtos.V1.Users.Create;
using Identity.Presentation.Api.Dtos.V1.Users.Update;
using Identity.Presentation.Api.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Presentation.Api.IntegrationTests.Endpoints;

public class UsersEndpointGroupIntegrationTests : BaseIntegrationTests
{
    private const string UsersPath = "api/users";
    private const string UsersInfoPath = $"{UsersPath}/info";
    private const string UsersPasswordPath = $"{UsersPath}/password";
    private const string TokensPath = "connect/token";

    public UsersEndpointGroupIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateUserAsync_ShouldSucceed()
    {
        // Arrange
        var request = new CreateUserApiDto
        {
            Email = "test@example.com",
            Password = "1SuperPassword.",
            FirstName = "FirstName",
            LastName = "LastName"
        };

        // Act
        var response = await Client.PostAsJsonAsync(UsersPath, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var userResponse = await response.Content.ReadFromJsonAsync<UserInfoApiDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(userResponse);
        Assert.Equal(request.Email, userResponse.Email);
        Assert.Equal(request.FirstName, userResponse.FirstName);
        Assert.Equal(request.LastName, userResponse.LastName);
        var userEntity = await UserManager.FindByEmailAsync(userResponse.Email);
        Assert.NotNull(userEntity);
        Assert.Equal(request.Email, userEntity.Email);
        Assert.Equal(request.FirstName, userEntity.Name.FirstName);
        Assert.Equal(request.LastName, userEntity.Name.LastName);
    }

    [Fact]
    public async Task CreateUserAsync_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateUserApiDto { Email = "invalidEmail", Password = null! };

        // Act
        var response = await Client.PostAsJsonAsync(UsersPath, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
    }

    [Fact]
    public async Task GetUserInfoAsync_ShouldSucceed()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@example.com";
        const string password = "1SuperPassword.";
        var user = await CreateUserAsync(email, password);
        var accessToken = await AuthenticateUserAsync(email, password);
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, UsersInfoPath);
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(Schemes.Bearer, accessToken);

        // Act
        var response = await Client.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var userResponse = await response.Content.ReadFromJsonAsync<UserInfoApiDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(userResponse);
        Assert.Equal(user.Email, userResponse.Email);
        Assert.Equal(user.FirstName, userResponse.FirstName);
        Assert.Equal(user.LastName, userResponse.LastName);
    }

    [Fact]
    public async Task GetUserInfoAsync_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, UsersInfoPath);
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(Schemes.Bearer, "someInvalidToken");

        // Act
        var response = await Client.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUserInfoAsync_ShouldSucceed()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@example.com";
        const string password = "1SuperPassword.";
        await CreateUserAsync(email, password);
        var accessToken = await AuthenticateUserAsync(email, password);
        var request = new UpdateUserInfoApiDto
        {
            Email = $"{Guid.NewGuid()}@example.com",
            FirstName = "NewFirstName",
            LastName = "NewLastName"
        };
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, UsersInfoPath);
        httpRequestMessage.Content = JsonContent.Create(request);
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(Schemes.Bearer, accessToken);

        // Act
        var response = await Client.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var userResponse = await response.Content.ReadFromJsonAsync<UserInfoApiDto>(TestContext.Current.CancellationToken);
        Assert.NotNull(userResponse);
        Assert.Equal(request.Email, userResponse.Email);
        Assert.Equal(request.FirstName, userResponse.FirstName);
        Assert.Equal(request.LastName, userResponse.LastName);
    }

    [Fact]
    public async Task UpdateUserInfoAsync_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@example.com";
        const string password = "1SuperPassword.";
        await CreateUserAsync(email, password);
        var accessToken = await AuthenticateUserAsync(email, password);
        var request = new UpdateUserInfoApiDto { Email = "invalidEmail" };
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, UsersInfoPath);
        httpRequestMessage.Content = JsonContent.Create(request);
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(Schemes.Bearer, accessToken);

        // Act
        var response = await Client.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
    }

    [Fact]
    public async Task UpdateUserInfoAsync_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new UpdateUserInfoApiDto();
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, UsersInfoPath);
        httpRequestMessage.Content = JsonContent.Create(request);
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(Schemes.Bearer, "someInvalidToken");

        // Act
        var response = await Client.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_ShouldSucceed()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@example.com";
        const string password = "1SuperPassword.";
        await CreateUserAsync(email, password);
        var accessToken = await AuthenticateUserAsync(email, password);
        var request = new UpdateUserPasswordApiDto
        {
            OldPassword = password,
            NewPassword = password + "new"
        };
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, UsersPasswordPath);
        httpRequestMessage.Content = JsonContent.Create(request);
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(Schemes.Bearer, accessToken);

        // Act
        var response = await Client.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var newAccessToken = await AuthenticateUserAsync(email, request.NewPassword);
        Assert.NotNull(newAccessToken);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_WhenInvalidRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@example.com";
        const string password = "1SuperPassword.";
        await CreateUserAsync(email, password);
        var accessToken = await AuthenticateUserAsync(email, password);
        var request = new UpdateUserPasswordApiDto
        {
            OldPassword = null!,
            NewPassword = null!
        };
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, UsersPasswordPath);
        httpRequestMessage.Content = JsonContent.Create(request);
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(Schemes.Bearer, accessToken);

        // Act
        var response = await Client.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problemDetails =
            await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)HttpStatusCode.BadRequest, problemDetails.Status);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_WhenNotAuthenticated_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new UpdateUserPasswordApiDto
        {
            OldPassword = "OldPassword",
            NewPassword = "NewPassword"
        };
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, UsersPasswordPath);
        httpRequestMessage.Content = JsonContent.Create(request);
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(Schemes.Bearer, "someInvalidToken");

        // Act
        var response = await Client.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<UserInfoApiDto> CreateUserAsync(string email, string password)
    {
        var request = new CreateUserApiDto { Email = email, Password = password };

        var response = await Client.PostAsJsonAsync(UsersPath, request, TestContext.Current.CancellationToken);

        var userResponse = await response.Content.ReadFromJsonAsync<UserInfoApiDto>(TestContext.Current.CancellationToken);

        if (userResponse is null)
        {
            throw new InvalidOperationException("Error creating the user");
        }

        return userResponse;
    }

    private async Task<string> AuthenticateUserAsync(string email, string password)
    {
        var formContent = new FormUrlEncodedContent([
            new KeyValuePair<string, string>(Parameters.GrantType, GrantTypes.Password),
            new KeyValuePair<string, string>(Parameters.ClientId, "test_client"),
            new KeyValuePair<string, string>(Parameters.ClientSecret, "test_secret"),
            new KeyValuePair<string, string>(Parameters.Username, email),
            new KeyValuePair<string, string>(Parameters.Password, password),
            new KeyValuePair<string, string>(Parameters.Scope, "identity_server")
        ]);

        var response = await Client.PostAsync(TokensPath, formContent, TestContext.Current.CancellationToken);

        var tokenResponse =
            await response.Content.ReadFromJsonAsync<OpenIddictResponse>(TestContext.Current.CancellationToken);

        if (tokenResponse?.AccessToken is null)
        {
            throw new InvalidOperationException("Error getting access token");
        }

        return tokenResponse.AccessToken;
    }
}