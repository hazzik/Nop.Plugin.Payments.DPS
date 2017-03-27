using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Hazzik.Nop.Plugin.Payments.PxPost.Core
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "Txn")]
    public class TxnRequest
    {
        public string Amount { get; set; }

        public string CardHolderName { get; set; }

        public string CardNumber { get; set; }

        public string BillingId { get; set; }

        public string Cvc2 { get; set; }

        public byte Cvc2Presence { get; set; }

        public string DateExpiry { get; set; }

        public string DpsBillingId { get; set; }

        public string DpsTxnRef { get; set; }

        public string EnableAddBillCard { get; set; }

        public string InputCurrency { get; set; }

        public string MerchantReference { get; set; }

        public string PostUsername { get; set; }

        public string PostPassword { get; set; }

        public string TxnType { get; set; }

        public string TxnData1 { get; set; }

        public string TxnData2 { get; set; }

        public string TxnData3 { get; set; }

        public string TxnId { get; set; }

        public string EnableAvsData { get; set; }

        public string AvsAction { get; set; }

        public string AvsPostCode { get; set; }

        public string AvsStreetAddress { get; set; }

        public string DateStart { get; set; }

        public string IssueNumber { get; set; }

        public string Track2 { get; set; }
    }
}