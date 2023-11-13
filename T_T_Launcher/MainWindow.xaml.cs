using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace T_T_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public enum GameState
    {
        NotRunning,
        Running,
        TTRunning
    }

    public partial class MainWindow : Window
    {
        private GameData _gameData;
        private BackgroundWorker worker;
        public MainWindow(GameData gameData)
        {
            _gameData = gameData;
            InitializeComponent();
            MainFrame.Navigate(new Uri("Lobby.xaml", UriKind.Relative));

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        ~MainWindow()
        {
            worker.CancelAsync();
        }

        private async void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            while (true)
            {
                var state = GameState.NotRunning;
                if (IsProcessRunning("League of Legends"))
                {
                    state = GameState.Running;
                    if (IsProcessRunning("T_T Pandoras Box"))
                    {
                        state = GameState.TTRunning;
                    }
                }

                Dispatcher.Invoke(new Action(() =>
                {
                    switch (state)
                    {
                        case GameState.NotRunning:
                            StatusLabel.Content = "Wating for game";
                            break;
                        case GameState.Running:
                            StatusLabel.Content = "Game running";
                            break;
                        case GameState.TTRunning:
                            StatusLabel.Content = "T_T Running";
                            break;
                    }
                }));

                if (state == GameState.Running)
                {
                    try
                    {
                        StartProcessAsAdmin(@"T_T\T_T Pandoras Box.exe");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error starting process: " + ex.Message);
                    }
                    await Task.Delay(5000);
                }

                await Task.Delay(1000);
            }
        }

        public static void StartProcessAsAdmin(string filePath)
        {
            var currentExecutablePath = Assembly.GetExecutingAssembly().Location;
            var currentDirectory = Path.GetDirectoryName(currentExecutablePath);
            if (string.IsNullOrWhiteSpace(currentDirectory))
            {
                return;
            }
            // Combine the current directory with the relative path to get the full path
            var fullPath = Path.Combine(currentDirectory, filePath);

            // Calculate the working directory based on the full path of the target executable
            var workingDirectory = Path.GetDirectoryName(fullPath);


            var processStartInfo = new ProcessStartInfo
            {
                FileName = fullPath,
                UseShellExecute = true,
                Verb = "runas",
                WorkingDirectory = workingDirectory,
            };

            try
            {
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Any();
        }
        
        private void TosButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("TosPage.xaml", UriKind.Relative));
        }

        private void OffsetsButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("OffsetsPage.xaml", UriKind.Relative));
        }

        private void LobbyButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("Lobby.xaml", UriKind.Relative));
        }

        private void GameDataButton_OnClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(_gameData);
        }
    }
}