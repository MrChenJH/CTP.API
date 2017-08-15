using System;
using System.Collections.Generic;
using System.Text;
using CTP.Model.Link;
using CTP.Util;

namespace CTP.RealtimeInteractive
{
    public interface ICore
    {

         Rule _rule { get; set; }

         Type _type { get; set; }

         object _data { get; set; }

        LinkType _linkType { get; set; }
        /// <summary>
        /// 解析数据
        /// </summary>
        void AnalyticData();

        /// <summary>
        /// 执行Sql
        /// </summary>
        int ExcuteSql();
    }
}
