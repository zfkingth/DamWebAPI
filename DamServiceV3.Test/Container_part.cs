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


        public IEnumerable<App> SearcyAppByName(string match)
        {


            Uri actionUri = new Uri(String.Format("{0}/Apps/SearcyAppByName", this.BaseUri.AbsoluteUri)
                );


            var result = this.Execute<App>(
                                    actionUri,
                                    "POST",
                                    true,
                                     new BodyOperationParameter("match", match)
                                );

            return result;


        }

        public IEnumerable<App> SearcyAppCalcName(string match)
        {


            Uri actionUri = new Uri(String.Format("{0}/Apps/SearcyAppCalcName", this.BaseUri.AbsoluteUri)
                );


            var result = this.Execute<App>(
                                    actionUri,
                                    "POST",
                                    true,
                                     new BodyOperationParameter("match", match)
                                );

            return result;


        }


        public IEnumerable<string> GetChildAppCalcName(string appCalcName, DateTimeOffset date)
        {


            Uri actionUri = new Uri(String.Format("{0}/Apps/GetChildAppCalcName", this.BaseUri.AbsoluteUri)
                );


            var result = this.Execute<string>(
                                    actionUri,
                                    "POST",
                                    true,
                                     new BodyOperationParameter("appCalcName", appCalcName),
                                     new BodyOperationParameter("date", date)
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
