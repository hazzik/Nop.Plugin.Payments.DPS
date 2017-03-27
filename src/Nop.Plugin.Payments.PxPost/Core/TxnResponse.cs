using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Hazzik.Nop.Plugin.Payments.PxPost.Core
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "Txn")]
    public class TxnResponse
    {
        public TxnTransaction Transaction { get; set; }

        public string ReCo { get; set; }

        public string ResponseText { get; set; }

        public string HelpText { get; set; }

        public byte Success { get; set; }

        public string DpsTxnRef { get; set; }

        public string TxnRef { get; set; }
    }
}