using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hammergo.Data;

namespace WpfApplication1.Import
{
    public class ImportCalcParam:ImportBase
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
                        foreach (var item in dam5Entities.CalculateParams.AsNoTracking())
                        {
                            var id = (from i in dam6Entities.Apps.AsNoTracking()
                                      where i.AppName == item.appName
                                      select i).First().AppId;

                            if (dam6Entities.CalculateParams.FirstOrDefault(i => i.ParamId == item.CalculateParamID) == null)
                            {
                                var newItem = new CalculateParam();

                                newItem.AppId =id;
                                newItem.ParamId = item.CalculateParamID;
                                newItem.Description = item.Description;
                                newItem.Order = item.Order==null?(byte)0:item.Order.Value;
                                newItem.ParamName = item.ParamName;
                                newItem.ParamSymbol = item.ParamSymbol;
                                newItem.PrecisionNum = item.PrecisionNum == null ? (byte)0 : item.PrecisionNum.Value;
                                newItem.UnitSymbol = item.UnitSymbol;

                                var formula = new Formula();
                                formula.FormulaID = Guid.NewGuid();
                                formula.ParamId = item.CalculateParamID;
                                formula.FormulaExpression = item.CalculateExpress;
                                byte calcOrder = 1;
                                if (item.CalculateOrder != null)
                                {
                                    calcOrder = item.CalculateOrder.Value; 
                                }
                                formula.CalculateOrder = calcOrder;
                                formula.StartDate = new DateTime(1970,1,1);//默认公式的时间刻度
                                formula.EndDate = new DateTime(9999, 12, 31);//默认终止时间

                                dam6Entities.CalculateParams.Add(newItem);
                                dam6Entities.Formulae.Add(formula);

                                dam6Entities.SaveChanges();
                                dam6Entities.Entry(newItem).State = System.Data.Entity.EntityState.Detached;;
                                dam6Entities.Entry(formula).State = System.Data.Entity.EntityState.Detached;;
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
