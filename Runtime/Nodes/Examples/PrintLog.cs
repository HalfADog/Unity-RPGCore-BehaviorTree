using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;
using RPGCore.BehaviorTree;

[BTNode("Action/PrintLog", "��ӡ��־��Ϣ")]
public class PrintLog : BTNodeAction
{
	public string logMsg;

	public override NodeResult Execute()
	{
		Debug.Log(logMsg);
		return NodeResult.success;
	}
}