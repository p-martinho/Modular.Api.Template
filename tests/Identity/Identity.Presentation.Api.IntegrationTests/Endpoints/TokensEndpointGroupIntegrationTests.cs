using System.Net;
using System.Net.Http.Json;
using Identity.Presentation.Api.Dtos.V1.Users;
using Identity.Presentation.Api.Dtos.V1.Users.Create;
using Identity.Presentation.Api.IntegrationTests.Fixtures;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Presentation.Api.IntegrationTests.Endpoints;

public class TokensEndpointGroupIntegrationTests : BaseIntegrationTest
{
    private const string UsersPath = "api/users";
    private const string TokensPath = "connect/token";
    private const string TestClientId = "test_client";
    private const string TestClientSecret = "test_secret";
    private const string IdentityScope = "identity_server";

    public TokensEndpointGroupIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ExchangeAsync_WhenPasswordGrant_ShouldSucceed()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@example.com";
        const string password = "1SuperPassword.";
        await CreateUserAsync(email, password);
        var formContent = GetFormContentForPasswordGrant(email, password);

        // Act
        var response = await Client.PostAsync(TokensPath, formContent, TestContext.Current.CancellationToken);


        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tokenResponse =
            await response.Content.ReadFromJsonAsync<OpenIddictResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(tokenResponse);
        Assert.NotNull(tokenResponse.AccessToken);
        Assert.NotNull(tokenResponse.RefreshToken);
    }

    [Fact]
    public async Task ExchangeAsync_WhenPasswordGrant_AndInvalidCredentials_ShouldReturnBadRequest()
    {
        // Arrange
        var formContent = GetFormContentForPasswordGrant("someInvalidEmail", "someInvalidPassword");

        // Act
        var response = await Client.PostAsync(TokensPath, formContent, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ExchangeAsync_WhenRefreshTokenGrant_ShouldSucceed()
    {
        // Arrange
        var email = $"{Guid.NewGuid()}@example.com";
        const string password = "1SuperPassword.";
        await CreateUserAsync(email, password);
        var refreshToken = await GetUserRefreshTokenAsync(email, password);
        var formContent = GetFormContentForRefreshTokenGrant(refreshToken);

        // Act
        var response = await Client.PostAsync(TokensPath, formContent, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tokenResponse =
            await response.Content.ReadFromJsonAsync<OpenIddictResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(tokenResponse);
        Assert.NotNull(tokenResponse.AccessToken);
        Assert.NotNull(tokenResponse.RefreshToken);
    }

    [Fact]
    public async Task ExchangeAsync_WhenRefreshTokenGrant_AndInvalidToken_ShouldReturnBadRequest()
    {
        // Arrange
        var formContent = GetFormContentForRefreshTokenGrant("someInvalidRefreshToken");

        // Act
        var response = await Client.PostAsync(TokensPath, formContent, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task CreateUserAsync(string email, string password)
    {
        var request = new CreateUserApiDto { Email = email, Password = password };

        var response = await Client.PostAsJsonAsync(UsersPath, request, TestContext.Current.CancellationToken);

        var userResponse = await response.Content.ReadFromJsonAsync<UserInfoApiDto>(TestContext.Current.CancellationToken);

        if (userResponse is null)
        {
            throw new InvalidOperationException("Error creating the user");
        }
    }

    private static FormUrlEncodedContent GetFormContentForPasswordGrant(string email, string password)
    {
        return new FormUrlEncodedContent([
            new KeyValuePair<string, string>(Parameters.GrantType, GrantTypes.Password),
            new KeyValuePair<string, string>(Parameters.ClientId, TestClientId),
            new KeyValuePair<string, string>(Parameters.ClientSecret, TestClientSecret),
            new KeyValuePair<string, string>(Parameters.Username, email),
            new KeyValuePair<string, string>(Parameters.Password, password),
            new KeyValuePair<string, string>(Parameters.Scope, $"{Scopes.OfflineAccess} {IdentityScope}")
        ]);
    }

    private async Task<string> GetUserRefreshTokenAsync(string email, string password)
    {
        var formContent = GetFormContentForPasswordGrant(email, password);

        var response = await Client.PostAsync(TokensPath, formContent, TestContext.Current.CancellationToken);

        var tokenResponse =
            await response.Content.ReadFromJsonAsync<OpenIddictResponse>(TestContext.Current.CancellationToken);

        if (tokenResponse?.RefreshToken is null)
        {
            throw new InvalidOperationException("Error getting access token");
        }

        return tokenResponse.RefreshToken;
    }

    private static FormUrlEncodedContent GetFormContentForRefreshTokenGrant(string refreshToken)
    {
        return new FormUrlEncodedContent([
            new KeyValuePair<string, string>(Parameters.GrantType, GrantTypes.RefreshToken),
            new KeyValuePair<string, string>(Parameters.ClientId, TestClientId),
            new KeyValuePair<string, string>(Parameters.ClientSecret, TestClientSecret),
            new KeyValuePair<string, string>(Parameters.RefreshToken, refreshToken)
        ]);
    }
}