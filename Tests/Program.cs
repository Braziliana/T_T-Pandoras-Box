﻿// See https://aka.ms/new-console-template for more information

using Tests;

Console.WriteLine("Hello, World!");

var window = new AppWindow();

Console.WriteLine("Create window");
window.Create();

var gp = new GameProcess();
gp.SetTargetProcessName("League of Legends.exe");
Console.WriteLine("hook: " + gp.Hook());
Console.WriteLine("isRunning: " + gp.IsProcessRunning());

Console.WriteLine(gp.FindOffset("48 8B 05 ? ? ? ? 4C 8B D2 4C 8B C1", 3).ToString("X"));
var im = new InputManager();
//im.KeyStateHandler += delegate(InputManager.KeyStateEvent evt) { Console.WriteLine($"{evt.key} {evt.isDown}"); };
//im.MouseMoveHandler += delegate(InputManager.MouseMoveEvent evt) { Console.WriteLine($"{evt.position.x} {evt.position.y} - {evt.delta.x} {evt.delta.y}"); };
Console.WriteLine("Run window");
window.Run();
