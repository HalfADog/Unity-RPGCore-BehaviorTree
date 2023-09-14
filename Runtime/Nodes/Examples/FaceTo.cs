using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;
using RPGCore.BehaviorTree;
using RPGCore.BehaviorTree.Variable;

[BTNode("Action/Face To", "朝向某个目标")]
public class FaceTo : BTNodeAction
{
	public RPGCore.BehaviorTree.Variable.TransformReference self = new RPGCore.BehaviorTree.Variable.TransformReference();
	public RPGCore.BehaviorTree.Variable.TransformReference target = new RPGCore.BehaviorTree.Variable.TransformReference();
	public RPGCore.BehaviorTree.Variable.FloatReference speed = new RPGCore.BehaviorTree.Variable.FloatReference();

	public override NodeResult Execute()
	{
		var tR = Quaternion.LookRotation(target.Value.position - self.Value.position);
		self.Value.rotation = Quaternion.RotateTowards(self.Value.rotation, tR, speed.Value);
		if (Vector3.Dot(self.Value.forward, (target.Value.position - self.Value.position).normalized) > 0.95)
		{
			//Debug.Log(Vector3.Dot(self.Value.forward, (target.Value.position - self.Value.position).normalized));
			return NodeResult.success;
		}
		return NodeResult.running;
	}
}