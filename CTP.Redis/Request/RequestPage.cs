using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTP.Redis.Request
{
    public class RequestPage<T> : RequsetBase
        where T : class
    {
     

        /// <summary>
        ///对应Model
        /// </summary>
        public T Model { get; set; }

        /// <summary>
        /// 开始索引
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// 结束索引
        /// </summary>
        public int Stop { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyValue { get; set; }

    }
}
