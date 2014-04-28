using System;

namespace ALGraph
{
	/// <summary>
	/// ArcNode 的摘要说明。弧结点的结构
	/// </summary>
	public class ArcNode
	{
		/// <summary>
		/// 该弧所指向的顶点
		/// </summary>
		public VNode adjvex=null;

		/// <summary>
		/// 下一条弧的引用
		/// </summary>
		public ArcNode nextarc=null;

		/// <summary>
		/// 与弧相关的权值，无权则为0
		/// </summary>
		public int weight=0;

		/// <summary>
		/// 弧的相关信息的引用
		/// </summary>
		public object info=null;



		/// <summary>
		/// 
		/// </summary>
		public ArcNode()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
	}
}
