using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Microsoft.Extensions.Logging;
using NLog;
using CTP.Util;

namespace CTP.Redis.Agent
{
    public class FactoryAgent
    {
        public FactoryAgent()
        {

        }

        public FactoryAgent(RequsetBase request, string method)
        {
            PropertyInfo modelInfo = request.GetType().GetProperty("Model");
            object value = modelInfo.GetValue(request, null);
            if (modelInfo.PropertyType.IsConstructedGenericType)
            {
                Type objType = value.GetType();

                object iValue = objType.GetProperty("Item").GetValue(value, new object[] { 0 });
                Type itemType = iValue.GetType();

                var rule = Profile.rules.FirstOrDefault(p => p.Class == itemType.FullName);
                if (rule != null)
                {
                    this.FactoryName = rule.Factory;
                }
            }

            else
            {
                Type objType = value.GetType();
                var rule = Profile.rules.FirstOrDefault(p => p.Class == objType.FullName);
                if (rule != null)
                {
                    this.FactoryName = rule.Factory;
                }
            }

            this.Request = request;
            this.Method = method;
        }

        /// <summary>
        /// 日记
        /// </summary>
        protected Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 工厂名称
        /// </summary>
        public string FactoryName { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 处理参数
        /// </summary>
        public RequsetBase Request { get; set; }

        /// <summary>
        /// 后台返回参数
        /// </summary>
        public ReturnData Result { get; set; }

        /// <summary>
        /// 调用后台工厂
        /// </summary>
        public void InvokeFactory()
        {
            try
            {
                Assembly assembly = Assembly.Load(new AssemblyName("CTP.Redis"));
                Type type = assembly.GetType(FactoryName);
                object instance = assembly.CreateInstance(FactoryName);
                Type[] params_type = new Type[1];
                params_type[0] = Request.GetType();
                Object[] params_obj = new Object[1];
                params_obj[0] = Request;
                Result = (ReturnData)type.GetMethod(Method, params_type).Invoke(instance, params_obj);
            }
            catch (Exception ex)
            {
                Logger.Info("Agent" + ex.Message);
                Result = new ErrorData()
                {
                    sucess = false,
                    code = ErrorCode.NotExistKeyErrorCode,

                };
            }
        }
    }
}
