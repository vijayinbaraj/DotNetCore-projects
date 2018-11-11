using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;

namespace multithreading4
{
    class Program
    {
        public static void funct1()
        {
            var sleep = 3000;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            //string Log = configuration.GetSection("T1Log").Value;
            string Log = "Child1_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            string subdir = configuration.GetSection("subdir").Value;
            LogWriter wt = new LogWriter("Start the child log 1", subdir, Log);

            while (true)
            {
                //                        Console.Write(@"Working, Pausing for {sleep}ms");
                wt.LogWrite("write something to the child log 1", subdir, Log);
                Thread.SpinWait(sleep);
            }
        }
        public static void funct2()
        {
            var sleep = 3000;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            //string Log = configuration.GetSection("T2Log").Value;
            string Log = "Child2_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            string subdir = configuration.GetSection("subdir").Value;
            LogWriter wt = new LogWriter("Start the child log 2", subdir, Log);
            
            while (true)
            {
                wt.LogWrite("write something to the child log 2", subdir, Log);
                Thread.SpinWait(sleep);
            }
        }
        public static void funct3()
        {
            var sleep = 3000;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            //string Log = configuration.GetSection("T3Log").Value;
            string Log = "Child3_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            string subdir = configuration.GetSection("subdir").Value;
            LogWriter wt = new LogWriter("Start the child log 3", subdir, Log);

            while (true)
            {
                //                        Console.Write(@"Working, Pausing for {sleep}ms");
                wt.LogWrite("write something to the child log 3", subdir, Log);
                Thread.SpinWait(sleep);
            }
        }
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            string subdir = configuration.GetSection("subdir").Value;
            string logname = configuration.GetSection("T2log").Value;
            Thread firstthread = new Thread(new ThreadStart(funct1));
            firstthread.Start();
            Thread secondthread = new Thread(new ThreadStart(funct2));
            secondthread.Start();
            Thread thirdthread = new Thread(new ThreadStart(funct3));
            thirdthread.Start();
        }
        

    }
}
