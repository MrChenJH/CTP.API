using System;
using System.Collections.Generic;
using System.Text;
using CTP.Util;

namespace CTP.DataSynchronization
{
    public interface ICore
    {
        void SetClient(RedisHelper helper);
        void InitRule(Rule rule);

        /// <summary>
        /// 提取数据
        /// </summary>
        void ExtractingData();

        /// <summary>
        /// 解析数据
        /// </summary>
        void AnalyticData();

        /// <summary>
        /// 执行Sql
        /// </summary>
        void ExcuteSql();
    }
}
