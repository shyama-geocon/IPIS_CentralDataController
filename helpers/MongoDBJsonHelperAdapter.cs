using System;
using System.Linq.Expressions;
using IpisCentralDisplayController.Helpers;

namespace IpisCentralDisplayController.Adapters
{
    public class MongoDBJsonHelperAdapter : IJsonHelper
    {
        private readonly MongoDBJsonHelper _mongoDBJsonHelper;

        public MongoDBJsonHelperAdapter(string connectionString, string databaseName)
        {
            _mongoDBJsonHelper = new MongoDBJsonHelper(connectionString, databaseName);
        }

        public void Save<T>(string collectionName, T value)
        {
            _mongoDBJsonHelper.Save(collectionName, value);
        }

        public T Load<T>(string collectionName)
        {
            return _mongoDBJsonHelper.Load<T>(collectionName, x => true);
        }

        public T Load<T>(string collectionName, Expression<Func<T, bool>> predicate)
        {
            return _mongoDBJsonHelper.Load(collectionName, predicate);
        }

        public void Delete<T>(string collectionName)
        {
            _mongoDBJsonHelper.DeleteAll<T>(collectionName);
        }

        public void Delete<T>(string collectionName, Expression<Func<T, bool>> predicate)
        {
            _mongoDBJsonHelper.Delete(collectionName, predicate);
        }

        public void Update<T>(string collectionName, T value)
        {
            _mongoDBJsonHelper.Update(collectionName, x => true, value);
        }

        public void Update<T>(string collectionName, Expression<Func<T, bool>> predicate, T value)
        {
            _mongoDBJsonHelper.Update(collectionName, predicate, value);
        }
    }
}
