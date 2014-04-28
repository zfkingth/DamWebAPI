using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hammergo.Data
{
    public class MessureParam:AppParam
    {

        public MessureParam()
        {
            this.MessureValues = new List<MessureValue>();
        }
        public virtual ICollection<MessureValue> MessureValues { get; set; }
    }
}