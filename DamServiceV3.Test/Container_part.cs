using System;
using System.Collections.Generic;
using System.Data.Services.Client;
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

        public int RateAllProducts(int rate)
        {

            Uri actionUri = new Uri(String.Format("{0}/Apps/RateAllProducts", this.BaseUri.AbsoluteUri)
              );

            var result = this.Execute<int>(
                                    actionUri,
                                    "POST",
                                    true,
                                    new BodyOperationParameter("Rating", 2)
                                ).First();

            return result;
        }

        public bool UpdateAppsProject(Guid projectPartID, IEnumerable<Guid> appids)
        {

            Uri actionUri = new Uri(String.Format("{0}/ProjectParts(guid'{1}')/UpdateAppsProject", this.BaseUri.AbsoluteUri, projectPartID)
              );

            var result = this.Execute<bool>(
                                    actionUri,
                                    "POST",
                                    true,
                                    new BodyOperationParameter("appids", appids)
                                ).First();

            return result;
        }


    }
}
