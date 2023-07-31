using RPGCore.BehaviorTree;
using RPGCore.BehaviorTree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BTNode("Decorator/Repeat", "重复执行指定次数")]
public class Repeat : BTNodeDecorator
{
	public int count = 1;
	private int currentCount = 1;

	public override void Enter()
	{
		currentCount = 1;
	}

	public override NodeResult Decorate(NodeResult result)
	{
		if (result.state == RPGCore.BehaviorTree.Nodes.BTNodeState.Succeed)
		{
			currentCount++;
			//将自己和所有子节点的状态全部恢复到默认
			//避免出现子节点不执行直接返回的情况
			ResetNodeAndChildNodeState(child);
			if (currentCount > count)
			{
				return NodeResult.success;
			}
			return runningChildResult;
		}
		return result;
	}
}