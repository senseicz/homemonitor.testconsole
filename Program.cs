using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.IO;

namespace HomeMonitorConsole
{
    class Program
    {
        const string API_URL = "http://localhost:58448/api/monitor";
        
        static void Main(string[] args)
        {
            var rnd = new Random();

            do
            {

                var tempList = new List<Measurement>
                    {
                        GetMeasurement(rnd, "1", "Teploměr vnitřní"),
                        GetMeasurement(rnd, "2", "Teploměr venkovní"),
                        GetMeasurement(rnd, "3", "Teploměr na chodbě")
                    };

                var batch = new MeasurementBatch
                    {
                        BatchDate = DateTime.Now,
                        DeviceId = "1",
                        DeviceName = "Tonda's raspberry",
                        MeasurementType = 1,
                        Measurements = tempList
                    };

                DoPost(batch);

                for (int i = 7; i > 0; i--)
                {
                    Console.Write("..." + i.ToString() + "...");
                    Thread.Sleep(10000);
                }

            } while (true);
            
            /*
            do
            {
                var temp1 = GetMeasurement(rnd, "1", "Teploměr vnitřní");
                DoPost(temp1);

                var temp2 = GetMeasurement(rnd, "2", "Teploměr venkovní");
                DoPost(temp2);

                var temp3 = GetMeasurement(rnd, "3", "Teploměr na chodbě");
                DoPost(temp3);

                //Console.ReadLine();

                Thread.Sleep(60000);
            } while (true); //(Console.ReadKey(true).Key != ConsoleKey.X);
            */
        }

        private static void DoPost(MeasurementBatch batch)
        {
            var measurementJson = JsonConvert.SerializeObject(batch);

            var myReq = (HttpWebRequest)WebRequest.Create(API_URL);
            myReq.Method = "POST";
            myReq.ContentType = "application/json";
            byte[] byteArray = Encoding.UTF8.GetBytes(measurementJson);
            myReq.ContentLength = byteArray.Length;
            Stream dataStream = myReq.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            var response = (HttpWebResponse)myReq.GetResponse();

            Console.WriteLine("Odeslána teplota: {0}", measurementJson);
            Console.WriteLine("============================");
        }

        private static Measurement GetMeasurement(Random rnd, string sensorId, string sensorName)
        {
            return new Measurement
                {
                    MeasurementDate = DateTime.Now,
                    SensorId = sensorId,
                    SensorName = sensorName,
                    Unit = "Celsius",
                    Value = ((float)rnd.Next(180, 260)/10).ToString("N1")
                };
        }
    }

    public class MeasurementBatch
    {
        public int BatchId { get; set; }
        public int MeasurementType { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public DateTime BatchDate { get; set; }
        public List<Measurement> Measurements { get; set; }
    }

    public class Measurement
    {
        public int MeasurementId { get; set; }
        public int BatchId { get; set; }
        public string SensorId { get; set; }
        public string SensorName { get; set; }
        public DateTime MeasurementDate { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public DateTime InsertDate { get; set; }
    }
}
