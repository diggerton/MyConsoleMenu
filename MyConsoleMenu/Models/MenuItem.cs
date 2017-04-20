using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConsoleMenu.Models
{
    public class MenuItem
    {
        const string exitText = "Exit";
        MenuItem() { }
        public MenuItem(string text, bool validItem = true, bool deleteSelf = false, bool exitMenuAfterAction = false)
        {
            Text = text;
            ValidItem = validItem;
            DeleteSelf = deleteSelf;
            ExitMenuAfterAction = exitMenuAfterAction;
        }
        public static MenuItem Spacer(string text = "", int indent = -1)
        {
            return new MenuItem(text)
            {
                IndentFromCursor = indent,
                ValidItem = false
            };
        }
        public static MenuItem Exit(string text = exitText)
        {
            return new MenuItem(text)
            {
                ValidItem = true,
                ExitMenuAfterAction = true
            };
        }

        public string Text { get; set; }
        public bool ValidItem { get; set; }
        public bool DeleteSelf { get; set; }
        public bool ExitMenuAfterAction { get; set; }
        public int IndentFromCursor { get; set; } = -1;
        public Guid Guid { get; private set; } = new Guid();
        public Action Action { get; set; }

        public ConsoleColor? ForegroundColor { get; set; }
        public ConsoleColor? BackgroundColor { get; set; }
        public ConsoleColor? HighlightColor { get; set; }
        public ConsoleColor? HighlightBackgroundColor { get; set; }
    }
}
