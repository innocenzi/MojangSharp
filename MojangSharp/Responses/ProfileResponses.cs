using MojangSharp.Api;
using Newtonsoft.Json.Linq;
using System;

namespace MojangSharp.Responses
{
    /// <summary>
    /// Profile response class
    /// </summary>
    public class ProfileResponse : Response
    {
        internal ProfileResponse(Response response) : base(response)
        {
        }

        /// <summary>
        /// Player's UUID.
        /// </summary>
        public Uuid Uuid { get; internal set; }

        /// <summary>
        /// Player's profile properties
        /// </summary>
        public ProfileProperties Properties { get; internal set; }

        /// <summary>
        /// Class representing all properties of a profile
        /// </summary>
        public class ProfileProperties
        {
            /// <summary>
            /// Instantiate properties thanks to the base64 string
            /// </summary>
            public ProfileProperties(string base64)
            {
                if (base64 == null)
                    throw new ArgumentNullException("Base64", "Base 64 string must not be null");

                string json = Requester.Encoding.GetString(Convert.FromBase64String(base64));
                JObject data = JObject.Parse(json);

                this.Date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddMilliseconds(data["timestamp"].ToObject<double>());
                this.ProfileUuid = data["profileId"].ToObject<string>();
                this.ProfileName = data["profileName"].ToObject<string>();
                this.SignatureRequired = (!json.Contains("signatureRequired") ? false :
                                          data["signatureRequired"].ToObject<bool>());

                JObject textures = data["textures"].ToObject<JObject>();

                if (textures["SKIN"] != null && textures["SKIN"]["url"] != null)
                    this.SkinUri = new Uri(textures["SKIN"]["url"].ToObject<string>());
                if (textures["CAPE"] != null && textures["CAPE"]["url"] != null)
                    this.CapeUri = new Uri(textures["CAPE"]["url"].ToObject<string>());
            }

            /// <summary>
            /// Timestamp value
            /// </summary>
            public DateTime Date { get; internal set; }

            /// <summary>
            /// Profile UUID
            /// </summary>
            public string ProfileUuid { get; internal set; }

            /// <summary>
            /// Profile name
            /// </summary>
            public string ProfileName { get; internal set; }

            /// <summary>
            /// No clue what this is about
            /// </summary>
            public bool SignatureRequired { get; internal set; }

            // The two elements below are part of an array.
            // If the API change, there likely be no trouble
            // with the parsing *but* future values will have
            // to be added manually

            /// <summary>
            /// Skin URI. May not work if the user has no skin.
            /// </summary>
            public Uri SkinUri { get; internal set; }

            /// <summary>
            /// Cape URI
            /// </summary>
            public Uri CapeUri { get; internal set; }
        }
    }

    /// <summary>
    /// Represents an error occured while accessing profile API
    /// </summary>
    public class ProfileResponseError : Error
    {
        internal ProfileResponseError(JObject json)
        {
            this.ErrorTag = json["error"].ToObject<string>();
            this.ErrorMessage = json["errorMessage"].ToObject<string>();
        }
    }
}