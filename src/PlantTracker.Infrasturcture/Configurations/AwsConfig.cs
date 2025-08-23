namespace PlantTracker.Infrastructure.Configurations
{
    public class AwsConfig
    {
        public string Region { get; set; } = string.Empty;
        public string ServiceUrl { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
    }
}