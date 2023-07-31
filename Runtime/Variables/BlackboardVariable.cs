using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Variable
{
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