using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NLog;
using System.Reflection;

using CTP.Util;

namespace CTP.Redis
{
    public abstract class ABaseFactory
    {
        #region 属性

        public ABaseFactory()
        {
            Logger = LogManager.GetCurrentClassLogger();
   
            Client = RedisHelper.GetInstance();
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 日记
        /// </summary>
        protected Logger Logger { get; set; }

        /// <summary>
        /// Redis 连接客户端
        /// </summary>
        public RedisHelper Client { get; set; }

        #endregion

        #region 虚方法
        /// <summary>
        /// 新增修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual ReturnData Add(object request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual ReturnData Delete(object request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取 新增修改删除参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual List<KeyValuePair<long, string>> GetAddOrUpdateOrDeleteValue(object request)
        {
            Type t = request.GetType();
            var model = t.GetProperty("Model").GetValue(request, null);
            return model.ToListKeyValuePair();
        }

        /// <summary>
        /// 获取 新增修改删除参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual List<KeyValuePair<long, string>> GetAddOrUpdateOrDeleteValuebyId(object request)
        {
            Type t = request.GetType();
            var model = t.GetProperty("Model").GetValue(request, null);
            return model.ToListKeyValuePairId();
        }

        #endregion

        #region  抽象方法

        /// <summary>
        ///获取对应表名 
        /// </summary>
        /// <returns></returns>
        public abstract string GetKey();

        #endregion
    }
}
