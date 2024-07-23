using System;
using System.Linq.Expressions;

namespace IpisCentralDisplayController.Helpers
{
    public class SettingsJsonHelperAdapter : IJsonHelper
    {
        public void Save<T>(string key, T value)
        {
            SettingsJsonHelper.Save(key, value);
        }

        public T Load<T>(string key)
        {
            return SettingsJsonHelper.Load<T>(key);
        }

        public T Load<T>(string key, Expression<Func<T, bool>> predicate)
        {
            // Implement logic if needed, for now returning default
            return default;
        }

        public void Delete(string key)
        {
            SettingsJsonHelper.Delete(key);
        }

        public void Delete<T>(string key, Expression<Func<T, bool>> predicate)
        {
            // Implement logic if needed
        }

        public void Update<T>(string key, T value)
        {
            SettingsJsonHelper.Update(key, value);
        }

        public void Update<T>(string key, Expression<Func<T, bool>> predicate, T value)
        {
            // Implement logic if needed
        }
    }
}
