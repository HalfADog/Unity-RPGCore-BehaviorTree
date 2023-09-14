using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;
using RPGCore.BehaviorTree;
using RPGCore.BehaviorTree.Variable;
using UnityEngine.AI;

[BTNode("Action/MoveTo", "向给定朝向移动,会自动避障")]
public class MoveTo : BTNodeAction
{
	public RPGCore.BehaviorTree.Variable.TransformReference self = new RPGCore.BehaviorTree.Variable.TransformReference();
	public RPGCore.BehaviorTree.Variable.TransformReference target = new RPGCore.BehaviorTree.Variable.TransformReference();
	public RPGCore.BehaviorTree.Variable.FloatReference speed = new RPGCore.BehaviorTree.Variable.FloatReference();
	public RPGCore.BehaviorTree.Variable.FloatReference stopDistance = new RPGCore.BehaviorTree.Variable.FloatReference();

	private CharacterController characterController;
	private NavMeshAgent meshAgent;

	private void Start()
	{
		//characterController = GetComponent<CharacterController>();
		meshAgent = GetComponent<NavMeshAgent>();
		//meshAgent.isStopped = true;
	}

	public override void Enter()
	{
		meshAgent.isStopped = false;
	}

	public override NodeResult Execute()
	{
		meshAgent.destination = target.Value.position;
		//var moveTarget = meshAgent.path.corners[0] - self.Value.position;
		//characterController.Move(moveTarget);
		if (Vector3.Distance(self.Value.position, target.Value.position) <= stopDistance.Value)
		{
			return NodeResult.success;
		}
		return NodeResult.running;
	}

	public override void Exit()
	{
		meshAgent.isStopped = true;
	}
}