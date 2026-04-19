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
using Microsoft.Data.Sqlite;


namespace gymTracker
{
    /// <summary>
    /// Interaction logic for SavedRoutines.xaml
    /// </summary>
    public partial class SavedRoutines : Window
    {
        public SavedRoutines()
        {
            InitializeComponent();
            LoadAllRoutines();
        }

        private void LoadAllRoutines()
        {
            using (var connection = new SqliteConnection("Data Source=gymData.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT WorkoutName FROM SavedWorkouts";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string workoutName = reader.GetString(0);
                        routinesLBx.Items.Add(workoutName);
                    }
                }
            }
        }

        private void savedRoutinesBackBtn_Click(object sender, RoutedEventArgs e)
        {
            // open main menu window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Owner = this; // set the owner of the main window to this saved routines window
            mainWindow.Show();
            // close this saved routines window
            this.Hide();
        }
    }
}
