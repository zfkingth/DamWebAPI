using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
                                    false
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
                                    false,
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
                                    false,
                                     new BodyOperationParameter("match", match)
                                );

            return result;


        }


        public IEnumerable<string> GetChildAppCalcName(string appCalcName, DateTimeOffset date)
        {



            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(TestConfig.serviceUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var data = new { appCalcName = appCalcName, date = date };

                HttpResponseMessage response =   client.PostAsJsonAsync("Apps/GetChildAppCalcName", data).Result;
                List<string> ret = null;
                if (response.IsSuccessStatusCode)
                {

                    var result = response.Content.ReadAsAsync<JObject>().Result;

                    ret = JsonConvert.DeserializeObject<List<string>>(result["value"].ToString());
                }


                return ret;

            }


        }

 
        public bool CheckExistData(Guid appid,DateTimeOffset date)
        {

            Uri actionUri = new Uri(String.Format("{0}/Apps(guid'{1}')/CheckExistData", this.BaseUri.AbsoluteUri, appid)
               );


            var result = this.Execute< bool>(
                                    actionUri,
                                    "POST",
                                    true,
                                     new BodyOperationParameter("date", date)
                                ).First();

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
                                ).FirstOrDefault();

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
                                ).FirstOrDefault();

            return result;
        }


    }
}
