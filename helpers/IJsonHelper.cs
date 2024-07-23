using System;
using System.Linq.Expressions;

namespace IpisCentralDisplayController.Helpers
{
    public interface IJsonHelper
    {
        void Save<T>(string key, T value);
        T Load<T>(string key);
        T Load<T>(string key, Expression<Func<T, bool>> predicate);
        void Delete<T>(string key, Expression<Func<T, bool>> predicate);
        void Update<T>(string key, Expression<Func<T, bool>> predicate, T value);
    }
}
