using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;

namespace hammergo.GlobalConfig
{
    [XmlInclude(typeof(System.Windows.Media.SolidColorBrush))]
    [XmlInclude(typeof(System.Windows.Media.MatrixTransform))]
    [Serializable]
    public class LineStyleInfo
    {
        private int id;

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        private string lineStyle;

        public string LineStyle
        {
            get
            {
                return lineStyle;
            }
            set
            {
                lineStyle = value;
            }
        }

 

        private double lineThickness;

        public double LineThickness
        {
            get
            {
                return lineThickness;
            }
            set
            {
                lineThickness = value;
            }
        }





        private C1.WPF.C1Chart.Marker symbolMarker;

        public C1.WPF.C1Chart.Marker SymbolMarker
        {
            get
            {
                return symbolMarker;
            }
            set
            {
                symbolMarker = value;
            }
        }



  


        private System.Windows.Size symbolSize;

        public System.Windows.Size SymbolSize
        {
            get
            {
                return symbolSize;
            }
            set
            {
                symbolSize = value;
            }
        }



        private Brush symbolFill;
        public Brush SymbolFill
        {
            get
            {
                return symbolFill;
            }
            set
            {
                symbolFill = value;
            }
        }

        private Brush symbolStroke;
        public Brush SymbolStroke
        {
            get { return symbolStroke; }
            set { symbolStroke = value; }
        }

        double symbolStrokeThickness;
        public double SymbolStrokeThickness
        {
            get { return symbolStrokeThickness; }
            set { symbolStrokeThickness = value; }
        }

        //private  DoubleCollection symbolStrokeDashes;
        //public DoubleCollection SymbolStrokeDashes { 
        //    get{
        //        return symbolStrokeDashes;
        //    }
        //    set
        //    {
        //        symbolStrokeDashes=value;
        //    }
        //}

        public Brush ConnectionStroke { get; set; }


        public DoubleCollection ConnectionStrokeDashes { get; set; }
    }
}
