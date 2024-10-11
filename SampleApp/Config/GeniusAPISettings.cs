namespace SampleApp.Config
{
    public class GeniusAPISettings
    {
        // This value refers to the appsettings.json block where these settings reside.
        public const String SettingsName = "GeniusAPISettings";

        public String FakeAPIKeyA { get; set; } = "";
        public String FakeAPIKeyB { get; set; } = "";
    }
}
