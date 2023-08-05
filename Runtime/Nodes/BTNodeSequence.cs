using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	[BTNode("Control/Sequence", "AND - &&")]
	public class BTNodeSequence : BTNodeControl
	{
		public BTNodeSequence()
		{
			nodeName = "Sequence";
			nodeType = BTNodeType.Sequence;
		}

		public override void Enter()
		{
			//Debug.Log("Sequence Enter!");
			//����ýڵ�ǰ���õ�ǰ�ӽڵ�
			ResetCurrentChild();
		}

		public override NodeResult Execute()
		{
			while (existNextChild)
			{
				if (currentChild.nodeState == BTNodeState.Succeed)
				{
					GetNextChild();
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

		public override void OnBehaviourTreeAbort()
		{
			ResetCurrentChild();
		}
	}
}