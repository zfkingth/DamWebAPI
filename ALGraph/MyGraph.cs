using System;
using System.Collections;
namespace ALGraph
{
	/// <summary>
	/// MyGraph 的摘要说明。图的邻接表结构定义
	/// </summary>
	public class MyGraph:ICloneable
	{

		/// <summary>
		/// 
		/// </summary>
		public readonly int startSize=10;
		/// <summary>
		/// 顶点数组
		/// </summary>
		public ArrayList vertices=null;

		/// <summary>
		/// 图的当前弧数
		/// </summary>
		public int Arcnum=0;

		/// <summary>
		/// 
		/// </summary>
		public MyGraph()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			vertices=new ArrayList(startSize);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			MyGraph graph=new MyGraph();

			graph.Arcnum=this.Arcnum;
			graph.vertices=new ArrayList(vertices.Capacity);

			for(int i=0;i<vertices.Count;i++)
			{
				graph.vertices.Add(((VNode)vertices[i]).Clone());

			}


			return graph;
		}

		/// <summary>
		/// 图的当前顶点数
		/// </summary>
		public int Vexnum
		{
		
			get
			{
				return vertices.Count;
			}
		}

		/// <summary>
		///  向顶点数组中添加一个新的顶点
		/// </summary>
		/// <param name="nodeData">顶点的数据</param>
		/// <returns></returns>
		public VNode addVNode(object nodeData)
		{
			
		
			for(int i=0;i<vertices.Count;i++)
			{
				VNode node=(VNode)vertices[i];

				if(nodeData.Equals(node.data))
					return node;
			}
				

			VNode newnode=new VNode();
			newnode.data=nodeData;

			vertices.Add(newnode);

				return newnode;
			
		}

		/// <summary>
		/// 根据顶点的数据信息，查找顶点
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public VNode getVNode(object data)
		{


			for(int i=0;i<vertices.Count;i++)
			{
				VNode node=(VNode)vertices[i];
				if(node.data.Equals(data))
					return node;
			}

			return null;
		}


		/// <summary>
		/// 添加弧
		/// </summary>
		/// <param name="arcNode"></param>
		/// <param name="fromData"></param>
		/// <param name="toData"></param>
		/// <returns></returns>

		public bool addArcNode(ArcNode arcNode,object fromData,object toData)
		{

//			if(vertices.Contains(from)==false||vertices.Contains(to)==false)
//			{
//				//添加弧时要保证弧所连接的两个顶点要存在
//				return false;
//			}

			//添加弧时要保证弧所连接的两个顶点要存在


			VNode from=addVNode(fromData);

			VNode to=addVNode(toData);



			//保证从一个顶点到另一个顶点只有一条弧

			ArcNode arc=from.firstarc;

			for(;arc!=null;arc=arc.nextarc)
			{
				if(arc.adjvex==to)
					return false;//已经存在一条从顶点from到顶点to的弧
			}

			//将弧添加到图中


			//如果这是从该顶点出发的第一条弧
			if(from.firstarc==null)
			{
				from.firstarc=from.endarc=arcNode;
			}
			else
			{
				//不是第一条弧
				from.endarc.nextarc=arcNode;
				from.endarc=arcNode;
			}
			
			arcNode.adjvex=to;

			
			to.inDegree++;
			from.outDegree++;

			return true;
		}




		/// <summary>
		/// 拓扑排序
		/// </summary>
		/// <returns>返回输出顶点数组</returns>
		public ArrayList topSort()
		{
			Stack stack=new Stack(startSize);

			ArrayList list=new ArrayList(startSize);

			//　将当前所有入度为零的顶点入栈

			for(int i=0;i<vertices.Count;i++)
			{
				VNode node=(VNode)vertices[i];

				if(node.inDegree==0)
					stack.Push(node);
			}


			while(stack.Count!=0)
			{
				VNode v=(VNode)stack.Pop();
				// 输出无前驱的顶点
				list.Add(v.data);
				
				ArcNode arc=v.firstarc;

				//遍历依附于该顶点的所有弧
				for(;arc!=null;arc=arc.nextarc)
				{

					//所有邻接点的入度减1

					arc.adjvex.inDegree--;
					if(arc.adjvex.inDegree==0)
					{
						//新的无前驱的顶点
						stack.Push(arc.adjvex);
					}
				}
			}
			return list;


		}
	}
}
