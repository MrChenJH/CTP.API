using System;
using System.Collections.Generic;
using System.Text;

namespace CTP.Redis.Request.Examination
{
    public class MenuResult
    {
        public MenuResult()
        {
            Child = new List<MenuResult>();
        }

        public string TabId { get; set; }
        public string Id { get; set; }

        public string name { get; set; }

        public string icon { get; set; }

        public string url { get; set; }

        public List<MenuResult> Child { get; set; }
    }
}
