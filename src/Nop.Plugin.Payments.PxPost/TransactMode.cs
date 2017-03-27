namespace Hazzik.Nop.Plugin.Payments.PxPost
{
    /// <summary>
    /// Represents payment processor transaction mode
    /// </summary>
    public enum TransactMode
    {
        /// <summary>
        /// Authorize
        /// </summary>
        Authorize = 0,

        /// <summary>
        /// Authorize and capture
        /// </summary>
        Purchase = 2
    }
}
