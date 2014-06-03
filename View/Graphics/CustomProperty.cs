using C1.WPF.C1Chart;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DamMVVM.View.Graphics
{
    public class CustomProperty
    {
        public DataSeries cds;

        public CustomProperty(DataSeries cds)
        {
            this.cds = cds;
        }


        [
            Description("设置线条的名称.")
        ]

        public string 线条名称
        {
            get
            {
                return cds.Label;
            }
            set
            {
                cds.Label = value;
            }
        }

        /// <summary>
        /// 设置线条的颜色
        /// </summary>
        [
            Description("设置线条的颜色.")
        ]

        public Brush 线条颜色
        {
            get
            {
                return cds.ConnectionStroke;
            }
            set
            {
                cds.ConnectionStroke = value;
            }
        }


        /// <summary>
        /// 设置线条的粗细
        /// </summary>
        /// 
        [
        Description("设置线条的粗细.")
        ]
        public double 线条粗细
        {
            get
            {
                return cds.ConnectionStrokeThickness;
            }
            set
            {
                cds.ConnectionStrokeThickness = value;
            }
        }

        [
              Description("设置线条是否是虚线.")
          ]

        public bool 线条是否是虚线
        {
            get
            {
                if (cds.ConnectionStrokeDashes == null||cds.ConnectionStrokeDashes.Count==0)
                {
                    cds.ConnectionStrokeDashes = new DoubleCollection(new double[] { 1, 0 });
                }

                if (cds.ConnectionStrokeDashes[1] == 1)
                {
                    return true;
                }else
                {
                    return false;
                }
            }
            set
            {
                if (cds.ConnectionStrokeDashes == null || cds.ConnectionStrokeDashes.Count == 0)
                {
                    cds.ConnectionStrokeDashes = new DoubleCollection(new double[] { 1, 0 });
                }

               if(value==true)
               {

                   cds.ConnectionStrokeDashes[0] = 1;
                   cds.ConnectionStrokeDashes[1] = 1;
               }else
               {
                   cds.ConnectionStrokeDashes[0] = 1;
                   cds.ConnectionStrokeDashes[1] = 0;
               }
            }
        }

        [
        Description("设置数据点形状.")
        ]
        public Marker 数据点形状
        {
            get
            {
                return cds.SymbolMarker;
            }
            set
            {
                cds.SymbolMarker = value;
            }
        }


        [
        Description("设置数据点大小.")
        ]
        public double 数据点大小
        {
            get
            {
                var val = cds.SymbolSize.Height;
                if(double.IsInfinity(val))
                {
                    cds.SymbolSize = new Size(1, 1);
                }

                return cds.SymbolSize.Height;
            }
            set
            {
                double val = 1;
                if(double.IsInfinity(value)==false)
                {
                    val = value;
                }
                cds.SymbolSize = new Size(val, val);
               
            }
        }


        [
        Description("设置数据点填充.")
        ]
        public Brush 数据点填充
        {
            get
            {
                return cds.SymbolFill;
            }
            set
            {
                cds.SymbolFill = value;
            }
        }


   


        [
        Description("设置数据点画笔.")
        ]
        public Brush 数据点画笔
        {
            get
            {
                return cds.SymbolStroke;
            }
            set
            {
                cds.SymbolStroke = value;
            }
        }





        [
        Description("设置数据点画笔粗细.")
        ]
        public double 数据点画笔粗细
        {
            get
            {
                return cds.SymbolStrokeThickness;
            }
            set
            {
                cds.SymbolStrokeThickness = value;
            }
        }
    }
}
