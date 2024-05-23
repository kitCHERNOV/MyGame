using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using Npgsql;
using System.Xml;
using System.Runtime.CompilerServices;

namespace MyGame
{
    public class User // класс для хранения данных, для дальнейшего взаимодействия и с бд и с xml файлом
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
                Console.Write("Введите путь к файлу: ");
                string filePath = Console.ReadLine();

                // Проверка на существование файла
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

        // Чтение xml файла
        static void ReadXMLFile(ref List<User> users)
        {
            Console.WriteLine("Теперь приступим к работе с самим xml файлом");
            XElement xml = null;
            string findtag;
            bool reload = false; // булевая переменная для контроля цикла do while

            do
            {
                try
                {
                    string filepath = EntryPathOfFile(); // Получаем рабочий путь файла
                    xml = XElement.Load(filepath);
                    reload = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Файл не удалось прочитать, сообщение об ошибке: {ex.Message}");
                    reload = true;
                }
            }
            while(reload);

            //Теперь запросим тег/элемент, который нам нужно найти
            do
            {
                Console.Write("Ввод искомого элемента: ");
                findtag = Console.ReadLine();

                foreach(XElement element in xml.Descendants()) // Получаем построчно все теги т.е. - их название и содержимое
                {
                    if (element.Name == findtag){
                        users.Add(new User(element.Value)); // добваляем данные искомого тега
                    }
                }
                // Проверка для пропуска стадии записи данных
                if (users.Count == 0)
                {
                    Console.WriteLine("Искомые данные не найдены, повторите попытку..");
                    reload = true;
                }else
                {
                    reload = false;
                }
            } while (reload);


        }

        // Функция чтения конфигурационного файла для подключения к бд
        static Dictionary<string, string> ReadConfigFile(string configPath)
        {
            Dictionary<string, string> confDict = new Dictionary<string, string>();
            XElement xml = XElement.Load(configPath);
            foreach (XElement element in xml.Descendants())
            {
                if(element.Name == "Host")
                {
                    confDict.Add("Host", element.Value);
                }
                else if (element.Name == "Port")
                {
                    confDict.Add("Port", element.Value);
                }
                else if (element.Name == "Username")
                {
                    confDict.Add("Username", element.Value);
                }
                else if (element.Name == "Password")
                {
                    confDict.Add("Password", element.Value);
                }
                else if (element.Name == "Database")
                {
                    confDict.Add("Database", element.Value);
                }
            }
            return confDict;
        }


        static Exception ConnToDB(ref List<User> users, string configPath)
        {
            // строка подключения к базе данных PostgreSQL
            Dictionary<string, string> conf = ReadConfigFile(configPath); // Берем список элементов для подключения
            
            string connectionString = String.Format("Host={0};Port={1};Username={2};Password={3};Database={4}", conf["Host"], conf["Port"], conf["Username"], conf["Password"], conf["Database"]);

            // Создание подключения 
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    // Открытие подключения
                    connection.Open();
                    
                    for (int i = 0; i < users.Count; i++)
                    {
                        // Создадим строку обращения к таблице
                        //string comandStr = String.Format("INSERT INTO users (date, name) VALUES (@n1, @q1);", users[i].Name);
                        //Console.WriteLine(comandStr);

                        // Пример выполнения SQL-запроса
                        using (var command = new NpgsqlCommand("INSERT INTO users (date, name) VALUES (@n1, @q1);", connection))
                        {
                            // Запись передаваемых значений
                            command.Parameters.AddWithValue("n1", DateTime.Now.Date); 
                            command.Parameters.AddWithValue("q1", users[i].Name);

                            command.ExecuteNonQuery(); // Выполнение запроса
                            //int nRows = command.ExecuteNonQuery();
                            //Console.Out.WriteLine(String.Format("Number of rows inserted={0}", nRows));
                            
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    // Обраюотка ошибок
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                    return ex;
                }
            }

        }

        static void Main(string[] args)
        {
            // variables
            Exception ex = null;
            List<User> users = new List<User>();
            Console.WriteLine("Сначала необходим конфигурационный файл для подключения к бд");
            var configPath = EntryPathOfFile();

            do
            {
                //string pathOfFile = EntryPathOfFile();
                ReadXMLFile(ref users);
                if (users.Count == 0) // Если нужных данных не нашлось, то переходить к записи в бд нет смысла
                {
                    Console.WriteLine("Искомые данные не найдены");
                    continue;
                }
                ex = ConnToDB(ref users, configPath);
                Console.WriteLine("Данные записаны");
                users.Clear();
            }
            while (ex == null);
            
            // Ожидание ввода пользователя перед завершением программы
            Console.WriteLine("Нажмите любую клавишу для завершения...");
            Console.ReadLine();
            
        }
    }
}
