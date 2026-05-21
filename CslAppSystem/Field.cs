using System;

namespace CslAppSystem
{
    public static class Field
    {
        public static ConsoleColor FieldForegroundColor { get; set; } = ConsoleColor.Black;
        public static ConsoleColor FieldBackgroundColor { get; set; } = ConsoleColor.Gray;
        public static bool EscapePressed { get; private set; }
        public static string Read(int left, int top, int maxLength, string initialValue = "")
        {
            EscapePressed = false;

            if (initialValue == null)
                initialValue = "";

            if (initialValue.Length > maxLength)
                initialValue = initialValue.Substring(0, maxLength);

            char[] buffer = new string(' ', maxLength).ToCharArray();
            for (int i = 0; i < initialValue.Length; i++)
            {
                buffer[i] = initialValue[i];
            }
            DrawField(left, top, new string(buffer));
            int index = initialValue.Length;
            Console.SetCursorPosition(left + index, top);
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    EscapePressed = true;
                    break;
                }
                if (key.Key == ConsoleKey.Enter)
                {
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                    break;
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (index > 0)
                    {
                        index--;
                        buffer[index] = ' ';
                        Console.SetCursorPosition(left + index, top);
                        SetFieldColors();
                        Console.Write(' ');
                        Console.SetCursorPosition(left + index, top);
                    }
                    continue;
                }
                if (!char.IsControl(key.KeyChar))
                {
                    if (index < maxLength)
                    {
                        buffer[index] = key.KeyChar;
                        Console.SetCursorPosition(left + index, top);
                        SetFieldColors();
                        Console.Write(key.KeyChar);
                        index++;
                    }
                }
            }
            Console.ResetColor();
            return new string(buffer).TrimEnd();
        }

        public static string Read(int left, int top, string mask, string initialValue = "")
        {
            EscapePressed = false;
            if (initialValue == null)
                initialValue = "";
            char[] result = mask.ToCharArray();
            int valueIndex = 0;
            for (int i = 0; i < mask.Length; i++)
            {
                if (mask[i] == '0')
                {
                    if (valueIndex < initialValue.Length)
                    {
                        result[i] = initialValue[valueIndex];
                        valueIndex++;
                    }
                }
            }
            DrawField(left, top, new string(result));
            int index = 0;
            for (int i = 0; i < mask.Length; i++)
            {
                if (mask[i] == '0')
                {
                    if (result[i] == '0')
                    {
                        index = i;
                        break;
                    }

                    index = i + 1;
                }
            }
            if (index >= mask.Length)
                index = mask.Length;
            Console.SetCursorPosition(left + index, top);
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    EscapePressed = true;
                    break;
                }
                if (key.Key == ConsoleKey.Enter)
                {
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);
                    break;
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    while (index > 0)
                    {
                        index--;
                        if (mask[index] == '0')
                        {
                            result[index] = '0';
                            Console.SetCursorPosition(left + index, top);
                            SetFieldColors();
                            Console.Write('0');
                            Console.SetCursorPosition(left + index, top);
                            break;
                        }
                    }
                    continue;
                }
                if (char.IsDigit(key.KeyChar))
                {
                    while (index < mask.Length)
                    {
                        if (mask[index] == '0')
                        {
                            result[index] = key.KeyChar;
                            Console.SetCursorPosition(left + index, top);
                            SetFieldColors();
                            Console.Write(key.KeyChar);
                            index++;
                            while (index < mask.Length && mask[index] != '0')
                            {
                                index++;
                            }
                            Console.SetCursorPosition(left + index, top);
                            break;
                        }
                        index++;
                    }
                }
            }
            Console.ResetColor();
            return new string(result);
        }

        private static void DrawField(int left, int top, string content)
        {
            Console.SetCursorPosition(left, top);

            SetFieldColors();
            Console.Write(content);

            Console.SetCursorPosition(left, top);
        }

        private static void SetFieldColors()
        {
            Console.ForegroundColor = FieldForegroundColor;
            Console.BackgroundColor = FieldBackgroundColor;
        }

        public static void View(int left, int top, int length, string value = "")
        {
            if (value == null) value = "";
            if (value.Length > length) value = value.Substring(0, length);
            value = value.PadRight(length);
            Console.SetCursorPosition(left, top);
            SetFieldColors();
            Console.Write(value);
            Console.ResetColor();
        }
    }
}