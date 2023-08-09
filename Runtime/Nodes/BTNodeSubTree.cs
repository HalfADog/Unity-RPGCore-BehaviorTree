using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	[BTNode("Action/Sub Tree", "ִ������")]
	public class BTNodeSubTree : BTNodeBase
	{
		public BehaviorTree subTree;

		public BTNodeSubTree()
		{
			nodeName = "Sub Tree";
			nodeType = BTNodeType.Action;
		}

		public override NodeResult Execute()
		{
			return NodeResult.success;
		}
	}
}