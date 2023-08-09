using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	/// <summary>
	/// 节点类型
	/// </summary>
	public enum BTNodeType
	{
		Sequence, //顺序节点 所有成功则成功 否则失败
		Select, //选择（备选）节点  有一个成功则成功 所有失败则失败
		Parallel, //并行节点 执行所有子节点 当有至少N个子节点返回成功则返回成功 若所有子节点返回失败则失败
		Decorator, //装饰器节点 对子节点的返回结果进行修饰 可以实现repeat、invert、timeout节点等
		Action, //动作节点 行为树的叶子节点 负责功能实现
		Condition, //条件节点 行为树的叶子节点 负责条件判断
		Undefine
	}

	/// <summary>
	/// 节点运行状态
	/// </summary>
	public enum BTNodeState
	{
		Succeed,
		Failed,
		Running,
		Noone
	}

	/// <summary>
	/// 使用此类返回节点的运行状态
	/// </summary>
	public class NodeResult
	{
		public BTNodeState state;
		public BTNodeBase targetNode;

		public NodeResult(BTNodeState nodeState, BTNodeBase targetNode = null)
		{
			this.state = nodeState;
			this.targetNode = targetNode;
		}

		public static readonly NodeResult success = new NodeResult(BTNodeState.Succeed);
		public static readonly NodeResult failed = new NodeResult(BTNodeState.Failed);
		public static readonly NodeResult running = new NodeResult(BTNodeState.Running);
	}

	public abstract class BTNodeBase : MonoBehaviour, IComparable<BTNodeBase>
	{
		#region 字段与属性

		/// <summary>
		/// 节点类型
		/// </summary>
		public BTNodeType nodeType;

		/// <summary>
		/// 节点名称 不唯一
		/// </summary>
		public string nodeName;

		/// <summary>
		/// 记录当前节点的状态
		/// </summary>
		[HideInInspector] public BTNodeState nodeState = BTNodeState.Noone;

		/// <summary>
		/// 节点运行中返回的状态
		/// </summary>

		[HideInInspector] public NodeResult runningNodeState;

		/// <summary>
		/// 当前节点的子节点
		/// </summary>
		[HideInInspector] public List<BTNodeBase> childNodes = new List<BTNodeBase>();

		/// <summary>
		/// 当前节点的父节点
		/// </summary>
		[HideInInspector] public BTNodeBase parentNode;

		/// <summary>
		/// 当前节点所运行的目标树
		/// </summary>
		[HideInInspector] public BehaviorTree targetTree;

		/// <summary>
		/// 节点优先级 越接近0 优先级越高
		/// </summary>
		[HideInInspector] public int nodePriority;

		/// <summary>
		///
		/// </summary>
		public float LastTick => targetTree.LastTick;

		/// <summary>
		/// 节点运行过程中 两次执行之间的时间间隔
		/// </summary>
		public float DeltaTime => Time.time - targetTree.LastTick;

#if UNITY_EDITOR

		/// <summary>
		/// 节点图在行为树编辑器中的位置
		/// </summary>
		[HideInInspector]
		public Vector2 graphNodePosition;

#endif

		#endregion 字段与属性

		#region 方法

		public BTNodeBase()
		{
			//节点默认名称为当前节点类的名称
			nodeName = this.GetType().Name;
		}

		/// <summary>
		/// 每个节点开始执行前会调用Enter方法
		/// </summary>
		public virtual void Enter()
		{ }

		/// <summary>
		/// 执行节点
		/// </summary>
		/// <returns>当前节点的运行状态</returns>
		public abstract NodeResult Execute();

		/// <summary>
		/// 每个节点正常结束执行后会调用Exit方法
		/// </summary>
		public virtual void Exit()
		{ }

		/// <summary>
		/// 将所有子节点的状态全部重置
		/// </summary>
		/// <param name="node"></param>
		public void ResetNodeAndChildNodeState(BTNodeBase node)
		{
			if (node.childNodes.Count > 0)
			{
				foreach (var item in node.childNodes)
				{
					ResetNodeAndChildNodeState(item);
				}
			}
			node.nodeState = BTNodeState.Noone;
		}

		/// <summary>
		/// 在被打断时且还处于running状态调用
		/// </summary>
		public virtual void OnBehaviourTreeAbort()
		{
		}

		/// <summary>
		/// 判断是否有共同父级
		/// </summary>
		/// <param name="other"></param>
		public bool SameParent(BTNodeBase other)
		{
			return parentNode == other.parentNode;
		}

#if UNITY_EDITOR

		/// <summary>
		/// 从当前行为树中删除节点 EditorOnly
		/// </summary>
		public void DeleteNode()
		{
			DestroyImmediate(this);
		}

		public int CompareTo(BTNodeBase other)
		{
			return -other.graphNodePosition.x.CompareTo(graphNodePosition.x);
		}

#endif

		#endregion 方法
	}
}