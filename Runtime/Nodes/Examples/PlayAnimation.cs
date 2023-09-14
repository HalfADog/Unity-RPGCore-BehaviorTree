using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;
using RPGCore.BehaviorTree;
using RPGCore.BehaviorTree.Variable;

[BTNode("Action/Play Animation", "播放指定动画")]
public class PlayAnimation : BTNodeAction
{
	private AnimPlayerManager animManager;
	public StringReference animName = new StringReference();
	public BoolReference abort = new BoolReference();

	private void Start()
	{
		animManager = GetComponent<AnimPlayerManager>();
	}

	public override void Enter()
	{
		Debug.Log(animName.Value);
		animManager.BeginTransition(animName.Value);
		animManager.SetCurrentAnimAbortState(abort.Value);
	}

	public override NodeResult Execute()
	{
		if (animManager.AnimFinishPlay)
		{
			return NodeResult.success;
		}
		return NodeResult.running;
	}
}