using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using RPGCore;

namespace RPGCore.BehaviorTree
{
	/// <summary>
	/// 控制行为树执行
	/// </summary>
	[RequireComponent(typeof(Blackboard.BehaviorTreeBlackboard))]
	[DisallowMultipleComponent]
	public class BehaviorTreeExecutor : MonoBehaviour
	{
		/// <summary>
		/// 当前执行的行为树 行为树执行的入口
		/// </summary>
		public BehaviorTree currentExecuteTree;

#if UNITY_EDITOR

		/// <summary>
		/// 当前正在Editor中编辑的树
		/// </summary>
		public BehaviorTree currentEditorTree;

#endif

		/// <summary>
		/// 当前对象包含的所有行为树
		/// </summary>
		public List<BehaviorTree> behaviorTrees = new List<BehaviorTree>();

		/// <summary>
		/// 是否执行树
		/// </summary>
		public bool treeExecuteStart = true;

		/// <summary>
		/// 是否暂停执行树
		/// </summary>
		public bool treeExecutePause = false;

		/// <summary>
		/// 是否重新执行树
		/// </summary>
		public bool treeRestart = false;

		/// <summary>
		/// 当前对象上的行为树黑板值
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
		/// 添加新的行为树
		/// </summary>
		public void AddBehaviorTree(string name, bool isEditorTree = false)
		{
			BehaviorTree tree = transform.AddComponent<BehaviorTree>();
			tree.treeName = name;
			tree.executor = this;
			behaviorTrees.Add(tree);
			//如果当前执行树为空 则将新添加的树设置为当前执行树
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
		/// 删除行为树
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