using System;

namespace CslAppSystem
{
    public static class Write
    {
        public static void Line(int left, int top, string label, string value, int length)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(label);
            Field.View(left + label.Length, top, length, value);
        }

        public static string Variable(string value, int left, int top, int maxLength, string initialValue, Func<string, bool> fn, string message = null)
        {
            while (true)
            {
                value = Field.Read(left, top, maxLength, initialValue);
                if (Field.EscapePressed)
                {
                    value = null;
                    break;
                }
                if (fn(value))
                {
                    break;
                }
                else
                {
                    ErrorMessage(message, left, top, maxLength);
                }
            }
            return value;
        }

        public static string Variable(string value, int left, int top, int maxLength, string mask, string initialValue, Func<string, bool> fn, string message = null)
        {
            while (true)
            {
                value = Field.Read(left, top, mask, initialValue);
                if (Field.EscapePressed)
                {
                    value = null;
                    break;
                }
                if (fn(value))
                {
                    break;
                }
                else
                {
                    ErrorMessage(message, left, top, maxLength);
                }
            }
            return value;
        }

        private static void ErrorMessage(string message, int left, int top, int maxLength)
        {
            string errorMessage = message ?? "(Error)";
            Console.SetCursorPosition(left + maxLength, top);
            Console.Write(errorMessage);
            Console.ReadKey(true);
            Console.SetCursorPosition(left + maxLength, top);
            Console.Write(new string(' ', errorMessage.Length));
        }
    }
}
