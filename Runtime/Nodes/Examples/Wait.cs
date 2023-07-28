using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;

public class Wait : BTNodeAction
{
	public float waitTime = 2;
	private float timer = 0;

	public override void Enter()
	{
		Debug.Log("Wait Enter!");
		timer = 0;
	}

	public override NodeResult Execute()
	{
		if (timer < waitTime)
		{
			timer += this.DeltaTime;
			return NodeResult.running;
		}
		return NodeResult.success;
	}
}