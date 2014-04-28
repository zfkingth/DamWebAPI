using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices; 
using System.Runtime.CompilerServices;


namespace hammergo.caculator
{
	/// <summary>
	/// CalcFunction 的摘要说明。
	/// </summary>
	/// 

	public interface ICalcFunction
	{
		/// <summary>
		/// 计算
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		double compute(string s);
		/// <summary>
		/// 计算
		/// </summary>
		/// <param name="s"></param>
		/// <param name="slist"></param>
		/// <returns></returns>
		double compute(string s,MyList slist);

	}


	/// <summary>
	/// 
	/// </summary>
	[ClassInterface(ClassInterfaceType.None)] 
	public sealed class CalcFunction:ICalcFunction
	{
		

		Calculator calculator=new Calculator();

		Stack stack=new Stack();

		AbstractScan scan=null;
		Type type=typeof(CalcFunction);

		/// <summary>
        /// 是否使用表达式前缀,usePrefix为true,引用其它二级变量前面要加上! 如!kk23.e4 或!kk-kk.e4
		/// </summary>
        /// <param name="usePrefix"></param>
        public CalcFunction(bool usePrefix)
		{
            scan = ScanFactory.createScan(usePrefix);
		}

        /// <summary>
        /// 引用其它仪器的数据默认方式，不使用!前缀
        /// </summary>
        public CalcFunction()
        {
            scan = ScanFactory.createScan(false);
        }

/// <summary>
/// 计算带函数的表达式的值
/// </summary>
/// <param name="s"></param>
/// <returns></returns>
	
		public double compute(string s)
		{
			if(s.Trim().Length==0)
			{
				throw new Exception("表达式为空");
			}
			LinkList list=scan.parse(s);

//			int [,]priorities=Calculator.priorities;
//			for(int i=0;i<6;i++)
//			{
//				for(int j=0;j<6;j++)
//				{
//					Console.Write(priorities[i,j].ToString());
//				}
//				Console.WriteLine();
//			}

			return calculator.compute(calcFuns(list));
		}

		/// <summary>
		/// 得到计算表达式的所有变量名称
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public ArrayList getVaribles(string s)
		{
			if(s.Trim().Length==0)
			{
				throw new Exception("表达式为空");
			}
			ArrayList array=new ArrayList(8);
			

			LinkList list=scan.parse(s);

			LinkNode node=list.First;
			while(node!=null)
			{
				if(node.getWord().wordType==WordType.Identifier||node.getWord().wordType==WordType.IdentifierWithDot)
				{
					array.Add(node.getWord().valueString);
				}

				node=node.Next;
			}

			return array;
		}

		/// <summary>
		/// 算带函数的表达式的值
		/// </summary>
		/// <param name="s">公式字符串</param>
		/// <param name="slist">参数列表</param>
		/// <returns></returns>
		public double compute(string s,MyList slist)
		{
			if(s.Trim().Length==0)
			{
				throw new Exception("表达式为空");
			}
			LinkList list=scan.parse(s);
			list.replaceVariable(slist);

			return calculator.compute(calcFuns(list));
		}

	
	
		/// <summary>
		/// 计算中缀表达式中的所有函数
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		private LinkList calcFuns(LinkList list)
		{
			//遍历整个链表

			
			ArrayList paramsList=new ArrayList(5);
			LinkNode funNode=null,insertNode=null,nextTempNode=null;
			for(LinkNode node=list.First;node!=null;node=nextTempNode)
			{
				nextTempNode=node.Next;//node用于循环，而每次计算里都要将)删除，所有提前保存node的下一个结点的值
				


				if(node.getWord().wordType==WordType.Leftp)
					stack.Push(node);
				else if(node.getWord().wordType==WordType.Rightp)
				{
					if(stack.Count<1)
						throw new Exception("括号不匹配，缺左括号");
					else if(((LinkNode)stack.Peek()).Previous==null||((LinkNode)stack.Peek()).Previous.getWord().wordType!=WordType.FunName)
					{
						//括号只是一个表达式，而不属于一个函数
						//条件是括号前面不是函数名，或括号前面没有元素
						stack.Pop();
						continue;

					}
					else 
					{

						paramsList.Clear();
						
						
						LinkNode start=(LinkNode)stack.Pop();
						funNode=start.Previous;
						insertNode=node.Next;
						list.removeBetween(start,node);
						LinkList innerList=new LinkList(start,node);//还包含括号

						//第一种情况，括号里面没有任何参数
						if(innerList.Count==2)//判断括号里是否为空
							
						{
							//计算函数的值
							
							double result=Function(funNode.getWord().valueString,paramsList);
							

							list.remove(funNode);
							list.insert(insertNode,new LinkNode(new Word(WordType.Number,result.ToString())));

							continue;
						}
						
						else
						{

							innerList.remove(start);//删除括号
							innerList.remove(node);
							
							LinkNode comma=null;
							comma=innerList.searchComma();

							//第二种情况，括号里只有一个参数（参数表达式）
							if(comma==null)//没有逗号
							{
								
								paramsList.Add(calculator.compute(innerList));

								//根据参数计算函数的值
								double result=Function(funNode.getWord().valueString,paramsList);

								list.remove(funNode);
								list.insert(insertNode,new LinkNode(new Word(WordType.Number,result.ToString())));


								continue;

							}
							else　/////第三种情况，括号里有多个参数表达式
							{


								
								while(comma!=null)
								{
									LinkNode newStart=innerList.First;
									innerList.removeBetween(newStart,comma);
									LinkList tempList=new LinkList(newStart,comma);
									
									if(tempList.Count<2)
									{
										throw new Exception("逗号前面有问题");
									}
									tempList.remove(tempList.End);//删除逗号


									paramsList.Add(calculator.compute(tempList));

									comma=innerList.searchComma();
																
								}

								if(innerList.Count==0)
								{
									throw new Exception("函数的逗号后面有问题");
								}
								else
								{
									
									paramsList.Add(calculator.compute(innerList));
									//用多个参数计算函数

									double result=Function(funNode.getWord().valueString,paramsList);
									
									list.remove(funNode);
									list.insert(insertNode,new LinkNode(new Word(WordType.Number,result.ToString())));


									
								}
							}

						}

					}
				}

				


				

			}


			return list;

		}


		/// <summary>
		/// 函数计算
		/// </summary>
		/// <param name="name"></param>
		/// <param name="array"></param>
		/// <returns></returns>
        

		private double Function(string name,ArrayList array)
		{


			string fname="f_"+name.ToLower();
			
			
			MethodInfo info=type.GetMethod(fname);

			if(info==null)
				throw new Exception(name+"函数还没有实现");

			

			return (double)info.Invoke(this,new object[]{array});
		}

		/// <summary>
		/// max计算函数
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("MAX(number1,number2,...)","返回一组值中的最大值。",
			 "Number1, number2, ...    是要从中找出最大值的数字参数。")]
		public double f_max(ArrayList array)
		{
			if(array.Count==0)
				throw new Exception("max函数的参数错误");

			double r=(double)array[0];

			for(int i=1;i<array.Count;i++)
			{
				double temp=(double)array[i];
				if(temp>r)
					r=temp;
			}

		
			return r;

		
		}


		/// <summary>
		/// 计算PI的值
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		[Desp("PI( )","返回数字 3.14159265358979，即数学常量 pi。",
			 "")]
		public double f_pi(ArrayList array)
		{
			if(array.Count!=0)
				throw new Exception("PI函数的参数错误");

			return Math.PI;


		}

		/// <summary>
		/// sin函数 参数为弧度数
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("SIN(number)","返回给定角度的正弦值。",
			 "Number    为需要求正弦的角度，以弧度表示。")]
		public double f_sin(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("sin函数的参数错误");

			return Math.Sin((double)array[0]);

		}

		/// <summary>
		/// 实现cos
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("COS(number)","返回给定角度的余弦值。",
			 "Number 为需要求余弦的角度，以弧度表示。")]
		public double f_cos(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("cos函数的参数错误");

			return Math.Cos((double)array[0]);

		}
		


		/// <summary>
		/// 求和函数
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>


		[Desp("SUM(number1,number2, ...)","返回某一单元格区域中所有数字之和。",
			 "Number1, number2, ...    为需要求和的参数。")]
		public double f_sum(ArrayList array)
		{
			if(array.Count==0)
				throw new Exception("sum函数的参数错误");

			double sum=0;
			for(int i=0;i<array.Count;i++)
			{
				sum+=(double)array[i];
				

			}

			return sum;

		}



		/// <summary>
		/// 求平均值
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("AVERAGE(number1,number2,...)","返回参数的平均值（算术平均值）。",
			 "Number1, number2, ...    为需要计算平均值的参数。")]
		public double f_average(ArrayList array)
		{
			if(array.Count==0)
				throw new Exception("average函数的参数错误");

			double average=0;
			for(int i=0;i<array.Count;i++)
			{
				average+=(double)array[i];
				

			}

			return average/array.Count;

		}

		/// <summary>
		/// 求最小值
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("MIN(number1,number2,...)","返回一组值中的最小值。",
			 "Number1, number2,...    是要从中找出最小值的数字参数。")]

		public double f_min(ArrayList array)
		{
			if(array.Count==0)
				throw new Exception("min函数的参数错误");

			double r=(double)array[0];

			for(int i=1;i<array.Count;i++)
			{
				double temp=(double)array[i];
				if(temp<r)
					r=temp;
			}

		
			return r;

		
		}
		

		/// <summary>
		/// 求正切
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("TAN(number)","返回给定角度的正切值。",
			 "Number    为要求正切的角度，以弧度表示。")]
	

		public double f_tan(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("tan函数的参数错误");

			return Math.Tan((double)array[0]);

		}

		/// <summary>
		/// 求绝对值
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>


		[Desp("ABS(number)","返回数字的绝对值。绝对值没有符号。","Number   需要计算其绝对值的实数。")]

		public double f_abs(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("abs函数的参数错误");

			double r=(double)array[0];

			if(r>=0)
				return r;
			else return -r;

		}


		
		/// <summary>
		/// 求反余弦
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("ACOS(number)","返回数字的反余弦值。反余弦值是角度，它的余弦值为数字。返回的角度值以弧度表示，范围是 0 到 pi。"
			 ,"Number    角度的余弦值，必须介于 -1 到 1 之间。")]

		public double f_acos(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("acos函数的参数错误");

			return Math.Acos((double)array[0]);

		}


		/// <summary>
		/// 求反正弦
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		[Desp("ASIN(number)","返回参数的反正弦值。反正弦值为一个角度，该角度的正弦值即等于此函数的 number 参数。返回的角度值将以弧度表示，范围为 -pi/2 到 pi/2。",
			 "Number    角度的正弦值，必须介于 -1 到 1 之间。")]

		public double f_asin(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("asin函数的参数错误");

			return Math.Asin((double)array[0]);

		}



		/// <summary>
		/// 求反正切
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("ATAN (number)","返回反正切值。反正切值为角度，其正切值即等于 number 参数值。返回的角度值将以弧度表示，范围为 -pi/2 到 pi/2。",
			 "Number    角度的正切值。")]
		public double f_atan(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("atan函数的参数错误");

			return Math.Atan((double)array[0]);

		}

		/// <summary>
		/// 根据指定的X轴及Y轴坐标值的，返回反正切值。
		/// 返回值在-Pi到Pi之间，不包括-Pi
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("ATAN2(x_num,y_num)","返回给定的 X 及 Y 坐标值的反正切值。反正切的角度值等于 X 轴与通过原点和给定坐标点 (x_num, y_num) 的直线之间的夹角。结果以弧度表示并介于 -pi 到 pi 之间（不包括 -pi）。",
			 "X_num    点的 X 坐标。Y_num    点的 Y 坐标。")]
		public double f_atan2(ArrayList array)
		{
			if(array.Count!=2)
				throw new Exception("atan2函数的参数错误");

			return Math.Atan2((double)array[1],(double)array[0]);

		}

		/// <summary>
		/// 返回反双曲余弦值
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>


		[Desp("ACOSH(number)","返回 number 参数的反双曲余弦值。参数必须大于或等于 1。反双曲余弦值的双曲余弦即为该函数的参数，因此 ACOSH(COSH(number)) 等于 number。",
			 "Number    大于等于 1 的实数。")]
		public double f_cosh(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("cosh函数的参数错误");

			return Math.Cosh((double)array[0]);

		}


		/// <summary>
		/// 阶乘
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("FACT(number)","返回数的阶乘，一个数的阶乘等于 1*2*3*...* 该数。",
			 "Number    要计算其阶乘的非负数。如果输入的 Number 不是整数，则截尾取整。")]
		public double f_fact(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("fact函数的参数错误");

			int fact=(int)(double)array[0],result=1;

			if(fact<0)
				throw new Exception("阶乘函数fack的参数不能是负数");

			if(fact==0)return 1;

			while(fact!=0)
			{
				result*=fact--;
			}


			return result;


			
		}


		/// <summary>
		/// 将数字向下取整
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("INT(number)","将数字向下舍入到最接近的整数。",
			 "Number    需要进行向下舍入取整的实数。")]

		public double f_int(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("int函数的参数错误");

			return (int)(double)array[0];
		}


		/// <summary>
		/// 求自然对数
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>


		[Desp("LN(number)","返回一个数的自然对数。自然对数以常数项 e (2.71828182845904) 为底。",
			 "Number    是用于计算其自然对数的正实数。")]
		public double f_ln(ArrayList array)
		{

			if(array.Count!=1)
				throw new Exception("ln函数的参数错误");

			double r=(double)array[0];

			if(r<=0)
			{
				throw new Exception("ln函数的参数必须大于零");
			}


			return Math.Log(r,Math.E);
		}




		/// <summary>
		/// 返回常数e
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		
		[Desp("E()","返回常数E。",
			 "")]
		public double f_e(ArrayList array)
		{
			if(array.Count!=0)
				throw new Exception("e()函数的参数个数必须为0");

			return Math.E;
		}



		/// <summary>
		/// 根据底数返回数字的对数
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("LOG(number,base)","按所指定的底数，返回一个数的对数。",
			 "Number    为用于计算对数的正实数。Base    为对数的底数。如果省略底数，假定其值为10。")]
		public double f_log(ArrayList array)
		{

			if(array.Count!=2)
				throw new Exception("log函数的参数必须为二个");

			double r1=(double)array[0],r2=(double)array[1];

			if(r1<=0||r2<=0)
				throw new Exception("log函数的参数必须大于0");

			return Math.Log(r1,r2);

		}

		/// <summary>
		/// 两数相除取余数
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("MOD(number,divisor)","返回两数相除的余数。结果的正负号与除数相同。",
			 "Number    为被除数。Divisor    为除数。")]
		public double f_mod(ArrayList array)
		{
			if(array.Count!=2)
				throw new Exception("mod函数的参数必须为二个");

			int r1=(int)(double)array[0],r2=(int)(double)array[1];

			return r1%r2;
		}

		/// <summary>
		/// 求两数的乘幂
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		[Desp("POWER(number,power)","返回给定数字的乘幂。",
			 "Number    底数，可以为任意实数。Power    指数，底数按该指数次幂乘方。")]
		public double f_power(ArrayList array)
		{
			if(array.Count!=2)
				throw new Exception("power函数的参数必须为二个");

			double r1=(double)array[0],r2=(double)array[1];

			return Math.Pow(r1,r2);
		}




		/// <summary>
		/// 求双曲正弦值
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		
		[Desp("SINH(number)","返回某一数字的双曲正弦值。",
			 "Number    为任意实数。")]
		public double f_sinh(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("sinh函数的参数必须为一个");

			double r1=(double)array[0];

			return Math.Sinh(r1);

		}

		/// <summary>
		/// 求双曲正切值
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		[Desp("TANH(number)","返回某一数字的双曲正切。",
			 "Number    为任意实数。")]
		public double f_tanh(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("tanh函数的参数必须为一个");

			double r1=(double)array[0];
			

			return Math.Tanh(r1);

		}



		/// <summary>
		/// 按指定的位数对数值进行四舍五入
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		[Desp("ROUND(number,num_digits)","返回某个数字按指定位数取整后的数字。",
			 "Number    需要进行四舍五入的数字。Num_digits    指定的位数，按此位数进行四舍五入。")]
		public double f_round(ArrayList array)
		{
			if(array.Count!=2)
				throw new Exception("round函数的参数必须为二个");

			double r1=(double)array[0];
			int r2=(int)(double)array[1];

			return Math.Round(r1,r2);
		}


		/// <summary>
		/// 进行开方运算
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		
		[Desp("SQRT(number)","返回正平方根。",
			 "Number    要计算平方根的数。")]
		public double f_sqrt(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("sqrt函数的参数必须为一个");

			double r1=(double)array[0];
			if(r1<0)throw new Exception("sqrt的参数必须大于零");

			return Math.Sqrt(r1);
		}


	

		/// <summary>
		/// e的幂运算
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		[Desp("exp(number)","返回自然对数e的number次方。",
			 "Number    幂指数。")]
		public double f_exp(ArrayList array)
		{
			if(array.Count!=1)
				throw new Exception("exp函数的参数必须为一个");
			double r1=(double)array[0];
			

			return Math.Pow(Math.E,r1);



		}

		/// <summary>
		/// 如果number1大于number2 返回1,否则返回0
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("big(number1,number2)","如果number1>number2 返回1,否则返回0",
			 "number1,number2 要比较的两个数")]
		public double f_big(ArrayList array)
		{
			if(array.Count!=2)
				throw new Exception("big函数的参数必须为二个");

			double r1=(double)array[0],r2=(double)array[1];

			return r1>r2?1:0;
		}

		
		/// <summary>
		/// 如果number1大于等于number2 返回1,否则返回0
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("bigeq(number1,number2)","如果number1>number2 返回1,否则返回0",
			 "number1,number2 要比较的两个数")]
		public double f_bigeq(ArrayList array)
		{
			if(array.Count!=2)
				throw new Exception("bigeq函数的参数必须为二个");

			double r1=(double)array[0],r2=(double)array[1];

			return r1>=r2?1:0;
		}

		/// <summary>
		/// 如果number1小于number2 返回1,否则返回0
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("sml(number1,number2)","如果number1<number2 返回1,否则返回0",
			 "number1,number2 要比较的两个数")]
		public double f_sml(ArrayList array)
		{
			if(array.Count!=2)
				throw new Exception("sml函数的参数必须为二个");

			double r1=(double)array[0],r2=(double)array[1];

			return r1<r2?1:0;
		}

		/// <summary>
		/// 如果number1小于等于number2 返回1,否则返回0
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("smleq(number1,number2)","如果number1<number2 返回1,否则返回0",
			 "number1,number2 要比较的两个数")]
		public double f_smleq(ArrayList array)
		{
			if(array.Count!=2)
				throw new Exception("smleq函数的参数必须为二个");

			double r1=(double)array[0],r2=(double)array[1];

			return r1<=r2?1:0;
		}
		/// <summary>
		/// 如果number1介于number2和number3之间 返回1,否则返回0
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("bet(number1,number2)","如果number1介于number2和number3之间 返回1,否则返回0",
			 "number1,number2,number3 要比较的三个数")]
		public double f_bet(ArrayList array)
		{
			if(array.Count!=3)
				throw new Exception("bet函数的参数必须为三个");

			double r1=(double)array[0],r2=(double)array[1],r3=(double)array[2];

			double t=0.0;
			if(r2>r3)
			{
				t=r2;
				r2=r3;
				r3=t;
			}

			if(r1>r2&&r1<r3)return 1;
			else return 0;


		}

		/// <summary>
		/// 如果number1介于number2和number3之间,包括等于 返回1,否则返回0
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>

		[Desp("beteq(number1,number2,number3)","如果number1介于number2和number3之间 返回1,否则返回0",
			 "number1,number2,number3 要比较的三个数")]
		public double f_beteq(ArrayList array)
		{
			if(array.Count!=3)
				throw new Exception("beteq函数的参数必须为三个");

			double r1=(double)array[0],r2=(double)array[1],r3=(double)array[2];

			double t=0.0;
			if(r2>r3)
			{
				t=r2;
				r2=r3;
				r3=t;
			}

			if(r1>=r2&&r1<=r3)return 1;
			else return 0;


		}




		}
}
