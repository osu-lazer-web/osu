// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading.Tasks;
using osu.Framework;
using osu.Framework.Browser;
using osu.Game;

namespace osu.Browser
{
    public static class Program
    {
#if DEBUG
        private const string base_game_name = @"osu-development";
#else
        private const string base_game_name = @"osu";
#endif

        public static async Task Main()
        {
            // This is a safe default. Localised usages should specify lower values as required.
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(1000));

            try
            {
                await BrowserGameHost.InitialiseAsync().ConfigureAwait(false);

                // The host must not be disposed here, as the game keeps running in the browser after Run() returns.
                var host = new BrowserGameHost(base_game_name, new HostOptions { FriendlyGameName = OsuGameBase.GAME_NAME });
                host.Run(new OsuGameBrowser());
            }
            catch (Exception e)
            {
                // Startup failures would otherwise only be reported as a rejected promise, without any stack trace.
                Console.Error.WriteLine($"osu! failed to start: {e}");
                throw;
            }
        }
    }
}
