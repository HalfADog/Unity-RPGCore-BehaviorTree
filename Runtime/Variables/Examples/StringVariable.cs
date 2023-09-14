using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Variable
{
	public class StringVariable : Variable<string>
	{
	}

	/// <summary>
	/// �洢bool����ֵ������bool blackboard����
	/// </summary>
	[System.Serializable]
	public class StringReference : VariableReference<StringVariable, string>
	{
		public StringReference(bool constant = true)
		{
			useConstant = constant;
		}

		public string Value
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