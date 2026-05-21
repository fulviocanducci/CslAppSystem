using CslAppSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CslAppSystem
{
    internal class Program
    {
        #region setFont
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public short dwFontSizeX;
            public short dwFontSizeY;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string FaceName;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetCurrentConsoleFontEx(
            IntPtr consoleOutput,
            bool maximumWindow,
            ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int dwType);

        const int STD_OUTPUT_HANDLE = -11;
        #endregion

        public static string ConnectionStringMysql = "Server=192.168.2.65;Database=db;Uid=adm;Pwd=adm123@;";
        public static MySqlConnection MySqlConnectionDabase { get; private set; }
        public static DalPeople DalPeople { get; private set; }

        static void ConfigureMysql()
        {
            MySqlConnectionDabase = new MySqlConnection(ConnectionStringMysql);
            DalPeople = new DalPeople(MySqlConnectionDabase);
        }

        static void Dispose()
        {
            try
            {
                if (MySqlConnectionDabase != null)
                {
                    MySqlConnectionDabase.Dispose();
                }
                DalPeople = null;
            }
            catch
            {
            }
            finally
            {

            }
        }

        static People Create(int left)
        {
            Console.Clear();
            int leftRead = left + 11;
            Console.SetCursorPosition(left, 5);
            Console.Write("Nome......: ");
            Console.SetCursorPosition(left, 6);
            Console.Write("Data Nasc.: ");
            string name = Field.Read(leftRead, 5, 50);
            if (Field.EscapePressed)
            {
                return null;
            }
            string dateBirtday = string.Empty;
            while (true)
            {
                dateBirtday = Field.Read(leftRead, 6, "00/00/0000");
                if (Field.EscapePressed)
                {
                    return null;
                }
                if (dateBirtday.IsDate())
                {
                    break;
                }
            }
            People people = new People(name, dateBirtday.ParseDate());
            DalPeople.Create(people);
            return people;
        }

        static bool? Update(int left)
        {
            People people = null;
            int leftRead = left + 11;
            while (true) {
                Console.Clear();                
                Console.SetCursorPosition(left, 5);
                Console.Write("Id......: ");
                string id = Field.Read(leftRead, 5, 5);
                if (Field.EscapePressed)
                {
                    return null;
                }
                if (id.IsLong() == false)
                {
                    return null;
                }
                people = DalPeople.Get(id.ParseLong());
                if (people)
                {
                    break;
                }
                else 
                {
                    Console.SetCursorPosition(left, 6);
                    Console.Write("Código inválido, pressione <Enter>");
                    Console.ReadKey();
                }
            }
            Console.SetCursorPosition(left, 6);
            Console.Write("Nome......: ");
            Field.View(leftRead, 6, 50, people.Name);
            Console.SetCursorPosition(left, 7);
            Console.Write("Data Nasc.: ");
            Field.View(leftRead, 7, 10, people.DateBirthday.ToString("dd/MM/yyyy"));
            //
            string name = Field.Read(leftRead, 6, 50, people.Name);
            if (Field.EscapePressed)
            {
                return null;
            }
            string dateBirtday = string.Empty;
            while (true)
            {
                dateBirtday = Field.Read(leftRead, 7, "00/00/0000", people.DateBirthday.ToString("ddMMyyyy"));
                if (Field.EscapePressed)
                {
                    return null;
                }
                if (dateBirtday.IsDate())
                {
                    break;
                }
            }
            people.Name = name;
            people.DateBirthday = dateBirtday.ParseDate();
            return DalPeople.Update(people);
        }

        static void List(int left)
        {
            Console.Clear();
            int leftRead = left + 11;
            IEnumerable<People> peoples = DalPeople.Get();
            var items = peoples.GetEnumerator();
            while (items.MoveNext())
            {
                People people = items.Current;
                Console.WriteLine("{0,5} {1,-52} {2,-10:dd/MM/yyyy}", people.Id, people.Name, people.DateBirthday);
            }
            Console.Write("Pessoa listadas com êxito. Pressione <ENTER>");
            Console.Read();
        }

        static void Main(string[] args)
        {
            ConfigureMysql();

            int left = 22;

            string menu = string.Empty;
            while (menu != "000")
            {
                menu = string.Empty;
                Console.Clear();
                Console.SetCursorPosition(left, 5);
                Console.WriteLine("001 - Lista de Nomes");
                Console.SetCursorPosition(left, 6);
                Console.WriteLine("002 - Cadastrar Nome");
                Console.SetCursorPosition(left, 7);
                Console.WriteLine("003 - Alterar Nome");
                Console.SetCursorPosition(left, 8);
                Console.WriteLine("000 - Sair");
                Console.SetCursorPosition(left, 9);
                Console.Write("Escolha: ");
                menu = Field.Read(left + 9, 9, 3, "");
                Console.Write(menu);
                Console.ReadKey();
                switch (menu.Trim())
                {
                    case "1":
                    case "01":
                    case "001":
                        {
                            List(left);
                            break;
                        }
                    case "2":
                    case "02":
                    case "002":
                        {
                            if (Create(left))
                            {
                                Console.SetCursorPosition(left, 7);
                                Console.Write("Pessoa inserida com êxito. Pressione <ENTER>");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.SetCursorPosition(left, 7);
                                Console.Write("Cancelado. Pressione <ENTER>");
                                Console.ReadKey();
                            }
                            break;
                        }
                    case "3":
                    case "03":
                    case "003":
                        {                            
                            Update(left);
                            break;
                        }
                }
            }

            Dispose();
        }
    }
}

