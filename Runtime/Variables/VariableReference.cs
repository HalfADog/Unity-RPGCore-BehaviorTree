using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Blackboard
{
	/// <summary>
	/// �̳���BlackboardReference
	/// ��Ҫ�ڽڵ����з��ʲ��޸�blackboard�洢��ֵ��ʱ��Ӧ�ü̳в�ʹ�ô���
	/// </summary>
	/// <typeparam name="T">blackboard�д洢��Variable��</typeparam>
	/// <typeparam name="U">��Variable���Ӧ��ֵ��ʵ������</typeparam>
	public class VariableReference<T, U> : BlackboardReference where T : BlackboardVariable
	{
		/// <summary>
		/// T��Variable�� value��һ��key ��Ӧ�˴洢��blackboard�е�ֵ
		/// </summary>
		protected T value = null;

		/// <summary>
		/// ����ֵ ��ζ�Ų���blackboard�л�ȡTֵ ����ֱ��ʹ�ö�������U�ĳ���ֵ
		/// </summary>
		[SerializeField]
		protected U constantValue = default(U);

		/// <summary>
		/// ��ȡReference���洢��ֵ
		/// ��blackboard�л�ȡkey�������variable
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
			//����key��blackboard�л�ȡvariable
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
}