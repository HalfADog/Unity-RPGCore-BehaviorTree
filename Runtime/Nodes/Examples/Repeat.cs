using RPGCore.BehaviorTree;
using RPGCore.BehaviorTree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BTNode("Decorator/Repeat", "�ظ�ִ��ָ������")]
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
			//���Լ��������ӽڵ��״̬ȫ���ָ���Ĭ��
			//��������ӽڵ㲻ִ��ֱ�ӷ��ص����
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