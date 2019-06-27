using MojangSharp.Api;
using MojangSharp.Responses;
using System;
using System.Threading.Tasks;
using static MojangSharp.Responses.BlockedServersResponse;

namespace MojangSharp.Endpoints
{
    /// <summary>
    /// Gets a list of SHA1 hashes used to check blocked servers addresses
    /// Clients check the lowercase name, using the ISO-8859-1 charset, against this list.
    /// They will also attempt to check subdomains, replacing each level with a *.
    /// Specifically, it splits based off of the . in the domain, goes through each section removing one at a time.
    /// For instance, for mc.example.com, it would try mc.example.com, *.example.com, and *.com. With IP addresses
    /// (verified by having 4 split sections, with each section being a valid integer between 0 and 255, inclusive)
    /// substitution starts from the end, so for 192.168.0.1, it would try 192.168.0.1, 192.168.0.*, 192.168.*, and 192.*.
    /// This check is done by the bootstrap class in netty.The default netty class is overridden by one in the com.mojang:netty dependency
    /// loaded by the launcher. This allows it to affect any version that used netty (1.7+)
    /// </summary>
    public class BlockedServers : IEndpoint<BlockedServersResponse>
    {
        /// <summary>
        /// Instance of BlockServer request class
        /// </summary>
        public BlockedServers()
        {
            this.Address = new Uri($"https://sessionserver.mojang.com/blockedservers");
        }

        /// <summary>
        /// Performs a Block Server request
        /// </summary>
        /// <returns></returns>
        public async override Task<BlockedServersResponse> PerformRequestAsync()
        {
            this.Response = await Requester.Get(this);

            if (this.Response.IsSuccess)
            {
                return new BlockedServersResponse(this.Response)
                {
                    BlockedServers = BlockedServer.Parse(this.Response.RawMessage)
                };
            }
            else
                return new BlockedServersResponse(Error.GetError(this.Response));
        }
    }
}