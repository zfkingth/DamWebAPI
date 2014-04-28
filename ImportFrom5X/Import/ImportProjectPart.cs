using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hammergo.Data;

namespace WpfApplication1.Import
{
    public class ImportProjectPart:ImportBase
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
                        _allRowCnt = dam5Entities.ProjectParts.Count();
                        foreach (var item in dam5Entities.ProjectParts)
                        {
                            if (dam6Entities.ProjectParts.FirstOrDefault(i => i.ProjectPartID == item.ProjectPartID) == null)
                            {

                                ProjectPart newItem = new ProjectPart();
                                newItem.ProjectPartID = item.ProjectPartID;
                                newItem.PartName = item.PartName;
                                newItem.ParentPart = item.ParentPart;


                                dam6Entities.ProjectParts.Add(newItem);
                                dam6Entities.SaveChanges();
                                dam6Entities.Entry(newItem).State = System.Data.Entity.EntityState.Detached;
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
