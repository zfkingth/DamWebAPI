using System;
using System.Collections.Generic;
using System.Text;

namespace hammergo.GlobalConfig
{
    public class ParamInfo
    {
        string name, unitSymbol, calcSymbol;
        byte precision = 0;


        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// 数据计量单位的符号
        /// </summary>
        public string UnitSymbol
        {
            get
            {
                return unitSymbol;
            }
            set
            {
                unitSymbol = value;
            }
        }

        /// <summary>
        /// 参与求值公式的符号
        /// </summary>
        public string CalcSymbol
        {
            get
            {
                return calcSymbol;
            }
            set
            {
                calcSymbol = value;
            }
        }

        /// <summary>
        /// 精度,表示保留多少位小数
        /// </summary>
        public byte Precision
        {
            get
            {
                return precision;
            }
            set
            {
                precision = value;
            }
        }



        public override string ToString()
        {
            return Name;
        }

    }
}
