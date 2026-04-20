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
            routinesLBx.Items.Clear(); // clear the listbox before loading the routines to avoid duplicates
            
            using (var connection = new SqliteConnection("Data Source=C:\\temp\\gymData.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = "SELECT WorkoutName, DateCreated FROM SavedWorkouts ORDER BY DateCreated DESC"; // this grabs name and date of all workouts in the database and orders them by date created w the most recent at the top

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string workoutName = reader.GetString(0);
                        string date = reader.GetString(1);
                        routinesLBx.Items.Add($"{workoutName} ({date})");
                    }
                }
            }
        }

        private void savedRoutinesBackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner is MainWindow main)
            { 
                main.RefreshTodaysWorkout(); // refresh the main menu workout list in case a workout was deleted or edited
                main.Show(); // show the MainWindow that opened this
            }
            this.Close(); // close this window permanently to save memory
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (routinesLBx.SelectedItem != null)
            {
                string selectedItem = routinesLBx.SelectedItem.ToString(); // gets the txt from lbx
                string workoutName = selectedItem.Split('(')[0].Trim().ToLower(); // splitting by "(" to get name before date brackets

                using (var connection = new SqliteConnection("Data Source=C:\\temp\\gymData.db"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();

                    // now we will run delete cmd
                    command.CommandText = "DELETE FROM SavedWorkouts WHERE LOWER(WorkoutName) = $name";
                    command.Parameters.AddWithValue("$name", workoutName);

                    command.ExecuteNonQuery(); 
                }
                if (this.Owner is MainWindow main)
                {
                    main.RefreshTodaysWorkout(); // refresh the main menu workout list in case the deleted workout was for today
                }

                new customMbx($"Successfully deleted {workoutName}").Show();

                LoadAllRoutines();
            }
            else
            {
                new customMbx("Please select a workout to delete").Show();
            }
        }
    }
}
