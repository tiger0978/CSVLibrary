using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSV
{
    [MemoryDiagnoser]
    public class OriginalWriterVSOptimizedWriter
    {
        static PropertyInfo[] props = typeof(Person).GetProperties();

        static int propLength = props.Length;
        static char[] buffer = new char[90];
        static StringBuilder builder = new StringBuilder(90);

        delegate object GetterDelegate(object target);
        private static readonly GetterDelegate[] _getterDelegates =
        props.Select(p => CreateGetter(p)).ToArray();
        private static GetterDelegate CreateGetter(PropertyInfo property)
        {

            //var targetParam = Expression.Parameter(typeof(object), "target");
            //var castTarget = Expression.Convert(targetParam, property.DeclaringType);
            //var propertyGetter = Expression.Property(castTarget, property);
            //var castResult = Expression.Convert(propertyGetter, typeof(object));

            //var lambda = Expression.Lambda<GetterDelegate>(castResult, targetParam);
            //return lambda.Compile();

            var targetType = typeof(object);
            var targetParam = Expression.Parameter(targetType, "target");
            var castTarget = Expression.Convert(targetParam, property.DeclaringType);
            var propertyGetter = Expression.Call(castTarget, property.GetGetMethod());
            var lambda = Expression.Lambda<GetterDelegate>(propertyGetter, targetParam);
            return lambda.Compile();
        }


        [Benchmark]
        public void OriginalWriter()
        {
            for (int i = 0; i < 3000000; i++)
            {
                Person t = new Person() { id = "1000", firstName = "Osmund", lastName = "MacKissack", email = "omackissackrr@eepurl.com", gender = "Male", ipAddress = "135.236.239.173" };
                var props = t.GetType().GetProperties();
                StringBuilder builder = new StringBuilder();
                {
                    foreach (var prop in props)
                    {
                        builder.Append(prop.GetValue(t).ToString() + ",");
                    }
                    builder.ToString(0, builder.Length - 1);
                    builder.Clear();
                    builder = null;
                }
            }

        }

        [Benchmark]
        public void OptimizedWriter()
        {
            for (int i = 0; i < 3000000; i++)
            {
                Person t = new Person() { id = "1000", firstName = "Osmund", lastName = "MacKissack", email = "omackissackrr@eepurl.com", gender = "Male", ipAddress = "135.236.239.173" };
                StringBuilder builder = new StringBuilder();
                {
                    int index = 0;
                    for (int j = 0; j < propLength; j++)
                    {
                        builder.Append(_getterDelegates[index](t));
                        if (j < propLength - 1)
                        {
                            builder.Append(',');
                        }
                        index += 1;
                    }
                    builder.Append('\n');
                    builder.ToString();
                    builder.Clear();
                    builder = null;
                }
            }


        }


        [Benchmark]
        public void Buffer()
        {

            for (int i = 0; i < 3000000; i++)
            {
                Person t = new Person() { id = "1000", firstName = "Osmund", lastName = "MacKissack", email = "omackissackrr@eepurl.com", gender = "Male", ipAddress = "135.236.239.173" };

                for (int j = 0; j < propLength; j++)
                {
                    builder.Append(_getterDelegates[j](t));
                    if (j < propLength - 1)
                    {
                        builder.Append(',');
                    }
                }
                builder.Append('\n');
                builder.CopyTo(0, buffer, 0, builder.Length);
                builder.Clear();
            }


        }
    }
}
