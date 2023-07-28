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
		//��Ͻڵ�
		Sequence, //˳��ڵ� ���гɹ���ɹ� ����ʧ��

		Select, //ѡ�񣨱�ѡ���ڵ�  ��һ���ɹ���ɹ� ����ʧ����ʧ��
		Parallel, //���нڵ� ִ�������ӽڵ� ��������N���ӽڵ㷵�سɹ��򷵻سɹ� �������ӽڵ㷵��ʧ����ʧ��

		//���νڵ�
		Decorator, //װ�����ڵ� ���ӽڵ�ķ��ؽ���������� ����ʵ��repeat��invert��timeout�ڵ��

		//Ҷ�ӽڵ�
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

	public abstract class BTNodeBase : MonoBehaviour
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
		public BTNodeState nodeState = BTNodeState.Noone;

		/// <summary>
		/// �ڵ������з��ص�״̬
		/// </summary>
		public NodeResult runningNodeState;

		/// <summary>
		/// ��ǰ�ڵ���ӽڵ�
		/// </summary>
		public List<BTNodeBase> childNodes = new List<BTNodeBase>();

		/// <summary>
		/// ��ǰ�ڵ�ĸ��ڵ�
		/// </summary>
		public BTNodeBase parentNode;

		/// <summary>
		/// ��ǰ�ڵ������е�Ŀ����
		/// </summary>
		public BehaviorTree targetTree;

		/// <summary>
		/// �ڵ����ȼ� Խ�ӽ�0 ���ȼ�Խ��
		/// </summary>
		public int nodePriority;

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

#if UNITY_EDITOR

		/// <summary>
		/// �ӵ�ǰ��Ϊ����ɾ���ڵ� EditorOnly
		/// </summary>
		public void DeleteNode()
		{
			DestroyImmediate(this);
		}

#endif

		#endregion ����
	}
}