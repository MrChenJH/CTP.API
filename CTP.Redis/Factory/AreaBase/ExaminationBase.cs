using System;
using System.Collections.Generic;
using System.Text;

namespace CTP.Redis.Factory.AreaBase
{
    public class ExaminationBase : ABaseFactory
    {
        public override string GetKey()
        {
            string key = string.Empty;
            Type t = this.GetType();
            switch (t.Name)
            {

                case "MenuFactory":
                    key = "Emenu";
                    break;
                case "TabFactory":
                    key = "Tab";
                    break;
                default:
                    break;
            }
            return key;
        }
    }
}
