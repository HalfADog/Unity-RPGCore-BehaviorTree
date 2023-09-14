using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;
using RPGCore.BehaviorTree;

[BTNode("Action/Set Animation", "…Ë÷√∂Øª≠")]
public class SetAnimation : BTNodeAction
{
	public string animationName;
	private AnimationTransition transition;

	private void Awake()
	{
		transition = GetComponent<AnimationTransition>();
	}

	public override NodeResult Execute()
	{
		if (transition.currentClipName != animationName)
		{
			transition.BeginTransition(animationName);
		}
		return NodeResult.success;
	}
}