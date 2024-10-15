using System;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{
    sealed class Logger
    {
        private static Logger? _instance;
   
        public static Logger Instance
        {
            get
            {            
                _instance ??= new Logger();
                return _instance;
            }
        }

        private Logger() { }

        public void Log(string path, string text)
        {
            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] input = Encoding.Default.GetBytes(text + "\n");
                fstream.Seek(0, SeekOrigin.End);
                fstream.Write(input);
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb = sb.Append(@"C:\Файлообмен\").Append("DP.P1.").Append(now.ToString("yyyy-MM-dd.HH-mm-ss")).Append(".log");

            string path = sb.ToString();
            Logger logger = Logger.Instance;

            //запуск пяти потоков
            for (int i = 1; i < 6; i++)
            {
                Thread myThread = new(StartLog);
                myThread.Name = $"Поток {i}";
                myThread.Start();
                myThread.Join();
            }

            void StartLog()
            {
                string mesLevel = "";
                string mesDesc = "";
                object locker = new(); // объект-заглушка

                lock (locker)
                {
                    int x = 1;
                    for (int i = 1; i < 6; i++)
                    {
                        Random rnd = new Random();
                        int randomNum = rnd.Next(0, 5);
                        if (randomNum == 0) { mesLevel = "TRACE"; mesDesc = "Детальная информация"; }
                        if (randomNum == 1) { mesLevel = "INFO"; mesDesc = "Программа работает нормально"; }
                        if (randomNum == 2) { mesLevel = "WARN"; mesDesc = "Предупреждение!"; }
                        if (randomNum == 3) { mesLevel = "ERROR"; mesDesc = "Ошибка!!"; }
                        if (randomNum == 4) { mesLevel = "FATAL"; mesDesc = "Разрушительная ошибка!!!"; }

                        DateTime now1 = DateTime.Now;
                        StringBuilder message = new StringBuilder();
                        message = message.Append(now1.ToString("yyyy-MM-dd HH-mm-ss")).Append(" [" + mesLevel + "] ").Append(mesDesc);
                        string messageToLog = message.ToString();
                        logger.Log(path, messageToLog);

                        Console.WriteLine($"{Thread.CurrentThread.Name}: {x}" /*+ x.ToString()*/ + ": " + logger.GetHashCode().ToString() + " " + messageToLog);
                        x++;
                        Thread.Sleep(500);

                    }
                }
            }
            Console.WriteLine("END");
        }
    }
}