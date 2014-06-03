using DamWebAPI.ViewModel;
using DevExpress.Xpf.Core;
using GalaSoft.MvvmLight.Messaging;
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
using System.Xaml;

namespace DamWebAPI.View
{
    /// <summary>
    /// ProjectPartView.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectPartView : UserControl
    {
        public ProjectPartView()
        {
            InitializeComponent();
        }


   

        private void tree_Loaded(object sender, RoutedEventArgs e)
        {
            if (tree.Items != null && tree.Items.Count != 0)
            {

                TreeViewItem container = tree.ItemContainerGenerator.ContainerFromItem(tree.Items[0]) as TreeViewItem;

                if (container != null)
                {

                    container.IsExpanded = true;

                    if (container.ItemContainerGenerator.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                    {

                        container.UpdateLayout();

                    }
                }
            }
        }

        TreeViewItem _sourceItem, _targetItem;





        public bool AllowDropPart
        {
            get { return (bool)GetValue(AllowDropPartProperty); }
            set { SetValue(AllowDropPartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowDropPart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowDropPartProperty =
            DependencyProperty.Register("AllowDropPart", typeof(bool), typeof(ProjectPartView), new PropertyMetadata(false));

        

   

        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (AllowDropPart&&e.LeftButton == MouseButtonState.Pressed && _sourceItem == null)
                {
                     _sourceItem = GetNearestContainer(e.OriginalSource as UIElement);
                     if (_sourceItem != null)
                     {
                         DataObject data = new DataObject();
                         data.SetData("data", tree.SelectedValue);
                         DragDrop.DoDragDrop(tree, data, DragDropEffects.Move);
                     }
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send<Exception>(ex);
            }
        }
        private void treeView_Drop(object sender, DragEventArgs e)
        {
            if (AllowDropPart)
            {
                try
                {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;

                    // Verify that this is a valid drop and then store the drop target
                    _targetItem = GetNearestContainer(e.OriginalSource as UIElement);
                    object sourceData = e.Data.GetData("data");
                    if (sourceData is ProjectPartViewModel)
                    {
                        if (_targetItem != _sourceItem && _sourceItem != null && _targetItem != null)
                        {

                            if (MessageBox.Show("是否确定将节点 " + _sourceItem.Header.ToString() + " 移动到节点 " + _targetItem.Header.ToString() + " 下面吗?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                Hammergo.Utility.Helper.InvodeCmd(this.DataContext, "CmdMovePart", new object[] { _sourceItem.DataContext, _targetItem.DataContext });
                            }

                            // e.Effects = DragDropEffects.Move;

                        }
                    }
                    else if (sourceData is System.Collections.ICollection)
                    {


                        if (MessageBox.Show("是否确定将所选测点 " + " 关联到节点 " + _targetItem.Header.ToString() + " 吗?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Hammergo.Utility.Helper.InvodeCmd(this.DataContext, "CmdAttachPart", new object[] { sourceData, _targetItem.DataContext });
                        }
                    }


                }
                catch (Exception ex)
                {

                    Messenger.Default.Send<Exception>(ex);
                }
                finally
                {
                    _sourceItem = _targetItem = null;
                }

            }

        }


        private void CopyItem(object _sourceItem, object _targetItem)
        {

            ////Asking user wether he want to drop the dragged TreeViewItem here or not
            //if (MessageBox.Show("Would you like to drop " + _sourceItem.Header.ToString() + " into " + _targetItem.Header.ToString() + "", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
            //    try
            //    {
            //        //adding dragged TreeViewItem in target TreeViewItem
            //        addChild(_sourceItem, _targetItem);

            //        //finding Parent TreeViewItem of dragged TreeViewItem 
            //        TreeViewItem ParentItem = FindVisualParent<TreeViewItem>(_sourceItem);
            //        // if parent is null then remove from TreeView else remove from Parent TreeViewItem
            //        if (ParentItem == null)
            //        {
            //            tree.Items.Remove(_sourceItem);
            //        }
            //        else
            //        {                        
            //            ParentItem.Items.Remove(_sourceItem);
            //        }
            //    }
            //    catch
            //    {

            //    }
            //}

        }
        public void addChild(object _sourceItem,object _targetItem)
        {
            //// add item in target TreeViewItem 
            //TreeViewItem item1 = new TreeViewItem();
            //item1.Header = _sourceItem.Header;
            //_targetItem.Items.Add(item1);    
            //foreach (TreeViewItem item in _sourceItem.Items)
            //{
            //    addChild(item, item1);               
            //}
        }
        static TObject FindVisualParent<TObject>(UIElement child) where TObject : UIElement
        {
            if (child == null)
            {
                return null;
            }

            UIElement parent = VisualTreeHelper.GetParent(child) as UIElement;

            while (parent != null)
            {
                TObject found = parent as TObject;
                if (found != null)
                {
                    return found;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }
            }

            return null;
        }


        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }



    }
    
    
}
