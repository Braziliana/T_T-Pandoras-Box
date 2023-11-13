using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LCU;

namespace T_T_Launcher;

public enum MultiSearchWebsiteType
{
    OP,
    U,
    PORO
}

public partial class Lobby : Page, INotifyPropertyChanged 
{
    public ObservableCollection<LcuPlayer> Players { get; set; } = new ObservableCollection<LcuPlayer>();
    private LcuClient? _lcuClient = null;
    private string _region = string.Empty;
    
    private MultiSearchWebsiteType _multiSearchWebsiteType;
    public MultiSearchWebsiteType MultiSearchWebsiteType
    {
        get => _multiSearchWebsiteType;
        set
        {
            _multiSearchWebsiteType = value;
            OnPropertyChanged(nameof(MultiSearchWebsiteType));
        }
    }
    
    public Lobby()
    {
        InitializeComponent();
        this.DataContext = this;
        
        var worker = new BackgroundWorker();
        worker.WorkerSupportsCancellation = true;
        worker.DoWork += Worker_DoWork;
        worker.RunWorkerAsync();

        MultiSearchWebsiteType = MultiSearchWebsiteType.OP;
    }
    
    private async void Worker_DoWork(object? sender, DoWorkEventArgs e)
    {
        if (sender is not BackgroundWorker worker)
        {
            return;
        }
        
        while (!worker.CancellationPending)
        {
            await Task.Delay(500);
            
            if (_lcuClient == null)
            {
                _lcuClient = LcuClient.Create();
                continue;
            }

            _region = _lcuClient.GetRegion();
            
            if (string.IsNullOrWhiteSpace(_region))
            {
                continue;
            }
            
            var lobbyPlayers = _lcuClient.GetLobbyPlayers();
            
            Dispatcher.Invoke(new Action(() =>
            {
                Players.Clear();
                foreach (var player in lobbyPlayers.Participants)
                {
                    Players.Add(player);
                }
            }));
        }
    }
    
    private void PlayerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        
        if(button.DataContext is not LcuPlayer player) return;

        switch (MultiSearchWebsiteType)
        {
            case MultiSearchWebsiteType.OP:
                OpenUrl($"https://www.op.gg/summoners/{_region.ToLower()}/{player.Name}");
                break;
            case MultiSearchWebsiteType.U:
                OpenUrl($"https://u.gg/lol/profile/{MapRegion(_region)}/{player.Name}/overview");
                break;
            case MultiSearchWebsiteType.PORO:
                OpenUrl($"https://poro.gg/summoner/{_region.ToLower()}/{player.Name}");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void OpenUrl(string url)
    {
        if (!Players.Any())
        {
            return;
        }
        if (SendToChatCheckBox.IsChecked == true)
        {
            _lcuClient?.SendChat(Players.First().Cid, url);
        }
        
        Process.Start(new ProcessStartInfo() { FileName = url, UseShellExecute = true });
    }

    private void MultiSearch_OnClick(object sender, RoutedEventArgs e)
    {
        switch (MultiSearchWebsiteType)
        {
            case MultiSearchWebsiteType.OP:
                OpenUrl($"https://www.op.gg/multisearch/{_region.ToLower()}?summoners={GetSearchNames()}");
                break;
            case MultiSearchWebsiteType.U:
                OpenUrl($"https://u.gg/multisearch?summoners={GetSearchNames()}&region={MapRegion(_region)}");
                break;
            case MultiSearchWebsiteType.PORO:
                OpenUrl($"https://poro.gg/multi?region={_region}&q={GetSearchNames()}");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    private string GetSearchNames()
    {
        return string.Join(", ", Players.Select(x => x.Name));
    }
    
    private string MapRegion(string region)
    {
        return region switch
        {
            "KR" => "kr",
            "RU" => "ru",
            "EUNE" => "eun1",
            _ => region + "1"
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}