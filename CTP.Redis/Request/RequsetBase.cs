using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTP.Redis
{
    public class RequsetBase
    {

        public RequsetBase()
        {
            isNeedSync = false;
        }

        /// <summary>
        /// 是否需要同步
        /// </summary>
        public bool isNeedSync { get; set; }
        /// <summary>
        /// 是否加密
        /// </summary>
        public int isSec { get; set; }

    }

}
