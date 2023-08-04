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
		public Dirction dirction = Dirction.Normal;

		/// <summary>
		/// 中断操作的类型
		/// </summary>
		public AbortType abortType = AbortType.Noone;

		/// <summary>
		/// 是否有下一个子节点
		/// </summary>
		protected bool existNextChild = true;

		/// <summary>
		/// 依据Direction获取当前子节点的下一个子节点
		/// </summary>
		/// <returns>是否已经获取了最后一个节点</returns>
		protected void GetNextChild()
		{
			switch (dirction)
			{
				case Dirction.Normal:
					childNodeIndex++;
					if (childNodeIndex == childNodes.Count)
					{
						childNodeIndex = 0;
						existNextChild = false;
					}
					break;

				case Dirction.Reverse:
					childNodeIndex--;
					if (childNodeIndex < 0)
					{
						childNodeIndex = childNodes.Count - 1;
						existNextChild = false;
					}
					break;
				//TODO:记录有哪些节点已经被执行 下次不会被随机到
				case Dirction.Random:
					Random.InitState(childNodeIndex);
					childNodeIndex = Random.Range(0, childNodes.Count - 1);
					break;
			}
			currentChild = childNodes[childNodeIndex];
		}

		/// <summary>
		/// 重置子节点
		/// </summary>
		protected void ResetCurrentChild()
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
			currentChild = childNodes[childNodeIndex];
			existNextChild = true;
		}

		public int GetCurrentChildNodeIndex()
		{
			return childNodeIndex;
		}
	}
}