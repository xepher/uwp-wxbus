using System;
using System.Windows;
using System.Windows.Input;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.wuxibus
{
    public partial class SearchPage : PhoneApplicationPage
    {
        private App _app;

        public SearchPage()
        {
            InitializeComponent();

            InitializeUIComponent();
        }

        private void InitializeUIComponent()
        {
            _app = (Application.Current as App);

            ACBSearch.DataContext = _app.Lines;
            ACBSearch.ItemsSource = _app.Lines;
            ACBSearch.ValueMemberPath = "line_name";
            ACBSearch.ItemFilter += SearchLine;
            ACBSearch.KeyUp += ACBSearch_KeyUp;
        }

        private bool SearchLine(string search, object item)
        {
            Line _line = item as Line;
            return _line != null && _line.line_name.Contains(search);
        }

        private void ACBSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Line _line = ACBSearch.SelectedItem as Line;
                if (_line == null)
                {
                    ToastPrompt toast = new ToastPrompt();
                    toast.Message = AppResource.MsgNoLine;
                    toast.Show();
                    ACBSearch.Text = string.Empty;
                    return;
                }
                _app.SelectedLine = _line;
                NavigationService.Navigate(new Uri("/StationPage.xaml", UriKind.Relative));
            }
        }
    }
}