using System.Text.Json.Serialization;

namespace Notlify.Core00.Models.Amadeus;

public class AuthenticationResponse
{
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("username")] public string Username { get; set; }

    [JsonPropertyName("application_name")] public string ApplicationName { get; set; }

    [JsonPropertyName("client_id")] public string ClientId { get; set; }

    [JsonPropertyName("token_type")] public string TokenType { get; set; }

    [JsonPropertyName("access_token")] public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; } // Time in seconds

    [JsonPropertyName("state")] public string State { get; set; }

    [JsonPropertyName("scope")] public string Scope { get; set; }
}