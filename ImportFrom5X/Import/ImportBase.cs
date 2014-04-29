using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hammergo.Data;
using System.Configuration;

namespace WpfApplication1.Import
{
    public class ImportBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        BackgroundWorker bgw = new BackgroundWorker();
        protected DateTime startTime;
        protected int highestPercentageReached = 0;
        protected int handledCnt = 0;//已经处理的行数
        protected string bgwResult = "";
        protected int _allRowCnt;

        public ImportBase()
        {
            bgw.WorkerReportsProgress = true;
            bgw.WorkerSupportsCancellation = true;
            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
        }

        public virtual void startWork()
        {
            startTime = DateTime.Now;

            bgw.RunWorkerAsync();
        }



        public event RunWorkerCompletedEventHandler WorkCompeleted;

        protected virtual void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            long seconds = (DateTime.Now.Ticks - startTime.Ticks) / TimeSpan.TicksPerSecond;
            ResultString = bgwResult + string.Format(" 用时 {0}秒", seconds);
            if (WorkCompeleted != null)
            {
                WorkCompeleted(sender, e);
            }
        }

        protected virtual void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Percentage = e.ProgressPercentage;

        }

        protected virtual void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void reportProgress()
        {
            int percentComplete =
          (int)((handledCnt + 1.0f) / _allRowCnt * 100);
            if (percentComplete > highestPercentageReached)
            {
                highestPercentageReached = percentComplete;
                bgw.ReportProgress(percentComplete);
            }

        }

        private double _percentage = 0;
        /// <summary>
        /// 完成的进度的百分比，一整数表好。
        /// </summary>
        public double Percentage
        {
            get { return _percentage; }
            set
            {
                _percentage = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Percentage"));
            }
        }



        protected void ResetConnectionString(DbContext dbcontext)
        {
            //if (dbcontext is EF5x.Models.DamDBContext)
            //{
            //    dbcontext.Database.Connection.ConnectionString = ConnectionString5x;
            //}
            //else
            //{
            //    dbcontext.Database.Connection.ConnectionString = ConnectionString6x;
            //}
        }



        static string _string5x = ConfigurationManager.ConnectionStrings["DamDBContext"].ConnectionString;
        public static string ConnectionString5x
        {
            get { return _string5x; }
            set { _string5x = value; }
        }

        static string _string6x = ConfigurationManager.ConnectionStrings["DamWCFContext"].ConnectionString;
        public static string ConnectionString6x
        {
            get { return _string6x; }
            set { _string6x = value; }
        }





        private string _resultString;
        /// <summary>
        /// 操作结果字符串。也可能包含异常信息
        /// </summary>
        public string ResultString
        {
            get { return _resultString; }
            set
            {
                _resultString = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("ResultString"));
                }
            }
        }




        public void TestConnection()
        {

            string result = "";
            try
            {
                using (EF5x.Models.DamDBContext dam5xEntities = new EF5x.Models.DamDBContext())
                {
                    //dam5xEntities.Database.Connection.ConnectionString = ConnectionString5x;
                    dam5xEntities.Database.Connection.Open();
                    dam5xEntities.Database.Connection.Close();
                }
                using (DamWCFContext dam6xEntities = new DamWCFContext(false))
                {
                    //dam6xEntities.Database.Connection.ConnectionString = ConnectionString6x;
                    dam6xEntities.Database.Connection.Open();
                    dam6xEntities.Database.Connection.Close();
                }
                result = "连接测试成功";
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            ResultString = result;


        }
    }
}
