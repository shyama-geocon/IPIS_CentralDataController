using System.Collections.Generic;
using System.Windows;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.Managers;

namespace IpisCentralDisplayController.views
{
    public partial class UserDialog : Window
    {
        private readonly UserCategoryManager _userCategoryManager;

        public User User { get; private set; }

        public UserDialog(UserCategoryManager userCategoryManager, User user = null)
        {
            InitializeComponent();
            _userCategoryManager = userCategoryManager;

            CategoryComboBox.ItemsSource = _userCategoryManager.LoadUserCategories();

            if (user != null)
            {
                User = user;
                EmailTextBox.Text = user.Email;
                NameTextBox.Text = user.Name;
                PhoneTextBox.Text = user.Phone;
                DesignationTextBox.Text = user.Designation;
                CategoryComboBox.SelectedItem = user.Category;
                PasswordBox.Password = user.Password;
                IsActiveCheckBox.IsChecked = user.IsActive;
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                CategoryComboBox.SelectedItem == null)
            {
                ErrorMessageTextBlock.Text = "Please fill in all required fields.";
                return;
            }

            var selectedCategory = CategoryComboBox.SelectedItem as UserCategory;

            User = new User
            {
                Email = EmailTextBox.Text,
                Name = NameTextBox.Text,
                Phone = PhoneTextBox.Text,
                Designation = DesignationTextBox.Text,
                Category = selectedCategory,
                Password = PasswordBox.Password,
                IsActive = IsActiveCheckBox.IsChecked ?? false,
                CreatedDate = User?.CreatedDate ?? System.DateTime.Now,
                LastLogin = User?.LastLogin ?? System.DateTime.Now
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
