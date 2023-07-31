using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;
using RPGCore.BehaviorTree;

[BTNode("Example/PrintLog", "打印日志信息")]
public class PrintLog : BTNodeAction
{
	public override NodeResult Execute()
	{
		Debug.Log("PrintLog Execute! " + Time.time);
		return NodeResult.success;
	}
}