using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	[BTNode("Control/Selector", "OR - ||")]
	public class BTNodeSelector : BTNodeControl
	{
		public BTNodeSelector()
		{
			nodeName = "Selector";
			nodeType = BTNodeType.Select;
		}

		public override void Enter()
		{
			//Debug.Log("Selector Enter!");
			//进入该节点前重置当前子节点
			ResetCurrentChild();
		}

		public override NodeResult Execute()
		{
			while (!GetNextChild())
			{
				if (currentChild.nodeState == BTNodeState.Succeed)
				{
					return NodeResult.success;
				}
				if (currentChild.nodeState == BTNodeState.Failed)
				{
					continue;
				}
				return currentChild.runningNodeState;
			}
			return NodeResult.failed;
		}
	}
}