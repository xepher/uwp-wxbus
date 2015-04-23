using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using System.Linq;
using Windows.UI.Popups;
using ReactiveUI;
using Splat;

namespace Org.Xepher.Kazuma.Utils
{
    class IAPHelper
    {
        public static async Task<List<ProductListing>> GetProdList()
        {
            List<ProductListing> productListings = new List<ProductListing>();
            try
            {
                var ProdList = await CurrentApp.LoadListingInformationAsync();

                productListings = ProdList.ProductListings.Select(x => x.Value).ToList();
            }
            catch (Exception)
            {
                IMessageBus messageBus = Locator.Current.GetService<IMessageBus>();
                messageBus.SendMessage<string>(Constants.MSG_NETWORK_UNAVAILABLE, Constants.MSGBUS_TOKEN_MESSAGEBAR);
            }

            return productListings;
        }

        public static async Task PurchaseProd()
        {
            var ProdList = await CurrentApp.LoadListingInformationAsync();
            var Prod = ProdList.ProductListings.FirstOrDefault(p => p.Value.ProductType == ProductType.Consumable);
            string result;
            try
            {
                var Receipt = await CurrentApp.RequestProductPurchaseAsync(Prod.Value.ProductId);

                if (CurrentApp.LicenseInformation.ProductLicenses[Prod.Value.ProductId].IsActive && Receipt.Status == ProductPurchaseStatus.Succeeded)
                {
                    // do someting with this license...

                    // Notify the marketplace that the application has delivered the paid-for goods to the user. 
                    var fulfillmentResult = await CurrentApp.ReportConsumableFulfillmentAsync(Prod.Value.ProductId, Receipt.TransactionId);

                    result = fulfillmentResult.ToString() + "\r\n\r\n" + Receipt.ReceiptXml;
                }
                else
                {
                    result = "License haven't been actived.";
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
                IMessageBus messageBus = Locator.Current.GetService<IMessageBus>();
                messageBus.SendMessage<string>(Constants.MSG_NETWORK_UNAVAILABLE, Constants.MSGBUS_TOKEN_MESSAGEBAR);
            }

            MessageDialog dialog = new MessageDialog(result, "Product Receipt");
            dialog.Commands.Add(new UICommand("确定", new UICommandInvokedHandler(cmd => { })));
            dialog.Commands.Add(new UICommand("取消", new UICommandInvokedHandler(cmd => { })));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            await dialog.ShowAsync();
        }
    }
}
