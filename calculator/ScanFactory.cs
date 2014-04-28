using System;

namespace hammergo.caculator
{
	/// <summary>
	/// ScanFactory 的摘要说明。静态工厂
	/// </summary>
	 internal  class ScanFactory
	{
		/// <summary>
		/// 产生扫描器
		/// </summary>
        /// <param name="usePrefix"></param>
		/// <returns></returns>
         public static AbstractScan createScan(bool usePrefix)
		{
			AbstractScan scan=null;

            if (!usePrefix)
			{
				scan=new StrictScan();

			}
			else
			{
				scan=new RelaxedScan();
			}

			return scan;
		}
	}
}
