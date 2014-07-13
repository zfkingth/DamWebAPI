using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Hammergo.GlobalConfig
{

    public class GlobalConfigData
    {

        string _serviceURI;
        [Description("连接字符串,请不要随便修改")]
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ServiceURI
        {
            get
            {
                return _serviceURI;
            }
            set
            {
                _serviceURI = value;
            }
        }

        List<ParamInfo> _constParamsList = null;
        [Description("常量参数列表，供设置参数时的预定选项")]
        /// <summary>
        /// 常量参数列表
        /// </summary>
        public List<ParamInfo> ConstParamsList
        {
            get
            {
                return _constParamsList;
            }

            set
            {
                _constParamsList = value;

            }
        }


        List<ParamInfo> _defaultParamsList = null;
        [Description("测量参数,计算参数列表,其设定的参数小数位数决定报表的的小数位数")]
        /// <summary>
        /// 测量参数,计算参数列表
        /// </summary>
        public List<ParamInfo> DefaultParamsList
        {
            get
            {
                return _defaultParamsList;
            }

            set
            {
                _defaultParamsList = value;

            }
        }

        List<double> _errorValList = null;
        [Description("异常值列表")]
        /// <summary>
        /// 异常值列表
        /// </summary>
        public List<double> ErrorValList
        {
            get
            {
                return _errorValList;
            }

            set
            {
                _errorValList = value;

            }
        }


        List<LineStyleInfo> _lineStyleInfoList = null;
        [Description("图形线条相关属性对象列表,可在图形窗体中设置并保存")]
        /// <summary>
        /// 图形线条相关属性对象列表
        /// </summary>
        public List<LineStyleInfo> LineStyleInfoList
        {
            get
            {
                return _lineStyleInfoList;
            }

            set
            {
                _lineStyleInfoList = value;

            }
        }


        private string _noDataConvertString;
        [Description("数据为空时报表中的转换字符串")]
        /// <summary>
        /// 数据为空时报表中的转换字符串
        /// </summary>
        public string NoDataConvertString
        {
            get
            {
                return _noDataConvertString;
            }
            set
            {
                _noDataConvertString = value;
            }
        }


        private string _errorConvertString;
        [Description("数据属于异常值时,报表中的转换字符串")]
        /// <summary>
        /// 数据属于异常值时,报表中的转换字符串
        /// </summary>
        public string ErrorConvertString
        {
            get
            {
                return _errorConvertString;
            }
            set
            {
                _errorConvertString = value;
            }
        }


        private int _lastedRecordNum=15;

        [Description("仪器原始数据中显示最近数据的个数")]
        /// <summary>
        /// 仪器原始数据中显示最近数据的个数
        /// </summary>
        public int LastedRecordNum
        {
            get
            {
                return _lastedRecordNum;
            }
            set
            {
                _lastedRecordNum = value;
            }
        }

        private int _changeRangeNumForMonth = 4;

        [Description("月变化日期上下浮动的天数")]
        /// <summary>
        /// 月变化日期上下浮动的天数
        /// </summary>
        public int  ChangeRangeNumForMonth
        {
            get
            {
                return _changeRangeNumForMonth;
            }
            set
            {
                _changeRangeNumForMonth = value;
            }
        }


        private int _changeRangeNumForYear = 15;

        [Description("年变化日期上下浮动的天数")]
        /// <summary>
        /// 年变化日期上下浮动的天数
        /// </summary>
        public int ChangeRangeNumForYear
        {
            get
            {
                return _changeRangeNumForYear;
            }
            set
            {
                _changeRangeNumForYear = value;
            }
        }

        private double _daysNumForNear = 3;

        [Description("搜索数据时与指定天数的可浮动天数，可以是小数")]
        /// <summary>
        /// 搜索数据时与指定天数的可浮动天数，可以是小数
        /// </summary>
        public double DaysNumForNear
        {
            get
            {
                return _daysNumForNear;
            }
            set
            {
                _daysNumForNear = value;
            }
        }


        private double _graphicWidth = 700;
        [Description("图形宽度")]
        /// <summary>
        ///图形右边填充空白区域的宽度
        /// </summary>
        public double GraphicWidth
        {
            get
            {
                return _graphicWidth;
            }
            set
            {
                _graphicWidth = value;
            }
        }

        private double _graphicHeight= 260;
        [Description("图形高度")]
        /// <summary>
        ///图形底部填充空白区域的高度
        /// </summary>
        public double GraphicHeight
        {
            get
            {
                return _graphicHeight;
            }
            set
            {
                _graphicHeight = value;
            }
        }


        string _skinName;
        [Description("当前程序使用的皮肤名称")]
        /// <summary>
        /// 当前程序使用的皮肤名称
        /// </summary>
        public string SkinName
        {
            get
            {
                return _skinName;
            }
            set
            {
                _skinName = value;
            }
        }

        private double _checkTimes = 1.3;

        [Description("在批量输入数据时，检查变化量时是否有提示的临界倍数，该值乘以前几次数据的最大变化量就是新数据变化的临界值")]
        /// <summary>
        /// 在批量输入数据时，检查变化量时是否有提示的临界倍数，该值乘以前几次数据的最大变化量就是新数据变化的临界值
        /// </summary>
        public double CheckTimes
        {
            get
            {
                return _checkTimes;
            }
            set
            {
                _checkTimes = value;
            }
        }


        string _registerCode;
        [Description("注册码")]
        /// <summary>
        /// 当前程序使用的皮肤名称
        /// </summary>
        public string RegisterCode
        {
            get
            {
                return _registerCode;
            }
            set
            {
                _registerCode = value;
            }
        }


    }
}
