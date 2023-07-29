using RPGCore.BehaviorTree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree
{
	/// <summary>
	/// 行为树 负责处理节点执行流和节点中断
	/// </summary>
	//[RequireComponent()]
	public class BehaviorTree : MonoBehaviour
	{
		/// <summary>
		/// 行为树名称
		/// </summary>
		public string treeName;

		/// <summary>
		/// 当前树的所有节点
		/// </summary>
		public List<BTNodeBase> treeNodes = new List<BTNodeBase>();

		/// <summary>
		/// 当前树的根节点
		/// </summary>
		public BTNodeBase rootNode = null;

		/// <summary>
		/// 当前节点执行流中的节点 该List最后一项表示正在执行的节点
		/// </summary>
		public List<BTNodeBase> executeNodes = new List<BTNodeBase>();

		/// <summary>
		/// 节点执行日志 记录下所有运行过的节点
		/// </summary>
		public List<BTNodeBase> executeNodesLog = new List<BTNodeBase>();

		public float LastTick { get; private set; }

		private void Awake()
		{
			//找到当前树的根节点
			rootNode = treeNodes.FindAll(node => node.GetType().IsSubclassOf(typeof(BTNodeControl)))
								.Find(node => (node as BTNodeControl).isRootNode);
			//将根节点加入节点流执行栈中
			if (rootNode)
			{
				executeNodes.Add(rootNode);
				executeNodesLog.Add(rootNode);
				rootNode.Enter();
			}

			//初始化所有节点
			for (int i = 0; i < treeNodes.Count; i++)
			{
				treeNodes[i].runningNodeState = new NodeResult(Nodes.BTNodeState.Running, treeNodes[i]);
			}
		}

		/// <summary>
		/// 执行当前树
		/// </summary>
		public void Tick()
		{
			while (executeNodes.Count > 0)
			{
				//获取当前正在执行的节点
				BTNodeBase currentNode = executeNodes[executeNodes.Count - 1];
				//Debug.Log("正在执行的节点 ：" + currentNode.nodeName);
				//执行并拿到运行结果
				NodeResult nodeResult = currentNode.Execute();
				//保存自己的运行结果
				currentNode.nodeState = nodeResult.state;
				//如果当前节点正在执行
				if (nodeResult.state == Nodes.BTNodeState.Running)
				{
					//如果当前节点返回的运行信息中不包含目标节点信息
					//说明当前节点就是最后一层的叶子节点
					if (nodeResult.targetNode == null)
					{
						LastTick = Time.time;
						//直接返回 即 不做任何处理 继续让当前叶子节点执行
						return;
					}
					//否则说明当前执行的节点不是叶子节点
					else
					{
						//返回的执行信息中的目标节点就是当前运行的子节点
						//这个加入执行栈中的子节点就会作为下一次循环的执行节点
						executeNodes.Add(nodeResult.targetNode);
						executeNodesLog.Add(nodeResult.targetNode);
						//设置当前节点的优先级 根据执行的先后顺序来确定 优先级越高越先执行
						nodeResult.targetNode.nodePriority = executeNodesLog.Count;
						Debug.Log(nodeResult.targetNode.nodeName + "加入了执行栈");
						//调用Enter方法
						nodeResult.targetNode.Enter();
						continue;
					}
				}
				//当前节点不是运行状态 是成功或者是失败状态
				else
				{
					//当前节点结束执行
					currentNode.Exit();
					//并从执行栈中移出 下一次循环执行的节点就是当前被移出节点的上一个节点
					Debug.Log(currentNode.nodeName + "移出了执行栈");
					executeNodes.RemoveAt(executeNodes.Count - 1);
				}
			}
			Restart();
			LastTick = Time.time;
		}

		/// <summary>
		/// 重置所有运行过的节点的状态
		/// </summary>
		public void ResetNodes()
		{
			//遍历所有执行过的节点
			for (int i = 0; i < executeNodesLog.Count; i++)
			{
				BTNodeBase node = executeNodesLog[i];
				if (node.nodeState == Nodes.BTNodeState.Running)
				{
					node.Exit();
				}
				//重置状态
				node.nodeState = Nodes.BTNodeState.Noone;
			}
			executeNodes.Clear();
			executeNodesLog.Clear();
		}

		/// <summary>
		/// 准备重新开始执行行为树
		/// </summary>
		public void Restart()
		{
			//重置所有节点
			ResetNodes();
			//把根节点重新加入执行栈
			executeNodes.Add(rootNode);
			executeNodesLog.Add(rootNode);
			rootNode.Enter();
		}

		public void AddNode(BTNodeBase node)
		{
			node.targetTree = this;
			treeNodes.Add(node);
		}

		public void RemoveNode(BTNodeBase node)
		{
			treeNodes.Remove(node);
		}
	}
}