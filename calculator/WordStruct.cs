using System;

namespace hammergo.caculator
{
	/// <summary>
	/// WordStruct 的摘要说明。
	/// 用于保存单词的信息，包括单词类型和值串
	/// </summary>
	internal class Word
	{
		/// <summary>
		/// 单词类型
		/// </summary>
		public WordType wordType; 

		/// <summary>
		/// 值串
		/// </summary>
		public string valueString;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="wordType"></param>
		/// <param name="s"></param>

		public Word(WordType wordType,string s)
		{
			this.wordType=wordType;
			valueString=s;
			
		}

		/// <summary>
		/// 删除数字的正负号
		/// </summary>


		public void removeSymbol()
		{
			if(valueString[0]=='+'||valueString[0]=='-')
			{
//				int i=0;
//				for(;i<valueString.Length-1;i++)
//					valueString[i]=valueString[i+1];
//				valueString[i]='\0';

				valueString.Remove(0,1);

			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public  override string  ToString()
		{
			return string.Format("wordType:{0},valueString:{1}",wordType,valueString);
		}
	}
}
