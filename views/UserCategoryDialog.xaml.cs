using System.Collections.Generic;
using System.Windows;
using IpisCentralDisplayController.models;

namespace IpisCentralDisplayController.views
{
    public partial class UserCategoryDialog : Window
    {
        public UserCategory UserCategory { get; private set; }

        public UserCategoryDialog(UserCategory category = null)
        {
            InitializeComponent();
            if (category != null)
            {
                CategoryNameTextBox.Text = category.Name;
                RightsListBox.SelectedItems.Clear();
                foreach (var right in category.Rights)
                {
                    RightsListBox.SelectedItems.Add(right);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRights = new List<UserRights>();
            foreach (var item in RightsListBox.SelectedItems)
            {
                selectedRights.Add((UserRights)item);
            }

            UserCategory = new UserCategory
            {
                Id = UserCategory?.Id ?? System.Guid.NewGuid(),
                Name = CategoryNameTextBox.Text,
                Rights = selectedRights
            };

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RightsListBox.SelectAll();
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // No action needed for unchecking "Select All"
            RightsListBox.UnselectAll();
        }

        private void PredefinedConfigComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (PredefinedConfigComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem && selectedItem.Content != null)
            {
                var configName = selectedItem.Content.ToString();
                switch (configName)
                {
                    case "Admin":
                        SetAdminRights();
                        break;
                    case "Guest":
                        SetGuestRights();
                        break;
                    case "IPIS Operator":
                        SetIPISOperatorRights();
                        break;
                    default:
                        //RightsListBox.UnselectAll();
                        break;
                }
            }
        }

        private void SetAdminRights()
        {
            RightsListBox.UnselectAll();
            foreach (var right in (UserRights[])System.Enum.GetValues(typeof(UserRights)))
            {
                RightsListBox.SelectedItems.Add(right);
            }
        }

        private void SetGuestRights()
        {
            RightsListBox.UnselectAll();
            var guestRights = new List<UserRights>
            {
                UserRights.UserCategoriesRead,
                UserRights.UserRead,
                UserRights.StationInfoRead,
                UserRights.DisplaysRead,
                UserRights.NetworkConfigurationRead,
                UserRights.TrainInfoRead,
                UserRights.SoundsPlay,
                UserRights.PlaylistRead,
                UserRights.MediaRead,
                UserRights.ReportsRead
            };

            foreach (var right in guestRights)
            {
                RightsListBox.SelectedItems.Add(right);
            }
        }

        private void SetIPISOperatorRights()
        {
            RightsListBox.UnselectAll();
            var operatorRights = new List<UserRights>
            {
                UserRights.TrainInfoCreate,
                UserRights.TrainInfoRead,
                UserRights.TrainInfoUpdate,
                UserRights.TrainInfoDelete,
                UserRights.SoundsRecord,
                UserRights.SoundsPlay,
                UserRights.SoundsDelete,
                UserRights.PlaylistCreate,
                UserRights.PlaylistRead,
                UserRights.PlaylistUpdate,
                UserRights.PlaylistDelete
            };

            foreach (var right in operatorRights)
            {
                RightsListBox.SelectedItems.Add(right);
            }
        }
    }
}
