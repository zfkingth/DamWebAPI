using GalaSoft.MvvmLight.Ioc;
using hammergo.GlobalConfig;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DamServiceV3.Test.DamServiceRef;
using Hammergo.Utility;

namespace DamWebAPI.ViewModel
{
    public class AppIntegratedInfo : ViewModelBase
    {
        public string _appName = null;
        public int _topNum = 0;
        public DateTimeOffset? _startDate, _endDate;

        #region DbContex
        protected Container _dbContext = null;
        protected Container DbContext
        {
            get
            {
                if (_dbContext == null)
                {
                    _dbContext = new Container(SimpleIoc.Default.GetInstance<Uri>());
                    //重新载入时覆盖
                    _dbContext.MergeOption = System.Data.Services.Client.MergeOption.OverwriteChanges;

                }
                return _dbContext;
            }
        }
        #endregion


        Dictionary<string, byte> _dotNumDic = null;
        /// <summary>
        /// 参数的小数位数字典
        /// </summary>
        public Dictionary<string, byte> DotNumDic
        {
            get
            {
                if (_dotNumDic == null)
                {
                    _dotNumDic = new Dictionary<string, byte>();

                    RefreshDotNumDic();
                }
                return _dotNumDic;
            }
        }

        /// <summary>
        /// 刷新小数位数字典
        /// </summary>
        public void RefreshDotNumDic()
        {
            _dotNumDic.Clear();
            foreach (AppParam param in MesParams)
            {

                _dotNumDic.Add(param.ParamName, param.PrecisionNum);

            }


            foreach (AppParam param in CalcParams)
            {

                _dotNumDic.Add(param.ParamName, param.PrecisionNum);

            }
        }


        private App _currentApp = null;
        public App CurrentApp
        {
            get
            {

                return _currentApp;
            }
            set
            {
                _currentApp = value;
            }

        }


        private ApparatusType _appType = null;
        public ApparatusType AppType
        {
            get
            {
                if (_appType == null)
                {
                    if (CurrentApp.AppTypeID != null)
                    {
                        _appType = DbContext.ApparatusTypes.First(s => s.Id == CurrentApp.AppTypeID);
                    }
                }
                return _appType;
            }
        }



        private List<Formula> _allFormulae = null;
        public List<Formula> AllFormulae
        {
            get
            {
                if (_allFormulae == null)
                {
                    _allFormulae = DbContext.GetAllFormulaeByAppID(CurrentApp.Id).ToList();

                }
                return _allFormulae;
            }
        }


        private List<AppParam> _allParams = null;
        public List<AppParam> AllParams
        {
            get
            {
                if (_allParams == null)
                {
                    _allParams = DbContext.AppParams.Where(s => s.AppId == CurrentApp.Id).ToList();

                }
                return _allParams;
            }
        }


        private List<MessureParam> _mesParams = null;
        public List<MessureParam> MesParams
        {
            get
            {
                if (_mesParams == null)
                {
                    _mesParams = (from i in AllParams
                                  where  i is MessureParam
                                  orderby i.Order
                                  select (MessureParam)i).ToList();

                }
                return _mesParams;
            }
        }

        private List<CalculateParam> _calcParams = null;
        public List<CalculateParam> CalcParams
        {
            get
            {
                if (_calcParams == null)
                {
                    _calcParams = (from i in AllParams
                                   where i is CalculateParam
                                   orderby i.Order
                                   select (CalculateParam)i).ToList();
                }

                return _calcParams;
            }
        }


        private List<MessureValue> _messureValues = null;
        public List<MessureValue> MesValues
        {
            get
            {
                if (_messureValues == null)
                {
                    _messureValues = DbContext.GetMesValues(new Guid[]{ CurrentApp.Id}, MesParams.Count * _topNum, _startDate, _endDate).ToList();//MesValueBLL.GetList(appName, topNum * MessureParams.Count, startDate, endDate);
                }
                return _messureValues;
            }
        }

        private List<CalculateValue> _calcValues = null;
        public List<CalculateValue> CalcValues
        {
            get
            {
                if (_calcValues == null)
                {
                    _calcValues = DbContext.GetCalcValues(new Guid[]{ CurrentApp.Id}, CalcParams.Count * _topNum, _startDate, _endDate).ToList();
                }
                return _calcValues;
            }
        }

        private List<ConstantParam> _constantParams = null;

        public List<ConstantParam> ConstantParams
        {
            get
            {
                if (_constantParams == null)
                {
                    _constantParams = (from i in AllParams
                                       where   i is ConstantParam
                                       select (ConstantParam)i).ToList();
                }
                return _constantParams;
            }

        }


        private List<Remark> _remarks = null;
        public List<Remark> Remarks
        {
            get
            {
                if (_remarks == null)
                {
                    _remarks = DbContext.GetRemarks(new Guid[]{ CurrentApp.Id}, _topNum, _startDate, _endDate).ToList();// RemarkBLL.GetList(appName, topNum, startDate, endDate);
                }
                return _remarks;
            }
        }
        /// <summary>
        /// 重置参数并清除测量数据和计算数据
        /// </summary>
        /// <param name="topNum"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public void Reset(int topNum, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            this._topNum = topNum;
            this._startDate = startDate;
            this._endDate = endDate;
            this._messureValues = null;
            this._calcValues = null;
        }

        private void SetInfo(string appName, int topNum, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            this._appName = appName;
            this._topNum = topNum;
            this._startDate = startDate;
            this._endDate = endDate;
        }

        public AppIntegratedInfo(string appName, int topNum, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            _currentApp = (from i in DbContext.Apps
                           where i.AppName == appName
                           select i).FirstOrDefault();
            SetInfo(appName, topNum, startDate, endDate);

        }



        public AppIntegratedInfo(string calcName, DateTimeOffset? date)
        {
            _currentApp = (from s in DbContext.Apps
                           where s.CalculateName == calcName
                           select s).FirstOrDefault();
            if (_currentApp == null)
            {
                throw new Exception(string.Format("找不到计算编号为{0}的测点", calcName));
            }
            SetInfo(_currentApp.AppName, 0, date, date);


        }

        public AppIntegratedInfo(AppIntegratedInfo clonedAppInfo, int topNum, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            _dbContext = clonedAppInfo.DbContext;
            _currentApp = clonedAppInfo.CurrentApp;
            _calcParams = clonedAppInfo.CalcParams;

            SetInfo(_currentApp.AppName, topNum, startDate, endDate);
        }

        public AppIntegratedInfo(App app, int topNum, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            _currentApp = app;
            SetInfo(_currentApp.AppName, topNum, startDate, endDate);

        }



        /// <summary>
        /// 根据AppIntegratedInfo的信息生成组合表，包括测量数据、计算数据、备注
        /// </summary>
        /// <param name="num">需要显示数据的个数</param>
        /// <returns></returns>
        public DataTable ConstructTable()
        {
            DataTable dt = null;
            if (CalcParams.Count > 0)
            {
                List<DateTimeOffset> timeList = null;
                //必须具有计算参数
                if (MesParams.Count > 0)
                {
                    timeList = (from i in MesValues
                                select i.Date).Distinct().ToList();
                }
                else
                {
                    //没有测量参数的情况
                    timeList = (from i in CalcValues
                                select i.Date).Distinct().ToList();

                }

                int temp = int.MaxValue;
                if (_topNum > 0)
                {
                    temp = _topNum;
                }

                dt = createDataTableSchema();

                for (int index = 0; index < timeList.Count && index < temp; index++)
                {
                    DataRow row = dt.NewRow();

                    DateTimeOffset date = timeList[index];
                    row[PubConstant.timeColumnName] = date;



                    foreach (MessureValue mv in MesValues.Where(s => s.Date == date))
                    {
                        string aParamName = MesParams.First(s => s.Id == mv.ParamId).ParamName; //mv.messureParam.ParamName;
                        if (mv.Val != null)
                        {
                            row[aParamName] = mv.Val;
                        }
                    }



                    foreach (CalculateValue cv in CalcValues.Where(s => s.Date == date))
                    {
                        string aParamName = CalcParams.First(s => s.Id == cv.ParamId).ParamName; //cv.calculateParam.ParamName;
                        if (cv.Val != null)
                        {
                            row[aParamName] = cv.Val;
                        }
                    }

                    Remark remark = Remarks.FirstOrDefault(s => s.Date == date);
                    if (remark != null)
                    {
                        row[PubConstant.remarkColumnName] = remark.RemarkText;
                    }
                    dt.Rows.Add(row);

                }

                dt.AcceptChanges();

            }

            return dt;
        }

        private DataTable createDataTableSchema()
        {

            DataTable dt = new DataTable();

            DataColumn dateColumn = new DataColumn(PubConstant.timeColumnName, typeof(DateTimeOffset));
            dateColumn.AllowDBNull = false;
            dateColumn.ReadOnly = true;
            dt.Columns.Add(dateColumn);
            dt.PrimaryKey = new DataColumn[] { dateColumn };

            foreach (AppParam param in MesParams)
            {

                DataColumn column = new DataColumn(param.ParamName, typeof(double));
                dt.Columns.Add(column);
            }



            foreach (AppParam param in CalcParams)
            {

                DataColumn column = new DataColumn(param.ParamName, typeof(double));
                dt.Columns.Add(column);
            }



            dt.Columns.Add(PubConstant.remarkColumnName, typeof(string)); //add remark column

            return dt;

        }



        /// <summary>
        /// 查询离指定时间最近的数据的记录
        /// </summary>

        /// <param name="preConcertDate">指定的时间,也就是预定时间</param>
        /// <param name="days">"最近"这个词所允许的与预期时间相隔最大的天数</param>
        /// <returns></returns>
        public AppIntegratedInfo getAppInfoNearTime(DateTimeOffset preConcertDate, double days)
        {

            AppIntegratedInfo appInfo = null;

            AppIntegratedInfo infoSuffix = new AppIntegratedInfo(this, 1, preConcertDate, null);

            AppIntegratedInfo infoPrefix = new AppIntegratedInfo(this, 1, null, preConcertDate);

            if (infoSuffix.CalcValues.Count == 0 && infoPrefix.CalcValues.Count == 0)
                return null;

            long after = long.MaxValue, before = long.MaxValue;



            if (infoSuffix.CalcValues.Count != 0)
            {

                after = (infoSuffix.CalcValues[0].Date).Ticks - preConcertDate.Ticks;

            }

            if (infoPrefix.CalcValues.Count != 0)
            {

                before = preConcertDate.Ticks - (infoPrefix.CalcValues[0].Date).Ticks;
            }

            long cha = 0;
            if (after > before)
            {
                cha = before;
                appInfo = infoPrefix;
            }
            else
            {
                cha = after;
                appInfo = infoSuffix;
            }

            if (cha > TimeSpan.TicksPerDay * days)
            {
                appInfo = null;
            }

            return appInfo;
        }

        public void Update()
        {

            DbContext.SaveChanges(SaveChangesOptions.Batch);
        }


        internal void HandleDataRowChanged(DataRow row, string feildName)
        {



            DateTimeOffset date = (DateTimeOffset)row[PubConstant.timeColumnName];

            if (feildName == PubConstant.remarkColumnName)
            {

                Remark remark = Remarks.FirstOrDefault(s => s.Date == date);

                object val = row[PubConstant.remarkColumnName];

                if (val is string && ((string)val).Trim().Length != 0)
                {
                    if (remark == null)
                    {
                        //create new object
                        remark = new Remark();

                        remark.Date = date;
                        remark.AppId = CurrentApp.Id;
                        //添加到列表中
                        Remarks.Add(remark);
                        //添加到dbcontext中
                        DbContext.AddToRemarks(remark);

                    }

                    remark.RemarkText = (val as string).Trim();

                }
                else
                {

                    if (remark != null)
                    {
                        Remarks.Remove(remark);
                        DbContext.DeleteObject(remark);
                    }
                }

                this.Update();


            }
            else
            {
                //更改的是其它列

                redirectToObjects(this, row, feildName);

                Task.Factory.StartNew(() => UpdateRefApps(date));
            }
        }

        private void UpdateRefApps(DateTimeOffset date)
        {
            this.Update();//出错会抛出异常

            //操作正常才会到这一步
            reCalculateLink(this.CurrentApp.CalculateName, date);
        }

        private void reCalculateLink(string calculateName, DateTimeOffset date)
        {
            try
            {
                //生成拓扑图
                ALGraph.MyGraph graph = new ALGraph.MyGraph();

                constructGraph(calculateName, graph, date);

                System.Collections.ArrayList toplist = graph.topSort();
                if (toplist.Count != graph.Vexnum)
                {
                    throw new Exception("仪器公式存在循环依赖");
                }

                //根据拓扑顺序依次更新

                for (int i = 0; i < toplist.Count; i++)
                {
                    string csn = (string)toplist[i];
                    //不必更新本身
                    if (csn != calculateName)
                    {
                        AppIntegratedInfo otherApp = new AppIntegratedInfo(csn, date);
                        reCalcValues(otherApp, null, date);
                        //要更新数据
                        otherApp.Update();
                    }
                }


            }
            catch (Exception ex)
            {
                throw new Exception("计算关联测点错误 " + ex.Message);
            }
        }


        /// <summary>
        /// 构建拓朴图
        /// </summary>
        /// <param name="calculateName">计算名称</param>
        /// <param name="graph">拓朴图</param>
        /// <param name="date">数据的日期，有可能不同日期的公式所引用的测点不一样</param>
        private void constructGraph(string calculateName, ALGraph.MyGraph graph, DateTimeOffset date)
        {
            hammergo.caculator.MyList list = getChildApp(calculateName, date);

            for (int i = 0; i < list.Length; i++)
            {
                string calcSn = list.getKey(i);
                graph.addArcNode(new ALGraph.ArcNode(), calculateName, calcSn);


                constructGraph(calcSn, graph, date);
            }

        }



        /// <summary>
        /// 获取仪器的子仪器数据
        /// </summary>
        /// <param name="calculateName"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private hammergo.caculator.MyList getChildApp(string calculateName, DateTimeOffset date)
        {
            hammergo.caculator.MyList list = new hammergo.caculator.MyList(5);

            List<string> calcNameList = DbContext.GetChildAppCalcName(calculateName, date).ToList();

            foreach (string name in calcNameList)
            {
                list.add(name, 0);
            }


            return list;



        }



        //重新计算相关值，并反映在gridview中
        public static void redirectToObjects(AppIntegratedInfo appInfo, DataRow row, string feildName)
        {
            DateTimeOffset date = (DateTimeOffset)row[PubConstant.timeColumnName];
            MessureParam mp = appInfo.MesParams.FirstOrDefault(s => s.ParamName == feildName);
            if (mp != null)
            {
                MessureValue editedValue = appInfo.MesValues.Find(delegate(MessureValue item)
                {
                    return item.Date == date && item.ParamId == mp.Id;
                });

                if (editedValue == null)
                {
                    //create new object
                    editedValue = new MessureValue();
                    editedValue.Date = date;
                    editedValue.ParamId = mp.Id;
                    appInfo.MesValues.Add(editedValue);

                    appInfo.DbContext.AddToMessureValues(editedValue);
                }


                editedValue.Val = (double)row[feildName];

                appInfo.DbContext.UpdateObject(editedValue);

                //recalculate the calc values
                reCalcValues(appInfo, row, date);

            }
            else
            {
                //直接编辑成果值，所有不用计算本测点的数据，但需要更新引用测点的数据
                CalculateParam cp = appInfo.CalcParams.Find(delegate(CalculateParam item)
                {
                    return item.ParamName == feildName;
                });

                if (cp != null)
                {

                    CalculateValue calcValue = appInfo.CalcValues.Find(delegate(CalculateValue item)
                    {
                        return item.Date == date && item.ParamId == cp.Id;
                    });

                    if (calcValue == null)
                    {
                        //create new object
                        calcValue = new CalculateValue();
                        calcValue.Date = date;
                        calcValue.ParamId = cp.Id;
                        appInfo.CalcValues.Add(calcValue);

                        appInfo.DbContext.AddToCalculateValues(calcValue);
                    }

                    calcValue.Val = (double)row[feildName];

                    //dbcontext标记要更新
                    appInfo.DbContext.UpdateObject(calcValue);

                }
            }

            //may be need reset the filter to null

        }

        class ParamHelper
        {
            public Formula FormulaEntity;
            public CalculateParam Param;
        }


        /// <summary>
        /// 由于messure values更改了,需要重新计算calc values的值
        /// </summary>
        /// <param name="appInfo"></param>
        /// <param name="currentRow"></param>
        /// <param name="date"></param>
        internal static void reCalcValues(AppIntegratedInfo appInfo, DataRow currentRow, DateTimeOffset date)
        {
            hammergo.caculator.CalcFunction calc = new hammergo.caculator.CalcFunction();
            hammergo.caculator.MyList list = new hammergo.caculator.MyList();

            //填充自身,要求在appInfo中的数据是更新后的数据
            fillListByCalcName_Date(list, date, false, appInfo);

            //分析此仪器是否引用了其它仪器的数据
            hammergo.caculator.MyList appCalcNameList = new hammergo.caculator.MyList();//引用的其它仪器名称的集合

            var sortedParams = new SortedDictionary<byte, ParamHelper>();



            //这里必须按照calc order 的顺序 
            foreach (CalculateParam cp in appInfo.CalcParams)
            {


                Formula formulaEntity = appInfo.AllFormulae.SingleOrDefault(s => s.ParamId == cp.Id && s.StartDate <= date && date < s.EndDate);

                if (formulaEntity == null)
                {
                    throw new Exception(string.Format("找不到对应时间:{0:u}的工式，请检查测点的参数信息", date));

                }

                sortedParams.Add(formulaEntity.CalculateOrder, new ParamHelper { FormulaEntity = formulaEntity, Param = cp });

                string formulaString = formulaEntity.FormulaExpression;//获取表达式

                System.Collections.ArrayList vars = calc.getVaribles(formulaString);
                //为了避免对某个测点的数据重复查询,将一次性填充一支仪器的数据


                for (int j = 0; j < vars.Count; j++)
                {
                    string vs = (string)vars[j];
                    int pos = vs.IndexOf('.');
                    if (pos != -1)
                    {
                        //引用了其它测点
                        string otherID = vs.Substring(0, pos);
                        appCalcNameList.add(otherID, 0);
                        //避免由于计算时依赖其它仪器，而其它仪器没有此刻的记录时，导致异常
                        list.add(vs, 0);
                    }
                }
            }

            //填充带点的参数
            for (int i = 0; i < appCalcNameList.Length; i++)
            {

                AppIntegratedInfo simpleAppInfo = new AppIntegratedInfo(appCalcNameList.getKey(i), date);
                fillListByCalcName_Date(list, date, true, simpleAppInfo);

            }

            //可以进行表达式求值了

            //根据计算顺序
            for (int i = 0; i < sortedParams.Count; i++)
            {
                var helperobj = sortedParams.ElementAt(i).Value;
                var cp = helperobj.Param;
                //顺序是以order 升序排列
                string formula = helperobj.FormulaEntity.FormulaExpression;//获取表达式

                double v = calc.compute(formula, list);

                byte precision = cp.PrecisionNum;

                if (precision >= 0)
                {
                    v = Helper.Round(v, precision);
                }



                CalculateValue calcValue = appInfo.CalcValues.Find(delegate(CalculateValue item)
                {
                    return item.Date == date && item.ParamId == cp.Id;
                });

                if (calcValue == null)
                {
                    //create new object
                    calcValue = new CalculateValue();
                    calcValue.Date = date;
                    calcValue.ParamId = cp.Id;

                    appInfo.CalcValues.Add(calcValue);

                    //添加到dbcontext中
                    appInfo.DbContext.AddToCalculateValues(calcValue);
                }

                calcValue.Val = v;

                appInfo.DbContext.UpdateObject(calcValue);

                //在填充时已添加，这里就不能添加
                list[cp.ParamSymbol] = v;

                if (currentRow != null)
                    currentRow[cp.ParamName] = v;
            }

        }


        /// <summary>
        /// 将实际的值填充到参数列表中，如果参数没有相应的值，将被赋初值0
        /// </summary>
        /// <param name="list">参数列表</param>
        /// <param name="appCalcName">测点的计算名称</param>
        /// <param name="date">日期</param>
        /// <param name="appendDot">是否将计算名称和点加在参数的前面</param>
        /// <param name="simpleInfo">测点数据信息</param>
        internal static void fillListByCalcName_Date(hammergo.caculator.MyList list, DateTimeOffset date, bool appendDot, AppIntegratedInfo simpleInfo)
        {
            string appCalcName = simpleInfo.CurrentApp.CalculateName;

            foreach (ConstantParam cp in simpleInfo.ConstantParams)
            {
                string key = cp.ParamSymbol;
                if (appendDot)
                {
                    key = String.Format("{0}.{1}", appCalcName, key);
                }
                list.add(key, cp.Val);

            }

            //赋初值0

            foreach (MessureParam mp in simpleInfo.MesParams)
            {
                string key = mp.ParamSymbol;
                if (appendDot)
                {
                    key = String.Format("{0}.{1}", appCalcName, key);
                }
                list.add(key, 0);
            }

            //赋初值0
            foreach (CalculateParam cp in simpleInfo.CalcParams)
            {
                string key = cp.ParamSymbol;
                if (appendDot)
                {
                    key = String.Format("{0}.{1}", appCalcName, key);
                }
                list.add(key, 0);
            }

            //加快速度
            foreach (MessureValue mv in simpleInfo.MesValues.Where(s => s.Date == date))
            {
                MessureParam mp = simpleInfo.MesParams.Find(delegate(MessureParam item) { return item.Id == mv.ParamId; });

                string key = mp.ParamSymbol;
                if (appendDot)
                {
                    key = String.Format("{0}.{1}", appCalcName, key);
                }

                list[key] = mv.Val.Value;
            }

            //加快速度
            foreach (CalculateValue cv in simpleInfo.CalcValues.Where(s => s.Date == date))
            {
                CalculateParam cp = simpleInfo.CalcParams.Find(delegate(CalculateParam item) { return item.Id == cv.ParamId; });
                string key = cp.ParamSymbol;
                if (appendDot)
                {
                    key = String.Format("{0}.{1}", appCalcName, key);
                }

                list[key] = cv.Val.Value;

            }

        }



        /// <summary>
        /// 根据日期删除对应的数据，修改只反应在dbcontext中，还没有提交到数据库中
        /// </summary>
        /// <param name="date"></param>
        internal void DeleteValuesByDate(DateTimeOffset date)
        {
            var delMesValues = this.MesValues.FindAll(s => s.Date == date);
            var delCalcValues = this.CalcValues.FindAll(s => s.Date == date);

            foreach (var item in delMesValues)
            {
                MesValues.Remove(item);
                this.DbContext.DeleteObject(item);
            }

            foreach (var item in delCalcValues)
            {
                CalcValues.Remove(item);
                this.DbContext.DeleteObject(item);
            }


        }
    }
}
