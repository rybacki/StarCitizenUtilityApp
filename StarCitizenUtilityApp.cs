using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Collections.ObjectModel;

namespace StarCitizenUtilityApp
{
    class Program
    {
        static readonly Dictionary<string, string> schedule = new()
        {
            ["2023-10-31T12:00:00"] = "Pacific Standard Time",
            ["2023-11-01T06:00:00"] = "Pacific Standard Time",
            ["2023-11-02T23:00:00"] = "Pacific Standard Time",
            ["2023-11-04T06:00:00"] = "Pacific Standard Time",
            ["2023-11-05T23:00:00"] = "Pacific Standard Time",
            ["2023-11-06T12:00:00"] = "Pacific Standard Time",
            ["2023-11-08T23:00:00"] = "Pacific Standard Time",
            ["2023-11-09T12:00:00"] = "Pacific Standard Time",
            ["2023-11-10T06:00:00"] = "Pacific Standard Time",
            // Add more schedule entries here
        };
        static void Main()
        {
            Console.WriteLine("Welcome to the Star Citizen Utility App.\n");
            Console.WriteLine("It is advisable to regularly clear your cache,\nespecially immediately following the release of a new game build or hotfix.\n");
            Console.WriteLine("1. Delete cache files for Star Citizen and Nvidia.");
            Console.WriteLine("2. Delete cache files for Star Citizen and AMD.");
            Console.WriteLine("3. Check Pyro Technical Preview Build server opening times. [Based on data from Nov 1st, 2023]");
            Console.WriteLine("4. Exit\n");
            Console.Write("Select an option: ");

            string? option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    DeleteNvidiaCacheFiles();
                    break;
                case "2":
                    DeleteAmdCacheFiles();
                    break;
                case "3":
                    CheckServerOpeningTimes();
                    break;
                case "4":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid option, please try again.");
                    break;
            }
            Console.WriteLine();
        }
        static void DeleteNvidiaCacheFiles()
        {
            ClearErrorLog();
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Star Citizen"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NVIDIA", "DXCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NVIDIA", "GLCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NVIDIA", "OptixCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D3DSCache"));

            Console.WriteLine("\nStar Citizen and Nvidia cache deletion process is complete.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        static void DeleteAmdCacheFiles()
        {
            ClearErrorLog();
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Star Citizen"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AMD", "DXCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AMD", "GLCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AMD", "VkCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D3DSCache"));

            Console.WriteLine("\nStar Citizen and AMD cache deletion process is complete.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        static void DeleteCacheFolder(string path)
        {
            string folderName = Path.GetFileName(path);
            bool isFolderExistent = Directory.Exists(path);

            if (isFolderExistent)
            {
                DirectoryInfo di = new DirectoryInfo(path);

                // Delete all files and subdirectories
                foreach (FileInfo file in di.GetFiles("*", SearchOption.AllDirectories))
                {
                    try
                    {
                        file.Delete();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{file.Name} deleted.");
                        Console.ResetColor();
                    }
                    catch (IOException)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Cannot delete (in use): {file.Name}");
                        Console.ResetColor();
                        File.AppendAllText("error_log.txt", $"{DateTime.Now}: Cannot delete (in use): {file.FullName}\n");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Failed to delete: {file.Name} - Error: {ex.Message}");
                        Console.ResetColor();
                        File.AppendAllText("error_log.txt", $"{DateTime.Now}: Failed to delete: {file.FullName} - Error: {ex.Message}\n");
                    }
                }

                // Delete directories if they are empty after file deletions
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    try
                    {
                        dir.Delete(true); // true => delete recursively
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{dir.Name} deleted.");
                        Console.ResetColor();
                    }
                    catch (IOException)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Cannot delete (in use): {dir.Name}");
                        Console.ResetColor();
                        File.AppendAllText("error_log.txt", $"{DateTime.Now}: Cannot delete (in use): {dir.FullName}\n");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Failed to delete: {dir.Name} - Error: {ex.Message}");
                        Console.ResetColor();
                        File.AppendAllText("error_log.txt", $"{DateTime.Now}: Failed to delete: {dir.FullName} - Error: {ex.Message}\n");
                    }
                }

                // Check if any files or directories are left
                if (Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length > 0 || Directory.GetDirectories(path).Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{folderName} partially cleared. Some items could not be deleted (see error log for details).");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{folderName} completely cleared.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{folderName} does not exist or has already been deleted.");
                Console.ResetColor();
            }
        }
        static void CheckServerOpeningTimes()
        {
            // Display local timezone
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            Console.WriteLine($"Your Local Timezone: {localZone.StandardName}");

            // Calculate and display the next opening time and time left until the server opens
            DateTime? nextOpeningTime = null;
            TimeSpan? timeLeft = null;
            foreach (var entry in schedule)
            {
                DateTime utcTime = DateTime.ParseExact(entry.Key, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, localZone);

                if (localTime > DateTime.Now)
                {
                    nextOpeningTime = localTime;
                    timeLeft = nextOpeningTime - DateTime.Now;
                    break;
                }
            }

            if (nextOpeningTime.HasValue) // Check if nextOpeningTime has a value
            {
                timeLeft = nextOpeningTime.Value - DateTime.Now;
                Console.Write("The next Pyro Technical Preview Build server is scheduled to open on ");
                Console.ForegroundColor = ConsoleColor.Green; // Set text color to green
                Console.WriteLine(nextOpeningTime.Value.ToString("f", CultureInfo.CreateSpecificCulture("en-US")));
                Console.ResetColor(); // Reset to default color

                Console.Write("Time remaining: ");
                Console.ForegroundColor = ConsoleColor.Green; // Set text color to green
                Console.WriteLine($"{timeLeft.Value.Days} days, {timeLeft.Value.Hours} hours, {timeLeft.Value.Minutes} minutes, and {timeLeft.Value.Seconds} seconds");
                Console.ResetColor(); // Reset to default color
            }
            else
            {
                Console.WriteLine("No upcoming server openings are scheduled or the schedule is out of date.");
            }


            // Ask user if they want to check a different timezone
            Console.Write("Would you like to check a different time zone? (yes/no): ");
            string response = Console.ReadLine()?.ToLower() ?? string.Empty;


            if (response == "yes")
            {
                ListTimeZoneOptions();
            }
            else
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
        static void ListTimeZoneOptions()
        {
            ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
            for (int i = 0; i < timeZones.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {timeZones[i].DisplayName}");
            }

            Console.WriteLine("Enter the number of the time zone you want to check:");
            if (int.TryParse(Console.ReadLine(), out int timeZoneSelection) && timeZoneSelection >= 1 && timeZoneSelection <= timeZones.Count)
            {
                DisplayTimesForSelectedTimeZone(timeZones[timeZoneSelection - 1]);
            }
            else
            {
                Console.WriteLine("Invalid selection. Please enter a number from the list.");
            }
        }
        static void DisplayTimesForSelectedTimeZone(TimeZoneInfo selectedTimeZone)
        {
            DateTime? nextOpeningTime = null;
            TimeSpan? timeLeft = null;

            // Calculate the next opening time in the selected timezone
            foreach (var entry in schedule)
            {
                DateTime utcTime = DateTime.ParseExact(entry.Key, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                DateTime selectedZoneTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, selectedTimeZone);

                if (selectedZoneTime > DateTime.UtcNow)
                {
                    nextOpeningTime = selectedZoneTime;
                    timeLeft = nextOpeningTime - TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, selectedTimeZone);
                    break;
                }
            }

            // Display results in the specified format
            Console.WriteLine($"Chosen Timezone: {selectedTimeZone.DisplayName}");

            if (nextOpeningTime.HasValue && timeLeft.HasValue)
            {
                string formattedTimeLeft = $"{timeLeft.Value.Days} days, {timeLeft.Value.Hours} hours, {timeLeft.Value.Minutes} minutes, and {timeLeft.Value.Seconds} seconds";

                Console.Write("The upcoming Pyro Technical Preview Build server is set to launch on ");
                Console.ForegroundColor = ConsoleColor.Green; // Set text color to green
                Console.WriteLine(nextOpeningTime.Value.ToString("f", CultureInfo.CreateSpecificCulture("en-US")));
                Console.ResetColor(); // Reset to default color

                Console.Write("Time remaining: ");
                Console.ForegroundColor = ConsoleColor.Green; // Set text color to green
                Console.WriteLine(formattedTimeLeft);
                Console.ResetColor(); // Reset to default color
            }
            else
            {
                Console.WriteLine("No upcoming server openings are scheduled or the schedule is out of date.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        static void ClearErrorLog()
        {
            string errorLogPath = "error_log.txt";
            if (File.Exists(errorLogPath))
            {
                try
                {
                    File.Delete(errorLogPath);
                    Console.WriteLine("Previous error logs cleared.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to clear error logs. Error: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}
