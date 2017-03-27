using Nop.Core.Configuration;

namespace Hazzik.Nop.Plugin.Payments.PxPost
{
    public class PxPostPaymentSettings : ISettings
    {
        public TransactMode TransactMode { get; set; }

        public bool AdditionalFeePercentage { get; set; }

        public decimal AdditionalFee { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
