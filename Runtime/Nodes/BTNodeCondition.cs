using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	/// <summary>
	/// �жϲ�������
	/// </summary>
	public enum AbortType
	{
		Self,
		LowPriority,
		Both,
		Noone
	}

	public abstract class BTNodeCondition : BTNodeLeaf
	{
		[HideInInspector] public bool lastCheckResult = false;

		/// <summary>
		/// �洢��ǰ�ڵ�ִ��ʱ��ִ��ջ״̬
		/// </summary>
		[HideInInspector] public BTNodeBase[] stackTreeSnapshot = new BTNodeBase[0];

		/// <summary>
		/// �жϲ���������
		/// </summary>
		public AbortType abortType = AbortType.Noone;

		public BTNodeCondition()
		{
			nodeType = BTNodeType.Condition;
		}

		public override void Enter()
		{
			//���û�д洢��ǰ��ִ��ջ״̬�ʹ洢
			//������ִ�д�ϲ���ʱ��ִ��ջ�ָ����˽ڵ�ִ��ʱ
			if (abortType != AbortType.Noone && stackTreeSnapshot.Length == 0)
			{
				targetTree.GetStack(ref stackTreeSnapshot);
				//Debug.Log(stackTreeSnapshot.Length);
			}
		}

		public override NodeResult Execute()
		{
			lastCheckResult = Check();
			if (!lastCheckResult)
			{
				return NodeResult.failed;
			}
			return NodeResult.success;
		}

		public virtual bool Check()
		{
			return true;
		}

		/// <summary>
		///	�Ƿ��ܹ�ִ�д�ϲ���
		/// </summary>
		/// <returns></returns>
		public bool IsCanAbort(BTNodeBase runningNode)
		{
			bool isBortherNode = IsBrotherNode(runningNode);
			//�����ǰ��AbortTypeΪSelf ��Ҫ��ǰRunning�Ľڵ����ֵܽڵ�
			//�����ǰ��AbortTypeΪLowPriority ��Ҫ��ǰRunning�Ľڵ��Ƿ��ֵܽڵ�
			//���򷵻�ʧ��
			if ((abortType == AbortType.Self && !isBortherNode) ||
				(abortType == AbortType.LowPriority && isBortherNode))
			{
				return false;
			}
			//������ϴβ�ͬ����ִ�д�ϲ���
			bool c = Check();
			if (c != lastCheckResult)
			{
				lastCheckResult = c;
				return true;
			}
			return false;
		}

		/// <summary>
		/// ��ȡ��ǰ�ڵ�ִ�е�ʱ��洢��ִ����״̬
		/// </summary>
		/// <returns></returns>
		public BTNodeBase[] GetStoredTreeSnapshot()
		{
			return stackTreeSnapshot;
		}
	}
}