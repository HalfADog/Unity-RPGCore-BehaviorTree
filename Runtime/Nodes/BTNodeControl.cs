using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	/// <summary>
	/// 控制节点执行顺序
	/// </summary>
	public enum Dirction
	{
		Normal,//bottom to top
		Reverse,//top to bottom
		Random
	}

	/// <summary>
	/// 中断操作类型
	/// </summary>
	public enum AbortType
	{
		Self,
		LowPriority,
		Both,
		Noone
	}

	public abstract class BTNodeControl : BTNodeBase
	{
		/// <summary>
		/// 是否是当前树的根节点
		/// </summary>
		public bool isRootNode = false;

		/// <summary>
		/// 当前子节点下标
		/// </summary>
		protected int childNodeIndex { get; set; } = 0;

		/// <summary>
		/// 当前子节点
		/// </summary>
		public BTNodeBase currentChild { get; set; } = null;

		/// <summary>
		/// 子节点执行顺序
		/// </summary>
		public Dirction dirction { get; protected set; } = Dirction.Normal;

		/// <summary>
		/// 中断操作的类型
		/// </summary>
		public AbortType abortType { get; protected set; } = AbortType.Noone;

		/// <summary>
		/// 根据direction得到下一个子节点
		/// </summary>
		protected bool GetNextChild()
		{
			if (currentChild == null)
			{
				currentChild = childNodes[childNodeIndex];
				return false;
			}
			bool finish = false;
			switch (dirction)
			{
				case Dirction.Normal:
					childNodeIndex++;
					if (childNodeIndex == childNodes.Count)
					{
						childNodeIndex = 0;
						finish = true;
					}
					break;

				case Dirction.Reverse:
					childNodeIndex--;
					if (childNodeIndex < 0)
					{
						childNodeIndex = childNodes.Count - 1;
						finish = true;
					}
					break;
				//TODO:记录有哪些节点已经被执行 下次不会被随机到
				case Dirction.Random:
					Random.InitState(childNodeIndex);
					childNodeIndex = Random.Range(0, childNodes.Count - 1);
					break;
			}
			currentChild = childNodes[childNodeIndex];
			return finish;
		}

		/// <summary>
		/// 重置子节点
		/// </summary>
		protected void ResetChild()
		{
			switch (dirction)
			{
				case Dirction.Normal:
					childNodeIndex = 0;
					break;

				case Dirction.Reverse:
					childNodeIndex = childNodes.Count - 1;
					break;
			}
			currentChild = null;
		}
	}
}