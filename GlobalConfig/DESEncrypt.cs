using System;
using System.Security.Cryptography;  
using System.Text;
namespace hammergo.GlobalConfig
{
	/// <summary>
	/// DES加密/解密类。
	/// </summary>
	public class DESEncrypt
	{
		public DESEncrypt()
		{			
		}

		#region ========加密======== 
 
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
		public static string Encrypt(string Text) 
		{
			return Encrypt(Text,"litianping");
		}
		/// <summary> 
		/// 加密数据 
		/// </summary> 
		/// <param name="Text"></param> 
		/// <param name="sKey"></param> 
		/// <returns></returns> 
		public static string Encrypt(string Text,string sKey) 
		{
            return "";
		} 

		#endregion
		
		#region ========解密======== 
   
 
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
		public static string Decrypt(string Text) 
		{
			return Decrypt(Text,"litianping");
		}
		/// <summary> 
		/// 解密数据 
		/// </summary> 
		/// <param name="Text"></param> 
		/// <param name="sKey"></param> 
		/// <returns></returns> 
		public static string Decrypt(string Text,string sKey) 
		{

            return "";
		} 
 
		#endregion 


	}
}
