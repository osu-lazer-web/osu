// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Localisation;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Chat;
using osu.Game.Online.Notifications.WebSocket;

namespace osu.Game.Online.API
{
    /// <summary>
    /// An <see cref="IAPIProvider"/> for platforms where online functionality is unavailable.
    /// The local user is always a guest, and all requests fail.
    /// </summary>
    public partial class OfflineAPIAccess : Component, IAPIProvider
    {
        private readonly OfflineLocalUserState localUserState = new OfflineLocalUserState();

        public IBindable<APIUser> LocalUser => localUserState.User;

        public ILocalUserState LocalUserState => localUserState;

        public string ScoreProcessingNoticeUrl => string.Empty;

        public Language Language => Language.en;

        public string AccessToken => string.Empty;

        public Guid SessionIdentifier { get; } = Guid.NewGuid();

        public bool IsLoggedIn => false;

        public string ProvidedUsername => string.Empty;

        public EndpointConfiguration Endpoints { get; }

        public int APIVersion => int.Parse(DateTime.Now.ToString("yyyyMMdd"));

        public Exception? LastLoginError { get; private set; }

        public IBindable<APIState> State { get; } = new Bindable<APIState>(APIState.Offline);

        public IBindable<string?> UserFacingOutageMessage { get; } = new Bindable<string?>();

        public SessionVerificationMethod? SessionVerificationMethod => null;

        public INotificationsClient NotificationsClient { get; } = new DummyNotificationsClient();

        public OfflineAPIAccess(EndpointConfiguration endpoints)
        {
            Endpoints = endpoints;
        }

        public void Queue(APIRequest request)
        {
            request.AttachAPI(this);
            Schedule(() => request.Fail(createException()));
        }

        public void Perform(APIRequest request)
        {
            request.AttachAPI(this);
            request.Fail(createException());
        }

        public Task PerformAsync(APIRequest request)
        {
            Perform(request);
            return Task.CompletedTask;
        }

        public void Login(string username, string password) => LastLoginError = createException();

        public void AuthenticateSecondFactor(string code) => LastLoginError = createException();

        public void Logout()
        {
        }

        void IAPIProvider.Schedule(Action action) => Schedule(action);

        public IHubClientConnector? GetHubConnector(string clientName, string endpoint) => null;

        public IChatClient GetChatClient() => new OfflineChatClient();

        public RegistrationRequest.RegistrationRequestErrors? CreateAccount(string email, string username, string password) => new RegistrationRequest.RegistrationRequestErrors
        {
            Message = createException().Message,
        };

        private static APIException createException() => new APIException("Online functionality is not available on this platform.", null);

        private class OfflineLocalUserState : ILocalUserState
        {
            public Bindable<APIUser> User { get; } = new Bindable<APIUser>(new GuestUser());

            IBindable<APIUser> ILocalUserState.User => User;

            public IBindableList<APIRelation> Friends { get; } = new BindableList<APIRelation>();

            public IBindableList<APIRelation> Blocks { get; } = new BindableList<APIRelation>();

            public IBindableList<int> FavouriteBeatmapSets { get; } = new BindableList<int>();

            public void UpdateFriends()
            {
            }

            public void UpdateBlocks()
            {
            }

            public void UpdateFavouriteBeatmapSets()
            {
            }
        }

        private class OfflineChatClient : IChatClient
        {
            public event Action<Channel>? ChannelJoined
            {
                add { }
                remove { }
            }

            public event Action<Channel>? ChannelParted
            {
                add { }
                remove { }
            }

            public event Action<List<Message>>? NewMessages
            {
                add { }
                remove { }
            }

            public event Action? PresenceReceived
            {
                add { }
                remove { }
            }

            public void RequestPresence()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
