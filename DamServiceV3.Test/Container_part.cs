using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DamServiceV3.Test.DamServiceRef
{
    public partial class Container
    {

        public IEnumerable<Formula> GetAllFormulaeByAppID(Guid id)
        {


            Uri actionUri = new Uri(String.Format("{0}/Apps(guid'{1}')/GetAllFormulaeByAppID", this.BaseUri.AbsoluteUri, id)
                );


            var result = this.Execute<Formula>(
                                    actionUri,
                                    "POST",
                                    true
                                );

            return result;


        }
    }
}
