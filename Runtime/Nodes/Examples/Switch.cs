using RPGCore.BehaviorTree;
using RPGCore.BehaviorTree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BTNode("Example/Switch", "һ�����أ������ڵ���Խڵ�")]
public class Switch : BTNodeCondition
{
	public bool state;

	public override bool Check()
	{
		return state;
	}
}