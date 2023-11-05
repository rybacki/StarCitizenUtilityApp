using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Collections.ObjectModel;

namespace StarCitizenUtilityApp
{
    class Program
    {
        private static Dictionary<string, string> schedule = new Dictionary<string, string>
        {
            // The time string format is directly in UTC, following the "yyyy-MM-ddTHH:mm:ss" pattern.
            ["2023-10-31T19:00:00"] = "UTC", // 12 PM Pacific is 7 PM UTC on the same day
            ["2023-11-01T13:00:00"] = "UTC", // 6 AM Pacific is 1 PM UTC on the same day
            ["2023-11-03T06:00:00"] = "UTC", // 11 PM Pacific (Nov 2) is 6 AM UTC the next day (Nov 3)
            ["2023-11-04T13:00:00"] = "UTC", // 6 AM Pacific is 1 PM UTC on the same day
            ["2023-11-06T07:00:00"] = "UTC", // 11 PM Pacific (Nov 5) is 7 AM UTC the next day (Nov 6)
            ["2023-11-06T20:00:00"] = "UTC", // 12 PM Pacific is 8 PM UTC on the same day
            ["2023-11-09T07:00:00"] = "UTC", // 11 PM Pacific (Nov 8) is 7 AM UTC the next day (Nov 9)
            ["2023-11-09T20:00:00"] = "UTC", // 12 PM Pacific is 8 PM UTC on the same day
            ["2023-11-10T14:00:00"] = "UTC", // 6 AM Pacific is 2 PM UTC on the same day
        };
        static void Main()
        {
            bool running = true;

            while (running)
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
                        running = false; // Setting running to false to exit the while loop and end the application
                        Console.Clear();
                        Console.WriteLine("           ________");
                        Console.WriteLine("          |        \\");
                        Console.WriteLine("  ______   \\$$$$$$$$");
                        Console.WriteLine(" /      \\     /  $$");
                        Console.WriteLine("|  $$$$$$\\   /  $$");
                        Console.WriteLine("| $$  | $$  /  $$");
                        Console.WriteLine("| $$__/ $$ /  $$");
                        Console.WriteLine(" \\$$    $$|  $$");
                        Console.WriteLine("  \\$$$$$$  \\$$");
                        Console.WriteLine();
                        Console.WriteLine("o7 See you in the 'verse!");
                        Console.WriteLine("App will close in 2 seconds...");
                        Thread.Sleep(2000); // Wait for 2 seconds
                        break;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
                Console.Clear(); // Clear the console for a clean slate after any option except exit.
            }
        }
        static void DeleteNvidiaCacheFiles()
        {
            Console.Clear();
            ClearErrorLog();
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Star Citizen"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NVIDIA", "DXCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NVIDIA", "GLCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NVIDIA", "OptixCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D3DSCache"));

            Console.WriteLine("\nStar Citizen and Nvidia cache deletion process is complete.");
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            Console.Clear(); 
        }
        static void DeleteAmdCacheFiles()
        {
            Console.Clear();
            ClearErrorLog();
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Star Citizen"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AMD", "DXCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AMD", "GLCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AMD", "VkCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D3DSCache"));

            Console.WriteLine("\nStar Citizen and AMD cache deletion process is complete.");
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
            Console.Clear(); 
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
            Console.Clear();
            // Display local timezone
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            Console.WriteLine($"Your Local Timezone: {localZone.StandardName}");

            // Calculate and display the next opening time and time left until the server opens
            DateTime? nextOpeningTime = null;
            TimeSpan? timeLeft = null;
            bool isServerOpen = false;

            DateTime localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);

            foreach (var entry in schedule)
            {
                DateTime utcTime = DateTime.ParseExact(entry.Key, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                DateTime localStartTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local); // Convert UTC opening time to local time
                DateTime localEndTime = localStartTime.AddHours(8); // Server is open for 8 hours

                if (localNow >= localStartTime && localNow <= localEndTime)
                {
                    nextOpeningTime = localEndTime;
                    timeLeft = nextOpeningTime - localNow;
                    isServerOpen = true;
                    break;
                }
                else if (localNow < localStartTime)
                {
                    nextOpeningTime = localStartTime;
                    timeLeft = nextOpeningTime - localNow;
                    break;
                }
            }
            // When the server is open
            if (isServerOpen && timeLeft.HasValue)
            {
                Console.Write("The Pyro Technical Preview Build server is ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("OPEN now!");
                Console.ResetColor();
                Console.WriteLine();
                Console.Write("Test time remaining: ");
                Console.ForegroundColor = ConsoleColor.Green;
                TimeSpan timeLeftValue = timeLeft.GetValueOrDefault(); // Safe to use as we checked HasValue above
                Console.WriteLine($"{timeLeftValue.Hours} hours, {timeLeftValue.Minutes} minutes, and {timeLeftValue.Seconds} seconds");
                Console.ResetColor();
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey();
                Console.Clear(); 
            }
            else if (nextOpeningTime.HasValue)
            {
                Console.Write("The next Pyro Technical Preview Build server is scheduled to open on ");
                Console.ForegroundColor = ConsoleColor.Green;
                DateTime nextOpeningTimeValue = nextOpeningTime.GetValueOrDefault(); // This is already in local time
                Console.WriteLine(nextOpeningTimeValue.ToString("f", CultureInfo.CreateSpecificCulture("en-US")));
                Console.ResetColor();
                Console.Write("Time remaining until opening: ");
                Console.ForegroundColor = ConsoleColor.Green;
                TimeSpan timeLeftValue = timeLeft.GetValueOrDefault();
                Console.WriteLine($"{timeLeftValue.Days} days, {timeLeftValue.Hours} hours, {timeLeftValue.Minutes} minutes, and {timeLeftValue.Seconds} seconds");
                Console.ResetColor();

                // Ask user if they want to check a different timezone only if the server is not open
                Console.Write("Would you like to check a different time zone? (yes/no): ");
                string response = Console.ReadLine()?.ToLower() ?? string.Empty;

                if (response == "yes")
                {
                    ListTimeZoneOptions();
                }
                else
                {
                    Console.WriteLine("Press any key to return to the main menu...");
                    Console.ReadKey();
                    Console.Clear(); 
                }
            }
            else
            {
                Console.WriteLine("No upcoming server openings are scheduled or the schedule is out of date.");
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey();
                Console.Clear(); 
            }
        }

        static void AskForTimeZoneCheck()
        {
            Console.Write("Would you like to check the server status in a different time zone? (yes/no): ");
            string response = Console.ReadLine()?.ToLower() ?? string.Empty;

            if (response == "yes")
            {
                ListTimeZoneOptions();
            }
            else
            {
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey();
                Console.Clear(); 
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
            Console.Clear();
            DateTime? nextOpeningTime = null;
            TimeSpan? timeLeft = null;

            // Get the current time in the selected time zone
            DateTime nowInSelectedTimeZone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, selectedTimeZone);

            // Calculate the next opening time in the selected timezone
            foreach (var entry in schedule)
            {
                DateTime utcTime = DateTime.ParseExact(entry.Key, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                DateTime selectedZoneTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, selectedTimeZone);

                // We compare the times in the selected time zone
                if (nowInSelectedTimeZone < selectedZoneTime)
                {
                    nextOpeningTime = selectedZoneTime;
                    // Calculate time left until the server opens in the selected time zone
                    timeLeft = nextOpeningTime.Value - nowInSelectedTimeZone;
                    break;
                }
            }

            // Display results
            Console.WriteLine($"Chosen Timezone: {selectedTimeZone.DisplayName}");
            if (nextOpeningTime.HasValue && timeLeft.HasValue)
            {
                Console.Write("The next Pyro Technical Preview Build server is scheduled to open on ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{nextOpeningTime.Value.ToString("f", CultureInfo.CreateSpecificCulture("en-US"))} ({selectedTimeZone.StandardName})");
                Console.ResetColor();
                Console.Write("Time remaining until opening: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{timeLeft.Value.Days} days, {timeLeft.Value.Hours} hours, {timeLeft.Value.Minutes} minutes, and {timeLeft.Value.Seconds} seconds");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("No upcoming server openings are scheduled or the schedule is out of date.");
            }

            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            Console.Clear(); 
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
