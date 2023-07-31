using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Blackboard
{
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
		public BehaviorTreeBlackboard blackboard;
	}
}