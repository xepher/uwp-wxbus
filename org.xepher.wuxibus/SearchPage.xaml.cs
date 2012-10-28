using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using org.xepher.model;

namespace org.xepher.wuxibus
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public List<Route> Routes { get; set; }

        public SearchPage()
        {
            InitializeComponent();

            Routes = (Application.Current as App).Routes;
            List<string> source = new List<string>();
            Routes.ForEach(r => source.Add(r.Name));
            ACBSearch.DataContext = Routes;
            ACBSearch.ItemsSource = Routes;
            ACBSearch.ValueMemberPath = "Name";
            ACBSearch.ItemFilter += SearchRoutes;
        }

        // 模糊搜索
        private bool SearchRoutes(string search, object item)
        {
            if (item != null)
            {
                if ((item as Route).Name.ToString().ToLower().IndexOf(search) >= 0)
                    return true;
            }
            return false;
        }

        private void ACBSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Serach_Click(sender, e);
            }
        }

        private void Serach_Click(object sender, EventArgs e)
        {
            Route route = ACBSearch.SelectedItem as Route;
            if(route==null)
            {
                foreach (Route r in Routes)
                {
                    if (r.Name.Contains(ACBSearch.Text))
                    {
                        ACBSearch.SelectedItem = r;
                        return;
                    }
                }
            }
            (Application.Current as App).SelectedRoute = route;
            NavigationService.Navigate(new Uri("/StationsPage.xaml", UriKind.Relative));
        }
    }
}