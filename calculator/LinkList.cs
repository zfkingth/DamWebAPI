using System;
using System.Collections;

namespace hammergo.caculator
{
	/// <summary>
	/// LinkList 的摘要说明。
	/// </summary>
	internal class LinkList
	{
		/// <summary>
		/// 首结点
		/// </summary>
		LinkNode first;
		

		/// <summary>
		/// 尾结点
		/// </summary>
		LinkNode end;


		/// <summary>
		/// 链表中结点的个数
		/// </summary>
		int count=0;


		/// <summary>
		/// Constructor 
		/// </summary>
		public LinkList()
		{
			first=end=null;
			count=0;
		}

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="first"></param>
		/// <param name="end"></param>

		public LinkList(LinkNode first,LinkNode end)
		{
			this.first=first;
			this.end=end;
			if(end!=null)
			first.Previous=end.Next=null;
			count=Acount();
		}

		/// <summary>
		/// 删除所有的结点
		/// </summary>

		public void  removeAll()
		{
			first=end=null;
			count=0;

		}

		/// <summary>
		/// 返回链表中结点的个数
		/// </summary>

		public int Count
		{
			get
			{
				return count;
			}
		}

		/// <summary>
		/// 删除结点
		/// </summary>
		/// <param name="node"></param>

		public void remove(LinkNode node)
		{
			
			
			if(node==first)
			{
				first=first.Next;
				if(first==null)
					end=null;
			}
			else if(node==end)
			{
				end=node.Previous;

				end.Next=null;
			}
				
			else
			{
				node.Previous.Next=node.Next;
				node.Next.Previous=node.Previous;
			}

			count--;

			
		}

		/// <summary>
		/// 删除两个结点之间的结点，包括这两个结点
		/// </summary>
		/// <param name="start"></param>
		/// <param name="finish"></param>
		public void removeBetween(LinkNode start,LinkNode finish)
		{
			if(start==first&&finish==end)
			{
				first=end=null;
				return;
			}

			if(start==first)
			{
				first=finish.Next;
				first.Previous=null;
				return;
			}

			if(finish==end)
			{
				end=start.Previous;
				end.Next=null;
				return;
			}
			else
			{
				start.Previous.Next=finish.Next;
				finish.Next.Previous=start.Previous;
			}





			count=Acount();


		}


		/// <summary>
		/// 返回首结点
		/// </summary>
		public LinkNode First
		{
			get
			{
				return first;
			}

		}


		/// <summary>
		/// 链表的最后一个结点
		/// </summary>
		public LinkNode End
		{
			get
			{
				return end;
			}
		}

		/// <summary>
		/// 在链表的末尾添加一个新的结点
		/// </summary>
		/// <param name="node"></param>

		public void add(LinkNode node)
		{
			if(first==null)
			{
				first=end=node;
				node.Previous=null;
				node.Next=null;
				
			}
			else
			{
				end.Next=node;
				node.Previous=end;
				end=node;
				end.Next=null;
			}
				
			count++;
		}



		
		/// <summary>
		/// 在结点node前插入一个新的结点newNode
		/// </summary>
		/// <param name="node"></param>
		/// <param name="newNode"></param>

		public void insert(LinkNode node,LinkNode newNode)
		{
			 if(node==null)
			{
				add(newNode);
			}

			else if(node==first)
			{
				first.Previous=newNode;
				newNode.Next=node;
				first=newNode;
			}

			

			else
			{
				LinkNode preNode=node.Previous;

				newNode.Next=node;

				newNode.Previous=preNode;
				node.Previous=newNode;
				preNode.Next=newNode;
				
			}


			count++;

		}

		


		/// <summary>
		/// 在+或-前添加数字0
		/// </summary>
		/// <returns></returns>
		public LinkList modify()
		{
			//修正头结点
			LinkNode node=this.First;
			if(node.getWord().wordType==WordType.Plus||node.getWord().wordType==WordType.Minus)
				insert(node,new LinkNode(new Word(WordType.Number,"0")));


			LinkNode preNode=node;//前一个结点
			node=node.Next;

			while(node!=null)
			{
				//在+ -号前而是(就在+ －号前添加一个0
				if(node.getWord().wordType==WordType.Plus||node.getWord().wordType==WordType.Minus)
				{
					if(preNode.getWord().wordType==WordType.Leftp||preNode.getWord().wordType==WordType.Comma)
						insert(node,new LinkNode(new Word(WordType.Number,"0")));

					
				}

				//确定函数名
				if(preNode.getWord().wordType==WordType.Identifier&&node.getWord().wordType==WordType.Leftp)
					preNode.getWord().wordType=WordType.FunName;

				 preNode=node;

				node=node.Next;

			}
				


			return this;
		}


		/// <summary>
		/// 统计整修链表结点的个数
		/// </summary>
		
		public int Acount()
		{
			
				int count=0;
				LinkNode node=first;
				while(node!=null)
				{
					count++;
					node=node.Next;
				}
				return count;

			
		}
		
		/// <summary>
		/// 在链表中寻找逗号，计这两个结点
		/// </summary>
		/// <returns></returns>


		public LinkNode searchComma()
		{
			LinkNode node=first;
			while(node!=null)
			{
				if(node.getWord().wordType==WordType.Comma)
					return node;

				node=node.Next;
			}
			return null;//没有逗号这个结点
		}


		
		/// <summary>
		/// 将表达式中的变量替换为具体的数字
		/// </summary>
		/// <param name="slist"></param>

		public void replaceVariable(MyList slist)
		{

			LinkNode node=first;
			while(node!=null)
			{
				Word word=node.getWord();
				if(word.wordType==WordType.Identifier||word.wordType==WordType.IdentifierWithDot)
					
				{
					string name=node.getWord().valueString;
				
					double v=slist[name];

				

					word.wordType=WordType.Number;
					word.valueString=v.ToString();
						
				
				}

				node=node.Next;
			}

		}



	}
}
