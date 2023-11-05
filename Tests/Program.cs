// See https://aka.ms/new-console-template for more information

using Tests;

Console.WriteLine("Hello, World!");

var window = new AppWindow();

Console.WriteLine("Create window");
window.Create();

var gp = new GameProcess();
gp.SetTargetProcessName("League of Legends.exe");
Console.WriteLine("hook: " + gp.Hook());
Console.WriteLine("isRunning: " + gp.IsProcessRunning());
InputManager.RegisterEvents();
Console.WriteLine("Run window");
window.Run();
