using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using org.xepher.lang;

namespace org.xepher.wuxibus
{
    public partial class AboutPage: PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

            btnMarketPlace.Content = AppResource.AboutBtnMarketPlace;
            btnDonate.Content = AppResource.AboutBtnDonate;
        }

        private void btnMarketPlace_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            new MarketplaceReviewTask().Show();
        }
    }
}