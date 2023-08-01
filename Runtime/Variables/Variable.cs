using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Variable
{
	/// <summary>
	/// 使用此类来定义一个blackboardvariable
	/// </summary>
	/// <typeparam name="T">该Variable实际存储了什么类型的值</typeparam>
	public abstract class Variable<T> : Variable.BlackboardVariable
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

	/// <summary>
	/// 继承自BlackboardReference
	/// 当要在节点类中访问并修改blackboard存储的值的时候应该继承并使用此类
	/// </summary>
	/// <typeparam name="T">blackboard中存储的Variable类</typeparam>
	/// <typeparam name="U">此Variable类对应的值的实际类型</typeparam>
	public class VariableReference<T, U> : Variable.BlackboardReference where T : Variable.BlackboardVariable
	{
		/// <summary>
		/// T是Variable类 value有一个key 对应了存储于blackboard中的值
		/// </summary>
		protected T value = null;

		/// <summary>
		/// 常量值 意味着不从blackboard中获取T值 而是直接使用对于类型U的常量值
		/// </summary>
		[SerializeField]
		protected U constantValue = default(U);

		/// <summary>
		/// 获取Reference所存储的值
		/// 在blackboard中获取key所代表的variable
		/// </summary>
		/// <returns></returns>
		public T GetVariable()
		{
			if (value != null)
			{
				return value;
			}
			if (blackboard == null || string.IsNullOrEmpty(key))
			{
				return null;
			}
			//根据key在blackboard中获取variable
			value = blackboard.GetVariable<T>(key);
#if UNITY_EDITOR
			if (value == null)
			{
				Debug.LogWarningFormat(blackboard, "Variable '{0}' does not exists on blackboard.", key);
			}
#endif
			return value;
		}
	}

	/// <summary>
	/// 用于节点类访问blackboard中的存储的值
	/// </summary>
	public abstract class BlackboardReference
	{
		/// <summary>
		/// 当前是常量值还是引用值
		/// </summary>
		public bool isConstant = false;

		/// <summary>
		/// 是否可以使用常量值
		/// </summary>
		public bool useConstant = false;

		public string key;
		public Blackboard.BehaviorTreeBlackboard blackboard;
	}

	/// <summary>
	/// 实际保存在blackboard中的值
	/// 节点类需要使用VariableReference来访问
	/// </summary>
	public abstract class BlackboardVariable : MonoBehaviour
	{
		//variable 和 reference之间通过key来一一对应
		public string key;
	}
}