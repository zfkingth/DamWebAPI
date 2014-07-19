using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DamWebAPI.ViewModel;
using DamWebAPI.ViewModel.Graphics;
using C1.WPF.C1Chart;
using Hammergo.GlobalConfig;
using DevExpress.Xpf.Grid;
using DamServiceV3.Test.DamServiceRef;
using System.Collections.ObjectModel;
using System.IO;
using C1.WPF.C1Chart.Extended;



namespace DamWebAPI.View.Graphics
{
    /// <summary>
    /// GraphicsView.xaml 的交互逻辑
    /// </summary>
    public partial class GraphicsView : UserControl
    {
        public GraphicsView()
        {
            InitializeComponent();

            defRow.Height = new GridLength(PubConstant.ConfigData.GraphicHeight);
            defCol.Width = new GridLength(PubConstant.ConfigData.GraphicWidth);
        }


        private DamWebAPI.ViewModel.Graphics.GraphicsViewModel ViewModel
        {
            get
            {
                return this.DataContext as DamWebAPI.ViewModel.Graphics.GraphicsViewModel;
            }
        }

        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            //only for test

            var selapp = listBoxApps.SelectedItem as App;




            ViewModel.AddAppInDS(selapp);


        }



        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                App selApp = e.AddedItems[0] as App;
                DamWebAPI.ViewModel.Entity.Graphics graInfo = ViewModel.CreateNewGraphicDS(selApp);

                DrawGhpics(graInfo);

                tbExtreamInfo.Text = ViewModel.GetAppsInfo(graInfo) + getExtreamInfo();

                //只有显示一支仪器时才显示这条信息

                tbcaption.Text = string.Format("{0} {1} {2} {3}", selApp.AppName, selApp.X, selApp.Y, selApp.Z);




                propertyGrid.SelectedObject = new GraphicProperty(c1Chart, tby1, tby2, tbcaption);

            }
        }


        private void DrawGhpics(DamWebAPI.ViewModel.Entity.Graphics graInfo)
        {

            c1Chart.BeginUpdate();

            double dashPixels = ResetChart();

            //一次性查询所有的数据

            var results = from i in graInfo.Lines
                          where i.IsShow == true
                          group i by i.UnitSymbol;  //根据 物理量单位 判断是不是同一类量

            List<CalculateValue> allCalcValues = null;

            tby1.Text = tby2.Text = tbcaption.Text = "";

            int lineIndex = 0;
            //根据多少个Y轴进行循环
            for (int i = 0; i < results.Count(); i++)
            {
                if (allCalcValues == null)
                {
                    //避免多个枚举
                    allCalcValues = ViewModel.GetAllCalcValues(graInfo).ToList();
                }

                //有N个Y轴
                Axis yAxis = null;
                TextBlock tbTitle = null;
                if (i == 0)
                {
                    yAxis = c1Chart.View.AxisY;
                    yAxis.Name = "y1";

                    //set axis title
                    tbTitle = tby1;

                    yAxis.MajorGridStrokeDashes = new DoubleCollection(new double[] { dashPixels, dashPixels });


                }
                else
                {
                    //创建新的axis
                    yAxis = new Axis
                    {
                        AxisType = AxisType.Y,
                        Position = AxisPosition.Far,
                        Name = "y" + (i + 1).ToString(),
                    };

                    c1Chart.View.Axes.Add(yAxis);
                    tbTitle = tby2;

                    //hid grid lines
                    yAxis.MajorGridStroke = Brushes.White;
                    yAxis.MinorGridStroke = Brushes.White;

                }

                string unitSymbol = results.ElementAt(i).Key;

                var groupItems = results.ElementAt(i).ToList();

                //物理量名称
                string calcName = groupItems.First().ParamName;

                if (groupItems.Count > 1)
                {
                    //有多个相同单位的物理量
                    calcName = "单位";
                }

                tbTitle.Text = string.Format("{0} {1}: {2}", tbTitle.Text, calcName, unitSymbol);



                for (int gi = 0; gi < groupItems.Count; gi++)
                {




                    var item = groupItems.ElementAt(gi);
                    XYDataSeries ds = new XYDataSeries();
                    if (gi == 0)
                        ds.PlotElementLoaded += ds_PlotElementLoaded;

                    ds.Label = item.LegendName;
                    var valCollection = (from val in allCalcValues
                                         where val.ParamId == item.ParamId
                                         orderby val.Date
                                         select val);

                    ObservableCollection<CalculateValue> collection = new ObservableCollection<CalculateValue>(valCollection);
                    //消除异常值

                    HandleErrorValue(collection);

                    ds.ItemsSource = collection;

                    ds.XValueBinding = new Binding("Date.DateTime");
                    ds.ValueBinding = new Binding("Val");

                    //ds.XValuesSource = (from val in valCollection
                    //                    select val.Date.DateTime).ToArray();
                    //ds.ValuesSource = (from val in valCollection
                    //                   select val.Val).ToArray();

                    ds.AxisY = yAxis.Name;
                    //set line style
                    lineIndex = setLineStyle(lineIndex, ds);
                    c1Chart.Data.Children.Add(ds);
                }

            }
            c1Chart.EndUpdate();
        }

        void ds_PlotElementLoaded(object sender, EventArgs e)
        {
            if (divideYear)
                handleDraw();
        }


        private ChartPanel cp = new ChartPanel();


        double xcor1 = 0, ycor1 = 0, xcor2 = 0, ycor2 = 0, xcor_yMax = 0, ycor_yMax = 0;
        double[,] cors = null;
        int minYear, maxYear;
        protected void calcAxis()
        {
            //c1Chart1.ChartArea.AxisX.AnnoMethod = C1.Win.C1Chart.AnnotationMethodEnum.ValueLabels;



            double xMin = c1Chart.View.AxisX.ActualMin;
            double xMax = c1Chart.View.AxisX.ActualMax;
            double yMin = c1Chart.View.AxisY.ActualMin;
            double yMax = c1Chart.View.AxisY.ActualMax;

            var tempPoint = c1Chart.View.PointFromData(new Point(xMin, yMin));
            xcor1 = tempPoint.X;
            ycor1 = tempPoint.Y;

            tempPoint = c1Chart.View.PointFromData(new Point(xMax, yMin));
            xcor2 = tempPoint.X;
            ycor2 = tempPoint.Y;

            tempPoint = c1Chart.View.PointFromData(new Point(xMax, yMax));
            xcor_yMax = tempPoint.X;
            ycor_yMax = tempPoint.Y;

            //c1Chart1.ChartGroups[0].DataCoordToCoord(xMin, yMin, ref xcor1, ref ycor1);
            //c1Chart1.ChartGroups[0].DataCoordToCoord(xMax, yMin, ref xcor2, ref ycor2);
            //c1Chart1.ChartGroups[0].DataCoordToCoord(xMax, yMax, ref xcor_yMax, ref ycor_yMax);


            DateTime minDate = Hammergo.Utility.Helper.NumToDateTime(xMin);
            DateTime maxDate = Hammergo.Utility.Helper.NumToDateTime(xMax);

            minYear = int.Parse(minDate.ToString("yyyy"));

            maxYear = int.Parse(maxDate.ToString("yyyy"));

            cors = new double[maxYear - minYear + 1, 3];//第一列为起始的x坐标,第二列为结束的x的坐标,第三列为时间

            for (int i = 0; i < cors.GetLength(0); i++)
            {
                int year = minYear + i;
                DateTime sdate = new DateTime(year, 1, 1);
                DateTime edate = new DateTime(year + 1, 1, 1);


                double _snum = Hammergo.Utility.Helper.DateTimeToNum(sdate);
                double _enum = Hammergo.Utility.Helper.DateTimeToNum(edate);

                if (_snum <= xMin)
                {
                    _snum = xMin;
                }
                if (_enum >= xMax)
                {
                    _enum = xMax;
                }

                var tp = c1Chart.View.PointFromData(new Point(_snum, yMin));
                cors[i, 0] = tp.X;

                tp = c1Chart.View.PointFromData(new Point(_enum, yMin));
                cors[i, 1] = tp.X;

                //c1Chart1.ChartGroups[0].DataCoordToCoord(_snum, yMin, ref cors[i, 0], ref temp);
                //c1Chart1.ChartGroups[0].DataCoordToCoord(_enum, yMin, ref cors[i, 1], ref temp);
                cors[i, 2] = year;


            }
        }



        private double ResetChart()
        {

            // Clear current chart c1Chart.Reset(true);
            c1Chart.Reset(true);
            cp.Children.Clear();
            dataGrid.ItemsSource = null;

            c1Chart.ChartType = ChartType.Line;

            var xAxis = c1Chart.View.AxisX;
            double dashPixels = 3;

            xAxis.IsTime = true;
            xAxis.MajorGridStrokeDashes = new DoubleCollection(new double[] { dashPixels, dashPixels });
            return dashPixels;
        }

        private static void HandleErrorValue(IEnumerable<CalculateValue> valCollection)
        {
            foreach (var valItem in valCollection)
            {
                if (valItem.Val != null && valItem.Val.HasValue)
                {
                    if (Hammergo.Utility.Helper.isErrorValue(valItem.Val.Value))
                    {
                        valItem.Val = double.NaN;
                    }
                }
            }
        }

        private static int setLineStyle(int lineIndex, XYDataSeries ds)
        {
            if (lineIndex < PubConstant.ConfigData.LineStyleInfoList.Count)
            {
                var lineInfo = Hammergo.GlobalConfig.PubConstant.ConfigData.LineStyleInfoList[lineIndex];
                ds.ConnectionStroke = lineInfo.ConnectionStroke;
                ds.ConnectionStrokeThickness = lineInfo.LineThickness;
                ds.ConnectionStrokeDashes = lineInfo.ConnectionStrokeDashes;
                ds.SymbolMarker = lineInfo.SymbolMarker;
                ds.SymbolSize = lineInfo.SymbolSize;
                ds.SymbolFill = lineInfo.SymbolFill;
                ds.SymbolStroke = lineInfo.SymbolStroke;
                ds.SymbolStrokeThickness = lineInfo.SymbolStrokeThickness;
                lineIndex++;
            }
            return lineIndex;
        }


        #region handle mouse on chart
        ToolTip m_toolTip = null;
        private void c1Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_toolTip == null)
            {
                m_toolTip = new ToolTip();


            }


            bool find = false;

            double dist;
            int si = 0;
            int index = -1;
            Point p = e.GetPosition(sender as IInputElement);
            for (; si < c1Chart.Data.Children.Count; si++)
            {

                index = c1Chart.View.DataIndexFromPoint(p, si, MeasureOption.XY, out dist);
                if (index >= 0 && dist < 3)
                {
                    //找到了附近的点
                    find = true;
                    break;
                }
            }

            if (find)
            {
                Cursor = Cursors.Cross;
                var ds = c1Chart.Data.Children[si] as XYDataSeries;


                IEnumerable<CalculateValue> source = ds.ItemsSource as IEnumerable<CalculateValue>;

                if (source != null && source.Count() > 0)
                {
                    var val = source.ElementAt(index);
                    string info = string.Format("{0}:{1} 日期:{2}", ds.Label, val.Val, (val.Date.DateTime).ToString("yyyy-MM-dd"));

                    m_toolTip.Content = new TextBlock { Text = info, TextWrapping = TextWrapping.Wrap };
                    m_toolTip.PlacementTarget = c1Chart;
                    //Point p = AvalonEdit.TextArea.TextView.GetVisualPosition(AvalonEdit.TextArea.Caret.Position, VisualYPosition.LineBottom);
                    m_toolTip.PlacementRectangle = new Rect(p.X + 10, p.Y + 10, 0, 0);
                    m_toolTip.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    m_toolTip.IsOpen = true;
                }

            }
            else
            {
                Cursor = Cursors.Arrow;
                m_toolTip.IsOpen = false;
            }

        }



        private void c1Chart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                bool find = false;

                double dist;
                int si = 0;
                int index = -1;
                Point p = e.GetPosition(sender as IInputElement);
                for (; si < c1Chart.Data.Children.Count; si++)
                {

                    index = c1Chart.View.DataIndexFromPoint(p, si, MeasureOption.XY, out dist);
                    if (index >= 0 && dist < 3)
                    {
                        //找到了附近的点
                        find = true;
                        break;
                    }
                }

                if (find)
                {
                    var serial = c1Chart.Data.Children[si];
                    propertyGrid.SelectedObject = new CustomProperty(serial);
                    //设置相应的数据

                    dataGrid.ItemsSource = serial.ItemsSource;


                    var ds = c1Chart.Data.Children[si] as XYDataSeries;


                    IEnumerable<CalculateValue> source = serial.ItemsSource as IEnumerable<CalculateValue>;

                    if (source != null && source.Count() > 0)
                    {

                        var val = source.ElementAt(index);
                        dataGrid.CurrentItem = val;
                    }
                }

            }

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int cha = c1Chart.Data.Children.Count - PubConstant.ConfigData.LineStyleInfoList.Count;

            if (cha > 0)
            {

                for (int i = 0; i < cha; i++)
                {
                    PubConstant.ConfigData.LineStyleInfoList.Add(new LineStyleInfo());
                }
            }

            for (int i = 0; i < c1Chart.Data.Children.Count; i++)
            {
                DataSeries ds = c1Chart.Data.Children[i];
                LineStyleInfo lineStyle = PubConstant.ConfigData.LineStyleInfoList[i];

                lineStyle.ID = i;
                lineStyle.ConnectionStroke = ds.ConnectionStroke;
                lineStyle.LineThickness = ds.ConnectionStrokeThickness;
                lineStyle.ConnectionStrokeDashes = ds.ConnectionStrokeDashes;

                lineStyle.SymbolMarker = ds.SymbolMarker;

                if (double.IsInfinity(ds.SymbolSize.Width))
                {
                    ds.SymbolSize = new Size(0, 0);
                }

                lineStyle.SymbolSize = ds.SymbolSize;
                lineStyle.SymbolFill = ds.SymbolFill;
                lineStyle.SymbolStroke = ds.SymbolStroke;
                lineStyle.SymbolStrokeThickness = ds.SymbolStrokeThickness;




            }
            //保存图形的大小
            PubConstant.ConfigData.GraphicHeight = defRow.ActualHeight;
            PubConstant.ConfigData.GraphicWidth = defCol.ActualWidth;


            PubConstant.updateConfigData();
        }

        #endregion

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            propertyGrid.SelectedObject = new GraphicProperty(c1Chart, tby1, tby2, tbcaption);

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            //lineGrid.SetBinding(GridControl.ItemsSourceProperty, new Binding("GraphicDS.Lines") { Source = this.DataContext });
            c1Chart.View.AxisX.AnnoCreated += AxisX_AnnoCreated;
        }

        void AxisX_AnnoCreated(object sender, AnnoCreatedEventArgs e)
        {
            if (divideYear)
            {
                //hide auto label
                var txtblock = e.Label as TextBlock;
                txtblock.Foreground = e.Canvas.Background;
            }


        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DrawGhpics(ViewModel.GraphicDS);
            tbExtreamInfo.Text = ViewModel.GetAppsInfo(ViewModel.GraphicDS) + getExtreamInfo();

            propertyGrid.SelectedObject = new GraphicProperty(c1Chart, tby1, tby2, tbcaption);
        }




        private void Button_Click_5(object sender, RoutedEventArgs e)
        {

            CalculateValue val = dataGrid.CurrentItem as CalculateValue;

            ObservableCollection<CalculateValue> collection = dataGrid.ItemsSource as ObservableCollection<CalculateValue>;
            collection.Remove(val);

        }




        private void handleDraw()
        {
            if (c1Chart.Data.Children.Count > 0)
            {

                clearMajorGrid();

                cp.Children.Clear();



                var obj = new ChartPanelObject();


                Size stringSize = Hammergo.Utility.Helper.MeasureTextSize("1983", c1Chart.FontFamily, c1Chart.FontStyle, c1Chart.FontWeight, c1Chart.FontStretch, c1Chart.FontSize);
                double stringWidth = stringSize.Width;

                //计算坐标
                calcAxis();

                var rect = c1Chart.View.AxisX.GetAxisRect();

                Canvas canvas = new Canvas();
                canvas.Width = rect.Width;
                canvas.Height = rect.Height;
                //Canvas.SetLeft(canvas, 0);
                ////矩形对象相对于父容器对象Canvas的位置，左边距、上边距
                //Canvas.SetTop(canvas, 0);

                obj.Content = canvas;

                Rectangle rectangle = new Rectangle();//矩形对象
                //属性设置，填充颜色、边粗细、边颜色、宽、高等
                rectangle.StrokeThickness = c1Chart.View.AxisX.AxisLine.StrokeThickness;
                rectangle.Stroke = c1Chart.View.AxisX.AxisLine.Stroke;

                rectangle.Width = rect.Width + 2 * c1Chart.View.AxisY.AxisLine.StrokeThickness;
                rectangle.Height = rect.Height;
                //矩形对象相对于父容器对象Canvas的位置，左边距、上边距
                Canvas.SetLeft(rectangle, -1);
                Canvas.SetTop(rectangle, 0);

                canvas.Children.Add(rectangle);


                //draw line 
                for (int i = 0; i < cors.GetLength(0); i++)
                {
                    //第一个和最后一个竖线不画
                    if (i > 0 && i < cors.GetLength(0) - 1)
                    {
                        Line line = new Line();
                        line.Stroke = c1Chart.View.AxisX.AxisLine.Stroke;
                        line.StrokeThickness = c1Chart.View.AxisX.AxisLine.StrokeThickness;

                        //相对于canvas的坐标

                        line.X1 = cors[i, 0] - rect.Left;

                        line.Y1 = 0;
                        line.X2 = line.X1;
                        line.Y2 = line.Y1 + rect.Height;

                        //Canvas.SetLeft(line, 0);
                        //Canvas.SetTop(line, 0);

                        canvas.Children.Add(line);

                        Line dashLine = new Line();
                        dashLine.Stroke = Brushes.Gray;
                        dashLine.StrokeThickness = 1;
                        dashLine.StrokeDashArray = new DoubleCollection(new double[] { 2, 2 });

                        dashLine.X1 = line.X1;
                        dashLine.Y1 = line.Y1;
                        dashLine.X2 = line.X2;
                        dashLine.Y2 = -c1Chart.View.AxisY.GetAxisRect().Height;
                        canvas.Children.Add(dashLine);

                    }


                    //显示年份
                    double distance = cors[i, 1] - cors[i, 0];
                    if (distance >= stringWidth)
                    {

                        //相对于canvas的坐标系
                        double startXCor = cors[i, 0] + (distance - stringWidth) / 2.0f - rect.Left;
                        double startYCor = (rect.Height - stringSize.Height) / 2.0f;

                        TextBlock tb = new TextBlock();
                        tb.Text = (minYear + i).ToString();

                        Canvas.SetTop(tb, startYCor);
                        Canvas.SetLeft(tb, startXCor);

                        canvas.Children.Add(tb);


                        //ghs.DrawString((minYear + i).ToString(), smallFont, Brushes.Black, new PointF(startXCor, startYCor));
                    }
                }






                obj.Action = ChartPanelAction.None;

                cp.Children.Add(obj);






                if (c1Chart.View.Layers.Contains(cp) == false)
                    c1Chart.View.Layers.Add(cp);



                cp.SetValue(Canvas.LeftProperty, rect.Left);
                cp.SetValue(Canvas.TopProperty, rect.Top);

                cp.Height = rect.Height;
                cp.Width = rect.Width;

            }

        }

        private void clearMajorGrid()
        {
            c1Chart.View.AxisX.MajorGridFill = null;
            c1Chart.View.AxisX.MajorGridStroke = null;
            c1Chart.View.AxisX.MajorGridStrokeThickness = 0;
            c1Chart.View.AxisX.MajorTickHeight = 0;
            c1Chart.View.AxisX.MajorTickOverlap = 0;
            c1Chart.View.AxisX.MajorTickThickness = 0;
        }

        bool divideYear = false;
        private void chkYear_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            divideYear = cb.IsChecked.Value;
        }

        private void chkYear_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            divideYear = cb.IsChecked.Value;
        }

        private void MenuItem_Click_bmp(object sender, RoutedEventArgs e)
        {

            //MemoryStream ms = new MemoryStream();
            //c1Chart.SaveImage(ms, ImageFormat.Bmp);

            //var data = new DataObject(DataFormats.Bitmap, ms);
            //Clipboard.Clear();
            //Clipboard.SetDataObject(data, true);
            CopyUIElementToClipboard(c1Chart);
        }

        /// <summary> 
        /// Copies a UI element to the clipboard as an image, and as text. 
        /// </summary> 
        /// <param name="element">The element to copy.</param> 
        public static void CopyUIElementToClipboard(FrameworkElement element)
        {
            //data object to hold our different formats representing the element
            DataObject dataObject = new DataObject();
            //lets start with the text representation 
            //to make is easy we will just assume the object set as the DataContext has the ToString method overrideen and we use that as the text
            // dataObject.SetData(DataFormats.Text, element.DataContext.ToString(), true);

            //now lets do the image representation 
            double width = element.ActualWidth;
            double height = element.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(dv);
            dataObject.SetData(DataFormats.Bitmap, bmpCopied, true);

            //now place our object in the clipboard 
            Clipboard.SetDataObject(dataObject, true);
        }


        private void MenuItem_Click_png(object sender, RoutedEventArgs e)
        {
            MemoryStream ms = new MemoryStream();
            c1Chart.SaveImage(ms, ImageFormat.Png);

            var data = new DataObject("PNG", ms);
            Clipboard.Clear();
            Clipboard.SetDataObject(data, true);

        }



        private string getExtreamInfo()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var ds in c1Chart.Data.Children)
            {
                ObservableCollection<CalculateValue> oc = ds.ItemsSource as ObservableCollection<CalculateValue>;

                var qmax = (from i in oc
                            select i.Val).Max();
                var qmin = (from i in oc
                            select i.Val).Min();
                var dateMax =( from i in oc
                              where i.Val == qmax
                              select i.Date).FirstOrDefault();
                var dateMin =( from i in oc
                              where i.Val == qmin
                              select i.Date).FirstOrDefault();

                sb.Append(ds.Label).Append(":");
                sb.Append(" 最小值：").Append(qmin).Append(" 日期：").Append(dateMin);
                sb.Append(" 最大值：").Append(qmax).Append(" 日期：").Append(dateMax);
                sb.Append("\n");

            }

            return sb.ToString();
        }





    }
}
