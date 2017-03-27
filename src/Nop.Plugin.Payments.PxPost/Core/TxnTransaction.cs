using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Hazzik.Nop.Plugin.Payments.PxPost.Core
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class TxnTransaction
    {
        public byte Authorized { get; set; }


        public string ReCo { get; set; }


        public string RxDate { get; set; }


        public string RxDateLocal { get; set; }


        public string LocalTimeZone { get; set; }


        public string MerchantReference { get; set; }


        public string CardName { get; set; }


        public string Retry { get; set; }


        public string StatusRequired { get; set; }


        public string AuthCode { get; set; }


        public string AmountBalance { get; set; }


        public string Amount { get; set; }


        public string CurrencyId { get; set; }


        public string InputCurrencyId { get; set; }


        public string InputCurrencyName { get; set; }


        public string CurrencyRate { get; set; }


        public string CurrencyName { get; set; }


        public string CardHolderName { get; set; }


        public string DateSettlement { get; set; }


        public string TxnType { get; set; }


        public string CardNumber { get; set; }


        public string TxnMac { get; set; }


        public string DateExpiry { get; set; }


        public string ProductId { get; set; }


        public string AcquirerDate { get; set; }


        public string AcquirerTime { get; set; }


        public string AcquirerId { get; set; }


        public string Acquirer { get; set; }


        public string AcquirerReCo { get; set; }


        public string AcquirerResponseText { get; set; }


        public string TestMode { get; set; }


        public string CardId { get; set; }


        public string CardHolderResponseText { get; set; }


        public string CardHolderHelpText { get; set; }


        public string CardHolderResponseDescription { get; set; }


        public string MerchantResponseText { get; set; }


        public string MerchantHelpText { get; set; }


        public string MerchantResponseDescription { get; set; }


        public string UrlFail { get; set; }


        public string UrlSuccess { get; set; }


        public string EnablePostResponse { get; set; }


        public string PxPayName { get; set; }


        public string PxPayLogoSrc { get; set; }


        public string PxPayUserId { get; set; }


        public string PxPayXsl { get; set; }


        public string PxPayBgColor { get; set; }


        public string PxPayOptions { get; set; }


        public string Cvc2ResultCode { get; set; }


        public string AcquirerPort { get; set; }


        public string AcquirerTxnRef { get; set; }


        public string GroupAccount { get; set; }


        public string DpsTxnRef { get; set; }


        public string AllowRetry { get; set; }


        public string DpsBillingId { get; set; }


        public string BillingId { get; set; }


        public string TransactionId { get; set; }


        public string PxHostId { get; set; }


        public string RmReason { get; set; }


        public string RmReasonId { get; set; }


        public string RiskScore { get; set; }


        public string RiskScoreText { get; set; }

        [XmlAttribute]
        public string success { get; set; }

        [XmlAttribute]
        public string reco { get; set; }

        [XmlAttribute]
        public string responseText { get; set; }

        [XmlAttribute]
        public string pxTxn { get; set; }
    }
}