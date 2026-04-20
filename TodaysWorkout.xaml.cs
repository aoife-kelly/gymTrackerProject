using Microsoft.Data.Sqlite;
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

namespace gymTracker
{
    /// <summary>
    /// Interaction logic for TodaysWorkout.xaml
    /// </summary>
    public partial class TodaysWorkout : Window
    {
        public TodaysWorkout()
        {
            InitializeComponent();
            LoadWorkoutData();
        }

        private void LoadWorkoutData()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            using (var connection = new SqliteConnection("Data Source=C:\\temp\\gymData.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT WorkoutName, Exercises FROM SavedWorkouts WHERE DateCreated = $date";
                command.Parameters.AddWithValue("$date", today);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // update labels/textblocks with the data from the database
                        workoutNameLabel.Content = reader.GetString(0);
                        exercisesTextBlock.Text = reader.GetString(1).Replace(",", "\n");
                    }
                }
            }
        }

        private void todaysWorkoutBackBtn_Click(object sender, RoutedEventArgs e)
        {
            // open main menu window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Owner = this; // set the owner of the main window to today's workout window
            mainWindow.Show();
            // close today's workout window
            this.Hide();
        }
    }
}
