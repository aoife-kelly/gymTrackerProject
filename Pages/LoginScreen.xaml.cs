using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace gymTracker.Pages
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        public LoginScreen()
        {
            InitializeComponent();
        }
        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            string correctUsername = "aoifekelly";
            string correctPassword = "password123";

            if (usernameTbx.Text == correctUsername && passwordPbx.Password == correctPassword)
            { // open MainWindow window
                MainWindow mainWindow = new MainWindow();
                mainWindow.Owner = this; // set the owner of the main window to this login window
                mainWindow.Show();
                // close this login window
                this.Close();
            }
            else
            {
                new customMbx("Incorrect username or password. Please try again.").ShowDialog();
            }
        }
        // hide placeholder text when username textbox gets focus
        private void usernameTbx_GotFocus(object sender, RoutedEventArgs e)
        {
            {
                usernameTbx.Tag = "";
            }
        }
        // show placeholder text when username textbox loses focus and is empty
        private void usernameTbx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameTbx.Text))
            {
                usernameTbx.Tag = "Enter Username";
            }
        }

    }
}
