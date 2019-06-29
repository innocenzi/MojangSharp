using MojangSharp.Api;
using System;

namespace MojangSharp.Responses
{
    /// <summary>
    /// Response containing the UUID of a player at the given time.
    /// </summary>
    public class UuidAtTimeResponse : Response
    {
        internal UuidAtTimeResponse(Response response) : base(response)
        {
        }

        /// <summary>
        /// Uuid
        /// </summary>
        public Uuid Uuid { get; internal set; }

        /// <summary>
        /// The date at which the UUID was corresponding.
        /// </summary>
        public DateTime Date { get; internal set; }
    }
}