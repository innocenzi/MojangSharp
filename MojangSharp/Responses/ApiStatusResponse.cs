namespace MojangSharp.Responses
{
    /// <summary>
    /// Contains all the status of Mojang services.
    /// </summary>
    public class ApiStatusResponse : Response
    {
        internal ApiStatusResponse(Response response) : base(response)
        {
        }

        /// <summary>
        /// Possible status of Mojang services.
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// Unable to retrieve the status. Either the API changed or the service is unavailable.
            /// </summary>
            Unknown = 0, // Setting it by default.

            /// <summary>
            /// The service is available and has no issue.
            /// </summary>
            Available,

            /// <summary>
            /// The service is available but has some issues.
            /// </summary>
            SomeIssues,

            /// <summary>
            /// The services has issues and is unavailable.
            /// </summary>
            Unavailable
        }

        /// <summary>
        /// Parses an input to get a status enum.
        /// </summary>
        internal static Status Parse(string input)
        {
            // input ?? "" is to avoid getting a reference exception
            switch ((input ?? "").ToLower().Trim())
            {
                case "green": return Status.Available;
                case "yellow": return Status.SomeIssues;
                case "red": return Status.Unavailable;
                default: return Status.Unknown;
            }
        }

        /// <summary>
        /// Status of Minecraft.net
        /// </summary>
        public Status Minecraft { get; internal set; }

        /// <summary>
        /// Status of mojang.com
        /// </summary>
        public Status Mojang { get; internal set; }

        /// <summary>
        /// Status of session.minecraft.net
        /// </summary>
        public Status Sessions { get; internal set; }

        /// <summary>
        /// Status of textures.minecraft.net
        /// </summary>
        public Status Textures { get; internal set; }

        /// <summary>
        /// Status of sessionserver.mojang.com
        /// </summary>
        public Status MojangSessionsServer { get; internal set; }

        /// <summary>
        /// Status of accounts.mojang.com
        /// </summary>
        public Status MojangAccounts { get; internal set; }

        /// <summary>
        /// Status of auth.mojang.com
        /// </summary>
        public Status MojangAuthenticationService { get; internal set; }

        /// <summary>
        /// Status of authserver.mojang.com
        /// </summary>
        public Status MojangAutenticationServers { get; internal set; }

        /// <summary>
        /// Status of skins.minecraft.net
        /// </summary>
        public Status Skins { get; internal set; }

        /// <summary>
        /// Status of api.mojang.com
        /// </summary>
        public Status MojangApi { get; internal set; }
    }
}