using System;
using System.Runtime.InteropServices; 
using System.Reflection; 
using System.Runtime.CompilerServices;

namespace hammergo.caculator
{
	/// <summary>
	/// MyList 的摘要说明。
	/// </summary>
	/// 

//	public interface IMyList
//	{
//
//		void add(string key,double val);
//		 void clear();
//
//		double this[string key]
//		{
//			get;
//			set;
//		}
//
//	}
//
//
//	[ClassInterface(ClassInterfaceType.None)] 
	public class MyList //:IMyList
	{
		/// <summary>
		/// 初始大小
		/// </summary>
		const int initialSize=5;
//		/// <summary>
//		/// 递增大小
//		/// </summary>
//		const int plus=10;
		/// <summary>
		/// 表的容量
		/// </summary>
		int maxSize;
		/// <summary>
		/// 表示数组中空元素的起始坐标，也表示数组中所拥有元素的实际数量
		/// </summary>
		int currentIndex=0;
		/// <summary>
		/// 键数组
		/// </summary>
		string [] keys;
		/// <summary>
		/// 值数组
		/// </summary>
		double [] values;

		/// <summary>
		/// 构造函数
		/// </summary>

		public MyList()
		{
			keys=new string [initialSize];
			values=new double[initialSize];
			maxSize=initialSize;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="initialSize">表的初始大小</param>
		public MyList(int initialSize)
		{
			keys=new string [initialSize];
			values=new double[initialSize];
			maxSize=initialSize;

		}


		/// <summary>
		/// 添加键值对,如果key键已存在，那么只是修改它对应的值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>

		public void add(string key,double val)
		{

			for(int i=0;i<currentIndex;i++)
			{
				if(key==keys[i])
				{
					values[i]=val;
					return;
				}
			}

			if(currentIndex>=maxSize)
			{
				double[] values2=new double[maxSize*2];
				string [] keys2=new string[maxSize*2];

				values.CopyTo(values2,0);

				keys.CopyTo(keys2,0);

				
				maxSize*=2;

				values=values2;
				keys=keys2;

			}

			keys[currentIndex]=key;
			values[currentIndex]=val;

			currentIndex++;
		}


		/// <summary>
		/// 清除整修表的元素s
		/// </summary>
		public void clear()
		{
			currentIndex=0;
		}


		/// <summary>
		/// 根据键来索引值
		/// </summary>
		public double this[string key]
		{
			get
			{
				for(int i=0;i<currentIndex;i++)
				{
					if(key==keys[i])
						return values[i];
				}

				throw new Exception("参数"+key+"在参数列表中不存在");

			}

			set
			{
				this.add(key,value);
			}
		}


		/// <summary>
		/// 获取键字符串
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string getKey(int index)
		{
			if(index<this.Length)
			{
				return keys[index];
			}
			else
			{
				throw new Exception("对MyList索引超出实际大小");
			}

		}


		/// <summary>
		/// 获取表的实际大小
		/// </summary>

		public int Length
		{
			get
			{
				return currentIndex;
			}
		}
		}
}
