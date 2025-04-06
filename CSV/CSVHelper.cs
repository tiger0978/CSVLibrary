using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization.Configuration;

namespace CSV
{
    public class CSVHelper
    {
        public string rootpath;
        public CSVHelper(string rootPath)
        {
            // \\ "" '' 
            // Hello\r\nWorld
            this.rootpath = rootPath;
        }
        public void WriteToCSV<T>(string path,T t)
        {
            string filePath = CheckPathExisted(path);
            using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8))
            {
                WriteDate<T>(writer, t);
            }
            Console.WriteLine("資料已成功寫入 CSV 檔案！");
        }

        public void WriteToCSV<T>(string path, List<T> datas)
        {
            string filePath = CheckPathExisted(path);
            using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8))
            {
                foreach (var t in datas)
                {
                    WriteDate<T>(writer, t);
                }
            }

            Console.WriteLine("資料已成功寫入 CSV 檔案！");
        }

        private void WriteDate<T>(StreamWriter writer, T t)
        {
            var props = t.GetType().GetProperties();
            string data = null;
            StringBuilder builder = new StringBuilder();    
            
            //using 使用可正確執行IDisposable
            {
                foreach (var prop in props)
                {
                    //if (prop.Name == "Item")
                    //{
                    //    continue;
                    //}
                    builder.Append(prop.GetValue(t).ToString()+",");
                }
                //data = data.TrimEnd(',');
                writer.WriteLine(builder.ToString(0,builder.Length-1));
                builder.Clear();
                builder = null;
            }
            //Console.WriteLine(data);
        }

        private string CheckPathExisted(string path)
        {
            string filePath = Path.Combine(rootpath, path);
            if (!filePath.Contains(".csv"))
            {
                throw new Exception("路徑中沒有包含檔案路徑存在");
            }
            var paths = path.Split('\\');
            string directoryPath = String.Join("\\", path.Split('\\').ToList().Take(paths.Length - 1));
            directoryPath = Path.Combine(rootpath, directoryPath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return filePath;
        }


        public List<T> ReadCSV<T>(string path, string date) where T : new()
        {
            string filePath = Path.Combine(rootpath,path);
            List<T> costRecords = new List<T>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string record = reader.ReadLine();
                    string[] recordArray = record.Split(',');
                    string datetime = DateTime.Parse(recordArray[1]).ToString("yyyy-MM-dd");
                    if (recordArray[2] == "Price")
                    {
                        continue;
                    }
                    if (datetime == date)
                    {
                        T t = new T();
                        var props = t.GetType().GetProperties();
                        for (int i = 0; i < props.Length; i++)
                        {
                            if (recordArray[i]!="")
                                props[i].SetValue(t, recordArray[i]);
                        }
                        costRecords.Add(t);
                    }
                }
            }
            return costRecords;
        }

        public List<T> ReadCSV<T>(string path, int startLine, int counts) where T : new()
        {
            string filePath = Path.Combine(rootpath, path);
            List<T> records = new List<T>();
            if (!File.Exists(filePath))
            {
                return records;
            }
            int count = 0;
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    count++;
                    if (count < startLine)
                    {
                        reader.ReadLine();
                        continue;
                    }
                    string record = reader.ReadLine();
                    string[] recordArray = record.Split(',');
                    T t = new T();
                    var props = t.GetType().GetProperties();
                    for (int i = 0; i < props.Length; i++)
                    {
                        if (recordArray[i] != "")
                        {
                            if (props[i].PropertyType == typeof(Guid))
                            {
                                props[i].SetValue(t, Guid.Parse(recordArray[i]));
                                continue;
                            }
                            props[i].SetValue(t, Convert.ChangeType(recordArray[i], props[i].PropertyType));
                        }
                    }
                    records.Add(t);
                    if (records.Count == counts)
                    {
                        break;
                    }
                }
            }
            return records;
        }
        public List<T> ReadCSVBySpan<T>(string path, int startLine, int counts) where T : new()
        {
            string filePath = Path.Combine(rootpath, path);
            List<T> records = new List<T>();
            if (!File.Exists(filePath))
            {
                return records;
            }
            int count = 0;
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    count++;
                    if (count < startLine)
                    {
                        reader.ReadLine();
                        continue;
                    }

                    string[] recordArray = new string[typeof(Person).GetProperties().Length];

                    var span = reader.ReadLine().AsSpan();
                    int resultCount = 0;
                    int startIndex = 0;

                    // 尋找所有逗號並提取欄位
                    for (int i = 0; i < span.Length; i++)
                    {
                        if (span[i] == ',')
                        {
                            // 提取欄位並加入results陣列
                            recordArray[resultCount++] = span.Slice(startIndex, i - startIndex).ToString();
                            startIndex = i + 1;
                        }
                    }
                    // 不要忘記最後一個欄位
                    recordArray[resultCount++] = span.Slice(startIndex).ToString();

                    T t = new T();
                    var props = t.GetType().GetProperties();
                    for (int i = 0; i < props.Length; i++)
                    {
                        if (recordArray[i] != "")
                        {
                            if (props[i].PropertyType == typeof(Guid))
                            {
                                props[i].SetValue(t, Guid.Parse(recordArray[i]));
                                continue;
                            }
                            props[i].SetValue(t, Convert.ChangeType(recordArray[i], props[i].PropertyType));
                        }
                    }
                    records.Add(t);
                    if (records.Count == counts)
                    {
                        break;
                    }
                }
            }
            return records;
        }

        public List<T> ReadCSV<T>(string path) where T : new()
        {
            return ReadDatas<T>(path);
        }


        private List<T> ReadDatas<T>(string path) where T : new()
        {
            string filePath = Path.Combine(rootpath, path);
            List<T> records = new List<T>();
            if (!File.Exists(filePath))
            {
                return records;
            }
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string record = reader.ReadLine();
                    string[] recordArray = record.Split(',');
                    T t = new T();
                    var props = t.GetType().GetProperties();
                    for (int i = 0; i < props.Length; i++)
                    {
                        if (recordArray[i] != "")
                        {
                            if (props[i].PropertyType == typeof(Guid))
                            {
                                props[i].SetValue(t, Guid.Parse(recordArray[i]));
                                continue;
                            }
                            props[i].SetValue(t, Convert.ChangeType(recordArray[i], props[i].PropertyType));
                        }
                    }
                    records.Add(t);
                }
            }
            return records;

        }

        //若library做更新，需要重新製作dll檔案，並在專案中重新插入參考
        public void DeleteCSV(string path,string recordDate)
        {
            string filePath = Path.Combine(rootpath,path);
            if (File.Exists(filePath))
            {
                List<string> lines =  File.ReadAllLines(filePath).ToList();
                string contentToDelete = recordDate;
                lines.RemoveAll(line => line.Contains(contentToDelete));
                File.WriteAllLines(filePath, lines);
                Console.WriteLine("特定內容已從 CSV 檔案中刪除。");
            }
        }
    }
}
