using System;
using System.Collections.Generic;
using System.Text;
using CTP.Redis.Request.Menu;

namespace CTP.Redis.Request.Examination
{
    /// <summary>
    /// 教育考试院菜单
    /// </summary>
    public class Etab : ExaminationCommon
    {   


        /// <summary>
        /// 菜单编号
        /// </summary>
        public long Mid { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string MIcon { get; set; }


        /// <summary>
        /// 名称
        /// </summary>
        public string MName { get; set; }


        /// <summary>
        /// 默认状态
        /// </summary>
        public string MDefaut { get; set; }

    }
}
