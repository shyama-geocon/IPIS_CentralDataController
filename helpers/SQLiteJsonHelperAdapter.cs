using System;
using System.Linq.Expressions;
using IpisCentralDisplayController.Helpers;

namespace IpisCentralDisplayController.Adapters
{
    public class SQLiteJsonHelperAdapter : IJsonHelper
    {
        private readonly SQLiteJsonHelper _sqliteJsonHelper;

        public SQLiteJsonHelperAdapter(string connectionString)
        {
            _sqliteJsonHelper = new SQLiteJsonHelper(connectionString);
        }

        public void Save<T>(string key, T value)
        {
            _sqliteJsonHelper.Save(key, value);
        }

        public T Load<T>(string key)
        {
            return _sqliteJsonHelper.Load<T>(key, x => true);
        }

        public T Load<T>(string key, Expression<Func<T, bool>> predicate)
        {
            return _sqliteJsonHelper.Load<T>(key, predicate);
        }

        public void Delete(string key)
        {
            _sqliteJsonHelper.DeleteAll(key);
        }

        public void Delete<T>(string key, Expression<Func<T, bool>> predicate)
        {
            _sqliteJsonHelper.Delete<T>(key, predicate);
        }

        public void Update<T>(string key, T value)
        {
            _sqliteJsonHelper.Update<T>(key, x => true, value);
        }

        public void Update<T>(string key, Expression<Func<T, bool>> predicate, T value)
        {
            _sqliteJsonHelper.Update<T>(key, predicate, value);
        }
    }
}
