using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

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
        }

        static void DeleteNvidiaCacheFiles()
        {
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Star Citizen"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NVIDIA", "DXCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NVIDIA", "GLCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NVIDIA", "OptixCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D3DSCache"));

            Console.WriteLine("Star Citizen and Nvidia cache deletion process is complete.");
        }

        static void DeleteAmdCacheFiles()
        {
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Star Citizen"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AMD", "DXCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AMD", "GLCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AMD", "VkCache"));
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "D3DSCache"));

            Console.WriteLine("Star Citizen and AMD cache deletion process is complete.");
        }

        static void DeleteCacheFolder(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                    Console.WriteLine($"{path} deleted successfully.");
                }
                catch (Exception ex)
                {
                    File.AppendAllText("error_log.txt", $"{DateTime.Now}: Failed to delete {path}. Error: {ex.Message}\n");
                    Console.WriteLine($"Failed to delete {path}. Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"{path} not found.");
            }
        }

        static void CheckServerOpeningTimes()
        {
            var nextEvent = GetNextServerOpeningTime();

            if (nextEvent.HasValue)
            {
                var (openingTimeLocal, openingTimeUtc, timeLeft) = nextEvent.Value;
                Console.WriteLine($"The next opening time is at {openingTimeLocal:yyyy-MM-dd HH:mm:ss} " +
                                  $"in your local time zone: {TimeZoneInfo.Local.DisplayName}.");
                Console.WriteLine($"Time left until the next opening: {timeLeft.Days} days, " +
                                  $"{timeLeft.Hours} hours, {timeLeft.Minutes} minutes.\n");

                Console.Write("Would you like to check the time in another time zone? (yes/no): ");
                string? response = Console.ReadLine();
                if (response?.Trim().ToLower() == "yes")
                {
                    ShowTimeInDifferentTimeZone(openingTimeUtc);
                }
            }
            else
            {
                Console.WriteLine("There are currently no scheduled server openings.");
            }
        }

        static void ShowTimeInDifferentTimeZone(DateTime utcTime)
{
    try
    {
        IReadOnlyList<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
        for (int i = 0; i < timeZones.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {timeZones[i].DisplayName}");
        }
        Console.Write("\nPlease enter the number corresponding to the target time zone: ");

        string? input = Console.ReadLine();
        if (int.TryParse(input, out int timeZoneIndex) && timeZoneIndex > 0 && timeZoneIndex <= timeZones.Count)
        {
            TimeZoneInfo selectedTimeZone = timeZones[timeZoneIndex - 1];
            DateTime targetTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, selectedTimeZone);
            DateTime localTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, selectedTimeZone);
            TimeSpan timeLeft = targetTime - localTimeNow;

            if (timeLeft < TimeSpan.Zero)
            {
                Console.WriteLine("The next opening time has already passed in the selected time zone.");
            }
            else
            {
                Console.WriteLine($"The next opening time in {selectedTimeZone.DisplayName} is {targetTime:yyyy-MM-dd HH:mm:ss}.");
                Console.WriteLine($"Time left until the next opening: {timeLeft.Days} days, " +
                                    $"{timeLeft.Hours} hours, {timeLeft.Minutes} minutes.");
            }
        }
        else
        {
            Console.WriteLine("Invalid selection. Please enter a number from the list.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred while converting time zones. Please try again.");
        File.AppendAllText("error_log.txt", $"{DateTime.Now}: An error occurred in ShowTimeInDifferentTimeZone - {ex}\n");
    }
    finally
    {
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}

        static (DateTime openingTimeLocal, DateTime openingTimeUtc, TimeSpan timeLeft)? GetNextServerOpeningTime()
        {
            DateTime nowUtc = DateTime.UtcNow;
            foreach (var entry in schedule)
            {
                DateTime openingTimeUtc = DateTime.ParseExact(entry.Key, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                if (nowUtc < openingTimeUtc)
                {
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(entry.Value)!;
                    DateTime openingTimeLocal = TimeZoneInfo.ConvertTimeFromUtc(openingTimeUtc, timeZoneInfo);
                    TimeSpan timeLeft = openingTimeLocal - TimeZoneInfo.ConvertTimeFromUtc(nowUtc, TimeZoneInfo.Local);
                    return (openingTimeLocal, openingTimeUtc, timeLeft);
                }
            }
            return null;
        }

    }
}
