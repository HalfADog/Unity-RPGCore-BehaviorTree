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
			//����ýڵ�ǰ���õ�ǰ�ӽڵ�
			ResetCurrentChild();
		}

		public override NodeResult Execute()
		{
			while (existNextChild)
			{
				if (currentChild.nodeState == BTNodeState.Succeed)
				{
					return NodeResult.success;
				}
				if (currentChild.nodeState == BTNodeState.Failed)
				{
					GetNextChild();
					continue;
				}
				return currentChild.runningNodeState;
			}
			return NodeResult.failed;
		}
	}
}