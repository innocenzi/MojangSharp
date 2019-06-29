using System;
using System.Collections.Generic;

namespace MojangSharp.Responses
{
    /// <summary>
    /// Response containing all name changes.
    /// </summary>
    public class NameHistoryResponse : Response
    {
        internal NameHistoryResponse(Response response) : base(response)
        {
            this.NameHistory = new List<NameHistoryEntry>();
        }

        /// <summary>
        /// History of all name changes.
        /// </summary>
        public List<NameHistoryEntry> NameHistory { get; internal set; }

        /// <summary>
        /// An entry of name history
        /// </summary>
        public class NameHistoryEntry
        {
            /// <summary>
            /// Chosen name.
            /// </summary>
            public string Name { get; internal set; }

            /// <summary>
            /// Date at which user changed is name to.
            /// </summary>
            public DateTime? ChangedToAt { get; internal set; }
        }
    }
}