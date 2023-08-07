using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using RPGCore;

namespace RPGCore.BehaviorTree
{
	/// <summary>
	/// ������Ϊ��ִ��
	/// </summary>
	[RequireComponent(typeof(Blackboard.BehaviorTreeBlackboard))]
	[DisallowMultipleComponent]
	public class BehaviorTreeExecutor : MonoBehaviour
	{
		/// <summary>
		/// ��ǰִ�е���Ϊ�� ��Ϊ��ִ�е����
		/// </summary>
		public BehaviorTree currentExecuteTree;

#if UNITY_EDITOR

		/// <summary>
		/// ��ǰ����Editor�б༭����
		/// </summary>
		public BehaviorTree currentEditorTree;

#endif

		/// <summary>
		/// ��ǰ���������������Ϊ��
		/// </summary>
		public List<BehaviorTree> behaviorTrees = new List<BehaviorTree>();

		/// <summary>
		/// �Ƿ�ִ����
		/// </summary>
		public bool treeExecuteStart = true;

		/// <summary>
		/// �Ƿ���ִͣ����
		/// </summary>
		public bool treeExecutePause = false;

		/// <summary>
		/// �Ƿ�����ִ����
		/// </summary>
		public bool treeRestart = false;

		/// <summary>
		/// ��ǰ�����ϵ���Ϊ���ڰ�ֵ
		/// </summary>
		public Blackboard.BehaviorTreeBlackboard treeBlackboard;

		private void Awake()
		{
			treeBlackboard = GetComponent<Blackboard.BehaviorTreeBlackboard>();
		}

		private void Update()
		{
			if (currentExecuteTree == null) return;
			if (treeExecuteStart && !treeExecutePause)
			{
				currentExecuteTree.Tick();
			}

			if (treeRestart)
			{
				currentExecuteTree.Restart();
				treeRestart = false;
			}
		}

		/// <summary>
		/// ����µ���Ϊ��
		/// </summary>
		public void AddBehaviorTree(string name, bool isEditorTree = false)
		{
			BehaviorTree tree = transform.AddComponent<BehaviorTree>();
			tree.treeName = name;
			tree.executor = this;
			behaviorTrees.Add(tree);
			//�����ǰִ����Ϊ�� ������ӵ�������Ϊ��ǰִ����
			if (currentExecuteTree == null)
			{
				currentExecuteTree = tree;
			}
			if (isEditorTree)
			{
				currentEditorTree = tree;
			}
		}

		/// <summary>
		/// ɾ����Ϊ��
		/// </summary>
		/// <param name="name"></param>
		public void RemoveBehaviorTree(string name)
		{
			int index = behaviorTrees.FindIndex(tree => tree.treeName == name);
			if (index != -1)
			{
				behaviorTrees.RemoveAt(index);
			}
		}
	}
}