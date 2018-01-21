using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Sd.Helpers.OData.InternalHelpers;

namespace Sd.Helpers.OData
{
    public class Converter
    {
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

                foreach (var res in collection)
                {
                    object instance = (T)Activator.CreateInstance(typeof(T));

                    resultList.Add((T)instance);
                }

                return resultList;
            }
            catch (Exception ex)
            {
                return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(collection)); // FallBack
            }
        }
    }
}
