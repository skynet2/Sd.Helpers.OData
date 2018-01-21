using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NLog;
using Sd.Helpers.OData.InternalHelpers;

namespace Sd.Helpers.OData
{
    public class Converter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> ToList<T>(IQueryable collection)
        {
            try
            {
                var resultList = new List<T>();

                if (collection is IEnumerable<T> simpleCast) // for simple queries without $extend and $select
                {
                    return simpleCast.ToList();
                }

                foreach (var res in collection)
                {
                    object instance = (T)Activator.CreateInstance(typeof(T));

                    ProcessContainer(ref instance, res);

                    resultList.Add((T)instance);
                }

                return resultList;
            }
            catch (Exception ex)
            {
                Logger.Warn(JsonConvert.SerializeObject(new
                {
                    Message = "Error durring collection mapping, using fallback to NewtonJson. Please create ticket at https://github.com/skynet2/Sd.Helpers.OData/issues and" +
                              "add exception data + State data to the ticket.",
                    Exception = ex,
                    State = collection
                }));

                return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(collection)); // FallBack
            }
        }

        private static void ProcessContainer(ref object rootObject, object queryItem)
        {
            var instance = ObjectHelper.GetPropValue(queryItem, "Instance");

            if (instance != null)
            {
                rootObject = instance;
            }

            var value = ObjectHelper.GetPropValue(queryItem, "Container");

            while (value != null)
            {
                var propName = ObjectHelper.GetPropValue(value, "Name") as string;
                var propValue = ObjectHelper.GetPropValue(value, "Value");

                var complexValue = ObjectHelper.GetPropValue(propValue, "Instance");

                if (complexValue != null)
                {
                    propValue = complexValue;
                }

                var container = ObjectHelper.GetPropValue(propValue, "Container");

                var currentVariable = ObjectHelper.GetPropValue(rootObject, propName);

                if (container != null)
                {
                    var propInfo = rootObject.GetType().GetProperty(propName);

                    var propInternalData = ObjectHelper.GetPropValue(rootObject, propName);

                    if (propInternalData == null)
                    {
                        ObjectHelper.SetPropValue(rootObject, propName, Activator.CreateInstance(propInfo.PropertyType));

                        currentVariable = ObjectHelper.GetPropValue(rootObject, propName);
                    }

                    ProcessContainer(ref currentVariable, propValue);
                }
                else
                {
                    ObjectHelper.SetPropValue(rootObject, propName, propValue);
                }

                value = ObjectHelper.GetPropValue(value, "Next");
            }
        }
    }
}
