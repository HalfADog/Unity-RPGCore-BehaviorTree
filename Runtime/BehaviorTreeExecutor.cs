using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree
{
	/// <summary>
	/// 控制行为树执行
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RPGCore.BehaviorTree.BehaviorTree))]
	public class BehaviorTreeExecutor : MonoBehaviour
	{
		/// <summary>
		/// 主行为树 行为树执行的入口
		/// </summary>
		public BehaviorTree mainTree;

		/// <summary>
		/// 当前对象包含的所有行为树
		/// </summary>
		public List<BehaviorTree> behaviorTrees = new List<BehaviorTree>();

		private void Awake()
		{
			behaviorTrees.AddRange(GetComponents<BehaviorTree>());
		}

		private void Update()
		{
			mainTree.Tick();
		}
	}
}