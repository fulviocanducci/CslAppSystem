using CslAppSystem.Models;
using Google.Protobuf.WellKnownTypes;
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
            int leftRead = left + 12;

            Write.Line(left, 5, "Nome......: ", "", 50);
            Write.Line(left, 6, "Data Nasc.: ", "00/00/0000", 10);

            string name = string.Empty;
            string dateBirtday = string.Empty;

            name = Write.Variable(name, leftRead, 5, 50, "", x => x.Trim().Length >= 1, "Digite o nome");
            if (name == null) return null;
            dateBirtday = Write.Variable(dateBirtday, leftRead, 6, 10, "00/00/0000", "00000000", x => x.IsDate(), "Data inválida");
            if (dateBirtday == null) return null;

            People people = new People(name, dateBirtday.ParseDate());
            DalPeople.Create(people);
            return people;
        }

        static bool? Update(int left)
        {
            Console.Clear();
            int leftRead = left + 12;
            void View(int positionLeft, People value)
            {
                Console.Clear();
                Write.Line(positionLeft, 4, "Id........: ", value == null ? "" : value?.Id.ToString(), 5);
                Write.Line(positionLeft, 5, "Nome......: ", value?.Name ?? "", 50);
                Write.Line(positionLeft, 6, "Data Nasc.: ", value == null ? "" : value?.DateBirthday.ToString("dd/MM/yyyy"), 10);
            }

            People people = null;

            View(left, people);

            string id = string.Empty;
            string name = string.Empty;
            string dateBirtday = string.Empty;

            id = Write.Variable(id, leftRead, 4, 5, "", x => x.IsLong());
            if (id == null) return null;

            people = DalPeople.Get(id.ParseLong());
            if (people == null) return null;
            View(left, people);

            name = Write.Variable(name, leftRead, 5, 50, people.Name, x => x.Trim().Length >= 1, "Digite o nome");
            if (name == null) return null;
            dateBirtday = Write.Variable(dateBirtday, leftRead, 6, 10, "00/00/0000", people.DateBirthday.ToString("ddMMyyyy"), x => x.IsDate(), "Data inválida");
            if (dateBirtday == null) return null;

            people.Name = name;
            people.DateBirthday = dateBirtday.ParseDate();
            return DalPeople.Update(people);
        }

        static void List(int left)
        {
            Console.Clear();
            IEnumerable<People> peoples = DalPeople.Get();
            var items = peoples.GetEnumerator();
            int top = 5;
            while (items.MoveNext())
            {
                Console.SetCursorPosition(left, top++);
                People people = items.Current;
                Console.WriteLine("{0,5} {1,-52} {2,-10:dd/MM/yyyy}", people.Id, people.Name, people.DateBirthday);
            }
            Console.SetCursorPosition(left, top++);
            Console.Write("Pessoa listadas com êxito. Pressione <ENTER>");
            Console.ReadKey(true);
        }

        static void ListByName(int left)
        {
            while (true)
            {
                Console.Clear();
                Write.Line(left, 4, "Digite o nome ou parte: ", "", 50);
                string name = string.Empty;
                name = Write.Variable(name, left + 24, 4, 50, "", x => x.Trim().Length >= 1, "Digite o nome ou parte");
                if (name == null) return;
                IEnumerable<People> peoples = DalPeople.Get(name);
                var items = peoples.GetEnumerator();
                int top = 5;
                while (items.MoveNext())
                {
                    Console.SetCursorPosition(left, top++);
                    People people = items.Current;
                    Console.WriteLine("{0,5} {1,-52} {2,-10:dd/MM/yyyy}", people.Id, people.Name, people.DateBirthday);
                }
                if (top <= 5)
                {
                    Console.SetCursorPosition(left, ++top);
                    Console.Write("Nenhuma pessoa foi encontrada. Pressione <ENTER>");
                }
                else
                {
                    Console.SetCursorPosition(left, ++top);
                    Console.Write("Pessoa listadas com êxito. Pressione <ENTER>");
                }
                Console.ReadKey(true);
            }
        }

        static void Menu(int left)
        {
            Console.Clear();
            Console.SetCursorPosition(left, 5);
            Console.WriteLine("001 - Lista de Nomes");
            Console.SetCursorPosition(left, 6);
            Console.WriteLine("002 - Cadastrar Nome");
            Console.SetCursorPosition(left, 7);
            Console.WriteLine("003 - Alterar Nome");
            Console.SetCursorPosition(left, 8);
            Console.WriteLine("004 - Buscar Nome");
            Console.SetCursorPosition(left, 9);
            Console.WriteLine("000 - Sair");
            Console.SetCursorPosition(left, 10);
            Console.Write("Escolha: ");
        }
        static void Main(string[] args)
        {
            ConfigureMysql();
            int left = 22;
            while (true)
            {
                Menu(left);
                string menu = Field.Read(left + 9, 10, 3, "").Trim();
                if (menu == "0" || menu == "00" || menu == "000")
                    break;
                if (Field.EscapePressed)
                    break;
                switch (menu.Trim())
                {
                    case "0":
                    case "00":
                    case "000":
                        Dispose();
                        break;
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
                            }
                            else
                            {
                                Console.SetCursorPosition(left, 7);
                                Console.Write("Cancelado. Pressione <ENTER>");
                            }
                            Console.ReadKey(true);
                            break;
                        }
                    case "3":
                    case "03":
                    case "003":
                        {
                            bool? updated = Update(left);
                            if (updated == null)
                            {
                                Console.SetCursorPosition(left, 7);
                                Console.Write("Cancelado. Pressione <ENTER>");
                            }
                            else if (updated.Value)
                            {
                                Console.SetCursorPosition(left, 7);
                                Console.Write("Pessoa alterada com êxito. Pressione <ENTER>");
                            }
                            Console.ReadKey(true);
                            break;
                        }
                    case "4":
                    case "04":
                    case "004":
                        {
                            ListByName(left);
                            break;
                        }
                }
            }
        }
    }
}

