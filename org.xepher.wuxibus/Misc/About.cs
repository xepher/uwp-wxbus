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
            //_about.Opacity = 0.9;
            AboutPrompt.Title = AppResource.ApplicationTitle;
            AboutPrompt.VersionNumber = "Ver 1.2.4.39";
            _items = new AboutPromptItem[8];
            _items[0] = new AboutPromptItem { AuthorName = AppResource.AuthorContent, EmailAddress = AppResource.Email, Role = AppResource.AuthorTitle };
            _items[1] = new AboutPromptItem { AuthorName = AppResource.DataSourceContent, Role = AppResource.DataSourceTitle };
            _items[2] = new AboutPromptItem { AuthorName = "1.添加查询附近站台", Role = AppResource.UpdateInformation };
            _items[3] = new AboutPromptItem { AuthorName = "2.添加公交信息显示" };
            _items[4] = new AboutPromptItem { AuthorName = "3.添加数据库更新" };
            _items[5] = new AboutPromptItem { AuthorName = "4.美化界面" };
            _items[6] = new AboutPromptItem { AuthorName = "5.修正一处闪退" };
            _items[7] = new AboutPromptItem { AuthorName = "6.修正一处显示错误" };

            AboutPrompt.Footer = AppResource.Copyright;
        }

        public void Show()
        {
            AboutPrompt.Show(_items);
        }
    }
}
