using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Hammergo.Data.Mapping;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Hammergo.Data;
using System;
using System.Data;
using System.Data.Entity.Core.Objects;

namespace Hammergo.Data.Logic
{
    public class ParamsValidatation
    {

        //不要使用这个上下文保存数据调用savechanges，否则有可能造成循环
        DamWCFContext dbcontext = null;

        App _modifiedApp = null;
        List<ObjectStateEntry> _paramsEntries = null;
        List<ObjectStateEntry> _formulaEntries = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据上下文</param>
        /// <param name="modifiedApp">参数被修改的测点</param>
        /// <param name="paramsEntries">修改的参数entry，可能包含其它测点的entry</param>
        /// <param name="formulaEntries">修改的公式参数entry，可能包含其它测点的entry</param>
        public ParamsValidatation(DamWCFContext context,App modifiedApp, List<ObjectStateEntry> paramsEntries, List<ObjectStateEntry> formulaEntries)
        {
            dbcontext = context;
            _modifiedApp = modifiedApp;
            _paramsEntries = paramsEntries;
            _formulaEntries = formulaEntries;
        }

        public void Validate()
        {
            List<string> nameList = new List<string>(20);
            List<string> symbolList = new List<string>(20);

            //_modifiedApp _modifiedParams和实例中的其它Entity属于不同的dbcontext

            //需要验证公式更新的逻辑，这是整个app最复杂的问题之一，其它有公式解析和拓扑排序
            //获取参数列表
            var paramList = (from i in dbcontext.AppParams
                             where i.AppId == _modifiedApp.Id
                             select i).AsNoTracking().ToList();
            //获取该测点的所有计算公式

            //当前数据库中的参数列表
            var formulaList = (from p in dbcontext.AppParams.OfType<CalculateParam>()
                               where p.AppId == _modifiedApp.Id
                               join f in dbcontext.Formulae
                               on p.Id equals f.ParamId
                               select f).ToList(); //需要更新formula的计算次序


            //处理added,modified,deleted
            //用修改的值替换内存中的值
            foreach (var entry in _paramsEntries)
            {
                var entity = entry.Entity as AppParam;
                if (entity.AppId == _modifiedApp.Id)
                {
                    //只能是modified或deleted
                    int index = paramList.FindIndex(s => s.Id == entity.Id);
                    if (index >= 0)
                        paramList.RemoveAt(index);
                    if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                    {
                        paramList.Add(entity);
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        //在删除参数时，会级联删除公式

                        formulaList.RemoveAll(s => s.ParamId == entity.Id);
                    }
                }
            }

            foreach (ObjectStateEntry entry in _formulaEntries)
            {
                var entity = entry.Entity as Formula;
                //公式必须依附于参数
                //paramList中的参数有可以是新增的参数，即数据库还没有记录
                if (paramList.Exists(s => s.Id == entity.Id))
                {

                    int index = formulaList.FindIndex(s => s.ParamId == entity.ParamId&&s.StartDate==entity.StartDate);
                    if (index >= 0)
                    {
                        //entity已被修改，或增加，删除，得先从列表中先先移除
                        formulaList.RemoveAt(index);
                    }
                    if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                    {
                        formulaList.Add(entity);
                    }
                }

            }


            //检查名称和符号是否有重复
            checkParamNames(paramList, nameList, symbolList);

            //测点公式的时间段必须具备连续性，所以需先对公式进行检查
            var formulaGroup = (from i in formulaList
                                orderby i.StartDate ascending
                                group i by i.StartDate).ToList();
            int gCnt = formulaGroup.Count();
            if (gCnt == 0)
            {
                throw new Exception("测点的计算参数必须带有公式");
            }

            int calcParamsCnt = paramList.OfType<CalculateParam>().Count();

            //每组公式的结束时间
            DateTimeOffset? endDate = null; ;
            for (int i = 0; i < gCnt; i++)
            {
                var item = formulaGroup[i];
                var startDate = item.Key;
                if (endDate != null)
                {
                    if (startDate != endDate)
                    {
                        throw new Exception(string.Format("起始时间为{0}的公式的开始时间与上一分段公式的结束时间没有衔接,\n即上一段公式的结束时间必须是下一段公式的开始时间", startDate));
                    }
                }

                //检查个数

                if (item.Count() != calcParamsCnt)
                {
                    throw new Exception(string.Format("起始时间为{0}的公式与计算参数的数目不一致", startDate));
                }

                //公式和参数要一一对应，检查对应关系 
                var compositList = (from ci in paramList.OfType<CalculateParam>()
                                    join f in item.AsEnumerable()
                                    on ci.Id equals f.ParamId
                                    select new
                                    {
                                        Param = ci,
                                        Formula = f
                                    }).ToList();
                if (compositList.Count != calcParamsCnt)
                {

                    throw new Exception(string.Format("起始时间为{0}的公式没有与计算参数一一对应", startDate));
                }


                endDate = item.ElementAt(0).EndDate;

                if (startDate >=endDate)
                {
                    throw new Exception("分段公式的开始时间必须小于结束时间");
                }

                if (item.Count(s => s.EndDate == endDate) != calcParamsCnt)
                {
                    throw new Exception(string.Format("起始时间为{0}的公式结束时间不一致", startDate));
                }

                //检查每组公式的计算逻辑

                hammergo.caculator.MyList list = new hammergo.caculator.MyList(10);

                int num = 1;//填充一些数据，测试计算的表达式
                foreach (string s in symbolList)
                {
                    list.add(s, num++);
                }



               // 简单的判断公式的依赖关系，只能精确到仪器,不能精确的量（如n01cf14.e的依赖关系，过于复杂）


                //生成新的图
                ALGraph.MyGraph graph = new ALGraph.MyGraph();
                hammergo.caculator.CalcFunction calc = new hammergo.caculator.CalcFunction();
                //引用测点的编号列表
                hammergo.caculator.MyList calcNameList = new hammergo.caculator.MyList(5);



                foreach (var cp in compositList)
                {
                    string formulaString = cp.Formula.FormulaExpression;
                    string symbol = cp.Param.ParamSymbol;

                    if (formulaString == null||formulaString.Trim().Length==0)
                    {
                        throw new Exception(string.Format("计算参数 {0} 的计算公式不能为空", cp.Param.ParamName));
                    }

                    ArrayList vars = calc.getVaribles(formulaString);

                    intitalVars(_modifiedApp.CalculateName, vars, graph, calcNameList, symbol);
                }


                ArrayList toplist = graph.topSort();
                if (toplist.Count != graph.Vexnum)
                {
                    throw new Exception("公式存在循环依赖");
                }

                ArrayList storeTopList = toplist;

                // 刚才是检查仪器内的循环依赖,现在和所有的仪器一起检查


                graph = new ALGraph.MyGraph();

                for (int j = 0; j< calcNameList.Length; j++)
                {
                    graph.addArcNode(new ALGraph.ArcNode(), calcNameList.getKey(j), _modifiedApp.CalculateName);//全部使用计算名称
                    var loopCheckList = new List<Guid>(5);
                    constructGraph( _modifiedApp, graph,loopCheckList);//递归加入其子结点
                }

                toplist = graph.topSort();
                if (toplist.Count != graph.Vexnum)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(128);
                    foreach (ALGraph.VNode node in graph.vertices)
                    {
                        sb.Append(node.data).Append("  ");

                    }
                    throw new Exception(sb.ToString() + " 仪器公式存在循环依赖");
                }


                toplist = storeTopList;

                //图不存在回路
                //引用的其它测点,填充
                for (int j = 0; j < calcNameList.Length; j++)
                {


                    string calcName = calcNameList.getKey(j);

                    var refApp = dbcontext.Apps.AsNoTracking().FirstOrDefault(s => s.CalculateName == calcName);

                    if (refApp == null)
                    {
                        throw new Exception(string.Format("计算名称为{0}的仪器不存在!", calcName));
                    }
                    //ArrayList paList = new ArrayList(12);
                    //paList.AddRange(consBLL.GetListByappName(refApp.AppName));
                    //paList.AddRange(mesBLL.GetListByappName(refApp.AppName));
                    //paList.AddRange(calcBLL.GetListByappName(refApp.AppName));

                    var paList = refApp.AppParams.ToList();

                    for (int k = 0; k < paList.Count; k++)
                    {
                        var pi = paList[k] ;

                        list.add(calcName + "." + pi.ParamSymbol, k + 1);
                    }
                }


                foreach (var cp in compositList)
                {
                    string formula = cp.Formula.FormulaExpression;
                    calc.compute(formula, list);

                    string symbol = cp.Param.ParamSymbol;

                    int index = toplist.IndexOf(symbol);
                    if (index < 0)
                    {
                        //此符号不在拓扑图中，没有公式依赖于它
                        index = toplist.Count;

                    }

                    //cp.CalculateOrder = (byte)index;
                    //更新计算次序
                    cp.Formula.CalculateOrder = (byte)index;

                }



            }

        }


        public void constructGraph(App app, ALGraph.MyGraph graph, List<Guid> loopCheckList)
        {
            foreach (App child in getChildApp(app))
            {
                if (loopCheckList.Exists(s => s == app.Id))
                {
                    throw new Exception("测点公式中引用了其它测点的公式，但是存在循环引用的问题");
                }
                else
                {
                    loopCheckList.Add(app.Id);
                }
                graph.addArcNode(new ALGraph.ArcNode(), app.CalculateName, child.CalculateName);
                constructGraph(app, graph, loopCheckList);
            }
        }

        /// <summary>
        ///  根据计算公式搜索引用该测点的所有子测点
        /// </summary>
        /// <param name="parentApp">父测点</param>
        /// <returns></returns>
        private List<App> getChildApp(App parentApp)
        {
            hammergo.caculator.MyList list = new hammergo.caculator.MyList(5);



            //需要考虑各时间段的不同
            var ids = (from i in dbcontext.Formulae
                       where i.FormulaExpression.Contains(parentApp.CalculateName + ".")
                       select i.ParamId).Distinct();

            //同一个子测点的计算公式可能多次引用父测点的计算编号
            var children = (from i in dbcontext.Apps
                            join p in dbcontext.CalculateParams
                            on i.Id equals p.Id
                            join calcID in ids
                            on p.Id equals calcID
                            select i).AsNoTracking().Distinct().ToList();

            return children;
        }

        /// <summary>
        /// 构造树型结点，并查找计算变量引用的测点
        /// </summary>
        /// <param name="appCalcName">计算名称</param>
        /// <param name="vars">计算公式中需要的计算变量</param>
        /// <param name="graph">拓扑树</param>
        /// <param name="idList">引用测点计算的计算名称列表</param>
        /// <param name="symbol">当前计算量的符号</param>
        private void intitalVars(string appCalcName, ArrayList vars, ALGraph.MyGraph graph, hammergo.caculator.MyList idList, string symbol)
        {



            for (int i = 0; i < vars.Count; i++)
            {
                string vs = (string)vars[i];
                int pos = vs.IndexOf('.');
                if (pos != -1)//带点的参数
                {
                    string otherID = vs.Substring(0, pos);
                    if (otherID == appCalcName)//计算名称
                    {
                        throw new Exception("公式中的变量不能包含本仪器的二级变量");
                    }
                    else
                    {
                        idList.add(otherID.ToUpper(), 0);
                    }
                }
                else
                {
                    //					//不带点的参数

                    //添加弧

                    graph.addArcNode(new ALGraph.ArcNode(), vs, symbol);



                }
            }

        }


        /// <summary>
        /// 检查参数的名称是否效,如果无效抛出异常
        /// </summary>
        /// <param name="consList">常量参数列表</param>
        /// <param name="mesList">测量参数列表</param>
        /// <param name="calList">计算参数列表</param>
        /// <param name="nameList">参数名称列表</param>
        /// <param name="symbolList">参数符号列表</param>
        private void checkParamNames(List<AppParam> paramList, List<string> nameList, List<string> symbolList)
        {
            foreach (var pi in paramList)
            {
                //验证在DataAnnotation中已进行
                //if (IsValidName(pi.ParamSymbol) == false)
                //{
                //    throw new Exception("参数符号只能以26个字母开头,其后接字母和数字");
                //}

                if (nameList.Contains(pi.ParamName))
                {
                    throw new Exception(string.Format("参数名称:{0} 重复", pi.ParamName));
                }

                if (symbolList.Contains(pi.ParamSymbol))
                {
                    throw new Exception(string.Format("参数符号:{0} 重复", pi.ParamSymbol));
                }

                nameList.Add(pi.ParamName);
                symbolList.Add(pi.ParamSymbol);


            }
        }
    }
}