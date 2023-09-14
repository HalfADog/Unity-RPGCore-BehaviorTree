using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;
using RPGCore.BehaviorTree;
using UnityEngine.AI;

[BTNode("Action/Stop NavMeshAgent", "ֹͣ�ƶ�")]
public class StopNavMeshAgent : BTNodeAction
{
	private NavMeshAgent agent;

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	public override NodeResult Execute()
	{
		if (agent) agent.SetDestination(transform.position);
		return NodeResult.success;
	}
}