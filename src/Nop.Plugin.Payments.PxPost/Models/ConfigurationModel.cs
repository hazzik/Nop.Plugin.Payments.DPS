using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Hazzik.Nop.Plugin.Payments.PxPost.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PxPost.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.PxPost.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        public int TransactModeId { get; set; }
        public bool TransactModeId_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.PayPalDirect.Fields.TransactMode")]
        public SelectList TransactModeValues { get; set; }
    }
}