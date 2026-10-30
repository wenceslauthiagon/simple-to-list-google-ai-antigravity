namespace TodoList.Api.Configuration;

public class CorsSettings
{
    public const string SectionName = "Cors";
    public const string PolicyName = "AppCorsPolicy";

    public string[] AllowedOrigins { get; set; } = [];
}
