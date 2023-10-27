// See https://aka.ms/new-console-template for more information

using Tests;

Console.WriteLine("Hello, World!");

var window = new AppWindow();

var windowSettings = new WindowSettings
{
    width = 800,
    height = 600,
    positionX = 200,
    positionY = 200,
    targetFps = 60
};

Console.WriteLine("Create window");
window.Create(windowSettings);

Console.WriteLine("Run window");
window.Run();
