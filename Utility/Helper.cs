using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Input;
using hammergo.GlobalConfig;

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

    }
}
