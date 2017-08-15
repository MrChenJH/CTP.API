using System;
using System.Collections.Generic;
using System.Text;

using CTP.Util;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using NLog;
using CTP.RealtimeInteractive;
using System.Linq;
using CTP.Model.Link;

namespace CTP.DataSynchronization
{
    public class Interactive
    {
        public Tuple<bool, int> Result = new Tuple<bool, int>(true, 0);

        private ICore _core { get; set; }

        /// <summary>
        /// 日记
        /// </summary>
        private Logger Logger = LogManager.GetCurrentClassLogger();
        private RedisHelper redisHelper { get; set; }

        private void InitRule(Object data)
        {
            var type = data.GetType();
            var rule = Profile.rules.FirstOrDefault(p => p.Class == type.FullName);
            SetCore(rule.DataDriver);
            _core._rule = rule;
            _core._type = type;
            _core._data = data;
        }

        private void SetCore(string driver)
        {
            if (driver.Equals("mysql"))
            {
                _core = new MySqlCore();
            }
        }

        private void Process(object data, LinkType linkType)
        {
            try
            {
                InitRule(data);
                _core._linkType = linkType;
                _core.AnalyticData();
                var maxId = _core.ExcuteSql();
                Result = new Tuple<bool, int>(true, maxId);
            }
            catch (Exception ex)
            {
                Result = new Tuple<bool, int>(false, 0);
                Logger.Error("异常" + ex.Message);
            }

        }
        public void Insert(object data)
        {
            try
            {
                Process(data, LinkType.InsertLinkType);
            }
            catch (Exception ex)
            {
                Result = new Tuple<bool, int>(false, 0);
                Logger.Error("异常" + ex.Message);
            }
        }

        public void Update(object data)
        {
            try
            {
                Process(data, LinkType.UpdateLinkType);
            }
            catch (Exception ex)
            {
                Result = new Tuple<bool, int>(false, 0);
                Logger.Error("异常" + ex.Message);
            }
        }

        public void Delete(object data)
        {
            try
            {
                Process(data, LinkType.DelLinkType);
            }
            catch (Exception ex)
            {
                Result = new Tuple<bool, int>(false, 0);
                Logger.Error("异常" + ex.Message);
            }
        }
    }
}
