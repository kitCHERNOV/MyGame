using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Xml;

namespace MyGame
{
    public class User
    {
        //public int Id { get; set; }
        //public DateTime Date { get; set; }
        public string Name { get; set; }
        public User(string name)
        {
            Name = name;
        }
    }

    class Program
    {   
        static string EntryPathOfFile() // функция для ввода пути файла и проверки наличия такового файла 
        {

            while (true)
            {
                Console.WriteLine("Введите путь к файлу: ");
                string filePath = Console.ReadLine();


                if (File.Exists(filePath))
                {
                    Console.WriteLine("Файл существует");
                    return filePath;
                }else
                {
                    Console.WriteLine("Файл не существует");
                }
            }                 
        }

        static void ReadXMLFile()
        {
            XmlDocument xDoc = new XmlDocument(); // XmlDocument - класс работы с xml документами
            xDoc.Load("C:/Users/Админ/source/repos/MyGame/MyGame/XMLFile1.xml"); // Найс оно работет 
            XmlElement xRoot = xDoc.DocumentElement; // Ну какой то корень xml файла возвращает
            if (xRoot != null)
            {
                // Обходим все узлы в корневом элементе
                foreach (XmlElement xnode in xRoot)
                {
                    // теперь получим атрибут name
                    XmlNode attr = xnode.Attributes.GetNamedItem("name");
                    Console.WriteLine(attr.Value);
                }
            }
        }

        static void ReadValuesOfFile(string filepath)
        {   
            
        }

        static void ConnToDB()
        {
            // строка подключения к базе данных PostgreSQL
            string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=7703;Database=MyGame";

            // Создание подключения 
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    // Открытие подключения
                    connection.Open();

                    // Пример выполнения SQL-запроса
                    using (var command = new NpgsqlCommand("Select name from aa;", connection))
                    {
                        // Чтение результата
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine(reader.GetString(0));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Обраюотка ошибок
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }

        }

        static void Main(string[] args)
        {
            //string filePath;
            //filePath = EntryPathOfFile();

            //var Users = new List<User> 
            //{ 
            //    new User ("Name"),
            //    new User ("Bill")
            //};

            ReadXMLFile();

            // Ожидание ввода пользователя перед завершением программы
            Console.WriteLine("Нажмите любую клавишу для заврешения...");
            Console.ReadLine();
            
        }
    }
}
