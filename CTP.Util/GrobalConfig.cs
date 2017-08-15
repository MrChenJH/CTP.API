using System;
using System.Collections.Generic;
using System.Text;

namespace CTP.Util
{
    /// <summary>
    /// Redis执行命令
    /// </summary>
    public enum RedisCommand
    {
        lpush,
        brpop,
        blpop
    }


    /// <summary>
    /// 规则
    /// </summary>
    public class Rule
    {

        public string DataDriver { get; set; }
        public string Class { get; set; }

        public string Factory { get; set; }

        public String MaxIdSql { get; set; }
        public string Key { get; set; }

        public string Del { get; set; }

        public string Update { get; set; }

        public string Insert { get; set; }

        public string LinkKey { get; set; }
    }
    /// <summary>
    /// 参数信息
    /// </summary>
    public class Profile
    {
        public static List<Rule> rules = new List<Rule>();
        public static string redisIp = string.Empty;
        public static string typeLink = string.Empty;
        public static string sqlseverCon = string.Empty;
        public static string mySqlCon = string.Empty;

    }

    /// <summary>
    /// 异常类型
    /// </summary>
    public class ErrorCode
    {
        /// <summary>
        /// Redis访问出错
        /// </summary>
        public const string ReadRedisErrorCode = "001";

        /// <summary>
        /// 值非法
        /// </summary>
        public const string IllegalValueErrorCode = "002";

        /// <summary>
        /// Keys值不存在
        /// </summary>
        public const string NotExistKeyErrorCode = "003";

        /// <summary>
        /// 系统异常
        /// </summary>
        public const string SystemErrorCode = "004";


        /// <summary>
        /// 代理异常
        /// </summary>
        public const string AgentFactoryErrorCode = "005";

    }

    /// <summary>
    /// Api后台几类方法
    /// </summary>
    public enum ExecMethod
    {
        /// <summary>
        /// 特殊查询
        /// </summary>
        Specialquery,

        /// <summary>
        /// 查询
        /// </summary>
        Query,

        /// <summary>
        /// 翻页查询
        /// </summary>
        PageQuery,

        /// <summary>
        /// 新增
        /// </summary>
        Add,

        /// <summary>
        /// 更新
        /// </summary>
        Update,

        /// <summary>
        /// 删除
        /// </summary>
        Delete,
    }

    public class ProcessException : Exception
    {
        public ProcessException(string msg) : base(msg)
        {

        }
    }
}
