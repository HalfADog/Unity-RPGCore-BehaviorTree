using RPGCore.BehaviorTree.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree
{
	/// <summary>
	/// ��Ϊ�� ������ڵ�ִ�����ͽڵ��ж�
	/// </summary>
	//[RequireComponent(typeof(RPGCore.BehaviorTree.Blackboard.BehaviorTreeBlackboard))]
	public class BehaviorTree : MonoBehaviour
	{
		/// <summary>
		/// ��Ϊ������
		/// </summary>
		public string treeName;

		/// <summary>
		/// ��������Ϊ��ִ����
		/// </summary>
		public BehaviorTreeExecutor executor;

		/// <summary>
		/// ��ǰ�������нڵ�
		/// </summary>
		public List<BTNodeBase> treeNodes = new List<BTNodeBase>();

		/// <summary>
		/// ��ǰ���ĸ��ڵ�
		/// </summary>
		public BTNodeBase rootNode = null;

		/// <summary>
		/// �Ƿ��ظ�ִ���� Ĭ���ظ�
		/// </summary>
		public bool repeatTree = true;

		/// <summary>
		/// ��ǰ�ڵ�ִ�����еĽڵ� ��List���һ���ʾ����ִ�еĽڵ�
		/// </summary>
		[HideInInspector]
		public List<BTNodeBase> executeNodes = new List<BTNodeBase>();

		/// <summary>
		/// �ڵ�ִ����־ ��¼���������й��Ľڵ�
		/// </summary>
		[HideInInspector]
		public List<BTNodeBase> executeNodesLog = new List<BTNodeBase>();

		/// <summary>
		/// ���Դ�ϲ�����Condition�ڵ�
		/// </summary>
		[HideInInspector]
		public List<BTNodeCondition> tryInterruptNodes = new List<BTNodeCondition>();

		public float LastTick { get; private set; }

		private void Awake()
		{
			//�ҵ���ǰ���ĸ��ڵ�
			rootNode = treeNodes.FindAll(node => node.GetType().IsSubclassOf(typeof(BTNodeControl)))
								.Find(node => (node as BTNodeControl).isRootNode);
			//�����ڵ����ڵ���ִ��ջ��
			if (rootNode)
			{
				executeNodes.Add(rootNode);
				executeNodesLog.Add(rootNode);
				rootNode.Enter();
			}

			//��ʼ�����нڵ�
			for (int i = 0; i < treeNodes.Count; i++)
			{
				treeNodes[i].runningNodeState = new NodeResult(Nodes.BTNodeState.Running, treeNodes[i]);
			}
		}

		/// <summary>
		/// ִ�е�ǰ��
		/// </summary>
		public void Tick()
		{
			EvaluateInterruptions();
			//Debug.Log("ִ��ջ���ȣ�" + executeNodes.Count);
			while (executeNodes.Count > 0)
			{
				//��ȡ��ǰ����ִ�еĽڵ�
				BTNodeBase currentNode = executeNodes[executeNodes.Count - 1];
				//ִ�в��õ����н��
				NodeResult nodeResult = currentNode.Execute();
				//�����Լ������н��
				currentNode.nodeState = nodeResult.state;
				//�����ǰ�ڵ�����ִ��
				if (nodeResult.state == Nodes.BTNodeState.Running)
				{
					//�����ǰ�ڵ㷵�ص�������Ϣ�в�����Ŀ��ڵ���Ϣ
					//˵����ǰ�ڵ�������һ���Ҷ�ӽڵ�
					if (nodeResult.targetNode == null)
					{
						LastTick = Time.time;
						//ֱ�ӷ��� �� �����κδ��� �����õ�ǰҶ�ӽڵ�ִ��
						return;
					}
					//����˵����ǰִ�еĽڵ㲻��Ҷ�ӽڵ�
					else
					{
						//���ص�ִ����Ϣ�е�Ŀ��ڵ���ǵ�ǰ���е��ӽڵ�
						//�������ִ��ջ�е��ӽڵ�ͻ���Ϊ��һ��ѭ����ִ�нڵ�
						executeNodes.Add(nodeResult.targetNode);
						executeNodesLog.Add(nodeResult.targetNode);
						//���Խ���ǰ�ڵ�������б���
						TryInterrupt(nodeResult.targetNode);
						//���õ�ǰ�ڵ�����ȼ� ����ִ�е��Ⱥ�˳����ȷ�� ���ȼ�Խ��Խ��ִ��
						nodeResult.targetNode.nodePriority = executeNodesLog.Count;
						//Debug.Log(nodeResult.targetNode.nodeName + "������ִ��ջ|" + nodeResult.targetNode.nodePriority);
						//����Enter����
						nodeResult.targetNode.Enter();
						continue;
					}
				}
				//��ǰ�ڵ㲻������״̬ �ǳɹ�������ʧ��״̬
				else
				{
					//��ǰ�ڵ����ִ��
					currentNode.Exit();
					//����ִ��ջ���Ƴ� ��һ��ѭ��ִ�еĽڵ���ǵ�ǰ���Ƴ��ڵ����һ���ڵ�
					//Debug.Log(currentNode.nodeName + "�Ƴ���ִ��ջ");
					executeNodes.RemoveAt(executeNodes.Count - 1);
				}
			}
			if (repeatTree) Restart();
			else ResetNodes();
			LastTick = Time.time;
		}

		/// <summary>
		/// �жϲ�ִ���жϲ���
		/// </summary>
		public void EvaluateInterruptions()
		{
			//Debug.Log("��ǰ����б����� " + tryInterruptNodes.Count + " ���ڵ�");
			if (tryInterruptNodes.Count == 0) return;
			BTNodeCondition abortNode = null;
			//���� Խ�ȱ����Ľڵ����ȼ�Խ��
			for (int i = 0; i < tryInterruptNodes.Count; i++)
			{
				//�ҵ�Ҫִ�д�ϲ��������ȼ���ߵĽڵ�
				if (tryInterruptNodes[i].IsCanAbort())
				{
					abortNode = tryInterruptNodes[i];
					break;
				}
			}
			if (abortNode != null)
			{
				Debug.Log("��ǰ��Ͻڵ� : " + abortNode.nodeName);
				//��ִ��ջ�ָ�����ǰ�ڵ�ִ�е�ʱ��(������ǰ�ڵ�)
				executeNodes.Clear();
				executeNodes.AddRange(abortNode.GetStoredTreeSnapshot());
				for (int i = 0; i < executeNodes.Count; i++)
				{
					BTNodeBase node = executeNodes[i];
					if (node.nodeState == Nodes.BTNodeState.Running)
					{
						node.OnBehaviourTreeAbort();
					}
					else if (node.nodeState == Nodes.BTNodeState.Succeed || node.nodeState == Nodes.BTNodeState.Failed)
					{
						//����ִ��enter ��Ϊ���ǲ�û�������ٴ�ִ�й���Щ�ڵ�
						node.Enter();
					}
					//�����нڵ�֮ǰ��״̬����Ϊrunning
					node.nodeState = Nodes.BTNodeState.Running;
				}
				//ͬʱҲ��Logջ�ָ�����ǰ�ڵ�ִ�е�ʱ��
				int cIndex = executeNodesLog.Count - 1;
				while (cIndex > 0)
				{
					BTNodeBase node = executeNodesLog[cIndex];
					if (node == abortNode) break;
					if (node.nodeState == Nodes.BTNodeState.Running)
					{
						//���ǰ����ִ�еĽڵ�ִ��exit
						node.Exit();
					}
					node.nodeState = Nodes.BTNodeState.Noone;
					cIndex -= 1;
				}
				cIndex += 1;
				if (cIndex < executeNodesLog.Count)
				{
					executeNodesLog.RemoveRange(cIndex, executeNodesLog.Count - cIndex);
				}
				abortNode.nodeState = Nodes.BTNodeState.Noone;
			}
		}

		/// <summary>
		/// �����������й��Ľڵ��״̬
		/// </summary>
		public void ResetNodes()
		{
			//��������ִ�й��Ľڵ�
			for (int i = 0; i < executeNodesLog.Count; i++)
			{
				BTNodeBase node = executeNodesLog[i];
				if (node.nodeState == Nodes.BTNodeState.Running)
				{
					node.Exit();
				}
				//����״̬
				node.nodeState = Nodes.BTNodeState.Noone;
			}
			executeNodes.Clear();
			executeNodesLog.Clear();
			tryInterruptNodes.Clear();
		}

		/// <summary>
		/// ׼�����¿�ʼִ����Ϊ��
		/// </summary>
		public void Restart()
		{
			//�������нڵ�
			ResetNodes();
			//�Ѹ��ڵ����¼���ִ��ջ
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

		/// <summary>
		/// ��ȡ��ǰִ��ջ�����нڵ�
		/// </summary>
		/// <param name="stack"></param>
		public void GetStack(ref BTNodeBase[] stack)
		{
			//������̫С��ʱ����������С
			if (executeNodes.Count > stack.Length)
			{
				Array.Resize(ref stack, executeNodes.Count);
			}
			//����ǰִ��ջ�еĽڵ�ȫ��copy��stack
			executeNodes.CopyTo(stack);
		}

		/// <summary>
		/// ���Խ���ǰ�ڵ�������б���
		/// </summary>
		/// <param name="node"></param>
		public void TryInterrupt(BTNodeBase node)
		{
			if (tryInterruptNodes.Contains(node as BTNodeCondition)) return;
			if (node.nodeType == Nodes.BTNodeType.Condition)
			{
				if ((node as BTNodeCondition).abortType != Nodes.AbortType.Noone)
				{
					tryInterruptNodes.Add(node as BTNodeCondition);
				}
			}
		}

#if UNITY_EDITOR

		/// <summary>
		/// ɾ��������Ϊ���ڵ�
		/// </summary>
		public void DeleteAllNodes()
		{
			foreach (var node in treeNodes)
			{
				DestroyImmediate(node);
			}
			treeNodes.Clear();
			executeNodes.Clear();
			executeNodesLog.Clear();
		}

		/// <summary>
		/// ɾ����Ϊ��
		/// </summary>
		public void DeleteBehaviorTree()
		{
			DeleteAllNodes();
			executor.RemoveBehaviorTree(treeName);
			DestroyImmediate(this);
		}

#endif
	}
}