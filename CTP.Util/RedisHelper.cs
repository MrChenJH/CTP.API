using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NLog;
using StackExchange.Redis;

namespace CTP.Util
{


    public class RedisHelper
    {


        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        public static RedisHelper GetInstance()
        {
            // 如果类的实例不存在则创建，否则直接返回
            if (rClient == null)
            {
                rClient = new RedisHelper();
            }
            return rClient;
        }

        #region  属性

        /// <summary>
        /// 客户端
        /// </summary>
        private static RedisHelper rClient;


        /// <summary>
        ///构造函数
        /// </summary>
        private RedisHelper()
        {
            Logger = LogManager.GetCurrentClassLogger();
            Result = new List<string>();

            Sucess = true;
        }



        public Boolean Sucess { get; set; }

        public string Message { get; set; }

        public string Code { get; set; }

        public List<string> Result { get; set; }

        public long Count { get; set; }


        /// <summary>
        /// 日记
        /// </summary>
        protected Logger Logger { get; set; }

        #endregion

        #region 查询

        /// <summary>
        /// ZSet中是否存在这条记录
        /// </summary>
        /// <param name="key">ZSetkey</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool isExistZSetValue(string key, string value)
        {
            Count = 0;
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var v = client.GetDatabase();

                    var result = v.SortedSetRangeByValue(key, value);

                    if (result.Count() > 0)
                    {
                        Sucess = true;
                    }
                    else
                    {
                        Message = "Redis库中不存在" + key;
                        Code = ErrorCode.NotExistKeyErrorCode;
                        Sucess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }
            return Sucess;
        }




        /// <summary>
        ///  判断 key 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsExistKey(string key)
        {
            Count = 0;
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var v = client.GetDatabase();
                    var result = v.KeyExists(key);
                    if (result)
                    {
                        Sucess = true;
                    }
                    else
                    {
                        Message = "Redis库中不存在" + key;
                        Code = ErrorCode.NotExistKeyErrorCode;
                        Sucess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }
            return Sucess;
        }


        /// <summary>
        /// 清除Zset
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ClearZSet(string key)
        {
            Count = 0;
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var v = client.GetDatabase();
                    var result = v.SortedSetRemoveRangeByRank(key, 0, -1);
                    Sucess = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }
            return Sucess;
        }

        /// <summary>
        /// 获取 ZSet 里的Value 值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void GetZsetMultiByValue(string key, string keyvalue)
        {
            var list = new List<string>();
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var vc = client.GetDatabase();
                    if (!string.IsNullOrWhiteSpace(keyvalue))
                    {
                        var result = vc.SortedSetScan(key, string.Format("{0}{1}{2}", "*", keyvalue, "*"), 1, 0, 0, CommandFlags.None);
                        var reg = new Regex("^\\d+$");
                        for (int i = 0; i < result.Count(); i++)
                        {
                            var v = result.ToList()[i].Element;
                            if (!reg.Match(v).Success)
                            {
                                list.Add(v);
                            }
                        }
                    }
                    else
                    {
                        var result = vc.SortedSetRangeByRank(key, 0, -1);
                        var reg = new Regex("^\\d+$");
                        for (int i = 0; i < result.Length; i++)
                        {
                            var v = result[i];
                            if (!reg.Match(v).Success)
                            {
                                list.Add(v);
                            }
                        }
                    }

                    Sucess = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }
            Count = list.Count();
            Result = list;
        }

        /// <summary>
        /// 获取 ZSet 里的Value 值
        /// </summary>
        /// <param name="key">查询Key</param>
        /// <returns></returns>
        public void GetZsetMultiByPage(string key, int start, int end = 1000000)
        {
            var list = new List<string>();
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var cv = client.GetDatabase();

                    var result = cv.SortedSetRangeByScore(key, 0, 9000000000000000000, Exclude.None, Order.Descending, start, end - start);
                    var reg = new Regex("^\\d+$");
                    for (int i = 0; i < result.Count(); i++)
                    {
                        var v = result[i];
                        if (!reg.Match(v).Success)
                        {
                            list.Add(v);
                        }
                    }
                    Count = cv.SortedSetLength(key);
                    Result = list;
                    Sucess = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }
        }


        /// <summary>
        /// 合并多个Zset
        /// </summary>
        /// <param name="key">合并后Key</param>
        /// <param name="keys">合并key 组合</param>
        public void ZUNIONSTORE(string key, string[] keys)
        {
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var cv = client.GetDatabase();
                    foreach (var p in keys)
                    {
                        var result = cv.SortedSetCombineAndStore(SetOperation.Union, key, key, p);
                    }
                    Sucess = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
                return;
            }
        }

        /// <summary>
        /// 根据key 获取所有值
        /// </summary>
        /// <param name="key">查询Key</param>
        public void GetZsetByKey(string key)
        {
            var list = new List<string>();
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var cv = client.GetDatabase();
                    var result = cv.SortedSetRangeByRank(key, 0, -1);
                    for (int i = 0; i < result.Count(); i++)
                    {
                        list.Add(result[i]);
                    }
                    Result = list;
                    Sucess = true;
                    Count = list.Count();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }
        }

        /// <summary>
        /// 根据散列获取相应 List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void GetList(string key, List<int> fileds)
        {
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var cv = client.GetDatabase();
                    if (fileds.Count > 0)
                    {
                        foreach (var t in fileds)
                        {
                            var result = cv.SortedSetRangeByScore(key, t, t);

                            for (int i = 0; i < result.Length; i++)
                            {
                                Result.Add(System.Text.Encoding.UTF8.GetString(result[i]));
                            }
                        }
                    }
                    else
                    {
                        var result = cv.SortedSetRangeByRank(key, 0, -1);
                        for (int i = 0; i < result.Length; i++)
                        {
                            Result.Add(System.Text.Encoding.UTF8.GetString(result[i]));
                        }
                    }
                    Sucess = true;
                    Count = Result.Count();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }

        }

        /// <summary>
        /// 通过分数获取值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="score"></param>
        public void GetZsetSingleByAuto(string key, long score)
        {
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var t = client.GetDatabase();
                    var result = t.SortedSetRangeByScore(key, score, score);
                    for (int i = 0; i < result.Length; i++)
                    {
                        Result.Add(System.Text.Encoding.UTF8.GetString(result[i]));
                    }
                    Sucess = true;
                    Count = Result.Count;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }

        }


        #endregion

        #region 操作

        /// <summary>
        /// Zset  删除对应值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool RemoveZsetByValue(string key, string value)
        {
            Count = 0;
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var cv = client.GetDatabase();
                    cv.SortedSetRemove(key, value);
                    Sucess = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }
            return Sucess;
        }

        /// <summary>
        /// Zset 批量添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool AddZset(string key, List<KeyValuePair<long, string>> values)
        {
            Count = 0;
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var cv = client.GetDatabase();

                    SortedSetEntry[] entryArray = new SortedSetEntry[values.Count];
                    for (int i = 0; i < values.Count; i++)
                    {
                        entryArray[i] = new SortedSetEntry(values[i].Value, values[i].Key);
                    }

                    var result = cv.SortedSetAdd(key, entryArray);

                    if (result > 0)
                    {
                        Sucess = true;
                    }
                    else
                    {
                        Message = "没有添加任何值";
                        Code = ErrorCode.NotExistKeyErrorCode;
                        Sucess = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }
            return Sucess;
        }


        /// <summary>
        /// Zset 批量添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool AddZsetAsync(string key, List<KeyValuePair<long, string>> values)
        {
            Count = 0;
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var cv = client.GetDatabase();
                    SortedSetEntry[] entryArray = new SortedSetEntry[values.Count];
                    for (int i = 0; i < values.Count; i++)
                    {
                        entryArray[i] = new SortedSetEntry(values[i].Value, values[i].Key);
                    }
                    var result = cv.SortedSetAddAsync(key, entryArray);
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }
            return Sucess;
        }

        public void RemoveAsync<T>(string key, List<KeyValuePair<long, string>> values, string searchpropertyName) where T : class, new()
        {
            Task.Run(delegate ()
            {
                RemoveZsetValuesAsync<T>(key, values, searchpropertyName).Wait();
            });
        }

        /// <summary>
        ///  Zset 删除  逻辑 先通过 score 去查找 然后通过序列化后属性值去配对
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="searchpropertyName">检索字段属性值</param>
        /// <returns></returns>
        private async Task<T> RemoveZsetValuesAsync<T>(string key, List<KeyValuePair<long, string>> values, string searchpropertyName) where T : class, new()
        {
            Count = 0;
            T entity = null;
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var cv = client.GetDatabase();
                    foreach (var p in values)
                    {
                        var result = await cv.SortedSetRangeByScoreAsync(key, p.Key, p.Key);
                        foreach (var v1 in result)
                        {
                            string val = v1;
                            entity = val.ToEntity<T>();
                            Type t = entity.GetType();
                            var value = t.GetProperty(searchpropertyName).GetValue(entity, null).Convert("");
                            if (value.Equals(p.Value))
                            {
                                await cv.SortedSetRemoveAsync(key, v1);
                            }
                        }
                    }
                    Sucess = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }
            return entity;
        }


        /// <summary>
        /// 异步更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="items"></param>
        /// <param name="targetvalue"></param>
        /// <param name="scoreName"></param>
        public void UpdateAsyc<T>(string key, List<KeyValuePair<long, string>> items, T targetvalue, string scoreName) where T : class, new()
        {
            Task.Run(delegate ()
                {
                    T newvalue = default(T);
                    var oldMenu = RemoveZsetValuesAsync<T>(key, items, "Id");
                    if (oldMenu != null)
                    {
                        newvalue = targetvalue.MergeOldEntity<T>(oldMenu.Result);
                    }
                    else
                    {
                        newvalue = targetvalue;
                    }
                    var addlist = new List<KeyValuePair<long, string>>();
                    addlist.Add(new KeyValuePair<long, string>(Convert.ToInt64(newvalue.GetType().GetProperty(scoreName).GetValue(newvalue, null)), newvalue.ToJson()));
                    AddZsetAsync(key, addlist);
                });
        }
        /// <summary>
        ///  Zset 删除  逻辑 先通过 score 去查找 然后通过序列化后属性值去配对
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="searchpropertyName">检索字段属性值</param>
        /// <returns></returns>
        public T RemoveZsetValues<T>(string key, List<KeyValuePair<long, string>> values, string searchpropertyName) where T : class, new()
        {
            Count = 0;
            T entity = null;
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var cv = client.GetDatabase();
                    foreach (var p in values)
                    {
                        var result = cv.SortedSetRangeByScore(key, p.Key, p.Key);
                        foreach (var v1 in result)
                        {
                            string val = v1;
                            entity = val.ToEntity<T>();
                            Type t = entity.GetType();
                            var value = t.GetProperty(searchpropertyName).GetValue(entity, null).Convert("");
                            if (value.Equals(p.Value))
                            {
                                cv.SortedSetRemove(key, v1);
                            }
                        }
                    }
                    Sucess = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }
            return entity;
        }


        /// <summary>
        /// Redis 自定义接口
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="values">命令数组</param>
        /// <returns></returns>
        public bool Command(RedisCommand cmd, List<string> values)
        {
            Count = 0;
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var cv = client.GetDatabase();
                    var result = cv.Execute(cmd.ToString(), values.ToArray());
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }
            return Sucess;
        }


        /// <summary>
        /// Redis 自定义异步接口  使用场景多线程,消息队列
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public string CommandOutValueAsync(RedisCommand cmd, List<string> values)
        {
            Count = 0;
            try
            {
                using (var client = ConnectionMultiplexer.Connect(Profile.redisIp + ",abortConnect=false"))
                {
                    var cv = client.GetDatabase();
                    var batch = cv.CreateBatch();
                    var task = batch.ExecuteAsync(cmd.ToString(), values.ToArray());
                    batch.Execute();
                    task.Wait();
                    RedisResult result = task.Result;
                    var val = ((RedisResult[])result)[1].ToString();
                    Sucess = !string.IsNullOrWhiteSpace(val);
                    return val;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Message = ex.Message;
                Code = ErrorCode.ReadRedisErrorCode;
                Sucess = false;
            }
            return string.Empty;
        }
        #endregion




    }

}
