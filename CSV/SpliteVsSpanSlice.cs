using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CSV
{
    [MemoryDiagnoser]
    public class SpliteVsSpanSlice
    {

        static PropertyInfo[] props = typeof(Person).GetProperties();

        static int PropsLength = props.Length;
        delegate void SetterDelegate(object target, object value);
        private static readonly SetterDelegate[] _setterDelegates =
        props.Select(p => CreateSetter(p)).ToArray();


        private static SetterDelegate CreateSetter(PropertyInfo property)
        {
            var targetType = typeof(object);
            var valueType = typeof(object);

            var targetParam = Expression.Parameter(targetType, "target");
            var valueParam = Expression.Parameter(valueType, "value");

            var castTarget = Expression.Convert(targetParam, property.DeclaringType);
            var castValue = Expression.Convert(valueParam, property.PropertyType);

            var propertySetter = Expression.Call(castTarget, property.GetSetMethod(), castValue);

            var lambda = Expression.Lambda<SetterDelegate>(propertySetter, targetParam, valueParam);
            return lambda.Compile();
        }


        [Benchmark]
        public void Splite() 
        {
            string testData = "1000,Osmund,MacKissack,omackissackrr@eepurl.com,Male,135.236.239.173";
            string[] recordArray = testData.Split(',');


            List<Person> persons = new List<Person>();

            Person t = new Person();

            PropertyInfo[] props = typeof(Person).GetProperties();
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
            persons.Add(t);
        }

        [Benchmark]
        public void Slice()
        {

            string input = "572,Christa,cornhill,ccornhillfv@theglobeandmail.com,Female,175.238.233.64";
            ReadOnlySpan<char> datas = input.AsSpan();

            Person dataModel = new Person();

            int start = 0;

            for (int i = 0; i < PropsLength; i++)
            {
                // 找逗號位置
                int commaIndex = datas.Slice(start).IndexOf(',');

                if (commaIndex == -1)
                {
                    // 最後一欄
                    _setterDelegates[i](dataModel, datas.Slice(start).ToString());
                    break;
                }
                else
                {
                    _setterDelegates[i](dataModel, datas.Slice(start, commaIndex).ToString());
                    start += commaIndex + 1;
                }
            }

            // 這裡就已經是完整填好的 DataModel
            List<Person> dataList = new List<Person> { dataModel };

        }
    }
}
