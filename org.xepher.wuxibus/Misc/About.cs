using Coding4Fun.Toolkit.Controls;
using org.xepher.lang;

namespace org.xepher.wuxibus.misc
{
    public class About
    {
        private AboutPromptItem[] _items;
        public AboutPrompt AboutPrompt;

        public About()
        {
            AboutPrompt = new AboutPrompt();
            AboutPrompt.Title = AppResource.ApplicationTitle;
            AboutPrompt.VersionNumber = "Ver 1.2.5.41";
#if DEBUG
            AboutPrompt.VersionNumber+=" DEBUG";
#else
            AboutPrompt.VersionNumber+=" RELEASE";
#endif
            _items = new AboutPromptItem[4];
            _items[0] = new AboutPromptItem { AuthorName = AppResource.AuthorContent, EmailAddress = AppResource.Email, Role = AppResource.AuthorTitle };
            _items[1] = new AboutPromptItem { AuthorName = AppResource.DataSourceContent, Role = AppResource.DataSourceTitle };
            _items[2] = new AboutPromptItem { AuthorName = "1.添加短信查询", Role = AppResource.UpdateInformation };
            _items[3] = new AboutPromptItem { AuthorName = "2.修正亮色主题显示问题" };
            _items[4] = new AboutPromptItem { AuthorName = "3.ListBox性能提升" };

            AboutPrompt.Footer = AppResource.Copyright;
        }

        public void Show()
        {
            AboutPrompt.Show(_items);
        }
    }
}
