namespace Infrastructure;

public class ApiSettings
{
    public string BaseApiUrl { get; set; }
    public TokenEndpoints TokenEndpoints { get; set; }
}

public class TokenEndpoints
{
    public string GetToken { get; set; }
    public string GetRefreshToken { get; set; }
}
