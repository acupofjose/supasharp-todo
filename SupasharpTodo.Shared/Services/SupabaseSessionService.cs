﻿using Supabase.Gotrue.Interfaces;
using Supabase.Gotrue;
using Newtonsoft.Json;
using SupasharpTodo.Shared.Interfaces;

namespace SupasharpTodo.Shared.Services
{
    public class SupabaseSessionService : IGotrueSessionPersistence<Session>
    {
        private const string CacheKey = ".gotrue.cache";

        private readonly ILocalStorageProvider _localStorageProvider;

        public void DestroySession() =>
            _localStorageProvider.RemoveItem(CacheKey);

        public SupabaseSessionService(ILocalStorageProvider localStorageProvider)
        {
            _localStorageProvider = localStorageProvider;
        }

        public void SaveSession(Session session)
        {
            try
            {
                var serialized = JsonConvert.SerializeObject(session);
                _localStorageProvider.SetItem(CacheKey, serialized);
            }
            catch (Exception err)
            {
                Console.WriteLine($"Failed to save session with:\n\t{err}");
            }
        }

        public Session? LoadSession()
        {
            try
            {
                var json = _localStorageProvider.GetItem(CacheKey);

                if (string.IsNullOrEmpty(json))
                    return null;

                var session = JsonConvert.DeserializeObject<Session>(json);

                if (session != null && !session.Expired())
                    return session;

                return null;
            }
            catch (Exception err)
            {
                Console.WriteLine($"Failed to load session with:\n\t{err}");
                return null;
            }
        }
    }
}