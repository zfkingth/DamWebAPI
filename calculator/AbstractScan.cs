using System;
using System.Collections;

namespace hammergo.caculator
{
	
	/// <summary>
	/// 单词类型的枚举结构,标识符:Identifier,函数名,FunName,
	/// 操作符:+,-,*,/,左括号(,leftp,右括号),rightp
	/// </summary>
	public enum WordType
	{
		/// <summary>
		/// +
		/// </summary>
		Plus=0,

		/// <summary>
		/// -
		/// </summary>
		Minus=1,

		/// <summary>
		/// *
		/// </summary>
		Mul=2,

		/// <summary>
		/// /
		/// </summary>
		Div=3,


		/// <summary>
		/// ^
		/// </summary>
		Power=4,

		/// <summary>
		/// (
		/// </summary>
		Leftp=5,




		
		
		/// <summary>
		/// )
		/// </summary>
		Rightp,

		/// <summary>
		/// 数字
		/// </summary>
		Number,

		
		/// <summary>
		/// 标识符
		/// </summary>
		
		Identifier,
		/// <summary>
		/// 函数名
		/// </summary>
		FunName,

		
	
	
	
	
		/// <summary>
		/// 带点的标识符，两层
		/// </summary>
		IdentifierWithDot,
		/// <summary>
		/// 空格
		/// </summary>

		Blank,
		/// <summary>
		/// 逗号
		/// </summary>
		Comma


	}


	
	
/// <summary>
/// 扫描类
/// </summary>
	internal abstract class AbstractScan
	{
		/// <summary>
		/// 定义单词的最大长度
		/// </summary>
		public  const int MaxSize=128;
		
		protected  char ch;//存放最新读进的字符
		protected  char [] strToken=new char[MaxSize];//存放单词符号的字符串

		protected  string calcStr;//表达式字符串

		protected  LinkList list=new LinkList();//存放单词信息的双向链表

		

		protected  int index=0;//存放字符的索引

		protected  static int stateCount=20;//DFA所有状态的个数,状态转换表的行数
		protected  static  int colCount=15;//状态转换表的列数

		protected  static int [,] states;//状态转换表

		

		protected  int state=0;//NFA的当前状态

		protected int strTokenIndex=0;//单词strToken的下标

	

		protected AbstractScan()
		{
			
		}
	
//		/// <summary>
//		/// 构造一个Scan类
//		/// </summary>
//		public Scan()
//		{
//			setStates();
//		}
//		

		/// <summary>
		/// 从扫描字符串中获取一个字符，当到达扫描字符串尾时返回'\0'
		/// </summary>
		/// <returns></returns>
		protected char getChar()
		{
			if(index==calcStr.Length)
				return '\0';//表达式扫描结束
			else
			{
				return calcStr[index++];
			}

		}

		/// <summary>
		/// 设置需要扫描的表达式
		/// </summary>
		/// <param name="calcString"></param>
		protected void setString(string calcString)
		{
			calcStr=calcString;
			index=0;
			state=0;
			
			clearToken();
		}


		/// <summary>
		///判断字符是否是a-z,A-Z中的一个字符
		/// </summary>
		/// <param name="achar"></param>
		/// <returns></returns>
		protected  bool isLetter(char achar)
		{
			if((achar>='a'&&achar<='z')||(achar>='A'&&achar<='Z'))
				return true;
			else return false;
		}

		/// <summary>
		/// 判断字符是否是一个数字
		/// </summary>
		/// <param name="achar"></param>
		/// <returns></returns>
		protected bool isDigit(char achar)
		{
			if(achar>='0'&&achar<='9')
				return true;
			else return false;
		}


		/// <summary>
		/// 将字符加入到strToken单词中
		/// </summary>
		/// <param name="c"></param>
		protected  void addToToken(char c)
		{
			if(strTokenIndex<strToken.Length)
				strToken[strTokenIndex++]=c;
			else
				throw new Exception(string.Format("数组越界，超过单词的最大长度{0}",MaxSize));
		}

		/// <summary>
		/// 将单词strToken的内容清空
		/// </summary>
		protected  void clearToken()
		{
			for(int i=0;i<strToken.Length;i++)
				strToken[i]='\0';
			strTokenIndex=0;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		protected  abstract int getCharType(char c);
//		{
//			switch(c)
//			{
//				case 'E':return 1;
//				case 'e':return 1;
//				case ' ':return 4;
//				case ',':return 5;
//				case '.':return 6;
//				case '+':return 7;
//				case '-':return 8;
//				case '*':return 9;
//				case '/':return 10;
//				
//				case '(':return 11;
//				case ')':return 12;
//				case '^':return 13;
//				case '!':return 14;
//
//			}
//
//			if(isLetter(c))return 2;
//			if(isDigit(c))return 3;
//
//
//			//没有匹配的类型
//			return -1;
//		}


	

		/// <summary>
		/// 解析字符串
		/// </summary>
		/// <returns></returns>
		public LinkList parse(string str)
		{
			setString(str.Trim());
			list.removeAll();
			ch=this.getChar();
			while(ch!='\0')
			{
				int typeCode=getCharType(ch);//获得字符ch的类型码
				if(typeCode==-1)
				throw new Exception("表达式中不能出现字符"+ch);

				int newstate=states[state,typeCode];
				if(newstate==-2)//没有相匹配的状态
				{
					if(states[state,0]==-1)//state为终结状态
					{
						//一个单词的扫描已完成
						addToList(state);

						//重置状态
						state=0;//重新扫描下一个单词
						clearToken();

						continue;//直接进入下一个循环，不用getChar();


					}
					else  //不有正确的匹配，出现错误
					{
						throw new Exception("表达式错误");
					}
				}
				else //有匹配的状态
				{
					state=newstate;
					if(state!=0)//防止将忽略的字符加入到strToken中
					addToToken(ch);//将字符加到单词中

				}
				ch=getChar();
			}
			
			if(states[state,0]==-1)//判断最后一个单词的状态
			{
				addToList(state);
				
			}
			else
				throw new Exception("表达式中的末尾出错");


			list.modify();
			return list;
		}

		/// <summary>
		/// 单词扫描完成后，将单词加入到list中
		/// </summary>
		/// <param name="state"></param>

		private void addToList(int state)
		{
			Word w;//要加入到链表的单词
			switch(state)
			{
				case 1:w=new Word(WordType.Identifier,makeString(strToken)); break;
				case 3:w=new Word(WordType.IdentifierWithDot,makeString(strToken)); break;//带点的标识符
				case 4:w=new Word(WordType.Plus,makeString(strToken)); break;
				case 5:w=new Word(WordType.Minus,makeString(strToken)); break;
				case 6:w=new Word(WordType.Number,makeString(strToken)); break;
				case 8: w=new Word(WordType.Number,makeString(strToken)); break;
				case 11:w=new Word(WordType.Number,makeString(strToken)); break;
				case 12:w=new Word(WordType.Mul,makeString(strToken)); break;
				case 13:w=new Word(WordType.Div,makeString(strToken)); break;
				case 14:w=new Word(WordType.Leftp,makeString(strToken)); break;
				case 15:w=new Word(WordType.Rightp,makeString(strToken)); break;
				case 17:w=new Word(WordType.Comma,makeString(strToken));break;
				case 18:w=new Word(WordType.Power,makeString(strToken));break;
				default:
					throw new Exception("接收状态编码错误");

			}

			
			
			
			list.add(new LinkNode(w));

			
		}
		

		/// <summary>
		/// 产生字符串
		/// </summary>
		/// <param name="chars"></param>
		/// <returns></returns>
		protected  string makeString(char [] chars)
		{
			int index=0;
			for(;index<chars.Length;index++)
			{
				if(chars[index]=='\0')
				break;
			}

			return new string(chars,0,index);

			

		}



		
	}


}
