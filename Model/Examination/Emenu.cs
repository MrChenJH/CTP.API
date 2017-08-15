using System;
using System.Collections.Generic;
using System.Text;
using CTP.Redis.Request.Menu;

namespace CTP.Redis.Request.Examination
{
    /// <summary>
    /// 菜单项
    /// </summary>
    public class Emenu : ExaminationCommon
    {
        /// <summary>
        /// 选项卡编号
        /// </summary>
        public int TabId { get; set; }


        /// <summary>
        /// 父编号
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Mname { get; set; }

        /// <summary>
        /// 菜单链接
        /// </summary>
        public string Mlink { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Micon { get; set; }

        

    }
}
