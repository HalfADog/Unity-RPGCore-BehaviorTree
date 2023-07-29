using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	public abstract class BTNodeCondition : BTNodeLeaf
	{
		[HideInInspector]
		public bool lastCheckResult = false;

		public BTNodeCondition()
		{
			nodeType = BTNodeType.Condition;
		}

		public override NodeResult Execute()
		{
			lastCheckResult = Check();
			if (!lastCheckResult)
			{
				return NodeResult.failed;
			}
			return NodeResult.success;
		}

		public virtual bool Check()
		{
			return true;
		}
	}
}