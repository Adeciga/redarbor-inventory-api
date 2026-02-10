namespace Inventory.Api.Contracts;
public sealed record AuthTokenResponse(string AccessToken, int ExpiresInSeconds);
