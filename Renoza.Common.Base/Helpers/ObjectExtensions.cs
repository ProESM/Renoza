using System.Reflection;

namespace Renoza.Common.Base.Helpers
{
    public static class ObjectExtensions
    {
        public static Dictionary<string, object?> GetPropertiesDictionary(this object obj)
        {
            return obj.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead)
                .ToDictionary(p => p.Name, p => p.GetValue(obj));
        }
    }
}
