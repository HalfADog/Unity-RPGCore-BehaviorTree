using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;
using RPGCore.BehaviorTree;
using RPGCore.BehaviorTree.Variable;

[BTNode("Action/Wait", "等待指定时间")]
public class Wait : BTNodeAction
{
	public RPGCore.BehaviorTree.Variable.FloatReference waitTime = new RPGCore.BehaviorTree.Variable.FloatReference();
	private float timer = 0;

	public override void Enter()
	{
		timer = 0;
	}

	public override NodeResult Execute()
	{
		if (timer < waitTime.Value)
		{
			timer += this.DeltaTime;
			return NodeResult.running;
		}
		return NodeResult.success;
	}
}