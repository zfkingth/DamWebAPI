using DevExpress.Xpf.Editors;
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
using DamWebAPI.ViewModel.AppManage;

using DamServiceV3.Test.DamServiceRef;

namespace DamWebAPI.View
{
    /// <summary>
    /// AppParamsView.xaml 的交互逻辑
    /// </summary>
    public partial class AppParamsView : UserControl
    {
        public AppParamsView()
        {
            InitializeComponent();
        }

        private void GridControl_CustomUnboundColumnData(object sender, DevExpress.Xpf.Grid.GridColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Formulae")
            {
                var id = (Guid)e.GetListSourceFieldValue("ParamId");
                var context = this.DataContext as  AppParamsViewModel;
                DateTime date = (DateTime)comboDates.SelectedValue;

                if (e.IsGetData)
                {
                    e.Value = context.GetFormulaString(id, date);
                }
                if (e.IsSetData)
                {
                    context.SetFormulaString(id, date, e.Value as string);
                }
            }
        }

        List<hammergo.GlobalConfig.ParamInfo> tempParamList = new List<hammergo.GlobalConfig.ParamInfo>();

        private void PART_Editor_ProcessNewValue(DependencyObject sender, DevExpress.Xpf.Editors.ProcessNewValueEventArgs e)
        {
            ComboBoxEdit ic = sender as ComboBoxEdit;
            var list = ic.ItemsSource as List<hammergo.GlobalConfig.ParamInfo>;

            hammergo.GlobalConfig.ParamInfo np = new hammergo.GlobalConfig.ParamInfo();
            np.Name = e.DisplayText;


            list.Add(np);

            tempParamList.Add(np);

            e.Handled = true;
        }

        private void PART_Editor_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            setParamValue(sender, constGrid);

            e.Handled = true;
        }

        private void PART_Editor_EditValueChanged2(object sender, EditValueChangedEventArgs e)
        {

            setParamValue(sender, mesGrid);


            e.Handled = true;
        }

        private void setParamValue(object sender, DevExpress.Xpf.Grid.GridControl grid)
        {
            var cb = sender as ComboBoxEdit;
            var selParam = cb.SelectedItem as hammergo.GlobalConfig.ParamInfo;
            var cp = grid.GetFocusedRow() as AppParam ;


            if (selParam != null)
            {
                cp.ParamName = selParam.Name;
                if (tempParamList.Contains(selParam) == false)
                {
                    cp.UnitSymbol = selParam.UnitSymbol;
                    cp.PrecisionNum = selParam.Precision;
                    cp.ParamSymbol = selParam.CalcSymbol;
                }
            }

        }


        private void PART_Editor_EditValueChanged3(object sender, EditValueChangedEventArgs e)
        {
            setParamValue(sender, calcGrid);

            e.Handled = true;
        }

        private void constGrid_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {
            Hammergo.Utility.Helper.InvodeCmd(this.DataContext, "CmdSelectedItemChangedConst", e.NewItem);
        }

        private void mesGrid_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {

            Hammergo.Utility.Helper.InvodeCmd(this.DataContext, "CmdSelectedItemChangedMes", e.NewItem);
        }

        private void calcGrid_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {

            Hammergo.Utility.Helper.InvodeCmd(this.DataContext, "CmdSelectedItemChangedCalc", e.NewItem);
        }

        private void comboDates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selval = (sender as ComboBox).SelectedValue;
            if (selval != null)
            {
                DateTime date = (DateTime)selval;
                Hammergo.Utility.Helper.InvodeCmd(this.DataContext, "CmdSelectedItemChangedDate", date);
                calcGrid.RefreshData();
            }
        }





    }


}
