using CTP.Redis.Factory.AreaBase;
using CTP.Redis.Request;
using CTP.Redis.Request.Examination;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CTP.Util;
using CTP.Model.Link;
using CTP.DataSynchronization;

namespace CTP.Redis.Factory.Examination
{
    public class TabFactory : ExaminationBase, IFactory
    {
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ReturnData Update(object request)
        {
            RequesList<Etab> tab = (RequesList<Etab>)request;
            Interactive interactive = new Interactive();
            interactive.Update(tab.Model);
            if (interactive.Result.Item1)
            {
                var items = new List<KeyValuePair<long, string>>();
                items.Add(new KeyValuePair<long, string>(tab.Model.Id, Convert.ToString(tab.Model.Id)));
                Client.UpdateAsyc<Etab>(GetKey(), items, tab.Model, "Id");
                return new ReturnData { sucess = Client.Sucess };
            }
            else
            {
                return new ErrorData { sucess = Client.Sucess, code = Client.Code };
            }
        }

        /// <summary>
        /// 删除选项卡
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override ReturnData Delete(object request)
        {
            RequesList<Etab> tab = (RequesList<Etab>)request;
            Interactive interactive = new Interactive();
            interactive.Delete(tab.Model);
            var items = new List<KeyValuePair<long, string>>();
            if (interactive.Result.Item1)
            {
                items.Add(new KeyValuePair<long, string>(tab.Model.Id, Convert.ToString(tab.Model.Id)));
                Client.RemoveAsync<Etab>(GetKey(), items, "Id");
                return new ReturnData { sucess = Client.Sucess, msg = tab.Model.Id.ToString() };
            }
            else
            {
                return new ErrorData { sucess = Client.Sucess, code = Client.Code, msg = "关系型数据库出错" };
            }
        }

        public ReturnData PageQuery(object request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 新增修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override ReturnData Add(object request)
        {
            RequesList<Etab> tab = (RequesList<Etab>)request;
            Interactive interactive = new Interactive();
            interactive.Insert(tab.Model);
            if (interactive.Result.Item1)
            {
                tab.Model.Id = interactive.Result.Item2;
                var values = new List<KeyValuePair<long, string>>();
                values.Add(new KeyValuePair<long, string>(tab.Model.Id, tab.Model.ToJson()));
                Client.AddZsetAsync(GetKey(), values);
                return new ReturnData { sucess = Client.Sucess, msg = interactive.Result.Item2.ToString() };
            }
            else
            {
                return new ErrorData { sucess = Client.Sucess, code = Client.Code };
            }
        }

        /// <summary>
        /// 查询所有选项卡
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ReturnData Query(object request)
        {
            RequesList<Etab> rp = (RequesList<Etab>)request;
            string conditon = rp.Model.ToQueryCondition();
            Client.GetZsetMultiByValue(GetKey(), conditon);
            if (Client.Sucess)
            {
                return new RList<string>()
                {
                    sucess = true,
                    data = Client.Result
                };
            }
            else
            {
                return new ErrorData()
                {
                    sucess = true,
                    code = Client.Code,
                    Occurrencetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }
        }

        public ReturnData Specialquery(object request)
        {
            throw new NotImplementedException();
        }


    }
}
