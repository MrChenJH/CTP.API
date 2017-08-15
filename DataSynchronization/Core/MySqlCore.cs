using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CTP.Util;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;
using NLog;
using CTP.Model.Link;

namespace CTP.DataSynchronization
{
    public class MySqlCore : ICore
    {

        /// <summary>
        /// 日记
        /// </summary>
        private Logger Logger = LogManager.GetCurrentClassLogger();
        private RedisHelper _redisHelper { get; set; }
        private MySqlParameter[] _mysqlParams { get; set; }
        private Rule _rule { get; set; }
        private Object _obj { get; set; }
        private Type _type { get; set; }

        private LinkType _linkType { get; set; }

        private MmsqlHelper _mySqlHepler { get; set; }
        public bool _isContinue { get; set; }

        public MySqlCore()
        {
            _isContinue = true;
        }
        public void ExtractingData()
        {
            if (_rule == null) return;
            string reuslt = string.Empty;
            var param = new List<string>();
            param.Add(_rule.LinkKey);
            param.Add("bank" + _rule.LinkKey);
            param.Add("0");
            Logger.Info("开始提取 key=" + _rule.LinkKey);
            var v = _redisHelper.CommandOutValueAsync(RedisCommand.blpop, param);
            Logger.Info("提取成功 key=" + _rule.LinkKey);
            if (_redisHelper.Sucess)
            {
                Assembly assembly = Assembly.Load(new AssemblyName("CTP.Model"));
                _type = assembly.GetType(_rule.Class);
                Type type = typeof(LinkCcntent<>);
                type = type.MakeGenericType(_type);
                _obj = type.GetProperty("Content").GetValue(v.ToEntity(type), null);
                _linkType = (LinkType)type.GetProperty("LinkType").GetValue(v.ToEntity(type), null);
            }
            else
            {
                _isContinue = false;
            }
        }

        public void AnalyticData()
        {
            if (_rule == null) return;
            if (_type == null) return;
            if (_obj == null) return;
            var textparams = new List<MySqlParameter>();
            var proerties = _type.Properties();
            Logger.Info("开始解析数据 key=" + _rule.LinkKey);
            for (var i = 0; i < proerties.Length; i++)
            {
                if (_linkType == LinkType.InsertLinkType)
                {
                    if (_rule.Insert.IndexOf("@" + proerties[i].Name) > 0)
                    {
                        var val = proerties[i].GetValue(_obj, null);
                        textparams.Add(new MySqlParameter("@" + proerties[i].Name, val));
                    }
                }
                else if (_linkType == LinkType.DelLinkType)
                {
                    if (_rule.Del.IndexOf("@" + proerties[i].Name) > 0)
                    {
                        var val = proerties[i].GetValue(_obj, null);
                        textparams.Add(new MySqlParameter("@" + proerties[i].Name, val));
                    }
                }
                else if (_linkType == LinkType.UpdateLinkType) {

                    if (_rule.Update.IndexOf("@" + proerties[i].Name) > 0)
                    {
                        var val = proerties[i].GetValue(_obj, null);
                        textparams.Add(new MySqlParameter("@" + proerties[i].Name, val));
                    }
                }
            }
            Logger.Info("解析数据完成 key=" + _rule.LinkKey);
            if (textparams.Count > 0)
            {
                _mysqlParams = textparams.ToArray();
            }
        }


        public void ExcuteSql()
        {
            Logger.Info("执行数据 key=" + _rule.LinkKey);
            if (_linkType == LinkType.InsertLinkType)
            {
                _mySqlHepler.ExecuteNonQuery(_rule.Insert, _mysqlParams);
            }
            else if (_linkType == LinkType.DelLinkType)
            {
                _mySqlHepler.ExecuteNonQuery(_rule.Del, _mysqlParams);
            }
        }

        /// <summary>
        ///初始化规则
        /// </summary>
        /// <param name="rule">规则</param>
        public void InitRule(Rule rule)
        {
            _rule = rule;
        }

        public void SetClient(RedisHelper helper)
        {
            _redisHelper = helper;
        }
    }
}
