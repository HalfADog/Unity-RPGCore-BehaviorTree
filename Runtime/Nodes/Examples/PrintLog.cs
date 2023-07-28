using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;

public class PrintLog : BTNodeAction
{
	public override NodeResult Execute()
	{
		Debug.Log("PrintLog Execute!");
		return NodeResult.success;
	}
}