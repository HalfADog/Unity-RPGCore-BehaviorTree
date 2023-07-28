using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree
{
	/// <summary>
	/// ������Ϊ��ִ��
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RPGCore.BehaviorTree.BehaviorTree))]
	public class BehaviorTreeExecutor : MonoBehaviour
	{
		/// <summary>
		/// ����Ϊ�� ��Ϊ��ִ�е����
		/// </summary>
		public BehaviorTree mainTree;

		/// <summary>
		/// ��ǰ���������������Ϊ��
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