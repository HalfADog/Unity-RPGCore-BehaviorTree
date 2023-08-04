using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore;
using RPGCore.BehaviorTree.Nodes;
using RPGCore.BehaviorTree;
using Unity.VisualScripting;

namespace RPGCore
{
	[BTNode("Example/CompareDistance", "±È½Ï¾àÀë")]
	public class CompareDistance : BTNodeCondition
	{
		public RPGCore.BehaviorTree.Variable.TransformReference targetA = new BehaviorTree.Variable.TransformReference();
		public RPGCore.BehaviorTree.Variable.TransformReference targetB = new BehaviorTree.Variable.TransformReference();
		public Comparator comparator;
		public RPGCore.BehaviorTree.Variable.FloatReference length = new BehaviorTree.Variable.FloatReference();

		public override bool Check()
		{
			float sqrMagnitude = (targetA.Value.position - targetB.Value.position).sqrMagnitude;
			float dist = length.Value;
			if (comparator == Comparator.GreaterThan)
			{
				return sqrMagnitude > dist * dist;
			}
			else
			{
				return sqrMagnitude < dist * dist;
			}
		}
	}

	public enum Comparator
	{
		GreaterThan, LessThan
	}
}