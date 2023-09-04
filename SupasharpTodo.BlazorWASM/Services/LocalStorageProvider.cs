using SupasharpTodo.Shared.Interfaces;
using Blazored.LocalStorage;

namespace SupasharpTodo.BlazorWASM.Services
{
    public class LocalStorageProvider : ILocalStorageProvider
    {
        private ISyncLocalStorageService StorageService { get; }

        public LocalStorageProvider(ISyncLocalStorageService storageService)
        {
            StorageService = storageService;
        }

        public string GetItem(string key) =>
            StorageService.GetItem<string>(key);


        public void RemoveItem(string key) =>
            StorageService.RemoveItem(key);

        public void SetItem(string key, string value) =>
            StorageService.SetItem(key, value);
    }
}

