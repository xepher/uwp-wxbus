using org.xepher.wuxibus.Localization.Lang;

namespace org.xepher.wuxibus.Localization
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
