using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Variable
{
	public class BoolVariable : Variable<bool>
	{
	}

	/// <summary>
	/// �洢bool����ֵ������bool blackboard����
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