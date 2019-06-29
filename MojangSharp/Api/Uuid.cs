using Newtonsoft.Json;

namespace MojangSharp.Api
{
    /// <summary>
    /// Represents the UUID of a player and contains some more data such as its username and
    /// booleans indicating if its account is paid and/or migrated.
    /// </summary>
    public class Uuid
    {
        /// <summary>
        /// Instantiate an Uuid item.
        /// </summary>
        public Uuid()
        {
            this.Legacy = false;
            this.Demo = false;
        }

        /// <summary>
        /// UUID of the player corresponding to the given date.
        /// </summary>
        [JsonProperty("id")]
        public string Value { get; internal set; }

        /// <summary>
        /// Nickname the player had at the given date.
        /// </summary>
        [JsonProperty("name")]
        public string PlayerName { get; internal set; }

        /// <summary>
        /// Indicates weither or not the player's account has migrated to Mojang.
        /// If true, this account has not migrated.
        /// </summary>
        [JsonProperty("legacy")]
        public bool? Legacy { get; internal set; }

        /// <summary>
        /// Indicates weither or not the player's account is a demo-only account.
        /// If true, this account has not bought Minecraft.
        /// </summary>
        [JsonProperty("demo")]
        public bool? Demo { get; internal set; }
    }
}