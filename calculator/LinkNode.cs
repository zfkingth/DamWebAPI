using System;

namespace hammergo.caculator
{
	/// <summary>
	/// LinkNode 的摘要说明。
	/// 
	/// </summary>
	internal class LinkNode
	{
		/// <summary>
		/// 前一个节点
		/// </summary>
		LinkNode preNode=null;


		/// <summary>
		/// 后一个节点
		/// </summary>
		
		LinkNode nextNode=null;

		Word word;
		/// <summary>
		/// 构造一个结点
		/// </summary>
		public LinkNode()
		{
			
		}

		/// <summary>
		/// 构造一个链表，它的头结点不为空
		/// </summary>
		/// <param name="word"></param>
		public LinkNode(Word word)
		{
			this.word=word;
		}

		

		

		/// <summary>
		/// 前一个结点属性
		/// </summary>
		public LinkNode Previous
		{
			get
			{
				return preNode;
			}

			set
			{
				preNode=value;
			}
			
		}

		/// <summary>
		/// 获取结点的值word
		/// </summary>
		/// <returns></returns>
		public Word getWord()
		{
			return word;
		}


		/// <summary>
		/// 下一个结点的属性
		/// </summary>
		public LinkNode Next
		{
			get
			{
				return nextNode;
			}
			set
			{
				nextNode=value;
			}

			
		}

		
		


	}
}
