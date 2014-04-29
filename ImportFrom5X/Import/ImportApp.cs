using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hammergo.Data;

namespace WpfApplication1.Import
{
    public class ImportApp:ImportBase
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
                        _allRowCnt = dam5Entities.Apparatus.Count();
                        foreach (var item in dam5Entities.Apparatus.AsNoTracking())
                        {
                            if (dam6Entities.Apps.FirstOrDefault(i => i.AppName == item.AppName) == null)
                            {
                                var newItem = new Hammergo.Data.App();

                                newItem.Id = Guid.NewGuid();
                                newItem.AppName = item.AppName;
                                newItem.CalculateName = item.CalculateName;
                                newItem.AppTypeID = item.AppTypeID;
                                newItem.ProjectPartID = item.ProjectPartID;
                                newItem.BuriedTime = item.BuriedTime;
                                newItem.OtherInfo = item.OtherInfo;
                                newItem.X = item.X;
                                newItem.Y = item.Y;
                                newItem.Z = item.Z;

                                dam6Entities.Apps.Add(newItem);
                               

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
