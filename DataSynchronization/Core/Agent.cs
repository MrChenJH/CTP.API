using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using CTP.Util;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using NLog;

namespace CTP.DataSynchronization
{
    public class Agent
    {
        /// <summary>
        /// 日记
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private static RedisHelper redisHelper { get; set; }
        private static void InitConfig()
        {
            Logger.Info("初始化配置信息");
            var ruleBuilder = new ConfigurationBuilder();
            var ruleConfig = ruleBuilder.AddJsonFile("Rules.json").Build();
            var ruls = ruleConfig.GetSection("rules").Get<List<Rule>>();
            Profile.rules = ruls;
            var config = new ConfigurationBuilder().AddJsonFile("Config.json").Build();
            var urlLocal = config.GetValue<string>("profile:url1");
            var urlIp = config.GetValue<string>("profile:url2");
            Profile.mySqlCon = config.GetValue<string>("profile:con");
            Profile.redisIp = config.GetValue<string>("profile:ip");
            redisHelper = RedisHelper.GetInstance();

        }
        private static ICore Core(string driver)
        {
            ICore coe = null;
            if (driver.Equals("mysql"))
            {
                coe = new MySqlCore();
            }
            return coe;
        }
        public static void Process()
        {
            try
            {
                InitConfig();

                foreach (var rule in Profile.rules)
                {
                    ThreadPool.QueueUserWorkItem((s) =>
                       {
                           var core = Core(rule.DataDriver);
                           core.InitRule(rule);
                           core.SetClient(redisHelper);
                           while (true)
                           {
                               try
                               {
                                   core.ExtractingData();
                                   core.AnalyticData();
                                   core.ExcuteSql();
                               }
                               catch (Exception ex)
                               {
                                   Logger.Error("异常" + ex.Message + "    key=" + rule.Key);
                               }
                           }
                       });
                }
                while (true)
                {
                    Thread.Sleep(100000);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("异常" + ex.Message);
            }
        }
    }
}
