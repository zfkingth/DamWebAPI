using C1.WPF.C1Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DamWebAPI.View.Graphics
{
    class GraphicProperty
    {
        /// <summary>
        /// 
        /// </summary>
        public C1Chart chart = null;
        TextBlock tby1 = null;
        TextBlock tby2;
        TextBlock tbCaption = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        public GraphicProperty(C1Chart chart, TextBlock tby1, TextBlock tby2, TextBlock tbCaption)
        {
            this.chart = chart;

            this.tby1 = tby1;
            this.tby2 = tby2;
            this.tbCaption = tbCaption;

            y1 = chart.View.AxisY;
            y2 = chart.View.Axes["y2"];
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        Axis y1 = null;
        Axis y2 = null;

        public double 主轴最大值
        {

            get
            {
                return y1.Max;
            }
            set
            {

                y1.Max = value;
            }
        }

        public double 主轴最小值
        {
            get
            {
                return y1.Min;
            }
            set
            {

                y1.Min = value;
            }

        }


        public double 主轴间距
        {
            get
            {
                return y1.MajorUnit;
            }
            set
            {
                y1.MajorUnit = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public double 副轴间距
        {
            get
            {
                return y2.MajorUnit;
            }
            set
            {
                y2.MajorUnit = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string 主轴标注
        {

            get
            {
                return tby1.Text;
            }
            set
            {

                tby1.Text = value;

            }
        }

        public string 副轴标注
        {
            get
            {
                return tby2.Text;
            }
            set
            {

                tby2.Text = value;

            }
        }


        public double 副轴最大值
        {

            get
            {
                return y2.Max;
            }
            set
            {

                y2.Max = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double 副轴最小值
        {
            get
            {
                return y2.Min;
            }
            set
            {

                y2.Min = value;
            }

        }



        public string 标题
        {
            get
            {
                return tbCaption.Text;
            }
            set
            {

                tbCaption.Text = value;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string X轴格式
        {
            get
            {
                return chart.View.AxisX.AnnoFormat;
            }
            set
            {
                chart.View.AxisX.AnnoFormat = value;
            }
        }


        public double 文本大小
        {
            get
            {
                return chart.FontSize;
            }
            set
            {

                chart.FontSize = value;
                

            }
        }



    }
}
