using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Collections.ObjectModel;
namespace StarCitizenUtilityApp
{
    class Program
    {
        private static Dictionary<string, string> schedule = new Dictionary<string, string>
        {

            ["2023-10-31T19:00:00"] = "UTC",
            ["2023-11-01T13:00:00"] = "UTC",
            ["2023-11-03T06:00:00"] = "UTC",
            ["2023-11-04T13:00:00"] = "UTC",
            ["2023-11-06T07:00:00"] = "UTC",
            ["2023-11-06T20:00:00"] = "UTC",
            ["2023-11-09T07:00:00"] = "UTC",
            ["2023-11-09T20:00:00"] = "UTC",
            ["2023-11-10T14:00:00"] = "UTC",
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
                Console.WriteLine("3. Manage screenshots.");
                Console.WriteLine("4. Create a backup or delete the 'USER' folder from the game build.");
                Console.WriteLine("5. Check Pyro Technical Preview Build server opening times. [Based on data from Nov 1st, 2023]");
                Console.WriteLine("6. Exit\n");
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
                        DeleteScreenShots();
                        break;
                    case "4":
                        ZipUserFolderToDesktop();
                        break;
                    case "5":
                        CheckServerOpeningTimes();
                        break;
                    case "6":
                        running = false;
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
                        Thread.Sleep(2000);
                        break;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
                Console.Clear();
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
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"));
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
            DeleteCacheFolder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache"));
            Console.WriteLine("\nStar Citizen and AMD cache deletion process is complete.");
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
            Console.Clear();
        }
        static string? FindStarCitizenFolder()
        {
            List<string> likelyPaths = new List<string>();

            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    likelyPaths.Add(drive.Name);
                }
            }

            likelyPaths.Add(@"C:\Program Files\");
            likelyPaths.Add(@"C:\Program Files (x86)\");
            likelyPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            likelyPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            likelyPaths.Add(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            foreach (string basePath in likelyPaths)
            {
                string searchPath = Path.Combine(basePath, "Roberts Space Industries", "StarCitizen");
                if (Directory.Exists(searchPath))
                {
                    return searchPath;
                }
            }
            return null;
        }
        static IEnumerable<string> GetGameBuilds(string starCitizenFolderPath)
        {
            var directories = Directory.GetDirectories(starCitizenFolderPath);
            foreach (var dir in directories)
            {
                yield return new DirectoryInfo(dir).Name;
            }
        }
        static void ZipUserFolderToDesktop()
        {
            Console.Clear();
            ClearErrorLog();
            string? starCitizenFolderPath = FindStarCitizenFolder();
            if (string.IsNullOrEmpty(starCitizenFolderPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Star Citizen folder not found.");
                Console.ResetColor();
                return;
            }
            string[] validChoices = GetGameBuilds(starCitizenFolderPath).ToArray();
            if (validChoices.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No game builds found.");
                Console.ResetColor();
                return;
            }
            Console.WriteLine("Do you want to backup or delete the USER folder?");
            Console.WriteLine("1: Backup\n2: Delete");
            string? actionChoice = Console.ReadLine();
            if (actionChoice == "1")
            {
                Console.WriteLine("\nSelect the game build you want to backup:");
                for (int i = 0; i < validChoices.Length; i++)
                {
                    Console.WriteLine($"{i + 1}: {validChoices[i]}");
                }
                string? userInput = Console.ReadLine();
                if (!int.TryParse(userInput, out int choice) || choice < 1 || choice > validChoices.Length)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Backup process aborted.");
                    Console.ResetColor();
                    return;
                }
                BackupUserFolder(starCitizenFolderPath, validChoices[choice - 1]);
            }
            else if (actionChoice == "2")
            {
                Console.WriteLine("Select the game build you want to delete the USER folder from:");
                Console.WriteLine("1: Specific game build\n2: All builds");
                string? deleteOption = Console.ReadLine();
                if (deleteOption == "1")
                {
                    Console.WriteLine("Select the specific game build:");
                    for (int i = 0; i < validChoices.Length; i++)
                    {
                        Console.WriteLine($"{i + 1}: {validChoices[i]}");
                    }
                    string? buildChoice = Console.ReadLine();
                    if (!int.TryParse(buildChoice, out int buildIndex) || buildIndex < 1 || buildIndex > validChoices.Length)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice. Deletion process aborted.");
                        Console.ResetColor();
                        return;
                    }
                    DeleteUserFolder(starCitizenFolderPath, validChoices[buildIndex - 1]);
                }
                else if (deleteOption == "2")
                {
                    foreach (string build in validChoices)
                    {
                        DeleteUserFolder(starCitizenFolderPath, build);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Deletion process aborted.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid action choice. Operation aborted.");
                Console.ResetColor();
            }
            Console.ResetColor();
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            Console.Clear();
        }
        static void BackupUserFolder(string starCitizenFolderPath, string buildChoice)
        {
            string userFolderPath = Path.Combine(starCitizenFolderPath, buildChoice, "USER");
            if (!Directory.Exists(userFolderPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Star Citizen USER folder for {buildChoice} not found.");
                Console.ResetColor();
                return;
            }
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string zipFilePath = Path.Combine(desktopPath, $"StarCitizen_USER_{buildChoice}_Backup.zip");
            try
            {
                if (File.Exists(zipFilePath))
                {
                    Console.WriteLine("\nA backup file already exists on the desktop. Overwriting...");
                    File.Delete(zipFilePath);
                }
                ZipFile.CreateFromDirectory(userFolderPath, zipFilePath);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nBackup created successfully at: {zipFilePath}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError during backup: {ex.Message}");
            }
            finally
            {
                Console.ResetColor();
            }
        }
        static void DeleteUserFolder(string baseFolderPath, string build)
        {
            string pathToDelete = Path.Combine(baseFolderPath, build, "USER");
            if (!Directory.Exists(pathToDelete))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"USER folder not found for {build} build.");
                Console.ResetColor();
                return;
            }
            try
            {
                Directory.Delete(pathToDelete, true);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Deleted USER folder for {build}.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to delete USER folder for {build}: {ex.Message}");
            }
            finally
            {
                Console.ResetColor();
            }
        }
        static void DeleteScreenShots()
        {
            Console.Clear();
            ClearErrorLog();
            string? baseFolder = FindStarCitizenFolder();
            if (baseFolder == null)
            {
                Console.WriteLine("Star Citizen folder not found.");
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey();
                Console.Clear();
                return;
            }
            string[] validChoices = GetGameBuilds(baseFolder).ToArray();
            if (validChoices.Length == 0)
            {
                Console.WriteLine("No game builds found for screenshots deletion.");
                Console.WriteLine("Press any key to return to the main menu...");
                Console.ReadKey();
                Console.Clear();
                return;
            }
            Console.WriteLine("Select an option:");
            Console.WriteLine("1: Provide a list of all the currently available screenshots for all game builds.");
            Console.WriteLine("2: Delete screenshots from a specific game build");
            Console.WriteLine("3: Delete screenshots from all builds");
            string? userOption = Console.ReadLine();
            if (userOption == "2")
            {
                Console.WriteLine("Select the specific game build:");
                for (int i = 0; i < validChoices.Length; i++)
                {
                    Console.WriteLine($"{i + 1}: {validChoices[i]}");
                }
                string? buildChoice = Console.ReadLine();
                if (!int.TryParse(buildChoice, out int buildIndex) || buildIndex < 1 || buildIndex > validChoices.Length)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Deletion process aborted.");
                    Console.ResetColor();
                    return;
                }
                DeleteScreenShotsFromBuild(baseFolder, validChoices[buildIndex - 1]);
            }
            else if (userOption == "3")
            {
                Console.WriteLine("Are you sure you want to delete Star Citizen screenshots for all game builds? (yes/no)");
                string? userInput = Console.ReadLine();
                if (userInput != null && userInput.Trim().ToLower() == "yes")
                {
                    foreach (var build in validChoices)
                    {
                        DeleteScreenShotsFromBuild(baseFolder, build);
                    }
                    Console.WriteLine("\nStar Citizen screenshots deletion process is complete.");
                }
                else
                {
                    Console.WriteLine("\nDeletion process canceled.");
                }
            }
            else if (userOption == "1")
            {
                ListAllScreenShots(baseFolder, validChoices);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice. Process aborted.");
                Console.ResetColor();
            }
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
            Console.Clear();
        }
        static void ListAllScreenShots(string baseFolderPath, string[] builds)
        {
            Console.WriteLine("Listing screenshots for all game builds:");
            foreach (var build in builds)
            {
                ListScreenShotsFromBuild(baseFolderPath, build);
            }
            Console.WriteLine("\nScreenshots listing process is complete.");
        }
        static void ListScreenShotsFromBuild(string baseFolderPath, string build)
        {
            string screenShotsPath = Path.Combine(baseFolderPath, build, "ScreenShots");
            string[] screenshotFiles = Directory.Exists(screenShotsPath) ? Directory.GetFiles(screenShotsPath) : new string[0];
            int screenshotCount = screenshotFiles.Length;
            if (screenshotCount > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Screenshots for {build} build [{screenshotCount}]:");
                foreach (var screenshot in screenshotFiles)
                {
                    Console.WriteLine(Path.GetFileName(screenshot));
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"No screenshots found for {build} build.");
            }
            Console.ResetColor();
        }
        static void DeleteScreenShotsFromBuild(string baseFolderPath, string build)
        {
            string screenShotsPath = Path.Combine(baseFolderPath, build, "ScreenShots");
            if (Directory.Exists(screenShotsPath) && Directory.EnumerateFiles(screenShotsPath).Any())
            {
                DeleteCacheFolder(screenShotsPath);
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"No screenshots found for {build} build.");
            }
            Console.ResetColor();
        }
        static void DeleteCacheFolder(string path)
        {
            string folderName = Path.GetFileName(path);
            string? parentPath = Path.GetDirectoryName(path);
            string parentFolderName = parentPath != null ? Path.GetFileName(parentPath) : string.Empty;
            string combinedFolderName = !string.IsNullOrEmpty(parentFolderName) ? $"{parentFolderName} {folderName}" : folderName;
            bool isFolderExistent = Directory.Exists(path);
            if (isFolderExistent)
            {
                DirectoryInfo di = new DirectoryInfo(path);

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

                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    try
                    {
                        dir.Delete(true);
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

                if (Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length > 0 || Directory.GetDirectories(path).Length > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{combinedFolderName} partially cleared. Some items could not be deleted (see error log for details).");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{combinedFolderName} completely cleared.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{combinedFolderName} does not exist or has already been deleted.");
                Console.ResetColor();
            }
        }
        static void CheckServerOpeningTimes()
        {
            Console.Clear();

            TimeZoneInfo localZone = TimeZoneInfo.Local;
            Console.WriteLine($"Your Local Timezone: {localZone.StandardName}");

            DateTime? nextOpeningTime = null;
            TimeSpan? timeLeft = null;
            bool isServerOpen = false;
            DateTime localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local);
            foreach (var entry in schedule)
            {
                DateTime utcTime = DateTime.ParseExact(entry.Key, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                DateTime localStartTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local);
                DateTime localEndTime = localStartTime.AddHours(8);
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

            if (isServerOpen && timeLeft.HasValue)
            {
                Console.Write("The Pyro Technical Preview Build server is ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("OPEN now!");
                Console.ResetColor();
                Console.WriteLine();
                Console.Write("Test time remaining: ");
                Console.ForegroundColor = ConsoleColor.Green;
                TimeSpan timeLeftValue = timeLeft.GetValueOrDefault();
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
                DateTime nextOpeningTimeValue = nextOpeningTime.GetValueOrDefault();
                Console.WriteLine(nextOpeningTimeValue.ToString("f", CultureInfo.CreateSpecificCulture("en-US")));
                Console.ResetColor();
                Console.Write("Time remaining until opening: ");
                Console.ForegroundColor = ConsoleColor.Green;
                TimeSpan timeLeftValue = timeLeft.GetValueOrDefault();
                Console.WriteLine($"{timeLeftValue.Days} days, {timeLeftValue.Hours} hours, {timeLeftValue.Minutes} minutes, and {timeLeftValue.Seconds} seconds");
                Console.ResetColor();

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

            DateTime nowInSelectedTimeZone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, selectedTimeZone);

            foreach (var entry in schedule)
            {
                DateTime utcTime = DateTime.ParseExact(entry.Key, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                DateTime selectedZoneTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, selectedTimeZone);

                if (nowInSelectedTimeZone < selectedZoneTime)
                {
                    nextOpeningTime = selectedZoneTime;

                    timeLeft = nextOpeningTime.Value - nowInSelectedTimeZone;
                    break;
                }
            }

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
