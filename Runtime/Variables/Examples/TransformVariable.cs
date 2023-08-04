using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Variable
{
	public class TransformVariable : Variable<Transform>
	{
	}

	/// <summary>
	/// 存储Transform或引用Transform blackboard变量
	/// </summary>
	[System.Serializable]
	public class TransformReference : VariableReference<TransformVariable, Transform>
	{
		public TransformReference(bool constant = true)
		{
			useConstant = constant;
		}

		public Transform Value
		{
			get
			{
				return (isConstant) ? constantValue : this.GetVariable().Value;
			}
			set
			{
				if (isConstant)
				{
					constantValue = value;
				}
				else
				{
					this.GetVariable().Value = value;
				}
			}
		}
	}
}