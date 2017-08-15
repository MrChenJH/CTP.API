using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTP.Redis
{


    public class ReturnData
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool sucess { get; set; }
        public string msg { get; set; }
    }

    public class ErrorData: ReturnData
    {
        /// <summary>
        /// 错误编码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 错误发生时间
        /// </summary>
        public string Occurrencetime { get; set; }

    }

    public class RSingle<T> : ReturnData where T : class
    {
        public T Result { get; set; }
    }

    public class RList<T> : ReturnData where T : class
    {
        public List<T> data { get; set; }
    }

    public class RPage<T> : ReturnData where T : class
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public long total { get; set; }

        public List<T> data { get; set; }
    }
}
