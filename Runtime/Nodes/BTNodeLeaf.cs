using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	public abstract class BTNodeLeaf : BTNodeBase
	{
		/// <summary>
		/// �жϴ���Ľڵ��Ƿ������ֵܽڵ㼴����ͬ�ĸ��ڵ�
		/// </summary>
		/// <returns></returns>
		protected bool IsBrotherNode(BTNodeBase node)
		{
			return node.parentNode == parentNode;
		}
	}
}