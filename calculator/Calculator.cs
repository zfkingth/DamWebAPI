using System;
using System.Collections;
namespace hammergo.caculator
{
	/// <summary>
	/// Calculator 的摘要说明。
	/// </summary>
	internal class Calculator
	{
		
		
		Stack stack=new Stack(16);

		LinkList suffixList;//存放后缀表达式

		LinkList list; //存放记法分析的结果

		
		const int opeCount=6;
		/// <summary>
		/// 操作符优先级比较表
		/// </summary>
		 public static readonly int [,] priorities=new int[opeCount,opeCount];


		/// <summary>
		/// 
		/// </summary>
		static Calculator()
		{
			//priorities=new int[opeCount,opeCount];

			for(int i=0;i<opeCount;i++)
				for(int j=0;j<opeCount;j++)
				{
					priorities[i,j]=0;
				}

			for(int i=0;i<opeCount-1;i++)
			{
				priorities[i,0]=1;
				priorities[i,1]=1;
			}

			priorities[2,2]=1;
			priorities[2,3]=1;
			priorities[3,2]=1;
			priorities[3,3]=1;

			
			priorities[4,0]=1;
			priorities[4,1]=1;
			priorities[4,2]=1;
			priorities[4,3]=1;
			priorities[4,4]=1;

			
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}


		public Calculator()
		{
		}


		/// <summary>
		/// 设置需要计算的链表
		/// </summary>

		public LinkList List
		{
			get
			{
				return list;
			}
			set
			{
				list=value;
			}
		}

	

		public double compute(string s,AbstractScan scan)
		{
			//记法扫描类
//			Scan scan=new Scan();

			if(s.Trim().Length==0)
			{
				throw new Exception("表达式为空");
			}
			list=scan.parse(s);
			
			 createSuffixExpression();
			return reckon();
		}

		/// <summary>
		/// 计算中缀表达式的值
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>


		public double compute(LinkList list)
		{
			this.list=list;
			createSuffixExpression();
			return reckon();
		}

		

		/// <summary>
		/// 扫描list产生后缀表达式
		/// </summary>

		internal LinkList createSuffixExpression()
		{
			if(list.Count==0)
				throw new Exception("表达式元素个数为零");
			 suffixList=new LinkList();
			stack.Clear();
			LinkNode node=list.First;

			while(node!=null)
			{
				LinkNode next=node.Next;;
				switch(node.getWord().wordType)
				{
					case WordType.Number:list.remove(node);	suffixList.add(node);break;
					case WordType.Plus:dealOperator(node);break;
					case WordType.Minus:dealOperator(node);break;
					case WordType.Mul:dealOperator(node);break;
					case WordType.Div:dealOperator(node);break;
					case WordType.Power:dealOperator(node);break;
					case WordType.Leftp:dealOperator(node);break;
					case WordType.Rightp:dealRight(node);break;
					 default:break;
				}
				node=next;
			}

			//弹出在栈中剩余的元素
			while(stack.Count!=0)
			{
				Word word=(Word)stack.Pop();
				if(word.wordType==WordType.Leftp)
					throw new Exception("在括号(匹配错误");
				suffixList.add(new LinkNode(word));
			}

			return suffixList;
		
		}


		/// <summary>
		/// 操作符入栈
		/// </summary>
		/// <param name="node"></param>

		private void dealOperator(LinkNode node)
		{
	



			list.remove(node);

			while(stack.Count!=0&&priorities[(int)((Word)stack.Peek()).wordType,(int)node.getWord().wordType]==1)
			{
				Word word=(Word)stack.Pop();
				suffixList.add(new LinkNode(word));


			}


			stack.Push(node.getWord());



		}


		/// <summary>
		/// 处理右括号
		/// </summary>
		/// <param name="node"></param>
		private void dealRight(LinkNode node)
		{
			list.remove(node);
			Word word=null;

			while(stack.Count!=0)
			{
				word=(Word)stack.Pop();
				

				if(word.wordType==WordType.Leftp)//左括号和右括号成功匹配
				return;

				suffixList.add(new LinkNode(word));

			}


			if(word==null||word.wordType!=WordType.Rightp)
				throw new Exception("括号匹配错误");
		}

		/// <summary>
		/// 计算表达式的值
		/// </summary>
		/// <returns></returns>


		private double reckon()
		{
			//此时stack里存放的是double对象
			stack.Clear();
			double r1,r2;

			LinkNode node=suffixList.First;

			while(node!=null)
			{
				if(node.getWord().wordType==WordType.Number)
					stack.Push(double.Parse(node.getWord().valueString));
				else if(node.getWord().wordType<WordType.Leftp)
				{
					if(stack.Count>=2)
					{
						r2=(double)stack.Pop();
						r1=(double)stack.Pop();

						switch(node.getWord().wordType)
						{
							case WordType.Plus:stack.Push(r1+r2);break;
							case WordType.Minus:stack.Push(r1-r2);break;
							case WordType.Mul:stack.Push(r1*r2);break;
							case WordType.Div:stack.Push(r1/r2);break;
							case WordType.Power:stack.Push(Math.Pow(r1,r2));break;
						}

					}
					else
						throw new Exception("表达式操作数数目不正确");
				}
				else throw new Exception("表达式错误 0");

				node=node.Next;
			}

			if(stack.Count!=1)
				throw new Exception("表达式操作数数目不正确");
			else 	return  (double)stack.Pop();
		}


		/// <summary>
		/// 检查+ - * / 操作符的格式是否正确
		/// </summary>

		private void observeList()
		{
			LinkNode node=list.First;
			LinkNode pre,next;

			while(node!=null)
			{
				//word是+ - * / 这四种类型
				if(node.getWord().wordType<=WordType.Div)
				{
					pre=node.Previous;
					next=node.Next;

					if(next==null)
						throw new Exception(node.getWord().valueString+"后面有误1");

					if(pre.getWord().wordType!=WordType.Rightp&&pre.getWord().wordType!=WordType.Number)
						throw new Exception(node.getWord().valueString+"前面有误");

					if(next.getWord().wordType!=WordType.Leftp&&next.getWord().wordType!=WordType.Number)
						throw new Exception(node.getWord().valueString+"后面有误2");


				


				}


					node=node.Next;

			}

		}



		


	
	
	
	}
}
