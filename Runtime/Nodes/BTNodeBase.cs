using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	/// <summary>
	/// �ڵ�����
	/// </summary>
	public enum BTNodeType
	{
		Sequence, //˳��ڵ� ���гɹ���ɹ� ����ʧ��
		Select, //ѡ�񣨱�ѡ���ڵ�  ��һ���ɹ���ɹ� ����ʧ����ʧ��
		Parallel, //���нڵ� ִ�������ӽڵ� ��������N���ӽڵ㷵�سɹ��򷵻سɹ� �������ӽڵ㷵��ʧ����ʧ��
		Decorator, //װ�����ڵ� ���ӽڵ�ķ��ؽ���������� ����ʵ��repeat��invert��timeout�ڵ��
		Action, //�����ڵ� ��Ϊ����Ҷ�ӽڵ� ������ʵ��
		Condition, //�����ڵ� ��Ϊ����Ҷ�ӽڵ� ���������ж�
		Undefine
	}

	/// <summary>
	/// �ڵ�����״̬
	/// </summary>
	public enum BTNodeState
	{
		Succeed,
		Failed,
		Running,
		Noone
	}

	/// <summary>
	/// ʹ�ô��෵�ؽڵ������״̬
	/// </summary>
	public class NodeResult
	{
		public BTNodeState state;
		public BTNodeBase targetNode;

		public NodeResult(BTNodeState nodeState, BTNodeBase targetNode = null)
		{
			this.state = nodeState;
			this.targetNode = targetNode;
		}

		public static readonly NodeResult success = new NodeResult(BTNodeState.Succeed);
		public static readonly NodeResult failed = new NodeResult(BTNodeState.Failed);
		public static readonly NodeResult running = new NodeResult(BTNodeState.Running);
	}

	public abstract class BTNodeBase : MonoBehaviour, IComparable<BTNodeBase>
	{
		#region �ֶ�������

		/// <summary>
		/// �ڵ�����
		/// </summary>
		public BTNodeType nodeType;

		/// <summary>
		/// �ڵ����� ��Ψһ
		/// </summary>
		public string nodeName;

		/// <summary>
		/// ��¼��ǰ�ڵ��״̬
		/// </summary>
		[HideInInspector] public BTNodeState nodeState = BTNodeState.Noone;

		/// <summary>
		/// �ڵ������з��ص�״̬
		/// </summary>

		[HideInInspector] public NodeResult runningNodeState;

		/// <summary>
		/// ��ǰ�ڵ���ӽڵ�
		/// </summary>
		[HideInInspector] public List<BTNodeBase> childNodes = new List<BTNodeBase>();

		/// <summary>
		/// ��ǰ�ڵ�ĸ��ڵ�
		/// </summary>
		[HideInInspector] public BTNodeBase parentNode;

		/// <summary>
		/// ��ǰ�ڵ������е�Ŀ����
		/// </summary>
		[HideInInspector] public BehaviorTree targetTree;

		/// <summary>
		/// �ڵ����ȼ� Խ�ӽ�0 ���ȼ�Խ��
		/// </summary>
		[HideInInspector] public int nodePriority;

		/// <summary>
		///
		/// </summary>
		public float LastTick => targetTree.LastTick;

		/// <summary>
		/// �ڵ����й����� ����ִ��֮���ʱ����
		/// </summary>
		public float DeltaTime => Time.time - targetTree.LastTick;

#if UNITY_EDITOR

		/// <summary>
		/// �ڵ�ͼ����Ϊ���༭���е�λ��
		/// </summary>
		[HideInInspector]
		public Vector2 graphNodePosition;

#endif

		#endregion �ֶ�������

		#region ����

		public BTNodeBase()
		{
			//�ڵ�Ĭ������Ϊ��ǰ�ڵ��������
			nodeName = this.GetType().Name;
		}

		/// <summary>
		/// ÿ���ڵ㿪ʼִ��ǰ�����Enter����
		/// </summary>
		public virtual void Enter()
		{ }

		/// <summary>
		/// ִ�нڵ�
		/// </summary>
		/// <returns>��ǰ�ڵ������״̬</returns>
		public abstract NodeResult Execute();

		/// <summary>
		/// ÿ���ڵ���������ִ�к�����Exit����
		/// </summary>
		public virtual void Exit()
		{ }

		/// <summary>
		/// �������ӽڵ��״̬ȫ������
		/// </summary>
		/// <param name="node"></param>
		public void ResetNodeAndChildNodeState(BTNodeBase node)
		{
			if (node.childNodes.Count > 0)
			{
				foreach (var item in node.childNodes)
				{
					ResetNodeAndChildNodeState(item);
				}
			}
			node.nodeState = BTNodeState.Noone;
		}

		/// <summary>
		/// �ڱ����ʱ�һ�����running״̬����
		/// </summary>
		public virtual void OnBehaviourTreeAbort()
		{
		}

		/// <summary>
		/// �ж��Ƿ��й�ͬ����
		/// </summary>
		/// <param name="other"></param>
		public bool SameParent(BTNodeBase other)
		{
			return parentNode == other.parentNode;
		}

#if UNITY_EDITOR

		/// <summary>
		/// �ӵ�ǰ��Ϊ����ɾ���ڵ� EditorOnly
		/// </summary>
		public void DeleteNode()
		{
			DestroyImmediate(this);
		}

		public int CompareTo(BTNodeBase other)
		{
			return -other.graphNodePosition.x.CompareTo(graphNodePosition.x);
		}

#endif

		#endregion ����
	}
}