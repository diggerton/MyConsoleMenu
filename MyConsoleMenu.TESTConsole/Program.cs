using MyConsoleMenu.Models;
using System;
using System.Collections.Generic;

namespace MyConsoleMenu.TESTConsole
{
    class Program
    {
        static void Main()
        {
            var consoleMenu = new ConsoleMenu()
            {
                CycleMenu = true
            };
            consoleMenu.AddMenuItem(MenuItem.Spacer("test spacer"));
            consoleMenu.AddMenuItem(new MenuItem("With action")
            {
                Action = () =>
                {
                    Console.WriteLine("ACTION TRIGGERED");
                    Console.ReadKey();
                }
            });
            consoleMenu.AddMenuItem(new MenuItem("closes menu after")
            {
                ExitMenuAfterAction = true,
                Action = () =>
                {
                    Console.WriteLine("Closing menu");
                    Console.ReadKey();
                }
            });
            consoleMenu.AddMenuItem(new MenuItem("Regular Item"));
            consoleMenu.AddMenuItem(new MenuItem("Override colors")
            {
                ForegroundColor = ConsoleColor.Black,
                BackgroundColor = ConsoleColor.White
            });
            consoleMenu.AddMenuItem(new MenuItem("Override highlight colors")
            {
                HighlightColor = ConsoleColor.Magenta,
                HighlightBackgroundColor = ConsoleColor.Gray
            });
            consoleMenu.AddMenuItem(MenuItem.Spacer("spacer"));
            consoleMenu.AddMenuItem(MenuItem.Spacer("spacer with indent", 5));
            consoleMenu.AddMenuItem(new MenuItem("Override indent")
            {
                IndentFromCursor = 5
            });

            consoleMenu.AddMenuItem(MenuItem.Spacer());
            consoleMenu.AddMenuItem(MenuItem.Exit());
            consoleMenu.ShowMenu();

        }
    }
}
