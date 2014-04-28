using System;

namespace ALGraph
{
	/// <summary>
	/// VNode 的摘要说明。顶点结点的结构
	/// </summary>
	public class VNode:ICloneable
	{
		/// <summary>
		/// 顶点信息
		/// </summary>
		public object data=null;

		/// <summary>
		/// 依附于该顶点的第一条弧
		/// </summary>
		public ArcNode firstarc=null;

		/// <summary>
		/// 依附于该顶点的最后一条弧
		/// </summary>
		public ArcNode endarc=null;

		/// <summary>
		/// 入度:指向该顶点的弧的个数
		/// </summary>
		public int inDegree=0;

		/// <summary>
		/// 出度:从该顶点出发的弧的个数
		/// </summary>
		public int outDegree=0;

		/// <summary>
		/// 克隆顶点
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			VNode cnode=new VNode();

			cnode.data=this.data;
			cnode.firstarc=this.firstarc;
			cnode.endarc=this.endarc;
			cnode.inDegree=this.inDegree;
			cnode.outDegree=this.outDegree;

			return cnode;

			
		}

	
		
		
		

		/// <summary>
		/// 构造函数
		/// </summary>
		public VNode()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		
	}
}
