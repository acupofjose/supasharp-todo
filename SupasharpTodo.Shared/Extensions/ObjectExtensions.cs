using Newtonsoft.Json;

namespace SupasharpTodo.Shared.Extensions;

public static class ObjectExtensions
{
    public static T DeepCopy<T>(this T self)
    {
        var serialized = JsonConvert.SerializeObject(self);
        return JsonConvert.DeserializeObject<T>(serialized);
    }
}