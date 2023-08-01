using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Variable
{
	/// <summary>
	/// ʹ�ô���������һ��blackboardvariable
	/// </summary>
	/// <typeparam name="T">��Variableʵ�ʴ洢��ʲô���͵�ֵ</typeparam>
	public abstract class Variable<T> : Variable.BlackboardVariable
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

	/// <summary>
	/// �̳���BlackboardReference
	/// ��Ҫ�ڽڵ����з��ʲ��޸�blackboard�洢��ֵ��ʱ��Ӧ�ü̳в�ʹ�ô���
	/// </summary>
	/// <typeparam name="T">blackboard�д洢��Variable��</typeparam>
	/// <typeparam name="U">��Variable���Ӧ��ֵ��ʵ������</typeparam>
	public class VariableReference<T, U> : Variable.BlackboardReference where T : Variable.BlackboardVariable
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
		public Blackboard.BehaviorTreeBlackboard blackboard;
	}

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