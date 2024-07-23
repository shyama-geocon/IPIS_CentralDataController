using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class MongoDBJsonHelper
{
    private readonly IMongoDatabase _database;

    public MongoDBJsonHelper(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public void Save<T>(string collectionName, T value)
    {
        var collection = _database.GetCollection<T>(collectionName);
        collection.InsertOne(value);
    }

    public T Load<T>(string collectionName, Expression<Func<T, bool>> predicate)
    {
        var collection = _database.GetCollection<T>(collectionName);
        return collection.Find(predicate).FirstOrDefault();
    }

    public void Delete<T>(string collectionName, Expression<Func<T, bool>> predicate)
    {
        var collection = _database.GetCollection<T>(collectionName);
        collection.DeleteMany(predicate);
    }

    public void Update<T>(string collectionName, Expression<Func<T, bool>> predicate, T value)
    {
        var collection = _database.GetCollection<T>(collectionName);
        collection.ReplaceOne(predicate, value);
    }

    public void DeleteAll<T>(string collectionName)
    {
        var collection = _database.GetCollection<T>(collectionName);
        collection.DeleteMany(Builders<T>.Filter.Empty);
    }
}
