using MojangSharp.Api;
using System.Collections.Generic;

namespace MojangSharp.Responses
{
    /// <summary>
    /// Response containing the list of UUIDs
    /// </summary>
    public class UuidByNamesResponse : Response
    {
        internal UuidByNamesResponse(Response response) : base(response)
        {
        }

        /// <summary>
        /// The list of UUID corresponding to the names.
        /// </summary>
        public List<Uuid> UuidList { get; internal set; }
    }
}