namespace MojangSharp.Responses
{
    /// <summary>
    /// Represends a statistics request response
    /// </summary>
    public class StatisticsResponse : Response
    {
        internal StatisticsResponse(Response response) : base(response)
        {
        }

        /// <summary>
        /// Total amount of sold
        /// </summary>
        public int Total { get; internal set; }

        /// <summary>
        /// Total sold in last 24 hours
        /// </summary>
        public int Last24h { get; internal set; }

        /// <summary>
        /// Average sales by seconds
        /// </summary>
        public double SaleVelocity { get; internal set; }
    }
}