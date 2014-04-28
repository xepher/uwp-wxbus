using Microsoft.Phone.Notification;
using System;
namespace Framework.Notification
{
    interface IPushNotificationProvider
    {
        event EventHandler<NotificationChannelUriEventArgs> ToastChannelUriUpdated;
        event EventHandler<NotificationChannelErrorEventArgs> ToastErrorOccurred;
        event EventHandler<NotificationEventArgs> ToastShellToastNotificationReceived;

        event EventHandler<NotificationChannelUriEventArgs> TileChannelUriUpdated;
        event EventHandler<NotificationChannelErrorEventArgs> TileErrorOccurred;

        event EventHandler<NotificationChannelUriEventArgs> RawChannelUriUpdated;
        event EventHandler<NotificationChannelErrorEventArgs> RawErrorOccurred;
        event EventHandler<HttpNotificationEventArgs> RawHttpNotificationReceived;

        void RegisterRawPushChannel(string channelName = "RawPushChannel");
        void RegisterTilePushChannel(string channelName = "TilePushChannel");
        void RegisterToastPushChannel(string channelName = "ToastPushChannel");
    }
}
