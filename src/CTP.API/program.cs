using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using CTP.Util;

namespace CTP.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string path = System.AppContext.BaseDirectory;
            var json = new ConfigurationBuilder().AddJsonFile("Json/Config.json").Build();
            var urlLocal = json.GetValue<string>("profile:IntranetAddress");
            var urlIp = json.GetValue<string>("profile:ExternalAddress");
            Profile.redisIp = json.GetValue<string>("profile:RedisAdress");
            Profile.typeLink = json.GetValue<string>("profile:LinkKey");
            Profile.mySqlCon = json.GetValue<string>("profile:MySqlCon");
            Profile.sqlseverCon = json.GetValue<string>("profile:SqlServerCon");

            var ruleBuilder = new ConfigurationBuilder();
            var ruleConfig = ruleBuilder.AddJsonFile("Json/Rules.json").Build();
            var ruls = ruleConfig.GetSection("rules").Get<List<Rule>>();
            Profile.rules = ruls;

            var host = new WebHostBuilder()
                                      .UseKestrel()
                                      .UseContentRoot(Directory.GetCurrentDirectory())
                                      .UseIISIntegration()
                                      .UseStartup<Startup>()
                                      //.UseUrls(new string[] { urlLocal, urlIp })
                                      .Build();

            host.Run();
        }
    }
}
