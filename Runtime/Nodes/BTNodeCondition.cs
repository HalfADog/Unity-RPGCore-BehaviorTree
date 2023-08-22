using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
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

	public abstract class BTNodeCondition : BTNodeLeaf
	{
		[HideInInspector] public bool lastCheckResult = false;

		/// <summary>
		/// 存储当前节点执行时的执行栈状态
		/// </summary>
		[HideInInspector] public BTNodeBase[] stackTreeSnapshot = new BTNodeBase[0];

		/// <summary>
		/// 中断操作的类型
		/// </summary>
		public AbortType abortType = AbortType.Noone;

		public BTNodeCondition()
		{
			nodeType = BTNodeType.Condition;
		}

		public override void Enter()
		{
			//如果没有存储当前的执行栈状态就存储
			//用以在执行打断操作时把执行栈恢复到此节点执行时
			if (abortType != AbortType.Noone && stackTreeSnapshot.Length == 0)
			{
				targetTree.GetStack(ref stackTreeSnapshot);
				//Debug.Log(stackTreeSnapshot.Length);
			}
		}

		public override NodeResult Execute()
		{
			lastCheckResult = Check();
			if (!lastCheckResult)
			{
				return NodeResult.failed;
			}
			return NodeResult.success;
		}

		public virtual bool Check()
		{
			return true;
		}

		/// <summary>
		///	是否能够执行打断操作
		/// </summary>
		/// <returns></returns>
		public bool IsCanAbort(BTNodeBase runningNode)
		{
			bool isBortherNode = IsBrotherNode(runningNode);
			//如果当前的AbortType为Self 需要当前Running的节点是兄弟节点
			//如果当前的AbortType为LowPriority 需要当前Running的节点是非兄弟节点
			//否则返回失败
			if ((abortType == AbortType.Self && !isBortherNode) ||
				(abortType == AbortType.LowPriority && isBortherNode))
			{
				return false;
			}
			//结果和上次不同才能执行打断操作
			bool c = Check();
			if (c != lastCheckResult)
			{
				lastCheckResult = c;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 获取当前节点执行的时候存储的执行树状态
		/// </summary>
		/// <returns></returns>
		public BTNodeBase[] GetStoredTreeSnapshot()
		{
			return stackTreeSnapshot;
		}
	}
}