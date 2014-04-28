using System;

namespace hammergo.caculator
{
	/// <summary>
	/// StrictScan 的摘要说明。
	/// </summary>
	internal class StrictScan:AbstractScan
	{
		public StrictScan()
		{
			//
			// TODO: 在此处添加构造函数逻辑

            //

		
			
		}

		static StrictScan()
		{

			stateCount=19;//DFA所有状态的个数,状态转换表的行数
			colCount=14;//状态转换表的列数

			states=new int[stateCount,colCount];

			setStates();
		}

		protected  static void setStates()
		{
			

			for(int i=0;i<stateCount;i++)
			{
				
				for(int j=0;j<colCount;j++)
					states[i,j]=-2;

				
			}
			
			
			states[0,1]=1;
			states[0,2]=1;
			states[0,3]=6;
			states[0,4]=0;
			states[0,5]=17;
			states[0,7]=4;
			states[0,8]=5;
			states[0,9]=12;
			states[0,10]=13;
			states[0,11]=14;
			states[0,12]=15;
			states[0,13]=18;
		



			states[1,0]=-1;
			states[1,1]=1;
			states[1,2]=1;
			states[1,3]=1;
			states[1,6]=2;
			

			states[2,1]=3;
			states[2,2]=3;


			states[3,0]=-1;
			states[3,1]=3;
			states[3,2]=3;
			states[3,3]=3;

			states[4,0]=-1;
			
			
			states[5,0]=-1;
			

			states[6,0]=-1;
			states[6,1]=9;
			states[6,2]=16;
			states[6,3]=6;
			states[6,6]=7;

			states[7,3]=8;
		
			states[8,0]=-1;
			states[8,1]=9;
			states[8,2]=16;
			states[8,3]=8;

			states[9,3]=11;
			states[9,7]=10;
			states[9,8]=10;

			states[10,3]=11;

			states[11,0]=-1;
			states[11,1]=16;
			states[11,2]=16;
			states[11,3]=11;

			states[12,0]=-1;
			states[13,0]=-1;
			states[14,0]=-1;
			states[15,0]=-1;
			states[17,0]=-1;
			states[18,0]=-1;
		
		}

		protected override int getCharType(char c)
		{
			switch(c)
			{
				case 'E':return 1;
				case 'e':return 1;
				case ' ':return 4;
				case ',':return 5;
				case '.':return 6;
				case '+':return 7;
				case '-':return 8;
				case '*':return 9;
				case '/':return 10;
							
				case '(':return 11;
				case ')':return 12;
				case '^':return 13;
				
			
			}
			
			if(isLetter(c))return 2;
			if(isDigit(c))return 3;
			
			
			//没有匹配的类型
			return -1;
		}

	}
}
