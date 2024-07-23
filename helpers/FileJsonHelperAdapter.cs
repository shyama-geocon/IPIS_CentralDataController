using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using IpisCentralDisplayController.Helpers;

namespace IpisCentralDisplayController.Adapters
{
    public class FileJsonHelperAdapter : IJsonHelper
    {
        private readonly string _filePath;

        public FileJsonHelperAdapter(string filePath)
        {
            _filePath = filePath;
        }

        public void Save<T>(string key, T value)
        {
            var fullPath = Path.Combine(_filePath, key + ".json");
            FileJsonHelper.Save(fullPath, value);
        }

        public T Load<T>(string key)
        {
            var fullPath = Path.Combine(_filePath, key + ".json");
            return FileJsonHelper.Load<T>(fullPath);
        }

        public T Load<T>(string key, Expression<Func<T, bool>> predicate)
        {
            var fullPath = Path.Combine(_filePath, key + ".json");
            var items = FileJsonHelper.Load<List<T>>(fullPath);
            return items?.Find(new Predicate<T>(predicate.Compile().Invoke));
        }

        public void Delete(string key)
        {
            var fullPath = Path.Combine(_filePath, key + ".json");
            FileJsonHelper.Delete(fullPath);
        }

        public void Delete<T>(string key, Expression<Func<T, bool>> predicate)
        {
            var fullPath = Path.Combine(_filePath, key + ".json");
            var items = FileJsonHelper.Load<List<T>>(fullPath);
            if (items != null)
            {
                items.RemoveAll(new Predicate<T>(predicate.Compile().Invoke));
                FileJsonHelper.Save(fullPath, items);
            }
        }

        public void Update<T>(string key, T value)
        {
            var fullPath = Path.Combine(_filePath, key + ".json");
            var items = FileJsonHelper.Load<List<T>>(fullPath);
            if (items != null)
            {
                var index = items.FindIndex(new Predicate<T>(x => x.Equals(value)));
                if (index >= 0)
                {
                    items[index] = value;
                    FileJsonHelper.Save(fullPath, items);
                }
                else
                {
                    items.Add(value);
                    FileJsonHelper.Save(fullPath, items);
                }
            }
            else
            {
                items = new List<T> { value };
                FileJsonHelper.Save(fullPath, items);
            }
        }

        public void Update<T>(string key, Expression<Func<T, bool>> predicate, T value)
        {
            var fullPath = Path.Combine(_filePath, key + ".json");
            var items = FileJsonHelper.Load<List<T>>(fullPath);
            if (items != null)
            {
                var index = items.FindIndex(new Predicate<T>(predicate.Compile().Invoke));
                if (index >= 0)
                {
                    items[index] = value;
                    FileJsonHelper.Save(fullPath, items);
                }
            }
        }
    }
}
