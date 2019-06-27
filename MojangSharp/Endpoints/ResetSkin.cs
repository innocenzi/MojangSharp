using MojangSharp.Api;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MojangSharp.Endpoints
{
    /// <summary>
    /// Deletes a user skin
    /// </summary>
    public class ResetSkin : IEndpoint<Response>
    {
        /// <summary>
        /// Creates a change skin request with a given UUID.
        /// </summary>
        /// <param name="accessToken">Access token of the player</param>
        /// <param name="uuid">UUID of the player.</param>
        public ResetSkin(string accessToken, string uuid)
        {
            this.Address = new Uri($"https://api.mojang.com/user/profile/{uuid}/skin");
            this.Arguments.Add(accessToken);
        }

        /// <summary>
        /// Performs the skin change.
        /// </summary>
        public async override Task<Response> PerformRequestAsync()
        {
            this.Response = await Requester.Delete(this);

            if (this.Response.Code == HttpStatusCode.NoContent || this.Response.IsSuccess)
                return new Response(this.Response) { IsSuccess = true };
            else
                return new Response(this.Response);
        }
    }
}