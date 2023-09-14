using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;
using RPGCore.BehaviorTree;
using UnityEngine.AI;

[BTNode("Action/Set NMA Desitination", "设置NavMeshAgent的目的地")]
public class SetNMADestination : BTNodeAction
{
	private NavMeshAgent agent;
	public RPGCore.BehaviorTree.Variable.TransformReference target = new RPGCore.BehaviorTree.Variable.TransformReference();

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	public override NodeResult Execute()
	{
		if (agent)
		{
			agent.isStopped = false;
			if (target.Value)
			{
				agent.SetDestination(target.Value.position);
			}
		}
		return NodeResult.success;
	}
}