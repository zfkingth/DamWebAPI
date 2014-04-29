using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hammergo.Data;

namespace WpfApplication1.Import
{
    public class ImportTask:ImportBase
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
                        //import tasktype
                        foreach (var taskType in dam5Entities.TaskTypes)
                        {
                            var newTaskType = new TaskType();
                            newTaskType.Id = taskType.TaskTypeID;
                            newTaskType.TypeName = taskType.TypeName;

                            dam6Entities.TaskTypes.Add(newTaskType);
                            dam6Entities.SaveChanges();
                            dam6Entities.Entry(newTaskType).State = System.Data.Entity.EntityState.Detached;
                        }

                        _allRowCnt = dam5Entities.AppCollections.Count();
                        DateTime minDate = new DateTime(1970, 1, 1);
                        var newAppCol = new AppCollection();
                        var newTaskApp = new TaskApp();

                        foreach (var oldItem in dam5Entities.AppCollections)
                        {
                            newAppCol.Id = oldItem.AppCollectionID;
                            newAppCol.CollectionName = oldItem.CollectionName;
                            newAppCol.Description = oldItem.Description;
                            newAppCol.Order = oldItem.Order;
                            newAppCol.TaskTypeID = oldItem.taskTypeID;

                            dam6Entities.AppCollections.Add(newAppCol);
                            dam6Entities.SaveChanges();
                            dam6Entities.Entry(newAppCol).State = System.Data.Entity.EntityState.Detached;;

                            var query = from i in dam5Entities.TaskAppratus
                                        where i.appCollectionID == newAppCol.Id
                                        select i;
                    
                            foreach (var item in query)
                            {
                                var id = (from i in dam6Entities.Apps
                                          where i.AppName == item.appName
                                          select i).First().Id;
                                newTaskApp.Id = Guid.NewGuid();
                               newTaskApp.AppId =id;
                               newTaskApp.Order = item.Order;
                               newTaskApp.AppCollectionID = item.appCollectionID;

                               dam6Entities.TaskApps.Add(newTaskApp);


                                dam6Entities.SaveChanges();
                                dam6Entities.Entry(newTaskApp).State = System.Data.Entity.EntityState.Detached;;
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
