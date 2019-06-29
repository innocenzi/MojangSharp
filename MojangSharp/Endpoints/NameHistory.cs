using MojangSharp.Api;
using MojangSharp.Responses;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using static MojangSharp.Responses.NameHistoryResponse;

namespace MojangSharp.Endpoints
{
    /// <summary>
    /// UUID to name history endpoint.
    /// </summary>
    public class NameHistory : IEndpoint<NameHistoryResponse>
    {
        /// <summary>
        /// Return all the usernames the user has used in the past.
        /// </summary>
        /// <param name="uuid">User's UUID.</param>
        public NameHistory(string uuid)
        {
            this.Address = new Uri($"https://api.mojang.com/user/profiles/{uuid}/names");
        }

        /// <summary>
        /// Performs a name history request.
        /// </summary>
        /// <returns></returns>
        public async override Task<NameHistoryResponse> PerformRequestAsync()
        {
            this.Response = await Requester.Get(this);

            if (this.Response.IsSuccess)
            {
                JArray entries = JArray.Parse(this.Response.RawMessage);

                NameHistoryResponse history = new NameHistoryResponse(this.Response);
                foreach (JToken entry in entries)
                {
                    history.NameHistory.Add(new NameHistoryEntry()
                    {
                        Name = entry["name"].ToObject<string>(),
                        ChangedToAt = (entry.Last != entry.First ?
                                      new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(entry["changedToAt"].ToObject<double>()).ToLocalTime() :
                                      new DateTime?())
                    });
                }
                return history;
            }
            else
                return new NameHistoryResponse(Error.GetError(this.Response));
        }
    }
}