using IpisCentralDisplayController.models;
using IpisCentralDisplayController.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using IpisCentralDisplayController.views;
using System.Windows;

namespace IpisCentralDisplayController.Managers
{
    public class UserManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _usersKey = "users";
        public User CurrentUser { get; private set; }

        public UserManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public List<User> LoadUsers()
        {
            return _jsonHelper.Load<List<User>>(_usersKey) ?? new List<User>();
        }

        public void SaveUsers(List<User> users)
        {
            _jsonHelper.Save(_usersKey, users);
        }

        public void AddUser(User user)
        {
            var users = LoadUsers();
            if (users.Any(u => u.Email == user.Email))
            {
                throw new Exception("User with this email already exists.");
            }
            users.Add(user);
            SaveUsers(users);
        }

        public void UpdateUser(User user)
        {
            var users = LoadUsers();
            var existingUser = users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser == null)
            {
                throw new Exception("User not found.");
            }
            // Update user properties here
            existingUser.Name = user.Name;
            existingUser.Phone = user.Phone;
            existingUser.Designation = user.Designation;
            existingUser.Category = user.Category;
            existingUser.Password = user.Password;
            existingUser.IsActive = user.IsActive;
            existingUser.LastLogin = user.LastLogin;

            SaveUsers(users);
        }

        public void DeleteUser(string email)
        {
            var users = LoadUsers();
            var user = users.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            users.Remove(user);
            SaveUsers(users);
        }

        public void DeleteAllUsers()
        {
            var users = LoadUsers();
            users.Clear();
            SaveUsers(users);
        }

        public User FindUserByEmail(string email)
        {
            var users = LoadUsers();
            return users.FirstOrDefault(u => u.Email == email);
        }

        public bool ValidateUserLogin(string email, string password)
        {
            var user = FindUserByEmail(email);
            if (user == null || user.Password != password)
            {
                return false;
            }
            user.LastLogin = DateTime.Now;
            SaveUsers(LoadUsers());
            CurrentUser = user;
            return true;
        }

        //public void CheckAndPromptForAdminUser()
        //{
        //    var users = LoadUsers();
        //    if (!users.Any())
        //    {
        //        MessageBox.Show("No users found. Please create an admin user.", "Admin User Creation", MessageBoxButton.OK, MessageBoxImage.Information);
        //        var userCategoryManager = new UserCategoryManager(new SettingsJsonHelperAdapter());
        //        var userWindow = new UserWindow(this, userCategoryManager);
        //        userWindow.ShowDialog();
        //    }
        //}

        //public void CheckAndPromptForAdminUser()
        //{
        //    var users = LoadUsers();
        //    if (!users.Any(u => u.Category.Name == "Admin"))
        //    {
        //        MessageBox.Show("No admin users found. Please create an admin user.", "Admin User Creation", MessageBoxButton.OK, MessageBoxImage.Information);
        //        var userCategoryManager = new UserCategoryManager(new SettingsJsonHelperAdapter());

        //        while (!users.Any(u => u.Category.Name == "Admin"))
        //        {
        //            var userWindow = new UserWindow(this, userCategoryManager);
        //            bool? result = userWindow.ShowDialog();
        //            if (result == true)
        //            {
        //                users = LoadUsers();
        //            }
        //        }
        //    }
        //}

        public void CheckAndPromptForAdminUser()
        {
            var users = LoadUsers();
            if (!users.Any(u => u.Category?.Name == "Admin"))
            {
                MessageBox.Show("No admin users found. Please create an admin user.", "Admin User Creation", MessageBoxButton.OK, MessageBoxImage.Information);
                var userCategoryManager = new UserCategoryManager(new SettingsJsonHelperAdapter());

                do
                {
                    var userWindow = new UserWindow(this, userCategoryManager);
                    bool? result = userWindow.ShowDialog();
                    users = LoadUsers();
                }
                while (!users.Any(u => u.Category?.Name == "Admin"));
            }
        }


        public void ActivateUser(string email, bool isActive)
        {
            var user = FindUserByEmail(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            user.IsActive = isActive;
            SaveUsers(LoadUsers());
        }

        public void AssignUserCategory(string email, string categoryName)
        {
            var user = FindUserByEmail(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            var categoryManager = new UserCategoryManager(_jsonHelper); // Assuming UserCategoryManager is already implemented
            var category = categoryManager.FindCategoryByName(categoryName);
            if (category == null)
            {
                throw new Exception("Category not found.");
            }
            user.Category = category;
            SaveUsers(LoadUsers());
        }
    }
}
