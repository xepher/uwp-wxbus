using System;
using System.IO;

namespace org.xepher.wuxibus.misc
{
    internal static class ReleaseResource
    {
        internal static Stream GetResource(string UriString)
        {
            Stream stream = App.GetResourceStream(new Uri(UriString, UriKind.Relative)).Stream;

            return stream;
        }
    }
}
