using System;
using System.Collections.Generic;
using System.Linq;
using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.models;

namespace IpisCentralDisplayController.Managers
{
    public class UserCategoryManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _userCategoriesKey = "userCategories";

        public UserCategoryManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public List<UserCategory> LoadUserCategories()
        {
            return _jsonHelper.Load<List<UserCategory>>(_userCategoriesKey) ?? new List<UserCategory>();
        }

        public void SaveUserCategories(List<UserCategory> categories)
        {
            _jsonHelper.Save(_userCategoriesKey, categories);
        }

        public void AddUserCategory(UserCategory category)
        {
            var categories = LoadUserCategories();
            if (categories.Any(c => c.Name == category.Name))
            {
                throw new Exception("Category with this name already exists.");
            }
            categories.Add(category);
            SaveUserCategories(categories);
        }

        public void UpdateUserCategory(UserCategory category)
        {
            var categories = LoadUserCategories();
            var existingCategory = categories.FirstOrDefault(c => c.Name == category.Name);
            if (existingCategory == null)
            {
                throw new Exception("Category not found.");
            }
            existingCategory.Rights = category.Rights;
            SaveUserCategories(categories);
        }

        public void DeleteUserCategory(string categoryName)
        {
            var categories = LoadUserCategories();
            var category = categories.FirstOrDefault(c => c.Name == categoryName);
            if (category == null)
            {
                throw new Exception("Category not found.");
            }
            categories.Remove(category);
            SaveUserCategories(categories);
        }

        public UserCategory FindCategoryByName(string categoryName)
        {
            var categories = LoadUserCategories();
            return categories.FirstOrDefault(c => c.Name == categoryName);
        }
    }
}
