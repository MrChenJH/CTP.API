using System;
using System.Collections.Generic;
using System.Text;

namespace CTP.Model.Link
{
    /// <summary>
    /// 链表类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkCcntent<T> where T : class,
        new()
    {
        public LinkType Type { get; set; }
        public T Content { get; set; }
    }


    /// <summary>
    /// 队列类型
    /// </summary>
    public enum LinkType
    {
        /// <summary>
        /// 删除队列
        /// </summary>
        DelLinkType,

        /// <summary>
        /// 插入队列
        /// </summary>
        InsertLinkType,

        /// <summary>
        /// 修改队列
        /// </summary>
        UpdateLinkType
    }
}
