using System;
using System.Collections.Generic;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DamWebAPI.ViewModel.DataImport
{

    public class ExcelHelper : IDisposable
    {
        protected Excel.Application excel = null;//excel 程序


        public void initialExcel()
        {
            if (excel == null)//当excel没有被初始化时才进行初始化
            {
                excel = new Excel.Application();
                if (excel == null)
                {
                    throw new Exception("错误: EXCEL 不能正确启动");

                }
                excel.ScreenUpdating = false;
                excel.DisplayAlerts = false;

                excel.Visible = false;




            }
        }


        public void closeWorkbooks()
        {
            if (excel != null)
            {
                excel.Workbooks.Close();
            }
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        public static void Kill(Microsoft.Office.Interop.Excel.Application excel)
        {
            IntPtr t = new IntPtr(excel.Hwnd);   //得到这个句柄，具体作用是得到这块内存入口 
            int k = 0;
            GetWindowThreadProcessId(t, out k);   //得到本进程唯一标志k
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);   //得到对进程k的引用
            p.Kill();     //关闭进程k
        }


        public void QuitExcel()
        {


            if (excel != null)
            {
                setExcelVisible(false, excel);//不显示关闭时的提示信息

                excel.Workbooks.Close();

                excel.Quit();

                Kill(excel);
                //int generation = System.GC.GetGeneration(excel);

                //System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);


                //excel = null;
                //System.GC.Collect(generation);
            }
        }


        public object[,] getArrayValue(Excel.Worksheet ws, int startRowIndex, int startColIndex, int rowCnt, int colCnt)
        {
            object cell1 = ws.Cells[startRowIndex, startColIndex];
            object cell2 = ws.Cells[startRowIndex + rowCnt - 1, startColIndex + colCnt - 1];
            Excel.Range ra = ws.Range[cell1, cell2];
            return (object[,])ra.get_Value(Type.Missing);


        }


        public void setExcelVisible(bool visible, Excel.Application aexcel)
        {
            if (aexcel != null)
            {
                aexcel.ScreenUpdating = visible;


                aexcel.Visible = visible;
            }
        }

        /// <summary>
        /// 获得工作表
        /// </summary>
        /// <returns></returns>
        public Excel.Workbook getWorkbook()
        {
            Excel.Workbook wb = null;

            if (excel.Workbooks.Count == 0)
            {
                wb = addWorkbook();
            }
            else
            {
                excel.Workbooks[1].Activate();  //com 的数据是从1开始的


                wb = excel.ActiveWorkbook;
            }

            return wb;
        }

        public Excel.Workbook addWorkbook()
        {
            Excel.Workbook wb = excel.Workbooks.Add(Type.Missing);

            Excel.Worksheet ws = (Excel.Worksheet)wb.ActiveSheet;



            return wb;

        }


        public Excel.Workbook openWorkbookWithoutDisplay(string fileName)
        {
            Excel.Workbook wb = excel.Workbooks.Open(fileName, Excel.XlUpdateLinks.xlUpdateLinksNever, false,
               5, "", "", false, Excel.XlPlatform.xlWindows, Type.Missing, true, true, 0, false, true, Excel.XlCorruptLoad.xlNormalLoad);

            return wb;
        }





        #region IDisposable 成员
        // 已经被处理过的标记
        private bool _alreadyDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(true);

        }

        // 虚拟的Dispose方法
        protected virtual void Dispose(bool isDisposing)
        {
            // 不要多次处理
            if (_alreadyDisposed)
                return;
            if (isDisposing)
            {
                // TODO: 此处释放受控资源

                QuitExcel();

            }
            // TODO: 此处释放非受控资源。设置被处理过标记
            _alreadyDisposed = true;
        }


        #endregion


    }
}
