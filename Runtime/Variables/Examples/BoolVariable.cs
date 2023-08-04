using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Variable
{
	public class BoolVariable : Variable<bool>
	{
	}

	/// <summary>
	/// 存储bool常量值或引用bool blackboard变量
	/// </summary>
	[System.Serializable]
	public class BoolReference : VariableReference<BoolVariable, bool>
	{
		public BoolReference(bool constant = true)
		{
			useConstant = constant;
		}

		public bool Value
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