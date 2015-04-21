using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ReactiveUI;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Org.Xepher.Kazuma.Views.UserControls
{
    public sealed partial class MessageBar : UserControl
    {
        private double _timesToTick = 2;
        DispatcherTimer _timer;

        public string Message
        {
            get
            {
                return this.MessageBlock.Text;
            }
            set
            {
                this.MessageBlock.Text = value;
            }
        }

        public MessageBar()
        {
            this.InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(_timesToTick);
            _timer.Tick += _timer_Tick;

            this.ObservableForProperty(v => v.MessageBlock.Text)
                .Subscribe(s =>
                {
                    if (string.IsNullOrEmpty(s.Value))
                    {
                        this.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.Visibility = Visibility.Visible;
                        _timer.Start();
                    }
                });
        }

        void _timer_Tick(object sender, object e)
        {
            _timesToTick--;
            if (_timesToTick == 0)
            {
                _timesToTick = 3;
                _timer.Tick -= _timer_Tick;
                _timer.Stop();
                Message = string.Empty;
            }

        }
    }
}
