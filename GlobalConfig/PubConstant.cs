using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace Hammergo.GlobalConfig
{

    public class PubConstant
    {


        public const string customString = "yyyy-MM-dd HH:mm:ss";//自定义日期格式字符串

        public const string shortString = "yyyy-MM-dd";


        private static DateTime initialTime = DateTime.MinValue;
        /// <summary>
        /// 默认公式的时间刻度最小值
        /// </summary>
        public static DateTime InitialTime
        {
            get
            {
                return initialTime;
            }
        }

        /// <summary>
        /// 默认公式的时间刻度最大值
        /// </summary>
        private static DateTime overTime = DateTime.MaxValue;
        /// <summary>
        /// 默认公式终止时间,9999年12月31日
        /// </summary>
        public static DateTime OverTime
        {
            get
            {
                return overTime;
            }
        }

        /// <summary>
        /// 批注列的名称
        /// </summary>
        public const string remarkColumnName = "批注";
        /// <summary>
        /// 时间列的名称
        /// </summary>
        public const string timeColumnName = "时间";

        /// <summary>
        /// 测点编号列名称
        /// </summary>
        public const string appColumnName = "测点编号";

        /// <summary>
        /// 数据输入任务在表TaskType中的TypeName
        /// </summary>
        public  const  string inputTaskName = "输入任务";


        /// <summary>
        /// 数据检索任务在表TaskType中的TypeName
        /// </summary>
        public const string searchTaskName = "检索任务";


        /// <summary>
        /// 用户权限值，值越大权限越大
        /// </summary>
        public static byte userPower = 0;

        //当前登录的用户名称
        public static string userName = "";

        /// <summary>
        /// 管理员对应的权限编码
        /// </summary>
        public const byte power_admin = 255;

        /// <summary>
        /// 普通用户对应的权限编码
        /// </summary>
        public const byte power_user = 2;


        /// <summary>
        /// 访客对应的权限编码
        /// </summary>
        public const byte power_guest = 1;

        ///// <summary>
        ///// 得到web.config里配置项的数据库连接字符串。
        ///// </summary>
        ///// <param name="configName"></param>
        ///// <returns></returns>
        //public static string GetConnectionString(string configName)
        //{
        //    string connectionString = ConfigurationManager.AppSettings[configName];
        //    string ConStringEncrypt = ConfigurationManager.AppSettings["ConStringEncrypt"];
        //    if (ConStringEncrypt == "true")
        //    {
        //        connectionString = DESEncrypt.Decrypt(connectionString);
        //    }
        //    return connectionString;
        //}

        //private static DataSet configDS =null;

        //private static readonly string configPath = Application.StartupPath + "\\Resources\\config.xml";


        private const string GlobalFileName = "GlobalConfigData.xml";



        private static GlobalConfigData configData = null;

        public static GlobalConfigData ConfigData
        {
            get
            {
                if (configData == null)
                {
                    if (File.Exists(GlobalFileName) == false) throw new Exception("找不到配置文件" + GlobalFileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(GlobalConfigData));
                    // A FileStream is needed to read the XML document.
                    FileStream fs = new FileStream(GlobalFileName, FileMode.Open);
                    configData = (GlobalConfigData)serializer.Deserialize(fs);
                    fs.Close();
                }
                return configData;
            }
        }

        public static void updateConfigData()
        {
            XmlSerializer ser = new XmlSerializer(typeof(GlobalConfigData));
            TextWriter writer = new StreamWriter(GlobalFileName);

            ser.Serialize(writer, ConfigData);

            writer.Close();

        }

        //public static void updateConfigDS()
        //{

        //    Config.WriteXml(configPath, XmlWriteMode.WriteSchema);

        //}

        //public static DataSet Config
        //{
        //    get
        //    {
        //        if (configDS == null)
        //        {
        //            if (File.Exists(configPath) == false) throw new Exception("找不到配置文件" + configPath);
        //            configDS = new DataSet();
        //            configDS.ReadXml(configPath, XmlReadMode.ReadSchema);
        //        }

        //        return configDS;
        //    }
        //}




        //private static void generateConfigDataFile()
        //{

        //    XmlSerializer ser = new XmlSerializer(typeof(GlobalConfigData));
        //    TextWriter writer = new StreamWriter(GlobalFileName);

        //    GlobalConfigData obj = new GlobalConfigData();

        //    obj.DALAssemblyName = @"hammergo.OleDbDAL";
        //    obj.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=DataBase\dam3Mode.mdb;Persist Security Info=False";


        //    //常量单位
        //    List<ParamInfo> listInfo = new List<ParamInfo>(10);

        //    foreach (DataRow row in configDS.Tables["常量单位"].Rows)
        //    {
        //        ParamInfo pi = new ParamInfo();
        //        pi.Name = row["名称"].ToString();
        //        pi.UnitSymbol = row["单位"].ToString();
        //        pi.CalcSymbol = row["符号"].ToString();
        //        pi.Precision = (byte)(int)row["精度"];

        //        listInfo.Add(pi);

        //    }

        //    obj.ConstParamsList = listInfo;


        //    //默认单位

        //    listInfo = new List<ParamInfo>(10);

        //    foreach (DataRow row in configDS.Tables["默认单位"].Rows)
        //    {
        //        ParamInfo pi = new ParamInfo();
        //        pi.Name = row["名称"].ToString();
        //        pi.UnitSymbol = row["单位"].ToString();
        //        pi.CalcSymbol = row["符号"].ToString();
        //        pi.Precision = (byte)(int)row["精度"];

        //        listInfo.Add(pi);

        //    }

        //    obj.DefaultParamsList = listInfo;


        //    //错误值

        //    List<double> listVals = new List<double>(4);

        //    foreach (DataRow row in configDS.Tables["错误值"].Rows)
        //    {

        //        listVals.Add((double)row["errorValue"]);

        //    }

        //    obj.ErrorValList = listVals;



        //    List<LineStyleInfo> listLineInfos = new List<LineStyleInfo>(6);

        //    foreach (DataRow row in configDS.Tables["样式表"].Rows)
        //    {
        //        LineStyleInfo lsi = new LineStyleInfo();

        //        lsi.ID = (int)row["ID"];

        //        lsi.LineStyle = (string)row["line样式"];

        //        lsi.LineColor = (int)row["line颜色"];

        //        lsi.LineThickness = (int)row["line粗细"];

        //        lsi.SymbolShape = (string)row["symbol形状"];

        //        lsi.SymbolColor = (int)row["symbol颜色"];

        //        lsi.SymbolSize = (int)row["symbol大小"];

        //        lsi.SymbolOutColor = (int)row["symbol外围颜色"];

        //        lsi.SymbolOutThickness = (int)row["symbol外围粗细"];

        //        listLineInfos.Add(lsi);

        //    }

        //    obj.LineStyleInfoList = listLineInfos;




        //    obj.ErrorConvertString = "/";
        //    obj.NoDataConvertString = "*";





        //    ser.Serialize(writer, obj);
        //}
    }
}
