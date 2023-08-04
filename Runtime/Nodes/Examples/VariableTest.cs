using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore.BehaviorTree.Nodes;
using RPGCore.BehaviorTree;
using RPGCore.BehaviorTree.Variable;

[BTNode("Example/Variable Test", "测试所有变量")]
public class VariableTest : BTNodeAction
{
	public RPGCore.BehaviorTree.Variable.FloatReference floatVariable = new RPGCore.BehaviorTree.Variable.FloatReference();
	public RPGCore.BehaviorTree.Variable.Vector3Reference vector3Variable = new RPGCore.BehaviorTree.Variable.Vector3Reference();
	public RPGCore.BehaviorTree.Variable.TransformReference transformVariable = new RPGCore.BehaviorTree.Variable.TransformReference();
	public RPGCore.BehaviorTree.Variable.BoolReference boolVariable = new RPGCore.BehaviorTree.Variable.BoolReference();

	public override NodeResult Execute()
	{
		return NodeResult.success;
	}
}