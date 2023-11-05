// See https://aka.ms/new-console-template for more information

using Tests;

Console.WriteLine("Hello, World!");

var window = new AppWindow();

Console.WriteLine("Create window");
window.Create();

InputManager.RegisterEvents();
Console.WriteLine("Run window");
window.Run();
