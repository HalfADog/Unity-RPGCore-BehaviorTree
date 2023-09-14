using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	[BTNode("Control/Parallel", "并行节点")]
	public class BTNodeParallel : BTNodeBase
	{
		//记录执行过的节点
		private List<BTNodeBase> executeLog = new List<BTNodeBase>();

		public BTNodeParallel()
		{
			nodeType = BTNodeType.Parallel;
			nodeName = "Parallel";
		}

		public override void Enter()
		{
			for (int i = 0; i < childNodes.Count; i++)
			{
				childNodes[i].Enter();
			}
		}

		public override NodeResult Execute()
		{
			for (int i = 0; i < childNodes.Count; i++)
			{
				if (executeLog.Contains(childNodes[i])) continue;
				var childResult = childNodes[i].Execute();
				childNodes[i].nodeState = childResult.state;
				if (childNodes[i].nodeState == BTNodeState.Failed)
				{
					return NodeResult.failed;
				}
				if (childNodes[i].nodeState == BTNodeState.Succeed)
				{
					executeLog.Add(childNodes[i]);
				}
			}
			Debug.Log(childNodes.Count);
			if (executeLog.Count == childNodes.Count)
			{
				return NodeResult.success;
			}
			return NodeResult.running;
		}

		public override void Exit()
		{
			executeLog.Clear();
			for (int i = 0; i < childNodes.Count; i++)
			{
				if (childNodes[i].nodeState == BTNodeState.Running)
				{
					childNodes[i].Exit();
				}
				childNodes[i].nodeState = BTNodeState.Noone;
			}
		}

		public override void OnBehaviourTreeAbort()
		{
			Exit();
		}
	}
}