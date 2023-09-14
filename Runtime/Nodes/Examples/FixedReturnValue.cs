using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;
using RPGCore.BehaviorTree;

[BTNode("Decorator/Fixed Return Value", "返回一个固定的值")]
public class FixedReturnValue : BTNodeDecorator
{
	public ReturnValue fixedValue;

	public override NodeResult Decorate(NodeResult result)
	{
		if (fixedValue == ReturnValue.Success)
		{
			return NodeResult.success;
		}
		return NodeResult.failed;
	}

	public enum ReturnValue
	{
		Success,
		Fail
	}
}