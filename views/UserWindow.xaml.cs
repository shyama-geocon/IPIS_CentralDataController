using System;
using System.Windows;
using IpisCentralDisplayController.Managers;
using IpisCentralDisplayController.models;

namespace IpisCentralDisplayController.views
{
    public partial class UserWindow : Window
    {
        private readonly UserManager _userManager;
        private readonly UserCategoryManager _userCategoryManager;
        private User _user;

        public UserWindow(UserManager userManager, UserCategoryManager userCategoryManager, User user = null)
        {
            InitializeComponent();
            _userManager = userManager;
            _userCategoryManager = userCategoryManager;
            _user = user;
            LoadUserCategories();
            if (_user != null)
            {
                LoadUserData();
            }
        }

        private void LoadUserCategories()
        {
            var categories = _userCategoryManager.LoadUserCategories();
            CategoryComboBox.ItemsSource = categories;
            if (categories.Count > 0)
            {
                CategoryComboBox.SelectedIndex = 0;
            }
        }

        private void LoadUserData()
        {
            EmailTextBox.Text = _user.Email;
            NameTextBox.Text = _user.Name;
            PhoneTextBox.Text = _user.Phone;
            DesignationTextBox.Text = _user.Designation;
            PasswordBox.Password = _user.Password;
            IsActiveCheckBox.IsChecked = _user.IsActive;
            CategoryComboBox.SelectedItem = _user.Category;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(EmailTextBox.Text) ||
                string.IsNullOrEmpty(NameTextBox.Text) ||
                string.IsNullOrEmpty(PhoneTextBox.Text) ||
                string.IsNullOrEmpty(DesignationTextBox.Text) ||
                string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_user == null)
            {
                _user = new User
                {
                    CreatedDate = DateTime.Now,
                    LastLogin = DateTime.Now
                };
            }

            _user.Email = EmailTextBox.Text;
            _user.Name = NameTextBox.Text;
            _user.Phone = PhoneTextBox.Text;
            _user.Designation = DesignationTextBox.Text;
            _user.Password = PasswordBox.Password;
            _user.IsActive = IsActiveCheckBox.IsChecked ?? false;
            _user.Category = CategoryComboBox.SelectedItem as UserCategory;

            if (_userManager.FindUserByEmail(_user.Email) == null)
            {
                _userManager.AddUser(_user);
                MessageBox.Show("User created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _userManager.UpdateUser(_user);
                MessageBox.Show("User updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Close();
        }
    }
}
