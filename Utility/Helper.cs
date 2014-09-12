using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Input;
using Hammergo.GlobalConfig;
using System.Windows;
using System.Windows.Media;
using System.Globalization;

namespace Hammergo.Utility
{
    public class Helper
    {
        public static void InvodeCmd(object viewModel, string cmdName, object arg)
        {
            if (viewModel != null)
            {

                Type modelType = viewModel.GetType();
                PropertyInfo pi = modelType.GetProperty(cmdName);

                ICommand cmd = pi.GetValue(viewModel) as ICommand;

                if (cmd.CanExecute(null))
                {
                    cmd.Execute(arg);
                }

            }

        }


 

        public static double Round(double value, int decimals)
        {
            if (value < 0)
            {
                return Math.Round(value + 5 / Math.Pow(10, decimals + 1), decimals, MidpointRounding.AwayFromZero);
            }
            else
            {
                return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
            }
        }

        public static bool isErrorValue(double v)
        {
            foreach (double vi in PubConstant.ConfigData.ErrorValList)
            {
                if (vi == v)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 将数字转化为时间
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static DateTime NumToDateTime(double num)
        {
            DateTime origin = new DateTime(1900, 1, 1, 0, 0, 0, 0);



            return origin.AddDays(num);

        }

        /// <summary>
        /// 将时间转化为数字
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static double DateTimeToNum(DateTime date)
        {

            DateTime origin = new DateTime(1900, 1, 1, 0, 0, 0, 0);

            return (date.Ticks - origin.Ticks) / TimeSpan.TicksPerDay;

        }


        public static Size MeasureTextSize(string text, FontFamily fontFamily, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, double fontSize)
        {
            FormattedText ft = new FormattedText(text,
                                                 CultureInfo.CurrentCulture,
                                                 FlowDirection.LeftToRight,
                                                 new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                                                 fontSize,
                                                 Brushes.Black);
            return new Size(ft.Width, ft.Height);
        }

    }
}
