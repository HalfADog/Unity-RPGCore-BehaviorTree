using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	public abstract class BTNodeLeaf : BTNodeBase
	{
		/// <summary>
		/// 判断传入的节点是否是其兄弟节点即有相同的父节点
		/// </summary>
		/// <returns></returns>
		protected bool IsBrotherNode(BTNodeBase node)
		{
			return node.parentNode == parentNode;
		}
	}
}