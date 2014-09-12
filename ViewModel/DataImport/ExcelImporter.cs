using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using Hammergo.Utility;
using Hammergo.GlobalConfig;
using System.Collections;
using DamServiceV3.Test.DamServiceRef;

namespace DamWebAPI.ViewModel.DataImport
{
    class ExcelImporter
    {
        internal ExcelHelper excelHelper = null;


        string fullPath = "";
        string workSheetName = "";
        /// <summary>
        /// 从excel导入数据到数据库
        /// </summary>
        /// <param name="fullPath">文件完整路径</param>
        public void import(string fullPath)
        {
            try
            {
                if (excelHelper == null)
                {
                    excelHelper = new ExcelHelper();
                }

                excelHelper.initialExcel();

                Excel.Workbook wb = excelHelper.openWorkbookWithoutDisplay(fullPath);


                this.fullPath = fullPath;
                foreach (Excel.Worksheet ws in wb.Sheets)
                {
                    //excel表名即测点编号
                    string appName = ws.Name;
                    workSheetName = ws.Name;
                    //可获取最大日期
                    //最后一次数据
                    AppIntegratedInfo appInfo = new AppIntegratedInfo(appName, 1, null, DateTimeOffset.MaxValue);
                    if (appInfo.CurrentApp != null)
                    {
                        //创建构架表
                        DataTable table = appInfo.createDataTableSchema();

                        //列名和其在数据中的列号对应表
                        Hashtable nameIndexMaps = createHashMap(appInfo);
                        //将excel中的有效数据读入数组中
                        object[,] allData = readData(ws, appInfo, nameIndexMaps);
                        //将数组中的数据到table
                        fillDataTable(appInfo, allData, nameIndexMaps, table);

                        saveData(appInfo, table);
                    }
                }
            }
            finally
            {
                if (excelHelper != null)
                {

                    excelHelper.QuitExcel();
                    excelHelper = null;
                }
            }

        }


        private Hashtable createHashMap(AppIntegratedInfo appInfo)
        {
            Hashtable ht = new Hashtable(10);
            ht.Add(PubConstant.timeColumnName, null);
            ht.Add(PubConstant.remarkColumnName, null);

            foreach (MessureParam item in appInfo.MesParams)
            {
                ht.Add(item.ParamName, null);
            }

            foreach (CalculateParam item in appInfo.CalcParams)
            {
                ht.Add(item.ParamName, null);
            }

            return ht;
        }

        /// <summary>
        /// 读取数据时的时间列表
        /// </summary>
        List<DateTime> listDates = null;

        //excel中表头所占的行数
        const int _tableHeaderRowsCnt = 1;
        /// <summary>
        /// 搜索时每次读取的数据个数
        /// </summary>
        const int _cntPerRead = 50;
        /// <summary>
        /// excel中实际数据的起始行号
        /// </summary>
        const int _dataStartRow = 2, _dataStartCol = 1;

        /// <summary>
        /// 读取Excel数据的最大列数
        /// </summary>
        const int _maxReadColCnt = 40;

        private object[,] readData(Microsoft.Office.Interop.Excel.Worksheet ws, AppIntegratedInfo appInfo, Hashtable nameIndexMaps)
        {
            object[,] allData = null;

            listDates = new List<DateTime>(100);
            int colsCnt = nameIndexMaps.Count;

            //读取表头
            object[,] tableHeader = excelHelper.getArrayValue(ws, _dataStartRow - _tableHeaderRowsCnt, _dataStartCol, _tableHeaderRowsCnt, _maxReadColCnt);


            //表头的列计数器
            int headerCnt = 0;
            //数组从1开始
            for (int i = 1; i <= _maxReadColCnt; i++)
            {
                object obj = tableHeader[_dataStartRow - _tableHeaderRowsCnt, i];
                if (obj != null)
                {
                    string val = obj.ToString().Trim();

                    if (nameIndexMaps.ContainsKey(val))
                    {
                        nameIndexMaps[val] = i;
                        //找到了一列
                        headerCnt++;
                    }
                    //else
                    //{
                    //    throw new Exception(string.Format("Excel文件{0}的表{1}中的'{2}'列与测点'{3}'的参数不配置,无法导入", fullPath, ws.Name, val, appInfo.appName));
                    //}
                }
                //else
                //{
                //    throw new Exception(string.Format("Excel文件{0}的表{1}中的第{2}列不能为空", fullPath, ws.Name, i));
                //}
            }

            if (headerCnt != colsCnt)
            {
                //在Excel中没有找到全部参数
                var query = from i in nameIndexMaps.Keys.Cast<string>()
                            where nameIndexMaps[i] == null
                            select i;
                string names = "";
                foreach (string name in query)
                {
                    names += name + " ";
                }

                throw new Exception(string.Format("Excel文件{0}\n表{1}中找不到以下列\n{2}", fullPath, ws.Name, names));
            }

            //找到所有表头数据

            bool goLoop = true;
            object[,] dateData = null;
            //确定数据的行数
            int dateColIndex = (int)nameIndexMaps[PubConstant.timeColumnName];
            for (int j = 0; goLoop; j++)
            {

                dateData = excelHelper.getArrayValue(ws, _dataStartRow + j * _cntPerRead, _dataStartCol, _cntPerRead, dateColIndex);
                for (int i = 1; i <= _cntPerRead; i++)
                {
                    object obj = dateData[i, dateColIndex];

                    if (obj != null)
                    {
                        DateTime? cDate = convertToDateTime(obj);
                        if (cDate != null)
                        {
                            //更改数组里的值，以免再一次做转换
                            listDates.Add(cDate.Value);
                        }
                        else
                        {
                            throw new Exception(
                                string.Format("Excel文件{0}的表{1}中，第{2}行日期有误\n 该数据必须是日期或字符串类型,如果是字符串类型,其格式是须是{3}或{4}",
                                fullPath, ws.Name, _dataStartRow + j * _cntPerRead + i - 1, PubConstant.shortString, PubConstant.customString));
                        }


                    }
                    else
                    {
                        goLoop = false;
                        break;//数据只有这么多行
                    }
                }

            }



            allData = excelHelper.getArrayValue(ws, _dataStartRow, _dataStartCol, listDates.Count, _maxReadColCnt);//包括时间列

            return allData;


        }

        private DateTime? convertToDateTime(object obj)
        {
            DateTime? date = null;
            if (obj is DateTime)
            {
                date = (DateTime)obj;
            }
            else if (obj is string)
            {
                string temp = obj.ToString().Trim();

                if (temp.Length == PubConstant.shortString.Length)
                {
                    try
                    {
                        date = DateTime.ParseExact(temp, PubConstant.shortString, null);
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (temp.Length == PubConstant.customString.Length)
                {
                    try
                    {
                        date = DateTime.ParseExact(temp, PubConstant.customString, null);
                    }
                    catch (Exception)
                    {
                    }
                }


            }

            return date;
        }



        private void fillDataTable(AppIntegratedInfo appInfo, object[,] allData, Hashtable nameIndexMaps, DataTable table)
        {
            for (int i = 0; i < listDates.Count; i++)
            {
                DataRow row = table.NewRow();

                foreach (string key in nameIndexMaps.Keys)
                {
                    if (key == PubConstant.timeColumnName)
                    {
                        row[key] = new DateTimeOffset(listDates[i]);//转换
                    }
                    else if (key == PubConstant.remarkColumnName)
                    {
                        row[key] = allData[i + 1, (int)nameIndexMaps[key]];//index从1开始
                    }
                    else
                    {
                        object val = allData[i + 1, (int)nameIndexMaps[key]];//

                        double? ret = null;
                        if (val is string)
                        {
                            string temp = val.ToString().Trim();
                            try
                            {
                                ret = double.Parse(temp);
                            }
                            catch (Exception)
                            {
                                throw new Exception(string.Format("Excel文件{0}的表{1}中，第{2}行的'{3}'列数据有误\n ", fullPath, workSheetName, i + 1 + _tableHeaderRowsCnt, key));//i从0开始，所有还原的话要加1，而excel的第一行是表头，所以还要加tableHeaderRowsCnt

                            }

                        }
                        else if (val is double)
                        {
                            ret = (double)val;
                        }

                        if (ret != null)
                        {
                            CalculateParam cp = appInfo.CalcParams.Find(delegate(CalculateParam item) { return item.ParamName == key; });
                            if (cp != null && cp.PrecisionNum != null)
                            {

                                ret = Hammergo.Utility.Helper.Round(ret.Value, cp.PrecisionNum);
                            }

                            row[key] = ret;
                        }
                    }
                }

                row.EndEdit();
                table.Rows.Add(row);
            }

            table.AcceptChanges();
        }

        private void saveData(AppIntegratedInfo appInfo, DataTable table)
        {
            DateTimeOffset maxDate = DateTimeOffset.MinValue;
            if (appInfo.CalcValues.Count > 0)
            {
                //最后一次数据
                maxDate = appInfo.CalcValues[0].Date;

            }

            foreach (DataRow row in table.Rows)
            {
                DateTimeOffset date = (DateTimeOffset)row[PubConstant.timeColumnName];
                if (date > maxDate)
                {


                    foreach (MessureParam item in appInfo.MesParams)
                    {
                        MessureValue mv = new MessureValue();
                        mv.Date = date;
                        mv.Id = Guid.NewGuid();
                        mv.ParamId = item.Id;
                        mv.Val = row[item.ParamName] as double?;

                        appInfo.DbContext.AddToMessureValues(mv);
                    }

                    foreach (CalculateParam item in appInfo.CalcParams)
                    {
                        CalculateValue mv = new CalculateValue();
                        mv.Date = date;
                        mv.Id = Guid.NewGuid();
                        mv.ParamId = item.Id;
                        mv.Val = row[item.ParamName] as double?;


                        appInfo.DbContext.AddToCalculateValues(mv);
                    }

                    object remarkVal = row[PubConstant.remarkColumnName];
                    if (remarkVal != null && remarkVal.ToString().Trim().Length != 0)
                    {
                        Remark remark = new Remark();
                        remark.Id = Guid.NewGuid();
                        remark.AppId = appInfo.CurrentApp.Id;
                        remark.Date = date;
                        remark.RemarkText = remarkVal.ToString().Trim();

                        appInfo.DbContext.AddToRemarks(remark);
                    }



                }
            }
            appInfo.Update();
        }



    }
}
