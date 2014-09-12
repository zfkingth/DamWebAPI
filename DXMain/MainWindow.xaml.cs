using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Ribbon;
using System.ComponentModel;
using GalaSoft.MvvmLight.Messaging;
using System.IO;
using System.Xml.Linq;
using DevExpress.Xpf.Core;
using DamWebAPI.View;
using DamWebAPI.ViewModel;
using DamWebAPI.ViewModel.AppManage;
using DamWebAPI.View.AppManage;
using DamWebAPI.ViewModel.Graphics;
using DamWebAPI.View.Graphics;

namespace DXMain
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DXRibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                Messenger.Default.Register<Exception>(this, m => ShowMessage(m));

                Messenger.Default.Register<NotificationMessageAction<string>>(this, msg => GetInput(msg));

                Messenger.Default.Register<NotificationMessageAction<DateTime>>(this, msg => GetInputDate(msg));

                Messenger.Default.Register<NotificationMessageAction<List<string>>>(this, msg => GetInputStrings(msg));

                Messenger.Default.Register<DialogMessage>(this,m=>Getchoose(m));

                Messenger.Default.Register<NotificationMessageAction<System.IO.DirectoryInfo>>(this, msg => GetDirectory(msg));

            }
        }

        private void GetDirectory(NotificationMessageAction<DirectoryInfo> msg)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var result = new System.IO.DirectoryInfo(folderBrowser.SelectedPath);

                msg.Execute(result);
            }
        }

        

        private void GetInputStrings(NotificationMessageAction<List<string>> msg)
        {
            object[] vals = new object[] { msg.Notification, null };
            var win = new InputAppNamesWindow(vals);
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.ShowDialog();

            if (vals[1] != null)
            {
                msg.Execute(vals[1]);
            }
        }

        private void GetInputDate(NotificationMessageAction<DateTime> msg)
        {
            object[] vals = new object[] { msg.Notification, null };
            var win = new InputDateWindow(vals);
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.ShowDialog();

            if (vals[1] != null)
            {
                msg.Execute(vals[1]);
            }
        }

        private void Getchoose(DialogMessage dlgmsg)
        {
            var res = MessageBox.Show(dlgmsg.Content, dlgmsg.Caption, dlgmsg.Button, dlgmsg.Icon);

            dlgmsg.ProcessCallback(res);
        }

        private void GetInput(NotificationMessageAction<string> msg)
        {

            string[] vals = new string[] { msg.Notification, null };
            var win = new InputWindow(vals);
            win.Owner = this;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            win.ShowDialog();

            if (vals[1] != null)
            {
                msg.Execute(vals[1]);
            }

         //   msg.Execute(MessageBox.Show("选择", "问题", MessageBoxButton.YesNo, MessageBoxImage.Question).ToString());



        }

        private void ShowMessage(Exception ex)
        {
            string message = ex.Message;
            //if (ex is System.Data.Services.Client.DataServiceRequestException)
            //{
            if (ex.InnerException != null)
            {
                var sr = new StringReader(ex.InnerException.Message);
                XElement root = XElement.Load(sr);
                IEnumerable<XElement> mesElements =
                  from el in root.Elements()
                  where el.Name.LocalName == "innererror"
                  select el;
                var mesElements2 =
                    from el in mesElements.Elements()
                    where el.Name.LocalName == "message"
                    select el;
                message = "";
                foreach (XElement el in mesElements2)
                    message += el.Value;
            }
            //}

            MessageBox.Show(this, message,"提示",MessageBoxButton.OK,MessageBoxImage.Asterisk);

            //  MessageBox.Show(this, this,message);
        }

        private void tabControl1_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var context = this.DataContext as MainWindowViewModel;
            if(context!=null)
            {
                context.EventBeforeWorkspaceRemove += context_EventBeforeWorkspaceRemove;
                context.EventWorkspaceAdded += context_EventWorkspaceAdded;
                context.EventWorkspaceActived += context_EventWorkspaceActived;
            }
        }

        void context_EventWorkspaceActived(object sender, WorkspaceViewModel e)
        {
            foreach (TabItem item in tabControl1.Items)
            {
                if (item.DataContext == e)
                {
                    tabControl1.SelectedItem = item;
                    break;
                }
            }
        }

        void context_EventWorkspaceAdded(object sender, WorkspaceViewModel e)
        {
            Control control = null;

            Type type = e.GetType();
            if(type==typeof(AllAppManageViewModel))
            {
                control = new AllAppManageView();
            }else if(type==typeof(AppParamsViewModel))
            {
                control = new AppParamsView();
            }else if(type==typeof(CreateAppViewModel))
            {
                control = new CreateAppView();
            }else if(type==typeof(AppSearchViewModel))
            {
                control = new AppSearchView();
            }else if (type==typeof(AppDataViewModel))
            {
                control = new AppDataView();
            }
            else if (type == typeof(GraphicsViewModel))
            {
                control = new GraphicsView();
            }else if(type==typeof(DamWebAPI.ViewModel.DataImport.ImportExcelDataViewModel))
            {
                control = new DamWebAPI.View.DataImport.ImportExcelDataView();
            }

            
            DockPanel panel = new DockPanel();
            panel.Children.Add(control);

            TabItem item = new TabItem();
            item.Header = e.DisplayName;
            item.DataContext = e;
            item.Content = panel;
            //    control.Dock = System.Windows.Forms.DockStyle.Fill;

            tabControl1.Items.Add(item);
            tabControl1.SelectedItem = item;

        }

        void context_EventBeforeWorkspaceRemove(object sender, WorkspaceViewModel e)
        {
           foreach(TabItem item in tabControl1.Items)
           {
               if(item.DataContext==e)
               {
                   tabControl1.Items.Remove(item);
                   item.Header = null;
                   item.Content = null;
                   item.DataContext = null;
                   break;
               }
           }
        }



        //public Control addUserControlInTabPage(Type type, string caption, bool exclusive, object[] args)
        //{
        //    Control ret = null;
        //    int index = -1;
        //    try
        //    {
        //        if (exclusive == false || isUserControlExist(type,ref index) == false)
        //        {
        //            var control = type.InvokeMember(null, System.Reflection.BindingFlags.CreateInstance, null, null, args)
        //                                                            as UserControl;
        //            TabItem item = new TabItem();
        //            item.Header = caption;
        //            DockPanel panel = new DockPanel();
        //            panel.Children.Add(control);

        //            item.Content = panel;
        //        //    control.Dock = System.Windows.Forms.DockStyle.Fill;

        //           // xtp.Controls.Add(control);
        //            tabControl1.Items.Add(item);
        //            tabControl1.SelectedItem = item;

        //            //xtraTabControl1.TabPages.Add(xtp);
        //            //xtraTabControl1.SelectedTabPage = xtp;

        //            ret = control;

        //        }
        //        else
        //        {
        //            tabControl1.SelectedIndex = index;
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        ShowMessage(ex);
        //    }

        //    return ret;
        //}



        ///// <summary>
        ///// 判断控件在tabcontrol中是否已经存在
        ///// </summary>
        ///// <param name="type">控件的类型</param>
        ///// <param name="existIndex">控件在tabitem中的编号</param>
        ///// <returns></returns>
        //private bool isUserControlExist(Type type ,ref int existIndex)
        //{
        //    for (int i=0;i<tabControl1.Items.Count;i++)
        //    {
        //        var xtp = tabControl1.Items[i] as TabItem;
        //        var panel = xtp.Content as Panel;
        //        if (panel!=null)
        //        {
        //            if (panel.Children[0].GetType().Equals(type))
        //            {
        //                existIndex = i;
        //                return true; //the same type control exist

        //            }
        //        }
        //    }

        //    return false;
        //}

       
    }
}
