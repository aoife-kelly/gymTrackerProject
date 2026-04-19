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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gymTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase();
            RefreshTodaysWorkout();
        }

        private void menuBackBtn_Click(object sender, RoutedEventArgs e)
        {
            // open LoginScreen window
            LoginScreen loginScreen = new LoginScreen();
            loginScreen.Owner = this; // set the owner of the login screen to this main window
            loginScreen.Show();
            // hide this main window
            this.Hide();
        }

        private void createWorkoutBtn_Click(object sender, RoutedEventArgs e)
        {
            // open CreateWorkout window
            CreateWorkout createWorkout = new CreateWorkout();
            createWorkout.Owner = this; // set the owner of the create workout window to this main window
            createWorkout.Show();
            this.Hide();
        }

        private void todaysWorkout_Click(object sender, RoutedEventArgs e)
        {
            if (todaysWorkout.Content.ToString() == "Rest Day" ||
                todaysWorkout.Content.ToString() == "Todays Workout") // here we check if the button content is "Rest Day" or "Todays Workout" which would indicate that there is no workout for today in the database, and if so we send the user to the create workout window to create a workout for today. If there is a workout for today in the database, then the button content will be the name of the workout, and we can send the user to the today's workout window to view the workout details.
            {
                // option 1 - no workout exists for today, send user to create one @ createWorkout window
                CreateWorkout createWorkout = new CreateWorkout();
                createWorkout.Owner = this; // set the owner of the create workout window to this main window
                createWorkout.Show();
                // hide this main window
                this.Hide();
            }
            else
            {
                // option 2 - a workout exists so we open the todaysWorkout window
                TodaysWorkout todaysWorkoutWindow = new TodaysWorkout();
                todaysWorkoutWindow.Owner = this; // set the owner of the today's workout window to this main window
                todaysWorkoutWindow.Show();
                // hide this main window
                this.Hide();
            }
        }

        private void savedRoutinesBtn_Click(object sender, RoutedEventArgs e)
        {
            SavedRoutines savedRoutines = new SavedRoutines();
            savedRoutines.Owner = this; // set the owner of the saved routines window to this main window
            savedRoutines.Show();
            this.Hide();
        }

        public void RefreshTodaysWorkout()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                using (var connection = new SqliteConnection("Data Source=C:\\temp\\gymData.db"))
                {
                    connection.Open(); // this will create the database file if it doesn't exist, and create the SavedWorkouts table if it doesn't exist, then insert the workout data
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT WorkoutName FROM SavedWorkouts WHERE DateCreated = $date LIMIT 1";
                    command.Parameters.AddWithValue("$date", today);

                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        todaysWorkout.Content = result.ToString(); // this will set the button content to the workout name for today if it exists in the database
                    }
                    else
                    {
                        todaysWorkout.Content = "Rest Day"; // if there is no workout for today in the database, we can show "Rest Day" or something similar to indicate that there is no workout scheduled
                    }
                }
            }
            catch (Exception)
            {
                todaysWorkout.Content = "Error Loading";
            }
        }

    }
}
