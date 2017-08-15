using System;
using System.Collections.Generic;
using System.Text;
using CTP.Redis.Factory.AreaBase;
using CTP.Redis.Request;

using CTP.Redis.Request.Examination;

using System.Reflection;
using CTP.Redis;
using CTP.Util;
using CTP.Model.Link;
using CTP.DataSynchronization;

namespace CTP.Redis.Factory.Examination
{

    /// <summary>
    /// 菜单
    /// </summary>
    public class MenuFactory : ExaminationBase, IFactory
    {


        /// <summary>
        /// 新增修改
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override ReturnData Add(object request)
        {
            RequesList<Emenu> menu = (RequesList<Emenu>)request;
            Interactive interactive = new Interactive();
            interactive.Insert(menu.Model);
            if (interactive.Result.Item1)
            {
                menu.Model.Id = interactive.Result.Item2;
                var values = new List<KeyValuePair<long, string>>();
                values.Add(new KeyValuePair<long, string>(menu.Model.Id, menu.Model.ToJson()));
                Client.AddZsetAsync(GetKey(), values);
                return new ReturnData { sucess = Client.Sucess, msg = interactive.Result.Item2.ToString() };
            }
            else
            {
                return new ErrorData { sucess = Client.Sucess, code = Client.Code };
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public override ReturnData Delete(object request)
        {
            RequesList<Emenu> menu = (RequesList<Emenu>)request;
            Interactive interactive = new Interactive();
            interactive.Delete(menu.Model);
            var items = new List<KeyValuePair<long, string>>();
            if (interactive.Result.Item1)
            {
                items.Add(new KeyValuePair<long, string>(menu.Model.Id, Convert.ToString(menu.Model.Id)));
                Client.RemoveAsync<Emenu>(GetKey(), items, "Id");
                return new ReturnData { sucess = Client.Sucess, msg = menu.Model.Id.ToString() };
            }
            else
            {
                return new ErrorData { sucess = Client.Sucess, code = Client.Code, msg = "关系型数据库出错" };
            }
        }


        public ReturnData Update(object request)
        {
            RequesList<Emenu> menu = (RequesList<Emenu>)request;
            Interactive interactive = new Interactive();
            interactive.Update(menu.Model);
            if (interactive.Result.Item1)
            {
                var items = new List<KeyValuePair<long, string>>();
                items.Add(new KeyValuePair<long, string>(menu.Model.Id, Convert.ToString(menu.Model.Id)));
                Client.UpdateAsyc<Emenu>(GetKey(), items, menu.Model, "Id");
                return new ReturnData { sucess = Client.Sucess };
            }
            else
            {
                return new ErrorData { sucess = Client.Sucess, code = Client.Code };
            }
        }

        public ReturnData PageQuery(object request)
        {

            throw new NotImplementedException();
        }


        private List<MenuResult> EmenuList = new List<MenuResult>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="menu"></param>
        /// <param name="emenuList"></param>
        private void SetMenu(string parentId, MenuResult menu, List<Emenu> emenuList)
        {
            for (int i = 0; i < emenuList.Count; i++)
            {
                if (emenuList[i].ParentId == parentId)
                {
                    var re = new MenuResult();
                    re.TabId = Convert.ToString(emenuList[i].TabId);
                    re.Id = Convert.ToString(emenuList[i].Id);
                    re.icon = emenuList[i].Micon;
                    re.name = emenuList[i].Mname;
                    re.url = emenuList[i].Mlink;
                    SetMenu(Convert.ToString(emenuList[i].Id), re, emenuList);
                    if (parentId.Equals("0"))
                    {
                        EmenuList.Add(re);
                    }
                    else
                    {
                        menu.Child.Add(re);
                    }
                }
            }
        }


        /// <summary>
        /// 查询所有
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ReturnData Query(object request)
        {

            RequesList<Emenu> rp = (RequesList<Emenu>)request;
            if (rp.isSec == 10)
            {

            }
            string conditon = rp.Model.ToQueryCondition();
            Client.GetZsetMultiByValue(GetKey(), conditon);

            List<Emenu> listMenu = new List<Emenu>();
            if (Client.Sucess)
            {
                foreach (var c in Client.Result)
                {
                    var v = c.ToEntity<Emenu>();
                    listMenu.Add(v);
                }


                SetMenu("0", null, listMenu);
                List<string> str = new List<string>();
                foreach (var p in EmenuList)
                {
                    str.Add(p.ToJson());
                }
                return new RList<string>()
                {
                    sucess = true,
                    data = str
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
            return new ReturnData();
        }


    }
}
