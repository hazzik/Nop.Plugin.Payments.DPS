using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Routing;
using System.Xml.Serialization;
using Hazzik.Nop.Plugin.Payments.PxPost.Controllers;
using Hazzik.Nop.Plugin.Payments.PxPost.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using XmlHelper = Nop.Core.XmlHelper;

namespace Hazzik.Nop.Plugin.Payments.PxPost
{
    public class PxPostPaymentProcessor : BasePlugin, IPaymentMethod
    {
	    readonly ILocalizationService _localizationService;
	    readonly IOrderTotalCalculationService _orderTotalCalculationService;
	    readonly ISettingService _settingService;
	    readonly PxPostPaymentSettings _pxPostPaymentSettings;

	    public PxPostPaymentProcessor(ILocalizationService localizationService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ISettingService settingService,
            PxPostPaymentSettings pxPostPaymentSettings)
        {
            _localizationService = localizationService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _settingService = settingService;
            _pxPostPaymentSettings = pxPostPaymentSettings;
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var request = new TxnRequest
            {
                PostUsername = _pxPostPaymentSettings.Username,
                PostPassword = _pxPostPaymentSettings.Password,
                CardHolderName = processPaymentRequest.CreditCardName,
                CardNumber = processPaymentRequest.CreditCardNumber,
                Cvc2 = processPaymentRequest.CreditCardCvv2,
                Cvc2Presence = (byte) (string.IsNullOrEmpty(processPaymentRequest.CreditCardCvv2) ? 0 : 1),
                DateExpiry = processPaymentRequest.CreditCardExpireYear.ToString("N2") + processPaymentRequest.CreditCardExpireMonth.ToString("N2"),
                InputCurrency = processPaymentRequest.CustomValues["CurrencyCode"].ToString(),
                Amount = processPaymentRequest.OrderTotal.ToString("F2"),
                MerchantReference = processPaymentRequest.OrderGuid.ToString(),
                TxnId = processPaymentRequest.OrderGuid.ToString("N").Substring(0, 16),
                TxnType = _pxPostPaymentSettings.TransactMode == TransactMode.Authorize ? "Auth" : "Purchase"
            }.ToStream();
            var response = new HttpClient().PostAsync("https://uat.paymentexpress.com/pxpost.aspx", new StreamContent(request)).Result;
            var responseStream = response.Content.ReadAsStreamAsync().Result;

            var txnResponse = (TxnResponse) new XmlSerializer(typeof(TxnResponse)).Deserialize(responseStream);

            var result = new ProcessPaymentResult {AllowStoringCreditCardNumber = false};

            if (txnResponse.Success == 0)
            {
                result.AddError(txnResponse.ResponseText);
                return result;
            }

            var transaction = txnResponse.Transaction;

            result.NewPaymentStatus = _pxPostPaymentSettings.TransactMode == TransactMode.Authorize ? PaymentStatus.Authorized : PaymentStatus.Paid;

            return result;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = this.CalculateAdditionalFee(_orderTotalCalculationService,  cart,
                _pxPostPaymentSettings.AdditionalFee, _pxPostPaymentSettings.AdditionalFeePercentage);
            return result;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var request = new TxnRequest
            {
                DpsTxnRef = capturePaymentRequest.Order.AuthorizationTransactionId,
                TxnType = "Complete"
            }.ToStream();
            var response = new HttpClient().PostAsync("https://uat.paymentexpress.com/pxpost.aspx", new StreamContent(request)).Result;
            var responseStream = response.Content.ReadAsStreamAsync().Result;

            var txnResponse = (TxnResponse)new XmlSerializer(typeof(TxnResponse)).Deserialize(responseStream);

            var result = new CapturePaymentResult();
            return result;
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult
            {
                AllowStoringCreditCardNumber = true,
                NewPaymentStatus = PaymentStatus.Pending
            };

            return result;
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return new CancelRecurringPaymentResult();
        }

        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            return false;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentPxPost";
            routeValues = new RouteValueDictionary { { "Namespaces", "Hazzik.Nop.Plugin.Payments.PxPost.Controllers" }, { "area", null } };
        }

        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentPxPost";
            routeValues = new RouteValueDictionary { { "Namespaces", "Hazzik.Nop.Plugin.Payments.PxPost.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentPxPostController);
        }

        public override void Install()
        {
            var settings = new PxPostPaymentSettings();
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PxPost.Fields.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PxPost.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PxPost.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PxPost.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PxPost.Fields.TransactMode", "After checkout mark payment as");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PxPost.Fields.TransactMode.Hint", "Specify transaction mode.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.PxPost.PaymentMethodDescription", "Pay by credit / debit card");

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<PxPostPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.PxPost.Fields.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payments.PxPost.Fields.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PxPost.Fields.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payments.PxPost.Fields.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PxPost.Fields.TransactMode");
            this.DeletePluginLocaleResource("Plugins.Payments.PxPost.Fields.TransactMode.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.PxPost.PaymentMethodDescription");

            base.Uninstall();
        }

        public bool SupportCapture => true;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.Automatic;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Standard;

        public bool SkipPaymentInfo => false;

        public string PaymentMethodDescription => _localizationService.GetResource("Plugins.Payments.PxPost.PaymentMethodDescription");
    }
}
