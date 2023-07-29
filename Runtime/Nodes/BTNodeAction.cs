using RPGCore.BehaviorTree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	public abstract class BTNodeAction : BTNodeLeaf
	{
		public BTNodeAction()
		{
			nodeType = BTNodeType.Action;
		}
	}
}