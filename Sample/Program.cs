using MojangSharp;
using MojangSharp.Api;
using MojangSharp.Endpoints;
using MojangSharp.Responses;
using System;
using System.IO;
using static MojangSharp.Endpoints.Statistics;
using static MojangSharp.Responses.ChallengesResponse;
using static MojangSharp.Responses.NameHistoryResponse;

namespace Sample
{
    class Program
    {
        static void Main(string[] args) => new Program().PerformTests();

        private void PerformTests()
        {
            // Status
            WriteColoredLine(ConsoleColor.DarkCyan, "[Get] Checking API status");
            ApiStatusResponse status = new ApiStatus().PerformRequestAsync().Result;
            if (status.IsSuccess)
            {
                WriteColoredLine(status.Mojang, $"  Mojang: {status.Mojang}");
                WriteColoredLine(status.Minecraft, $"  Minecraft.net: {status.Minecraft}");
                WriteColoredLine(status.MojangAccounts, $"  Mojang accounts: {status.MojangAccounts}");
                WriteColoredLine(status.MojangApi, $"  Mojang API: {status.MojangApi}");
                WriteColoredLine(status.MojangAutenticationServers, $"  Mojang auth. servers: {status.MojangAutenticationServers}");
                WriteColoredLine(status.MojangAuthenticationService, $"  Mojang auth. service: {status.MojangAuthenticationService}");
                WriteColoredLine(status.MojangSessionsServer, $"  Mojang sessions: {status.MojangSessionsServer}");
                WriteColoredLine(status.Sessions, $"  Minecraft.net sessions: {status.Sessions}");
                WriteColoredLine(status.Skins, $"  Minecraft.net skins: {status.Skins}");
                WriteColoredLine(status.Textures, $"  Minecraft.net textures: {status.Textures}");
            }
            else
                WriteColoredLine(ConsoleColor.Red, status.Error.Exception == null ? status.Error.ErrorMessage : status.Error.Exception.Message);


            // Authentication
            WriteColoredLine(ConsoleColor.DarkCyan, "\n[Post] Authenticate");
            string u = "<name>@<provider>.com", p = "<password>";
            AuthenticateResponse auth = new Authenticate(new Credentials() { Username = u, Password = p }).PerformRequestAsync().Result;
            if (auth.IsSuccess)
            {
                Console.WriteLine($"  Access token: {auth.AccessToken}");
                Console.WriteLine($"  Client token: {auth.ClientToken}");
                Console.WriteLine($"  Profiles: {auth.AvailableProfiles.Count}");
                Console.WriteLine($"  Profile name: {auth.SelectedProfile.PlayerName}");
                if (auth.User.Properties != null)
                    Console.WriteLine($"  Properties: {auth.User.Properties.Count}");
                
               Response v = new Validate(auth.AccessToken).PerformRequestAsync().Result;
                if (v.IsSuccess)
                    WriteColoredLine(ConsoleColor.DarkGreen, "  Validated!");
                else
                    WriteColoredLine(ConsoleColor.DarkYellow, "  Not validated.");
            }
            else
                WriteColoredLine(ConsoleColor.Red, auth.Error.Exception == null ? auth.Error.ErrorMessage : auth.Error.Exception.Message);

            // Secured IP
            WriteColoredLine(ConsoleColor.DarkCyan, "\n[Get] Location");
            WriteColoredLine(ConsoleColor.DarkGray, "If this test fails, requests using beared authentication will fail as well.");
            Response secured = new SecureIP(auth.AccessToken).PerformRequestAsync().Result;
            if (secured.IsSuccess)
                WriteColoredLine(ConsoleColor.DarkGreen, "Mojang seems to accept requests coming from this IP. :)");
            else
                WriteColoredLine(ConsoleColor.Red, secured.Error.Exception == null ? secured.Error.ErrorMessage : secured.Error.Exception.Message);

            // Challenges
            WriteColoredLine(ConsoleColor.DarkCyan, "\n[Get] Challenges");
            ChallengesResponse challenges = new Challenges(auth.AccessToken).PerformRequestAsync().Result;
            if (challenges.IsSuccess)
            {
                foreach (Challenge challenge in challenges.Challenges)
                    Console.WriteLine($"  Question [A: {challenge.AnswerID}, ID: {challenge.QuestionID}]: {challenge.QuestionText}");
            }
            else
                WriteColoredLine(ConsoleColor.Red, challenges.Error.Exception == null ? challenges.Error.ErrorMessage : challenges.Error.Exception.Message);


            // Uuid at time
            WriteColoredLine(ConsoleColor.DarkCyan, $"\n[Get] Getting current UUID for {auth.SelectedProfile.PlayerName}");
            UuidAtTimeResponse uuid = new UuidAtTime(auth.SelectedProfile.PlayerName, DateTime.Now).PerformRequestAsync().Result;
            if (uuid.IsSuccess)
            {
                Console.WriteLine($"  Username: {uuid.Uuid.PlayerName}");
                Console.WriteLine($"  Uuid: {uuid.Uuid.Value}");
                Console.WriteLine($"  Legacy: {uuid.Uuid.Legacy}");
                Console.WriteLine($"  Demo: {uuid.Uuid.Demo}");
            }
            else
                WriteColoredLine(ConsoleColor.Red, uuid.Error.Exception == null ? uuid.Error.ErrorMessage : uuid.Error.Exception.Message);

            // Name history
            WriteColoredLine(ConsoleColor.DarkCyan, $"\n[Get] Getting {auth.SelectedProfile.PlayerName}'s name history");
            NameHistoryResponse names = new NameHistory(uuid.Uuid.Value).PerformRequestAsync().Result;
            if (names.IsSuccess)
            {
                int id = 0;
                foreach (NameHistoryEntry entry in names.NameHistory)
                {
                    Console.WriteLine($"  {id++}. Name: {entry.Name}");
                    if (entry.ChangedToAt.HasValue)
                        Console.WriteLine($"    Changed to at: {entry.ChangedToAt.Value.ToShortDateString()}");
                }
            }
            else
                WriteColoredLine(ConsoleColor.Red, names.Error.Exception == null ? names.Error.ErrorMessage : names.Error.Exception.Message);

            // Uuid list
            WriteColoredLine(ConsoleColor.DarkCyan, $"\n[Post] Gets a list of UUID ({auth.SelectedProfile.PlayerName}, Hawraith)");
            UuidByNamesResponse uuids = new UuidByNames(auth.SelectedProfile.PlayerName, "Hawraith").PerformRequestAsync().Result;
            if (uuids.IsSuccess)
            {
                foreach (Uuid id in uuids.UuidList)
                {
                    Console.WriteLine($"  Username: {id.PlayerName}");
                    Console.WriteLine($"    Uuid: {id.Value}");
                    Console.WriteLine($"    Legacy: {id.Legacy}");
                    Console.WriteLine($"    Demo: {id.Demo}");
                }
            }
            else
                WriteColoredLine(ConsoleColor.Red, uuids.Error.ErrorMessage ?? uuids.Error.Exception.Message);

            // Profile
            WriteColoredLine(ConsoleColor.DarkCyan, $"\n[Get] Gets {auth.SelectedProfile.PlayerName}'s profile");
            ProfileResponse profile = new Profile(uuid.Uuid.Value, true).PerformRequestAsync().Result;
            if (profile.IsSuccess)
            {
                Console.WriteLine($"  Username: {profile.Uuid.PlayerName}");
                Console.WriteLine($"    Uuid: {profile.Uuid.Value}");
                Console.WriteLine($"    Date: {profile.Properties.Date.ToShortDateString()}");
                Console.WriteLine($"    Skin: {profile.Properties.SkinUri.ToString()}");
                if (profile.Properties.CapeUri != null)
                    Console.WriteLine($"    Cape: {profile.Properties.CapeUri.ToString()}");
                Console.WriteLine($"    Cape: none");
            }
            else
                WriteColoredLine(ConsoleColor.Red, profile.Error.ErrorMessage ?? profile.Error.Exception.Message);

            // Upload skin - TODO
            //http://i.imgur.com/OdTEea8.png
            WriteColoredLine(ConsoleColor.DarkCyan, "\n[Put] Changes current skin");
            WriteColoredLine(ConsoleColor.Yellow, "Change skin? This will not work if you have not modified the source. :)");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Response skin = new UploadSkin(auth.AccessToken, auth.SelectedProfile.Value, new FileInfo(@"C:\Path\To\Skin.png"), false)
                    .PerformRequestAsync().Result;
                if (skin.IsSuccess)
                    WriteColoredLine(ConsoleColor.DarkGreen, "Changed skin!");
                else
                    WriteColoredLine(ConsoleColor.DarkYellow, "Skin change failed: " + skin.Error.ErrorMessage);
            }


            // Blocked servers
            WriteColoredLine(ConsoleColor.DarkCyan, "\n[Get] Gets blocked servers");
            BlockedServersResponse servers = new BlockedServers().PerformRequestAsync().Result;
            if (servers.IsSuccess)
            {
                Console.WriteLine($"  {servers.BlockedServers.Count} blocked servers");
                Console.WriteLine($"  {servers.BlockedServers.FindAll(x => x.Cracked).Count} cracked");
            }
            else
                WriteColoredLine(ConsoleColor.Red, servers.Error.ErrorMessage ?? servers.Error.Exception.Message);


            // Stats
            WriteColoredLine(ConsoleColor.DarkCyan, "\n[Get] Gets stats for Minecraft");
            StatisticsResponse stats = new Statistics(Item.MinecraftAccountsSold).PerformRequestAsync().Result;
            if (stats.IsSuccess)
            {
                Console.WriteLine($"  Total Minecraft accounts sold: {stats.Total}");
                Console.WriteLine($"  Last 24h: {stats.Last24h}");
                Console.WriteLine($"  Average sell/s: {stats.SaleVelocity}");
            }
            else
                WriteColoredLine(ConsoleColor.Red, stats.Error.ErrorMessage ?? stats.Error.Exception.Message);

            WriteColoredLine(ConsoleColor.DarkGray, "\nPress any key to leave.");
            Console.Read();
        }


        #region Console

        private void WriteColoredLine(ConsoleColor color, string message)
        {
            ConsoleColor currentColor = Console.ForegroundColor; Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = currentColor;
        }

        private void WriteColoredLine(ApiStatusResponse.Status status, string message)
        {
            ConsoleColor currentColor = Console.ForegroundColor;

            ConsoleColor color = ConsoleColor.Gray;
            if (status == ApiStatusResponse.Status.Available)
                color = ConsoleColor.DarkGreen;
            if (status == ApiStatusResponse.Status.SomeIssues)
                color = ConsoleColor.DarkYellow;
            if (status == ApiStatusResponse.Status.Unavailable)
                color = ConsoleColor.Red;
            if (status == ApiStatusResponse.Status.Unknown)
                color = ConsoleColor.Gray;

            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = currentColor;
        }

        #endregion



    }
}
