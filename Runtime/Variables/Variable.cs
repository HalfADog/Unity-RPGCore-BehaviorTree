using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Blackboard
{
	/// <summary>
	/// ʹ�ô���������һ��blackboardvariable
	/// </summary>
	/// <typeparam name="T">��Variableʵ�ʴ洢��ʲô���͵�ֵ</typeparam>
	public abstract class Variable<T> : BlackboardVariable
	{
		//ʵ��ֵ T������ֵ���ͺ���������
		[SerializeField]
		protected T val = default(T);

		public T Value
		{
			get { return val; }
			set { val = value; }
		}
	}
}