namespace SampleApp.Config
{
    public class GeniusAPISettings
    {
        // This value refers to the appsettings.json block where these settings reside.
        public const String SettingsName = "GeniusAPISettings";

        private String _ClientAccessToken = "";
        public String ClientAccessToken 
        { 
            get { return this._ClientAccessToken; }
            set 
            {
                this._ClientAccessToken = (value ?? "").Trim(); 
            }
        }
    }
}
