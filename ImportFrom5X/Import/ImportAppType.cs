using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hammergo.Data;


namespace WpfApplication1.Import
{
    public class ImportAppType:ImportBase
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
                        _allRowCnt = dam5Entities.ApparatusTypes.Count();
                        foreach (var item in dam5Entities.ApparatusTypes.AsNoTracking())
                        {
                            if (dam6Entities.ApparatusTypes.FirstOrDefault(i => i.Id == item.ApparatusTypeID) == null)
                            {
                                ApparatusType newItem = new ApparatusType();
                                newItem.Id = item.ApparatusTypeID;
                                newItem.TypeName = item.TypeName;

                                dam6Entities.ApparatusTypes.Add(newItem);

                                dam6Entities.SaveChanges();
                                dam6Entities.Entry(newItem).State = System.Data.Entity.EntityState.Detached;// System.Data.Entity.EntityState.Detached;;
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
