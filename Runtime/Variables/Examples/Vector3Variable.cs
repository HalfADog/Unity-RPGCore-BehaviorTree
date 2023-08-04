using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Variable
{
	public class Vector3Variable : Variable<Vector3>
	{
	}

	/// <summary>
	/// 存储Vector3常量值或引用Vector3 blackboard变量
	/// </summary>
	[System.Serializable]
	public class Vector3Reference : VariableReference<Vector3Variable, Vector3>
	{
		public Vector3Reference(bool constant = true)
		{
			useConstant = constant;
		}

		public Vector3Reference(Vector3 constantV)
		{
			useConstant = true;
			value.Value = constantV;
		}

		public Vector3 Value
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