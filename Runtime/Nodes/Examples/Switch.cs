using RPGCore.BehaviorTree;
using RPGCore.BehaviorTree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BTNode("Example/Switch", "一个开关，条件节点测试节点")]
public class Switch : BTNodeCondition
{
	public bool state;

	public override bool Check()
	{
		return state;
	}
}