using System;
using System.Collections.Generic;
using System.Text;
using CTP.Util;
using System.Linq;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;
using NLog;
using CTP.Model.Link;

namespace CTP.RealtimeInteractive
{
    class MySqlCore : ICore
    {
        private MmsqlHelper _mySqlHepler = new MmsqlHelper();
        public Rule _rule { get; set; }
        public Type _type { get; set; }
        public object _data { get; set; }
        public LinkType _linkType { get; set; }

        private List<MySqlParameter> _textparams = new List<MySqlParameter>();
        private Logger _logger = LogManager.GetCurrentClassLogger();
        public void AnalyticData()
        {
            var proerties = _type.Properties();
            _logger.Info("开始解析数据 key=" + _rule.LinkKey);
            for (var i = 0; i < proerties.Length; i++)
            {
                if (_linkType == LinkType.InsertLinkType)
                {
                    if (_rule.Insert.IndexOf("@" + proerties[i].Name) > 0)
                    {
                        var val = proerties[i].GetValue(_data, null);
                        _textparams.Add(new MySqlParameter("@" + proerties[i].Name, val));
                    }
                }
                if (_linkType == LinkType.DelLinkType)
                {
                    if (_rule.Del.IndexOf("@" + proerties[i].Name) > 0)
                    {
                        var val = proerties[i].GetValue(_data, null);
                        _textparams.Add(new MySqlParameter("@" + proerties[i].Name, val));
                    }
                }
                if (_linkType == LinkType.UpdateLinkType)
                {
                    if (_rule.Update.IndexOf("@" + proerties[i].Name) > 0)
                    {
                        var val = proerties[i].GetValue(_data, null);
                        _textparams.Add(new MySqlParameter("@" + proerties[i].Name, val));
                    }
                }
            }
        }

        public int ExcuteSql()
        {
            _logger.Info("执行数据 key=" + _rule.LinkKey);
            if (_linkType == LinkType.InsertLinkType)
            {
                var maxId = 0;
                var dr = _mySqlHepler.ExecuteDataReader(_rule.MaxIdSql);
                if (dr.Read())
                {
                    var obj = Convert.ToString(dr[0]);
                    if (!obj.Contains("}") && !string.IsNullOrEmpty(obj))
                    {
                        maxId = Convert.ToInt32(obj) + 1;
                    }
                }
                _textparams.Add(new MySqlParameter("@maxId", maxId));
                _mySqlHepler.ExecuteNonQuery(_rule.Insert, _textparams.ToArray());
                return maxId;
            }
            else if (_linkType == LinkType.DelLinkType)
            {
                _mySqlHepler.ExecuteNonQuery(_rule.Del, _textparams.ToArray());
                return 0;
            }
            else if (_linkType == LinkType.UpdateLinkType)
            {
                _mySqlHepler.ExecuteNonQuery(_rule.Update, _textparams.ToArray());
                return 0;
            }
            return 0;
        }


    }
}
