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
    /// Interaction logic for PastWorkouts.xaml
    /// </summary>
    public partial class PastWorkouts : Window
    {
        public PastWorkouts()
        {
            InitializeComponent();
            LoadPastWorkouts();
            UpdateChart();
        }

        private void pastWorkoutsBackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner is MainWindow main)
            {
                main.Show(); // show the MainWindow that opened this
            }
            this.Close(); // close this window permanently to save memory
        }

        private void LoadPastWorkouts()
        {
            // this method will load past workouts from the database and display them in the listbox
            pastWorkoutsLBx.Items.Clear(); // clear the listbox before loading the workouts to avoid duplicates

            string today = DateTime.Now.ToString("yyyy-MM-dd");

            using (var connection = new SqliteConnection("Data Source=C:\\temp\\gymData.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                // only selecting workouts where date is before today, so we don't show today's workout in the past workouts list
                command.CommandText = "SELECT WorkoutName, Exercises, DateCreated FROM SavedWorkouts WHERE DateCreated < $date ORDER BY DateCreated DESC"; // this grabs name, exercises, and date of all workouts in the database that are before today and orders them by date created w the most recent at the top
                command.Parameters.AddWithValue("$date", today);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        string date = reader.GetString(1);
                        string exercises = reader.GetString(2);
                        pastWorkoutsLBx.Items.Add($"{name} ({date}): {exercises}");
                    }
                }
            }
        }

        private void pastWorkoutsLBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pastWorkoutsLBx.SelectedItem != null)
            {
                // reusing code again
                string selectedItem = pastWorkoutsLBx.SelectedItem.ToString();
                string workoutName = selectedItem.Split('(')[0].Trim();

                using (var connection = new SqliteConnection("Data Source=C:\\temp\\gymData.db"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();

                    // grab exercises column for the selected workout
                    command.CommandText = "SELECT Exercises FROM SavedWorkouts WHERE WorkoutName = $name";
                    command.Parameters.AddWithValue("$name", workoutName);

                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        exerciseDetailsTxt.Text = result.ToString().Replace(",", "\n"); // new line every time it hits a comma
                        exerciseDetailsTxt.FontStyle = FontStyles.Normal;
                        exerciseDetailsTxt.Foreground = Brushes.Black;
                    }
                }
            }
        }

        private void UpdateChart()
        {
            int[] weekCounts = new int[4]; // [0]=week1, [1]=week2, etc.
            DateTime today = DateTime.Now;

            using (var connection = new SqliteConnection("Data Source=C:\\temp\\gymData.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT DateCreated FROM SavedWorkouts WHERE DateCreated >= $limit";
                command.Parameters.AddWithValue("$limit", today.AddDays(-28).ToString("yyyy-MM-dd"));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (DateTime.TryParse(reader.GetString(0), out DateTime workoutDate))
                        {
                            double daysAgo = (today - workoutDate).TotalDays;

                            if (daysAgo >= 0) // ignore future workouts for this chart
                            {
                                if (daysAgo <= 7) weekCounts[0]++;
                                else if (daysAgo <= 14) weekCounts[1]++;
                                else if (daysAgo <= 21) weekCounts[2]++;
                                else if (daysAgo <= 28) weekCounts[3]++;
                            }
                        }
                    }

                    // update charts bar heights (multiply by 20 so 1 workout = 20 pixels high)
                    week1Bar.Height = weekCounts[0] * 20;
                    week2Bar.Height = weekCounts[1] * 20;
                    week3Bar.Height = weekCounts[2] * 20;
                    week4Bar.Height = weekCounts[3] * 20;
                }

            }
        }
    }
}