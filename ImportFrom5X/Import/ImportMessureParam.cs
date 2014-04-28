using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hammergo.Data;

namespace WpfApplication1.Import
{
    public class ImportMessureParam:ImportBase
    {
        protected override void bgw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                using (DamWCFContext dam6Entities = new DamWCFContext())
                {
                    ResetConnectionString(dam6Entities);

                    using (EF5x.Models.DamDBContext dam5Entities = new EF5x.Models.DamDBContext())
                    {
                        ResetConnectionString(dam5Entities);
                        _allRowCnt = dam5Entities.MessureParams.Count();
                        foreach (var item in dam5Entities.MessureParams.AsNoTracking())
                        {
                            var id = (from i in dam6Entities.Apps.AsNoTracking()
                                      where i.AppName == item.appName
                                      select i).First().AppId; 

                            if (dam6Entities.MessureParams.FirstOrDefault(i => i.ParamId == item.MessureParamID) == null)
                            {
                                var newItem = new MessureParam();

                                newItem.AppId = id;
                                newItem.ParamId = item.MessureParamID;
                                newItem.Description = item.Description;
                            
                                newItem.Order = item.Order==null?(byte)0:item.Order.Value;

                                newItem.ParamName = item.ParamName;
                                newItem.ParamSymbol = item.ParamSymbol;
                       
                                newItem.PrecisionNum = item.PrecisionNum == null ? (byte)0 : item.PrecisionNum.Value;

                                newItem.UnitSymbol = item.UnitSymbol;


                                dam6Entities.MessureParams.Add(newItem);
                              

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
