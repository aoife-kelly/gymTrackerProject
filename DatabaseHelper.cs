using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace gymTracker
{
    public static class DatabaseHelper // i used ai for quite a bit of help w implementing the db
    {
        // This sets the path to your project folder
        private static string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gymData.db"); // this will create the database file in the same directory as the executable

        public static void InitializeDatabase()
        {
            SQLitePCL.Batteries.Init(); // had to troubleshoot this w copilot, it was giving me an error about not being able to find the SQLite library, hopefully this fixes it [it did]
            // creates the file if it doesn't exist
            using (var connection = new SqliteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // create a table to store the workout name, the list of exercises, and the date
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS SavedWorkouts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    WorkoutName TEXT NOT NULL,
                    Exercises TEXT NOT NULL,
                    Sets INTEGER, -- New column
                    Reps INTEGER, -- New column
                    DateCreated TEXT NOT NULL
                    );;";
                command.ExecuteNonQuery(); // execute the command to create the table if it doesn't exist
            }
        }
    }
}
