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
            AboutPrompt.VersionNumber = "Ver 1.2.5.45";
#if DEBUG
            AboutPrompt.VersionNumber+=" DEBUG";
#else
            AboutPrompt.VersionNumber+=" RELEASE";
#endif
            _items = new AboutPromptItem[9];
            _items[0] = new AboutPromptItem { AuthorName = AppResource.AuthorContent, EmailAddress = AppResource.Email, Role = AppResource.AuthorTitle };
            //_items[1] = new AboutPromptItem { WebSiteUrl = AppResource.AuthorWeiboContent, Role = AppResource.AuthorWeiboTitle };
            _items[1] = new AboutPromptItem { AuthorName = AppResource.DataSourceContent, Role = AppResource.DataSourceTitle };
            _items[2] = new AboutPromptItem { AuthorName = "1.添加短信查询", Role = AppResource.UpdateInformation };
            _items[3] = new AboutPromptItem { AuthorName = "2.添加站台查询" };
            _items[4] = new AboutPromptItem { AuthorName = "3.添加明暗两种主题选择" };
            _items[5] = new AboutPromptItem { AuthorName = "4.添加固定线路到开始屏幕" };
            _items[6] = new AboutPromptItem { AuthorName = "5.同步官方数据库" };
            _items[7] = new AboutPromptItem { AuthorName = "6.修正亮色主题显示问题" };
            _items[8] = new AboutPromptItem { AuthorName = "7.提升部分性能" };

            AboutPrompt.Footer = AppResource.Copyright;
        }

        public void Show()
        {
            AboutPrompt.Show(_items);
        }
    }
}
