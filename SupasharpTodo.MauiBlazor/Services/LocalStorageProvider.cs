using SupasharpTodo.Shared.Interfaces;

namespace SupasharpTodo.MauiBlazor.Services
{
    public class LocalStorageProvider : ILocalStorageProvider
    {
        private ISecureStorage Instance => SecureStorage.Default;

        public string GetItem(string key) =>
            Instance.GetAsync(key).Result;


        public void RemoveItem(string key) =>
            Instance.Remove(key);

        public void SetItem(string key, string value) =>
            Instance.SetAsync(key, value);
    }
}

