using MyConsoleMenu.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyConsoleMenu
{
    public class ConsoleMenu
    {
        #region Private fields.
        private string cursorCharacter;
        private int indentFromLeft;
        private int indentFromCursor;
        private int cursorPosition;
        private bool exitMenu;
        private Dictionary<int, MenuItem> menuItems = new Dictionary<int, MenuItem>();
        #endregion

        public ConsoleMenu(string cursorCharacter = "->")
        {
            this.cursorCharacter = cursorCharacter;
            this.indentFromLeft = 1;
            this.indentFromCursor = 1;
            this.cursorPosition = 0;
            this.exitMenu = false;
        }

        #region Public Properties
        public ConsoleColor HeaderColor { get; set; } = ConsoleColor.White;
        public ConsoleColor SubheaderColor { get; set; } = ConsoleColor.Gray;
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Green;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
        public ConsoleColor HighlightColor { get; set; } = ConsoleColor.Yellow;
        public ConsoleColor HighlightBackgroundColor { get; set; } = ConsoleColor.Black;

        public Action PreMenuItemAction { get; set; }
        public Action PostMenuItemAction { get; set; }

        public int IndentFromLeft
        {
            get
            {
                if (indentFromLeft < 0)
                    return 0;
                else
                    return indentFromLeft;
            }
            set { indentFromLeft = value; }
        }
        public int IndentFromCursor
        {
            get
            {
                if (indentFromCursor < 0)
                    return 0;
                else
                    return indentFromCursor;
            }
            set { indentFromCursor = value; }
        }
        public string HeaderText { get; set; }
        public string SubheaderText { get; set; }
        public bool CycleMenu { get; set; } = false;
        #endregion

        public virtual void AddMenuItem(MenuItem menuItem)
        {
            int _id = 0;
            if (menuItems.Count() == 0 || menuItems.Max(m => m.Key) < 0)
                _id = 0;
            else
                _id = menuItems.Max(m => m.Key) + 1;

            menuItems.Add(_id, menuItem);
        }
        public virtual void RemoveMenuItem(int id)
        {
            menuItems.Remove(id);
        }
        public virtual void RefreshMenuItems()
        {
            if (menuItems.Count > 0)
            {
                var dict = new Dictionary<int, MenuItem>();
                int iterator = 0;
                Guid currentlySelectedGuid = Guid.Empty;
                if (menuItems.TryGetValue(cursorPosition, out MenuItem currentlySelectedMenuItem))
                    currentlySelectedGuid = currentlySelectedMenuItem.Guid;

                foreach (var kvp in menuItems)
                {
                    if (!kvp.Value.DeleteSelf)
                    {
                        dict.Add(iterator, kvp.Value);
                        iterator++;
                    }
                }
                menuItems = dict;
                cursorPosition = FindDictionaryKeyByMenuItemGuid(currentlySelectedGuid);
            }
        }
        public virtual void ExitMenu()
        {
            this.exitMenu = true;
        }

        private int FindDictionaryKeyByMenuItemGuid(Guid guid)
        {
            int key = 0;
            foreach (var kvp in menuItems)
            {
                if (kvp.Value.Guid.Equals(guid))
                    return kvp.Key;
            }
            return key;
        }

        public void ShowMenu()
        {
            while (!exitMenu)
            {
                Console.Clear();
                DrawHeaderText();
                DrawSubheaderText();
                DrawMenuItems();
                AwaitUserInput();
            }
        }

        public virtual void DrawHeaderText()
        {
            ConsoleUtil.WriteLineColor(HeaderText, HeaderColor);
        }
        public virtual void DrawSubheaderText()
        {
            ConsoleUtil.WriteLineColor(SubheaderText, SubheaderColor);
        }
        public virtual void DrawMenuItems()
        {
            RefreshMenuItems();
            if (menuItems.Count > 0)
            {
                for (int i = 0; i < menuItems.Count; i++)
                {
                    Console.Write(new string(' ', indentFromLeft));
                    if (cursorPosition == i)
                    {
                        ConsoleUtil.WriteColor(cursorCharacter, HighlightColor, HighlightBackgroundColor);
                        Console.Write(new string(' ', menuItems[i].IndentFromCursor >= 0 ? menuItems[i].IndentFromCursor : IndentFromCursor));
                        ConsoleUtil.WriteLineColor(menuItems[i].Text,
                            menuItems[i].HighlightColor ?? HighlightColor,
                            menuItems[i].HighlightBackgroundColor ?? HighlightBackgroundColor);
                    }
                    else
                    {
                        Console.Write(new string(' ', cursorCharacter.Length));
                        Console.Write(new string(' ', menuItems[i].IndentFromCursor >= 0 ? menuItems[i].IndentFromCursor : IndentFromCursor));
                        ConsoleUtil.WriteLineColor(menuItems[i].Text,
                            menuItems[i].ForegroundColor ?? ForegroundColor,
                            menuItems[i].BackgroundColor ?? BackgroundColor);
                    }
                }
            }
            else
            {
                Console.WriteLine("No items.");
            }
        }
        public virtual void AwaitUserInput()
        {
            if (menuItems.Count > 0)
            {
                Console.CursorVisible = false;
                var userKeyInput = Console.ReadKey().Key;
                switch (userKeyInput)
                {
                    case ConsoleKey.UpArrow:
                        cursorPosition = ResolveCursorPosition(-1);
                        break;
                    case ConsoleKey.DownArrow:
                        cursorPosition = ResolveCursorPosition(1);
                        break;
                    case ConsoleKey.Enter:
                        TriggerMenuItemAction(cursorPosition);
                        break;
                    case ConsoleKey.Escape:
                        exitMenu = true;
                        break;
                    default:
                        // Unsupported key press
                        break;
                }
            }
            else
            {
                Console.Write("Press any key to exit menu...");
                Console.ReadKey();
                this.exitMenu = true;
            }
        }
        public virtual int ResolveCursorPosition(int move)
        {
            var validMenuItems = menuItems.Where(m => m.Value.ValidItem);
            var minValue = validMenuItems.Min(m => m.Key);
            var maxValue = validMenuItems.Max(m => m.Key);
            var desiredPosition = cursorPosition + move;

            if (desiredPosition < minValue && CycleMenu)
                return maxValue;
            else if (desiredPosition < minValue)
                return minValue;
            else if (desiredPosition > maxValue & CycleMenu)
                return minValue;
            else if (desiredPosition > maxValue)
                return maxValue;
            else if (validMenuItems.Any(kvp => kvp.Key == desiredPosition))
                return desiredPosition;
            else
                return ResolveCursorPosition(move + (move < 0 ? -1 : 1));
        }
        public virtual void TriggerMenuItemAction(int key)
        {
            PreMenuItemAction?.Invoke();

            if (menuItems.ContainsKey(key))
            {
                menuItems[key].Action?.Invoke();
                if (menuItems[key].ExitMenuAfterAction)
                    exitMenu = true;
            }
            else
                throw new ArgumentException($"Provided key: { key } was not found in current dictionary of MenuItems.  This should not happen.");

            PostMenuItemAction?.Invoke();
        }
    }
}