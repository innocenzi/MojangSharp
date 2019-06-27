using MojangSharp.Api;
using MojangSharp.Responses;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MojangSharp.Responses.ChallengesResponse;

namespace MojangSharp.Endpoints
{
    /// <summary>
    /// Challenges request class
    /// </summary>
    public class Challenges : IEndpoint<ChallengesResponse>
    {
        /// <summary>
        /// Instantiates the endpoints which allows to get an user's challenges.
        /// </summary>
        /// <param name="token">A valid user token.</param>
        public Challenges(string accessToken)
        {
            this.Address = new Uri("https://api.mojang.com/user/security/challenges");
            this.Arguments.Add(accessToken);
        }

        /// <summary>
        /// Performs the request and return the Response property.
        /// </summary>
        public override async Task<ChallengesResponse> PerformRequestAsync()
        {
            this.Response = await Requester.Get(this, true);

            if (this.Response.IsSuccess)
            {
                JArray jchallenges = JArray.Parse(this.Response.RawMessage);
                List<Challenge> challenges = new List<Challenge>();
                foreach (JToken token in jchallenges)
                    challenges.Add(ChallengesResponse.Parse(token));

                return new ChallengesResponse(this.Response)
                {
                    Challenges = challenges
                };
            }
            else
                return new ChallengesResponse(Error.GetError(this.Response));
        }
    }
}