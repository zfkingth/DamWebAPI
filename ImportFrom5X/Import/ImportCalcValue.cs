using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hammergo.Data;

namespace WpfApplication1.Import
{
    public class ImportCalcValue : ImportBase
    {

        protected override void bgw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                using (DamWCFContext dam6Entities = new DamWCFContext(false))
                {
                    ResetConnectionString(dam6Entities);

                    using (EF5x.Models.DamDBContext dam5Entities = new EF5x.Models.DamDBContext())
                    {
                        ResetConnectionString(dam5Entities);
                        _allRowCnt = dam5Entities.CalculateParams.Count();
                        DateTime minDate = new DateTime(1970, 1, 1);
                        var newItem = new CalculateValue();

                        foreach (var paramItem in dam5Entities.CalculateParams.AsNoTracking())
                        {
                            //寻找已有数据中的最大date
                            DateTimeOffset maxDate = (from i in dam6Entities.CalculateValues
                                                where i.ParamId == paramItem.CalculateParamID
                                                select i.Date).DefaultIfEmpty(minDate).Max();
                            //get all mes values which >maxDate
                            var query = from i in dam5Entities.CalculateValues.AsNoTracking()
                                        where i.calculateParamID == paramItem.CalculateParamID && i.Date > maxDate
                                        select i;
                          
                            foreach (var item in query)
                            {

                                newItem.Id = Guid.NewGuid();
                                newItem.ParamId = item.calculateParamID;
                                newItem.Date = item.Date;
                                newItem.Val = item.Val;

                                dam6Entities.CalculateValues.Add(newItem);


                                dam6Entities.SaveChanges();
                                dam6Entities.Entry(newItem).State = System.Data.Entity.EntityState.Detached;;
                            }

                            handledCnt++;
                            reportProgress();
                        }


                    }
                }
                reportProgress();

                bgwResult = "导入成功!";
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    bgwResult = ex.InnerException.Message;
                }
                else
                {
                    bgwResult = ex.Message;
                }

            }

        }
    }
}

