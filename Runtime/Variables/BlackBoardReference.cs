using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Blackboard
{
	/// <summary>
	/// ���ڽڵ������blackboard�еĴ洢��ֵ
	/// </summary>
	public abstract class BlackboardReference
	{
		/// <summary>
		/// ��ǰ�ǳ���ֵ��������ֵ
		/// </summary>
		public bool isConstant = false;

		/// <summary>
		/// �Ƿ����ʹ�ó���ֵ
		/// </summary>
		public bool useConstant = false;

		public string key;
		public BehaviorTreeBlackboard blackboard;
	}
}