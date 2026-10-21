// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game;
using osu.Game.Online.API;
using osu.Game.Updater;

namespace osu.Browser
{
    public partial class OsuGameBrowser : OsuGame
    {
        public OsuGameBrowser()
        {
            // Online functionality is not yet supported, as browsers block cross-origin requests to the osu! API (and the API client relies on threads).
            API = new OfflineAPIAccess(CreateEndpoints());
        }

        // Updates are handled by the page being served the latest version.
        protected override UpdateManager CreateUpdateManager() => new NoActionUpdateManager();
    }
}
