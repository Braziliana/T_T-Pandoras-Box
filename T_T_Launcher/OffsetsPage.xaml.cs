using NativeWarper;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace T_T_Launcher;

public partial class OffsetsPage : Page
{
    private List<Offset> _offsets = new List<Offset>();
    public OffsetsPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        LogsRTB.Document = new FlowDocument();
        var jsonContent = File.ReadAllText("T_T/appsettings.json");
        var token = JToken.Parse(jsonContent);

        LoadOffsetsView(token);
    }

    private void LoadOffsetsView(JToken token)
    {
        OffsetsPanel.Children.Clear();
        _offsets.Clear();

        foreach (var property in token.Children<JProperty>())
        {
            if (property.Name.Contains("Offset"))
            {
                CreateSection(property.Name, property.Value);
            }
        }
    }

    private void CreateSection(string header, JToken sectionToken)
    {
        TextBlock headerText = new TextBlock { Text = header, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 0) };
        OffsetsPanel.Children.Add(headerText);

        if (sectionToken is JObject sectionObject)
        {
            foreach (var property in sectionObject.Properties())
            {
                StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 0, 0, 0) };
                stackPanel.Children.Add(new TextBlock { Text = property.Name, Width = 200 });

                var offsetTextbox = new TextBox { Text = FormatOffsetString(property.Value.ToString()), Name = $"TextBox_{header}_{property.Name}", Width = 200, };
                offsetTextbox.TextChanged += HexTextBox_TextChanged;
                stackPanel.Children.Add(offsetTextbox);
                OffsetsPanel.Children.Add(stackPanel);

                _offsets.Add(new Offset(header, property.Name, offsetTextbox));
            }
        }
    }

    private void HexTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            return;       
        }

        string text = textBox.Text;

        if (IsValidHexString(text))
        {
            textBox.Background = Brushes.White;
        }
        else
        {
            textBox.Background = Brushes.LightPink;
        }
    }

    private bool IsValidHexString(string hexString)
    {
        Regex hexRegex = new Regex(@"\A(0x|0X)?[0-9a-fA-F]+\Z");
        return hexRegex.IsMatch(hexString);
    }

    private string FormatOffsetString(string offset)
    {
        offset = offset.ToLower().Trim();
        var data = offset.Split("x");

        var offsetStr = string.Empty;
        if(data.Length < 2)
        {
            offsetStr = data[0].ToUpper();
        }
        else
        {
            offsetStr = data[1].ToUpper();
        }

        while(offsetStr.Length < 4) 
        {
            offsetStr = "0" + offsetStr;
        }

        while (offsetStr.Length > 4 && offsetStr[0] == '0')
        {
            offsetStr = offsetStr.Substring(1);
        }

        return "0x" + offsetStr;
    }


    private class Offset
    {
        public string Category { get; }
        public string Name { get; }
        public TextBox TextBox { get; }

        public Offset(string category, string name, TextBox textBox)
        {
            Category = category;
            Name = name;
            TextBox = textBox;
        }
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        await SaveOffsets();
    }

    private async Task SaveOffsets()
    {
        var jsonContent = await File.ReadAllTextAsync("T_T/appsettings.json");
        var token = JToken.Parse(jsonContent);

        foreach (var offset in _offsets)
        {
            UpdateOffset(token, offset.Category, offset.Name, offset.TextBox.Text);
        }

        string jsonString = token.ToString(Formatting.Indented);
        await File.WriteAllTextAsync("T_T/appsettings.json", jsonString);
    }

    private void UpdateOffset(JToken token, string category, string name, string offset)
    {
        var sectionToken = token.Children<JProperty>().FirstOrDefault(x => x.Name == category);
 
        if (sectionToken != null && sectionToken.Value is JObject sectionObject)
        {
            var property = sectionObject.Properties().FirstOrDefault(x => x.Name == name);

            if (property != null)
            {
                property.Value = FormatOffsetString(FormatOffsetString(offset));
                return;
            }
        }

        Log($"Failed to update offset {category}.{name}. Failed to find json property");
    }

    private async void AutoUpdate_Click(object sender, RoutedEventArgs e)
    {
        LogsRTB.Document = new FlowDocument();
        var gp = new GameProcess();
        gp.SetTargetProcessName("League of Legends.exe");
        gp.Hook();

        if (!gp.IsProcessRunning())
        {
            MessageBox.Show("Game is not running");
            Log("Game is not running");
            return;
        }


        var jsonContent = await File.ReadAllTextAsync("T_T/appsettings.json");
        var token = JToken.Parse(jsonContent);

        var offsetPatternsStr = await File.ReadAllTextAsync("Resources/OffsetPatterns.json");
        var offsetPatterns = JsonConvert.DeserializeObject<List<OffsetPattern>>(offsetPatternsStr);

        if(offsetPatterns is null)
        {
            MessageBox.Show("Failed to load offset patterns");
            Log("Failed to load offset patterns");
            return;
        }

        foreach(var offsetPattern in offsetPatterns)
        {
            var offset = await Task.Run(() => gp.FindOffset(offsetPattern.Pattern, offsetPattern.OffsetInPattern));
            Log($"Pattern search for {offsetPattern.Category}.{offsetPattern.Name}, pattern: {offsetPattern.Pattern}, result: {FormatOffsetString(offset.ToString("X"))}");

            UpdateOffset(token, offsetPattern.Category, offsetPattern.Name, offset.ToString("X"));

        }

        await SaveOffsets();

        LoadOffsetsView(token);
    }

    private void Log(string text)
    {
        var paragraph = new Paragraph(new Run(text))
        {
            Margin = new Thickness(0)
        };
        LogsRTB.Document.Blocks.Add(paragraph);
        ScrollToBottom();
    }
    private void ScrollToBottom()
    {
        if (LogsRTB.Document == null) return;

        LogsRTB.ScrollToEnd();
    }

    private async void ExportCSharp_Click(object sender, RoutedEventArgs e)
    {
        LogsRTB.Document = new FlowDocument();
        var sb = new StringBuilder();
        var jsonContent = await File.ReadAllTextAsync("T_T/appsettings.json");
        var token = JToken.Parse(jsonContent);

        foreach (var sectionProperty in token.Children<JProperty>())
        {
            if (sectionProperty.Name.Contains("Offset"))
            {
                Log($"class {sectionProperty.Name}");
                Log("{");
                if (sectionProperty.Value is JObject sectionObject)
                {
                    foreach (var offsetProperty in sectionObject.Properties())
                    {
                        Log($"IntPtr {offsetProperty.Name} = new IntPtr({offsetProperty.Value});");
                    }
                }
                Log("}");
                Log("");
            }
        }
    }

    private async void ExportCPP_Click(object sender, RoutedEventArgs e)
    {
        LogsRTB.Document = new FlowDocument();
        var sb = new StringBuilder();
        var jsonContent = await File.ReadAllTextAsync("T_T/appsettings.json");
        var token = JToken.Parse(jsonContent);

        foreach (var sectionProperty in token.Children<JProperty>())
        {
            if (sectionProperty.Name.Contains("Offset"))
            {
                Log($"namespace {sectionProperty.Name}");
                Log("{");
                if (sectionProperty.Value is JObject sectionObject)
                {
                    foreach (var offsetProperty in sectionObject.Properties())
                    {
                        Log($"\tconstexpr inline uintptr_t {offsetProperty.Name} = {offsetProperty.Value};");
                    }
                }
                Log("}");
                Log("");
            }
        }
    }
}