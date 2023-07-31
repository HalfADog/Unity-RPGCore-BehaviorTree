using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	public abstract class BTNodeDecorator : BTNodeBase
	{
		public BTNodeBase child;

		/// <summary>
		/// 返回运行状态并把子节点作为target传递
		/// </summary>
		protected NodeResult runningChildResult;

		public BTNodeDecorator()
		{
			nodeType = BTNodeType.Decorator;
		}

		private void Start()
		{
			//Debug.Log("Decorator Awake");
			if (childNodes.Count > 0)
			{
				child = childNodes[0];
				runningChildResult = new NodeResult(RPGCore.BehaviorTree.Nodes.BTNodeState.Running, child);
			}
		}

		public override sealed NodeResult Execute()
		{
			if (child == null) { return NodeResult.failed; }
			NodeResult result = null;
			switch (child.nodeState)
			{
				case BTNodeState.Succeed:
					result = NodeResult.success;
					break;

				case BTNodeState.Failed:
					result = NodeResult.failed;
					break;
			}
			if (result != null)
			{
				return Decorate(result);
			}
			return child.runningNodeState;
		}

		public virtual NodeResult Decorate(NodeResult result)
		{
			return result;
		}
	}
}