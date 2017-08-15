using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


using Newtonsoft.Json;

namespace CTP.Redis.Request.Menu
{
    public class ExaminationCommon
    {
        public ExaminationCommon()
        {
            Id = 100000000;
        }
        [DefaultValue(100000000)]
        public long Id { get; set; }
    }
}
