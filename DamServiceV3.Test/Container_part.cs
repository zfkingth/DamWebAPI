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


        public IEnumerable<string> GetChildAppCalcName2(string appCalcName, DateTimeOffset date)
        {



            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(TestConfig.serviceUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                Uri actionUri = new Uri(String.Format("{0}/Apps/GetChildAppCalcName", this.BaseUri.AbsoluteUri)
                    );

                var data = new { appCalcName = appCalcName, date = date };

                HttpResponseMessage response = client.PostAsJsonAsync(actionUri, data).Result;
                IEnumerable<string> ret = null;
                if (response.IsSuccessStatusCode)
                {

                    var result = response.Content.ReadAsAsync<JObject>().Result;

                    ret = JsonConvert.DeserializeObject<List<string>>(result["value"].ToString());
                  //  ret = result["value"].ToObject<IEnumerable<string>>();
                }


                return ret;

            }


        }


        public IEnumerable<string> GetChildAppCalcName(string appCalcName, DateTimeOffset date)
        {

            Uri actionUri = new Uri(String.Format("{0}/Apps/GetChildAppCalcName", this.BaseUri.AbsoluteUri)
             );


            var result = this.Execute<string>(
                                    actionUri,
                                    "POST",
                                     false,
                                     new BodyOperationParameter("appCalcName", appCalcName),
                                     new BodyOperationParameter("date", date)
                                );

            return result;



        }


        public bool CheckExistData(Guid appid, DateTimeOffset date)
        {

            Uri actionUri = new Uri(String.Format("{0}/Apps(guid'{1}')/CheckExistData", this.BaseUri.AbsoluteUri, appid)
               );


            var result = this.Execute<bool>(
                                    actionUri,
                                    "POST",
                                    true,
                                     new BodyOperationParameter("date", date)
                                ).First();

            return result;

        }


        public IEnumerable<CalculateValue> GetCalcValues(IEnumerable<Guid> appids, int topNum, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            Uri actionUri = new Uri(String.Format("{0}/Apps/GetCalcValues", this.BaseUri.AbsoluteUri)
              );

            var result = this.Execute<CalculateValue>(
                                    actionUri,
                                    "POST",
                                    false,
                                    new BodyOperationParameter("topNum", topNum),
                                    new BodyOperationParameter("startDate", startDate),
                                    new BodyOperationParameter("endDate", endDate),
                                    new BodyOperationParameter("appids", appids)
                                );
            return result;
        }

        public IEnumerable<MessureValue> GetMesValues(IEnumerable<Guid> appids, int topNum, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            Uri actionUri = new Uri(String.Format("{0}/Apps/GetMesValues", this.BaseUri.AbsoluteUri)
              );

            var result = this.Execute<MessureValue>(
                                    actionUri,
                                    "POST",
                                    false,
                                    new BodyOperationParameter("topNum", topNum),
                                    new BodyOperationParameter("startDate", startDate),
                                    new BodyOperationParameter("endDate", endDate),
                                      new BodyOperationParameter("appids", appids)
                                );
            return result;
        }


        public IEnumerable<Remark> GetRemarks(IEnumerable<Guid> appids, int topNum, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            Uri actionUri = new Uri(String.Format("{0}/Apps/GetRemarks", this.BaseUri.AbsoluteUri)
              );

            var result = this.Execute<Remark>(
                                    actionUri,
                                    "POST",
                                    false,
                                    new BodyOperationParameter("topNum", topNum),
                                    new BodyOperationParameter("startDate", startDate),
                                    new BodyOperationParameter("endDate", endDate),
                                      new BodyOperationParameter("appids", appids)
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

        public bool UpdateAppsProjectByNames(Guid projectPartID, IEnumerable<string> names)
        {

            Uri actionUri = new Uri(String.Format("{0}/ProjectParts(guid'{1}')/UpdateAppsProjectByNames", this.BaseUri.AbsoluteUri, projectPartID)
              );

            var result = this.Execute<bool>(
                                    actionUri,
                                    "POST",
                                    true,
                                    new BodyOperationParameter("names", names)
                                ).FirstOrDefault();

            return result;
        }
    }
}
