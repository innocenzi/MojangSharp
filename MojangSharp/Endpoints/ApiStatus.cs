using System;
using System.Threading.Tasks;
using MojangSharp.Api;
using Newtonsoft.Json.Linq;
using MojangSharp.Responses;
using System.Net;

namespace MojangSharp.Endpoints
{
    public class ApiStatus : IEndpoint<ApiStatusResponse>
    {

        /// <summary>
        /// Instantiates the endpoints which allows to get Mojang's APIs status.
        /// </summary>
        public ApiStatus()
        {
            this.Address = new Uri("https://status.mojang.com/check");
        }

        /// <summary>
        /// Performs the request and return the Response property.
        /// </summary>
        public async override Task<ApiStatusResponse> PerformRequest()
        {
           this.Response = await Requester.Get(this);

            if (this.Response.IsSuccess)
            {
                JArray status = JArray.Parse(this.Response.RawMessage);
                
                return new ApiStatusResponse(this.Response)
                {

                    // If the order in the response changes, this is all fucked up.
                    // It can be avoided by using the Deserialization method of JSON
                    // but this implies to create a new class and a single one with only an array.

                    Minecraft = ApiStatusResponse.Parse(status[0]["minecraft.net"].ToObject<string>()), // minecraft.net
                    Sessions = ApiStatusResponse.Parse(status[1]["session.minecraft.net"].ToObject<string>()), // session.minecraft.net
                    MojangAccounts = ApiStatusResponse.Parse(status[2]["account.mojang.com"].ToObject<string>()), // account.mojang.com
                    MojangAuthenticationService = ApiStatusResponse.Parse(status[3]["auth.mojang.com"].ToObject<string>()), // auth.mojang.com
                    Skins = ApiStatusResponse.Parse(status[4]["skins.minecraft.net"].ToObject<string>()), // skins.minecraft.net
                    MojangAutenticationServers = ApiStatusResponse.Parse(status[5]["authserver.mojang.com"].ToObject<string>()), // authserver.mojang.com
                    MojangSessionsServer = ApiStatusResponse.Parse(status[6]["sessionserver.mojang.com"].ToObject<string>()), // sessionserver.mojang.com
                    MojangApi = ApiStatusResponse.Parse(status[7]["api.mojang.com"].ToObject<string>()), // api.mojang.com
                    Textures = ApiStatusResponse.Parse(status[8]["textures.minecraft.net"].ToObject<string>()), // textures.minecraft.net
                    Mojang = ApiStatusResponse.Parse(status[9]["mojang.com"].ToObject<string>()), // mojang.com
                };
            }
            else
                return new ApiStatusResponse(Error.GetError(this.Response));

        }
    }
    

}
