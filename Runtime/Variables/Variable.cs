using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Blackboard
{
	/// <summary>
	/// 使用此类来定义一个blackboardvariable
	/// </summary>
	/// <typeparam name="T">该Variable实际存储了什么类型的值</typeparam>
	public abstract class Variable<T> : BlackboardVariable
	{
		//实际值 T可以是值类型和引用类型
		[SerializeField]
		protected T val = default(T);

		public T Value
		{
			get { return val; }
			set { val = value; }
		}
	}
}