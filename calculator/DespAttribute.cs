using System;

namespace hammergo.caculator
{
	/// <summary>
	/// DespAttribute 的摘要说明。
	/// </summary>
	public class DespAttribute:Attribute
	{
		/// <summary>
		/// 函数形式
		/// </summary>
		string pattern;
		/// <summary>
		/// 函数功能说明
		/// </summary>
		string description;
		/// <summary>
		/// 函数参数说明
		/// </summary>
		string paDescription;
		
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="pattern">函数形式</param>
		/// <param name="description">函数功能描述</param>
		/// <param name="paDescription">函数参数描述</param>
		public DespAttribute(string pattern,string description,string paDescription)
		{
			this.pattern=pattern;
			this.description=description;
			this.paDescription=paDescription;
		}

		/// <summary>
		/// 函数的形式
		/// </summary>

		public string Pattern
		{
			get
			{
				return pattern;
			}
		}

		/// <summary>
		/// 函数描述
		/// </summary>
		public string Description
		{
			get
			{
				return description;
			}
		}


		/// <summary>
		/// 函数参数说明
		/// </summary>
		public string PaDescription
		{
			get
			{
				return paDescription;
			}
		}

	}
}
