
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTP.Redis.Request
{
    public class RequesList<T> : RequsetBase
        where T : class, new()
    {
        public RequesList()
        {
            Model = new T();
        }
        
        /// <summary>
        ///对应Model
        /// </summary>
        public T Model { get; set; }


    }
}
