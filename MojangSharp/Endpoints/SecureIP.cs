using MojangSharp.Api;
using System;
using System.Threading.Tasks;

namespace MojangSharp.Endpoints
{
    /// <summary>
    /// Location secured IP request class
    /// </summary>
    public class SecureIP : IEndpoint<Response>
    {
        /// <summary>
        /// Instantiates the endpoints which allows to see if this IP is secured.
        /// </summary>
        /// <param name="token">A valid user token.</param>
        public SecureIP(string accessToken)
        {
            this.Address = new Uri("https://api.mojang.com/user/security/location");
            this.Arguments.Add(accessToken);
        }

        /// <summary>
        /// Performs the request and return the Response property.
        /// </summary>
        public async override Task<Response> PerformRequestAsync()
        {
            this.Response = await Requester.Get(this, true);

            if (this.Response.IsSuccess)
                return new Response(this.Response);
            else
                return new Response(Error.GetError(this.Response));
        }
    }
}