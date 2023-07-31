using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Blackboard
{
	/// <summary>
	/// 浮点型blackboard变量
	/// </summary>
	public class FloatVariable : Variable<float>
	{
	}

	/// <summary>
	/// 存储float常量值或引用float blackboard变量
	/// </summary>
	[System.Serializable]
	public class FloatReference : VariableReference<FloatVariable, float>
	{
		public FloatReference(bool constant = true)
		{
			useConstant = constant;
		}

		public FloatReference(float constantV)
		{
			useConstant = true;
			value.Value = constantV;
		}

		public float Value
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