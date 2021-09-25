using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace VideoScreenSaver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private Dictionary<string, string> settings = new Dictionary<string, string>();
        private string settingsDir;
        private string settingFile;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                string firstArgument = e.Args[0].ToLower().Trim();
                string secondArgument = null;

                // Handle cases where arguments are separated by colon.
                // Examples: /c:1234567 or /P:1234567
                if (firstArgument.Length > 2)
                {
                    secondArgument = firstArgument.Substring(3).Trim();
                    firstArgument = firstArgument.Substring(0, 2);
                }
                else if (e.Args.Length > 1)
                    secondArgument = e.Args[1];

                if (firstArgument == "/c")           // Configuration mode
                {
                    Configurations configurations = new Configurations();
                    configurations.ShowDialog();
                    Current.Shutdown();
                }
                else if (firstArgument == "/p")      // Preview mode
                {
                    Current.Shutdown();
                }
                else if (firstArgument == "/s")      // Full-screen mode
                {
                    settingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VideoScreenSaver");
                    settingFile = Path.Combine(settingsDir, "settings.ini");

                    if (!File.Exists(settingFile))
                    {
                        Directory.CreateDirectory(settingsDir);
                        File.WriteAllLines(settingFile, new List<string>() { "file|", "fill|Fill", "screens|All" });
                    }

                    settings = File.ReadAllLines(settingFile).Select(c => new KeyValuePair<string, string>(c.Split('|')[0], c.Split('|')[1])).ToDictionary(c => c.Key, c => c.Value);
                    string file = settings.FirstOrDefault(c => c.Key.Equals("file")).Value;
                    string fill = settings.FirstOrDefault(c => c.Key.Equals("fill")).Value ?? "Fill";
                    string scrs = settings.FirstOrDefault(c => c.Key.Equals("screens")).Value ?? "All";
                    Enum.TryParse(fill, out Stretch fillEnum);

                    if (scrs.Equals("All"))
                    {
                        OpenWindow(file, fillEnum, SystemParameters.VirtualScreenTop, SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight, true, e.Args.Contains("/d"));
                    }
                    else if (scrs.Equals("Duplicate"))
                    {
                        foreach (Screen item in Screen.AllScreens)
                        {
                            OpenWindow(file, fillEnum, item.Bounds.Top, item.Bounds.Left, item.Bounds.Width, item.Bounds.Height, true, e.Args.Contains("/d"));
                        }
                    }
                    else if (scrs.Equals("Primary"))
                    {
                        foreach (Screen item in Screen.AllScreens)
                        {
                            OpenWindow(file, fillEnum, item.Bounds.Top, item.Bounds.Left, item.Bounds.Width, item.Bounds.Height, item.Primary, e.Args.Contains("/d"));
                        }
                    }
                    else
                    {
                        OpenWindow(file, fillEnum, SystemParameters.VirtualScreenTop, SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight, true, e.Args.Contains("/d"));
                    }
                    return;
                }
                else    // Undefined argument
                {
                    System.Windows.MessageBox.Show("Sorry, but the command line argument \"" + firstArgument + "\" is not valid.", "ScreenSaver", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Current.Shutdown();
                }
            }
            else
            {
                Configurations configurations = new Configurations();
                configurations.ShowDialog();
                Current.Shutdown();
            }
        }

        private static void OpenWindow(string file, Stretch fillEnum, double top, double left, double width, double height, bool playVideo = true, bool isDebug = false)
        {
            MainWindow window = new MainWindow();
            window.Top = top - 10;
            window.Left = left - 10;
            window.Width = width + 20;
            window.Height = height + 20;
            window.Topmost = !isDebug;

            if (playVideo)
            {
                window.player.Stretch = fillEnum;
                if (string.IsNullOrWhiteSpace(file) && File.Exists(Path.Combine(new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName, "videoplayback.mp4")))
                    window.player.Source = new Uri(Path.Combine(new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName, "videoplayback.mp4"));
                else
                    window.player.Source = new Uri(file);
            }

            window.Show();
        }
    }
}
