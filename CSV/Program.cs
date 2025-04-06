using BenchmarkDotNet.Running;
using Iced.Intel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CSV
{
    internal class Program
    {
       

        static void Main(string[] args)
        {



            // char[] c = new char[100];
            //char[] a = "44,Kellsie,Middlemiss,kmiddlemiss17@scientificamerican.com,Genderqueer,103.209.96.193".ToArray();
            //char[] b = "44,Kellsie,Middlemiss,kmiddlemiss17@scientificamerican.com,Genderqueer,103.209.96.193".ToArray();

            //StreamWriter writer = new StreamWriter("data.csv",true);
            //writer.WriteLine(a);
            //writer.WriteLine(b);
            //writer.Close();


            var summary = BenchmarkRunner.Run<OriginalWriterVSOptimizedWriter>();


            //    string[] results = new string[typeof(Person).GetProperties().Length]; 
            //    string testData = "1000,Osmund,MacKissack,omackissackrr@eepurl.com,Male,135.236.239.173";
            //    var span = testData.AsSpan();

            //    int resultCount = 0;
            //    int startIndex = 0;

            //    // 尋找所有逗號並提取欄位
            //    for (int i = 0; i < span.Length; i++)
            //    {
            //        if (span[i] == ',')
            //        {
            //            // 提取欄位並加入results陣列
            //            results[resultCount++] = span.Slice(startIndex, i - startIndex).ToString();
            //            startIndex = i + 1;
            //        }
            //    }
            //    // 不要忘記最後一個欄位
            //    results[resultCount++] = span.Slice(startIndex).ToString();





            //    CSVHelper helper = new CSVHelper(ConfigurationManager.AppSettings["filePath"]);
            //    helper.WriteToCSV<CSVModel>("data.csv", new CSVModel()
            //    {
            //        dateTime = "2023-07-26",
            //        cost = "222",
            //        account = "現金",
            //        item = "飲食",
            //        member = "自己"

            //    });
            //    var list = helper.ReadCSV<CSVModel>("data.csv", "2023-07-26");
            //    foreach (var data in list)
            //    {
            //        Console.WriteLine(data.cost);
            //    }
            //    Console.ReadKey();
            //}
        }
}
}
