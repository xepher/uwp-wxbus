namespace org.xepher.lang
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static AppResource _localizedResources = new AppResource();

        public AppResource LocalizedResources { get { return _localizedResources; } }
    }
}
