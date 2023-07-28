using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	[BTNode("Control/Sequence")]
	public class BTNodeSequence : BTNodeControl
	{
		public override void Enter()
		{
			ResetChild();
		}

		public override NodeResult Execute()
		{
			while (!GetNextChild())
			{
				if (currentChild.nodeState == BTNodeState.Succeed)
				{
					continue;
				}
				if (currentChild.nodeState == BTNodeState.Failed)
				{
					return NodeResult.failed;
				}
				return currentChild.runningNodeState;
			}
			return NodeResult.success;
		}
	}
}