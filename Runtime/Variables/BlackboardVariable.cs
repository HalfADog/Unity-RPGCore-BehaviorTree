using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Variable
{
	/// <summary>
	/// ʵ�ʱ�����blackboard�е�ֵ
	/// �ڵ�����Ҫʹ��VariableReference������
	/// </summary>
	public abstract class BlackboardVariable : MonoBehaviour
	{
		//variable �� reference֮��ͨ��key��һһ��Ӧ
		public string key;
	}
}