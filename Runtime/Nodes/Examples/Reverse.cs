using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;
using RPGCore.BehaviorTree;

[BTNode("Decorator/Reverse", "反转结果")]
public class Reverse : BTNodeDecorator
{
	public override NodeResult Decorate(NodeResult result)
	{
		switch (result.state)
		{
			case RPGCore.BehaviorTree.Nodes.BTNodeState.Succeed:
				return NodeResult.failed;

			case RPGCore.BehaviorTree.Nodes.BTNodeState.Failed:
				return NodeResult.success;
		}
		return result;
	}
}