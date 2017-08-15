using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CTP.Redis;
using NLog;
using CTP.Util;

namespace CTP.API.Controllers
{

    public class BaseController : Controller
    {

        /// <summary>
        /// 日记
        /// </summary>
        protected static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Text
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_func"></param>
        public string TextInvork<T>(Func<ReturnData> _func) where T : class
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return ExceptionHook(() =>
            {
                return _func().ToJson().RedisDataToJson();
            });
        }

        /// <summary>
        /// Grid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_func"></param>
        public  string GridInvork<T>(Func<RPage<T>> _func) where T : class
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return ExceptionHook(() =>
            {
                return _func().ToJson().RedisDataToJson();
            });

        }


        /// <summary>
        /// 集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_func"></param>
        public string ListInvork<T>(Func<RList<T>> _func) where T : class
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return ExceptionHook(() =>
            {
                return _func().ToJson().RedisDataToJson();
            });
        }


        /// <summary>
        ///一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_func"></param>
        public string SingleInvork<T>(Func<RSingle<T>> _func) where T : class
        {
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return ExceptionHook(() =>
            {
                return _func().ToJson().RedisDataToJson();
            });
        }



        /// <summary>
        /// 错误钩子
        /// </summary>
        /// <param name="_fun"></param>
        /// <returns></returns>
        private string ExceptionHook(Func<string> _fun)
        {
            try
            {
                return _fun();
            }
            catch (ProcessException ex)
            {
                Logger.Error(ex);
                return ex.Message;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new ErrorData { code = ErrorCode.IllegalValueErrorCode, sucess = false }.ToJson();
            }
        }
    }
}
