using System.Net.Http.Headers;

internal class AuthorizationHeaderValue : AuthenticationHeaderValue
{
    public AuthorizationHeaderValue(string scheme, string? parameter) : base(scheme, parameter)
    {
    }
}