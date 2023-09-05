namespace SupasharpTodo.Shared.Interfaces
{
	public interface ILocalStorageProvider
	{
		string GetItem(string key);
		void RemoveItem(string key);
		void SetItem(string key, string value);
	}
}

