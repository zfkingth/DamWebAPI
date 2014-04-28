using System;
using System.Collections;
namespace hammergo.caculator
{
	/// <summary>
	/// 存存函数信息的类
	/// </summary>

	internal class FunInfo
	{
		private string funName;
		private string description;
		private int paramsCount;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="funName"></param>
		/// <param name="paramsCount"></param>
		/// <param name="description"></param>

		public FunInfo(string funName,int paramsCount,string description)
		{
			this.funName=funName.ToLower();
			this.paramsCount=paramsCount;
			this.description=description;
		}

		/// <summary>
		/// 
		/// </summary>
		public string FunName
		{
			get
			{
				return funName;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string Description
		{
			get
			{
				return description;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public int ParamsCount
		{ 
			get
			{
				return paramsCount;
			}
		}
	}


	/// <summary>
	/// 
	/// 用于保存需要实现的数学函数的信息，包括函数名和参数个数
	/// </summary>
	internal class FunNames
	{
		private Hashtable funs=new Hashtable(40);
		/// <summary>
		/// 
		/// </summary>
		public FunNames()
		{
			
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		/// <summary>
		/// 往哈希表里添加函数信息
		/// </summary>
		/// <param name="funName"></param>
		/// <param name="paramsCount"></param>
		/// <param name="description"></param>
		/// <returns></returns>
		
		public bool addFun(string funName,int paramsCount,string description)
		{
			string name=funName.ToLower();
			if(funs.ContainsKey(name))//该函数已存在
				return false;
			funs.Add(name,new FunInfo(name,paramsCount,description));
			return true;
		}



		
		/// <summary>
		/// 判断函数名为funNamet的函数在哈希表里是否存在
		/// </summary>
		/// <param name="funName"></param>
		/// <returns></returns>
		public bool funExist(string funName)
		{
			return funs.ContainsKey(funName.ToLower());
		}

		/// <summary>
		/// 根据函数名返回对应的参数的个数
		/// </summary>
		/// <param name="funName"></param>
		/// <returns></returns>
		public FunInfo getFunInfo(string funName)
		{
			return (FunInfo)funs[funName.ToLower()];
		}
	}
}
