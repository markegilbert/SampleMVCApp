namespace SampleApp.Config
{
    public class SampleAppConfig
    {
        public String FakeAPIKeyA { get; set; } = "";
        public String FakeAPIKeyB { get; set; } = "";

        public SampleAppConnectionStrings? ConnectionStrings { get; set; }


        public SampleAppConfig()
        {
            ConnectionStrings = new SampleAppConnectionStrings();
        }
    }
}
