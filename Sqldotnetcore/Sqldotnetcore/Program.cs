using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
using KafkaNet.Protocol;
using KafkaNet.Model;
using KafkaNet;
using System.Collections.Generic;

namespace Sqldotnetcore
{
    class Program
    {
        public static void funct1()
        {
            // The below code will look for the "appsettings.json" in the working directory from where the application will run
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            string Log = configuration.GetSection("T1Log").Value;
            string subdir = configuration.GetSection("subdir").Value;
            LogWriter wt = new LogWriter("Trade input Processor - started", subdir, Log);
            string dbconnst = configuration.GetSection("ConnectionString").Value;
            SqlConnection dbconn = new SqlConnection(dbconnst);
            dbconn.Open();
            int traderefno = 0;
            string sqlText = "SELECT MAX(traderef) FROM [trade].[dbo].[tradeinput]";
            SqlCommand command = new SqlCommand(sqlText, dbconn);
            traderefno = (int)(command.ExecuteScalar());
            int skaccno = 0;
            sqlText = "SELECT MAX(skacc) FROM [trade].[dbo].[tradeinput]";
            command = new SqlCommand(sqlText, dbconn);
            skaccno = (int)(command.ExecuteScalar());
            SqlDataAdapter da = new SqlDataAdapter();
            dbconn.Close();
            int nameInt=0;
            Random randomnumber = new Random();
            while (true)
            {
                //                        Console.Write(@"Working, Pausing for {sleep}ms");
                
                traderefno = traderefno + 1;
                nameInt = nameInt + 2;
                skaccno = skaccno + 1;
                da.InsertCommand = new SqlCommand("INSERT INTO [trade].[dbo].[tradeinput] VALUES(@traderef,@tradestatus,@tradequantity,@tradetype,@skacc,@customername)", dbconn);
                da.InsertCommand.Parameters.Add("@traderef", SqlDbType.Int).Value = traderefno;
                
                da.InsertCommand.Parameters.Add("@tradestatus", SqlDbType.Char).Value = "new";
                da.InsertCommand.Parameters.Add("@tradequantity", SqlDbType.Int).Value = randomnumber.Next(1000,9999);
                if ((nameInt % 2 ==0))
                {
                    da.InsertCommand.Parameters.Add("@tradetype", SqlDbType.Char).Value = "buy";
                }
                else
                { 
                    da.InsertCommand.Parameters.Add("@tradetype", SqlDbType.Char).Value = "sell";
                }
                da.InsertCommand.Parameters.Add("@skacc", SqlDbType.Int).Value = skaccno;
                da.InsertCommand.Parameters.Add("@customerName", SqlDbType.Text).Value = "Customer Name " + randomnumber.Next(1,2000000);
                dbconn.Open();
                da.InsertCommand.ExecuteNonQuery();
                dbconn.Close();
                if (nameInt > 5000)
                {
                    wt.LogWrite("Total count : " + nameInt, subdir, Log);
                }
                //Thread.SpinWait(sleep);
            }
        }
        //This thread will publish the messages to kafka
        public static void funct2()
        {
//            var sleep = 3000;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            string Log = configuration.GetSection("T2Log").Value;
            string subdir = configuration.GetSection("subdir").Value;
            LogWriter wt = new LogWriter("Publish messages to Kafka!!!!!", subdir, Log);
            string dbconnst = configuration.GetSection("ConnectionString").Value;
            SqlConnection dbconn = new SqlConnection(dbconnst);
            string sqlText = "";
            SqlCommand command = new SqlCommand(sqlText, dbconn);
            SqlDataReader rdr = null;
            int traderefno = 0;
            String tradestatustxt = null;
            int tradequanityno = 0;
            string tradetyptxt = null;
            int skaccno = 0;
            string cust_name = null;
            while (true)
            {
                dbconn.Open();
                int message_counter = 0;
                sqlText = "SELECT MAX(skacc) FROM [trade].[dbo].[tradeinput]";
                command = new SqlCommand(sqlText, dbconn);
                message_counter = (int)(command.ExecuteScalar());
                dbconn.Close();
         		dbconn.Open();
                SqlDataAdapter da = new SqlDataAdapter();
                if (message_counter > 0)
                {
                    //
                    sqlText = "SELECT [traderef],[tradestatus],[tradequantity],[tradetype],[skacc],[customername] FROM [trade].[dbo].[tradeinput]";
                    command = new SqlCommand(sqlText, dbconn);
                    rdr = command.ExecuteReader();
                    while (rdr.Read())
                    {
                        traderefno = (int)rdr["traderef"];
                        tradestatustxt = rdr["tradestatus"].ToString();
                        tradequanityno = (int)rdr["tradequantity"];
                        tradetyptxt = rdr["tradetype"].ToString();
                        skaccno = (int)rdr["traderef"];
                        cust_name = rdr["customername"].ToString();

                        string payload = traderefno + "||" + tradestatustxt + "||" + tradequanityno + "||" + tradetyptxt + "||" +
                            skaccno + "||" + cust_name;
                        string topic = "TradeInQueue";
                        Message msg = new Message(payload);
                        Uri uri = new Uri("http://localhost:9092");
                        var options = new KafkaOptions(uri);
                        var router = new BrokerRouter(options);
                        var client = new Producer(router);
                        client.SendMessageAsync(topic, new List<Message> { msg }).Wait();
                    }
//                    Thread.SpinWait(sleep);
                }
                dbconn.Close();
            }
        }
        // Dummy thread, This is not required and can be deleted 
        public static void funct3()
        {
            var sleep = 3000;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            string Log = configuration.GetSection("T3Log").Value;
            string subdir = configuration.GetSection("subdir").Value;
            LogWriter wt = new LogWriter("Start the child log 3", subdir, Log);

            while (true)
            {
                wt.LogWrite("write something to the child log 3", subdir, Log);
                Thread.SpinWait(sleep);
            }
        }
        // This is the main routine which will invoke the child threads
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
