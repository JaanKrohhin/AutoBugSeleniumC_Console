using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoBugSelenium
{
    public enum TestType
    {
        Auth,
        Buy,
        Search,
        Compare
    }
    public class LogHelpers
    {   
        public StreamWriter? stream;
        public static bool EndAllTests = true;
        public string folderpath { get; set; }
        public string? filepath;
        public LogHelpers(string folderpath)
        {
            this.folderpath = folderpath;
        }
        private string generateId(TestType type)
        {
            Random rnd = new Random();
            return $"{type}_{DateTime.Now.Ticks}-{rnd.Next(0, 2147483647)}";
        }
        public void CreateLogFile(int i, TestType testType)
        {
            filepath = folderpath + @"\Test" +generateId(testType)+ ".log";
            stream = File.AppendText(filepath);
        }

        public void WriteToFile(string Message)
        {
            stream.WriteLine("{0} {1} {2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), Message);
            stream.Flush();
        }

        public void Info(string message)
        {
            stream.WriteLine("{0} {1} {2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), "Info: "+message);
            stream.Flush();
        }
        public void Error(string message)
        {
            stream.WriteLine("{0} {1} {2}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), "Error: "+message);
            stream.Flush();
        }

        //Static functions
        public static void CheckDirectoryExists()
        {
            string filePath = @"Logs\";

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
                Console.WriteLine("Directry 'Logs' Created");
            }

        }
        public static string CreateFolderForSession()
        {
            string filePath = string.Format(@"Logs\session_{0:yyyy_mm_dd_hh_mm_ss}", DateTime.Now);
            Directory.CreateDirectory(filePath);
            return filePath;
        }
    }
}
