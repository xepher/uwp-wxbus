using System;

namespace org.xepher.common
{
    public class Common
    {
        public static string GetViewState(string rawHtml)
        {
            // get VIEWSTATE
            string viewStateFlag = "id=\"__VIEWSTATE\" value=\"";
            int i = rawHtml.IndexOf(viewStateFlag) + viewStateFlag.Length;
            int j = rawHtml.IndexOf("\"", i);
            string viewState = rawHtml.Substring(i, j - i);

            return Uri.EscapeDataString(viewState);
        }
    }
}
