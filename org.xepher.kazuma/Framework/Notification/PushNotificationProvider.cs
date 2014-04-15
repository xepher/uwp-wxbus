using Microsoft.Phone.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Notification
{
    public class PushNotificationProvider : IPushNotificationProvider
    {
        public event EventHandler<NotificationChannelUriEventArgs> ToastChannelUriUpdated;
        public event EventHandler<NotificationChannelErrorEventArgs> ToastErrorOccurred;
        public event EventHandler<NotificationEventArgs> ToastShellToastNotificationReceived;

        public event EventHandler<NotificationChannelUriEventArgs> TileChannelUriUpdated;
        public event EventHandler<NotificationChannelErrorEventArgs> TileErrorOccurred;

        public event EventHandler<NotificationChannelUriEventArgs> RawChannelUriUpdated;
        public event EventHandler<NotificationChannelErrorEventArgs> RawErrorOccurred;
        public event EventHandler<HttpNotificationEventArgs> RawHttpNotificationReceived;

        public void RegisterToastPushChannel(string channelName = "ToastPushChannel")
        {
            // Holds the push channel that is created or found.
            HttpNotificationChannel _toastPushChannel;
            
            // Try to find the push channel.
            _toastPushChannel = HttpNotificationChannel.Find(channelName);

            // If the channel was not found, then create a new connection to the push service.
            if (_toastPushChannel == null)
            {
                _toastPushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                _toastPushChannel.ChannelUriUpdated += ToastChannelUriUpdated;
                _toastPushChannel.ErrorOccurred += ToastErrorOccurred;

                // Register for this notification only if you need to receive the notifications while your application is running.
                _toastPushChannel.ShellToastNotificationReceived += ToastShellToastNotificationReceived;

                _toastPushChannel.Open();

                // Bind this new channel for toast events.
                _toastPushChannel.BindToShellToast();

            }
            else
            {
                // The channel was already open, so just register for all the events.
                _toastPushChannel.ChannelUriUpdated += ToastChannelUriUpdated;
                _toastPushChannel.ErrorOccurred += ToastErrorOccurred;

                // Register for this notification only if you need to receive the notifications while your application is running.
                _toastPushChannel.ShellToastNotificationReceived += ToastShellToastNotificationReceived;

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.

            }
        }

        public void RegisterTilePushChannel(string channelName = "TilePushChannel")
        {
            // Holds the push channel that is created or found.
            HttpNotificationChannel _tilePushChannel;

            // Try to find the push channel.
            _tilePushChannel = HttpNotificationChannel.Find(channelName);

            // If the channel was not found, then create a new connection to the push service.
            if (_tilePushChannel == null)
            {
                _tilePushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                _tilePushChannel.ChannelUriUpdated += TileChannelUriUpdated;
                _tilePushChannel.ErrorOccurred += TileErrorOccurred;

                _tilePushChannel.Open();

                // Bind this new channel for toast events.
                _tilePushChannel.BindToShellToast();

            }
            else
            {
                // The channel was already open, so just register for all the events.
                _tilePushChannel.ChannelUriUpdated += TileChannelUriUpdated;
                _tilePushChannel.ErrorOccurred += TileErrorOccurred;

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.

            }
        }

        public void RegisterRawPushChannel(string channelName = "RawPushChannel")
        {
            // Holds the push channel that is created or found.
            HttpNotificationChannel _rawPushChannel;

            // Try to find the push channel.
            _rawPushChannel = HttpNotificationChannel.Find(channelName);

            // If the channel was not found, then create a new connection to the push service.
            if (_rawPushChannel == null)
            {
                _rawPushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                _rawPushChannel.ChannelUriUpdated += RawChannelUriUpdated;
                _rawPushChannel.ErrorOccurred += RawErrorOccurred;
                _rawPushChannel.HttpNotificationReceived += RawHttpNotificationReceived;

                _rawPushChannel.Open();

                // Bind this new channel for toast events.
                _rawPushChannel.BindToShellToast();

            }
            else
            {
                // The channel was already open, so just register for all the events.
                _rawPushChannel.ChannelUriUpdated += RawChannelUriUpdated;
                _rawPushChannel.ErrorOccurred += RawErrorOccurred;
                _rawPushChannel.HttpNotificationReceived += RawHttpNotificationReceived;

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.

            }
        }
    }
}
