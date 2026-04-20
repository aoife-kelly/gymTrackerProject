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
    /// Interaction logic for CreateWorkout.xaml
    /// </summary>
    public partial class CreateWorkout : Window
    {
        private System.Threading.CancellationTokenSource _cts; // declare a CancellationTokenSource to manage cancellation of the API call
        private List<string> currentWorkoutExercises = new List<string>();
        public CreateWorkout()
        {
            InitializeComponent();
            workoutDatePicker.SelectedDate = DateTime.Now; // set the date picker to default to today's date when the window opens
        }

        private void menuBackBtn_Click(object sender, RoutedEventArgs e)
        {
            // open MainWindow window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Owner = this; // set the owner of the main window to this create workout window
            mainWindow.Show();
            // close this create workout window
            this.Hide();
        }

        private void exerciseSearchTbx_GotFocus(object sender, RoutedEventArgs e)
        {
            {
                exerciseSearchTbx.Tag = "";
            }
        }
        // show placeholder text when username textbox loses focus and is empty
        private void exerciseSearchTbx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(exerciseSearchTbx.Text))
            {
                exerciseSearchTbx.Tag = "Search...";
            }
        }

        private void workoutNameTbx_GotFocus(object sender, RoutedEventArgs e)
        {
            workoutNameTbx.Tag = "";
        }

        private void workoutNameTbx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(workoutNameTbx.Text))
            {
                workoutNameTbx.Tag = "Enter workout name...";
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = workoutNameTbx.Text;

            //turn list of exercises into one string ("squats - sets x reps ")
            string exercises = string.Join(", ", currentWorkoutExercises);

            // get date from date picker
            string selectedDate = workoutDatePicker.SelectedDate?.ToString("yyyy-MM-dd"); // format the date as a string in the format "yyyy-MM-dd" which is a common format for storing dates in databases

            // this will create the database file if it doesn't exist, and create the SavedWorkouts table if it doesn't exist, then insert the workout data
            using (var connection = new SqliteConnection("Data Source=C:\\temp\\gymData.db"))
            {
                connection.Open();

                var createTableCmd = connection.CreateCommand(); // create table if its missing
                createTableCmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS SavedWorkouts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    WorkoutName TEXT NOT NULL,
                    Exercises TEXT NOT NULL,
                    DateCreated TEXT NOT NULL
                    );";
                createTableCmd.ExecuteNonQuery(); // execute the command to create the table if it doesn't exist

                var command = connection.CreateCommand(); // create a command object to execute SQL commands against the database
                command.CommandText = "INSERT INTO SavedWorkouts (WorkoutName, Exercises, DateCreated) VALUES ($name, $ex, $date)"; // use parameterized query to prevent SQL injection [[copilot suggestion?]] this is important for security, especially if you ever expand this app (which iwill) to allow users to input more data or share workouts with others

                command.Parameters.AddWithValue("$name", name); // add the workout name parameter
                command.Parameters.AddWithValue("$ex", exercises); // add the exercises parameter
                command.Parameters.AddWithValue("$date", selectedDate); // add the date parameter

                command.ExecuteNonQuery(); // execute the command to insert the workout into the database
            }

            new customMbx("Workout successfully saved for today!").Show();

            if (this.Owner != null) // if the owner window (MainWindow) is still open, refresh the workout list to show the newly added workout
            {
                if (this.Owner is MainWindow mainMenu) // check if the owner window is of type MainWindow, and if so we can call the RefreshTodaysWorkout method to update the workout list on the main menu
                {
                    mainMenu.RefreshTodaysWorkout(); // call the method in MainWindow to refresh the workout list, this will show the newly added workout without needing to restart the app
                }
                this.Owner.Show(); // show the owner window (MainWindow) again after saving the workout
            }

            this.Close(); // close the window after saving
        }

        private async void exerciseSearchTbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            _cts?.Cancel();
            _cts = new System.Threading.CancellationTokenSource();

            try
            {
                await Task.Delay(500, _cts.Token);

                string query = exerciseSearchTbx.Text;

                if (string.IsNullOrWhiteSpace(query))
                {
                    exercisesLBx.ItemsSource = null;
                    exercisesLBx.Visibility = Visibility.Collapsed; // hide results if search box is empty
                    return;
                }

                var results = await API.SearchExercises(query);

                if (results.Any())
                {
                    exercisesLBx.ItemsSource = results.Select(r => r.name).ToList();
                    exercisesLBx.Visibility = Visibility.Visible; // show results if there are any
                }
                else
                {
                    exercisesLBx.Visibility = Visibility.Collapsed;
                }
            }
            catch (TaskCanceledException) { }
        }
        private void addExerciseBtn_Click(object sender, RoutedEventArgs e)
        {
            // get the selected item from the search results (exercisesLBx)
            var selectedExercise = exercisesLBx.SelectedItem as string;

            if (selectedExercise != null)
            {
                // grab the numbers but check if they are still just the placeholder labels
                string sets = (setsTbx.Text == setsTbx.Tag.ToString()) ? "0" : setsTbx.Text;
                string reps = (repsTbx.Text == repsTbx.Tag.ToString()) ? "0" : repsTbx.Text;

                // combine it all into one string for the list
                string formattedExercise = $"{selectedExercise} - {sets} sets x {reps} reps";

                // add to our list
                currentWorkoutExercises.Add(formattedExercise);

                // refresh the display ListBox (addedExercisesLBx)
                addedExercisesLBx.ItemsSource = null;
                addedExercisesLBx.ItemsSource = currentWorkoutExercises;

                // clear everything and reset placeholders
                exercisesLBx.Visibility = Visibility.Collapsed;

                exerciseSearchTbx.Text = exerciseSearchTbx.Tag.ToString();
                setsTbx.Text = setsTbx.Tag.ToString();
                repsTbx.Text = repsTbx.Tag.ToString();

                // set focus back to the search box for our convenience
                exerciseSearchTbx.Focus();
            }
            else
            {
                MessageBox.Show("Please select an exercise from the search results list first!");
            }
        }

        private void setsTbx_GotFocus(object sender, RoutedEventArgs e)
        {
            setsTbx.Tag = "";
        }

        private void setsTbx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(setsTbx.Text))
            {
                setsTbx.Tag = "Sets";
            }
        }

        private void repsTbx_GotFocus(object sender, RoutedEventArgs e)
        {
            repsTbx.Tag = "";
        }

        private void repsTbx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(repsTbx.Text))
            {
                repsTbx.Tag = "Reps";
            }
        }
    }
}
