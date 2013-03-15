using System;
using System.Windows;
using System.Windows.Controls;
using mogo_WP7_sdk;
using mogo_WP7_sdk.Common.EventArgs;

namespace org.xepher.wuxibus.misc
{
    internal class AdHelper
    {
        private const string AD_UNIT_ID = "4ebaee716906425db9fd0afd16f9f008";
        private AdsMogoView _adsmogo;
        private Control _container;

        internal void InitializeAds(Panel panel, Control container)
        {
            if (_adsmogo == null)
            {
                _container = container;
                _adsmogo = new AdsMogoView()
                               {
                                   AppID = AD_UNIT_ID,
                                   QuickMode = true,
                                   ClosedAD = true,
                                   VerticalAlignment = VerticalAlignment.Bottom
                               };
                //_adsmogo.MogoClickEvent += _adsmogo_MogoClickEvent;
                _adsmogo.MogoViewEvent += _adsmogo_MogoViewEvent;
                _adsmogo.MogoCloseEvent += _adsmogo_MogoCloseEvent;
                _adsmogo.SetValue(Grid.RowProperty, 1);
                panel.Children.Add(_adsmogo);
            }
        }

        private void _adsmogo_MogoViewEvent(object sender, DescriptionEventArgs e)
        {
            switch (e.Description)
            {
                case DescriptionEventArgs.AdStatus.ADSUCCEED:
                    _container.Margin = new Thickness(0, 0, 0, 72);
                    _adsmogo.Height = 72;
                    break;
            }
        }

        private void _adsmogo_MogoCloseEvent(object sender, EventArgs e)
        {
            _container.Margin = new Thickness(0, 0, 0, 0);
            _adsmogo.StopAdService();
        }
    }
}