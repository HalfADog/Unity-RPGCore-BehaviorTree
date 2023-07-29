using RPGCore.BehaviorTree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree
{
	/// <summary>
	/// ��Ϊ�� ������ڵ�ִ�����ͽڵ��ж�
	/// </summary>
	//[RequireComponent()]
	public class BehaviorTree : MonoBehaviour
	{
		/// <summary>
		/// ��Ϊ������
		/// </summary>
		public string treeName;

		/// <summary>
		/// ��ǰ�������нڵ�
		/// </summary>
		public List<BTNodeBase> treeNodes = new List<BTNodeBase>();

		/// <summary>
		/// ��ǰ���ĸ��ڵ�
		/// </summary>
		public BTNodeBase rootNode = null;

		/// <summary>
		/// ��ǰ�ڵ�ִ�����еĽڵ� ��List���һ���ʾ����ִ�еĽڵ�
		/// </summary>
		public List<BTNodeBase> executeNodes = new List<BTNodeBase>();

		/// <summary>
		/// �ڵ�ִ����־ ��¼���������й��Ľڵ�
		/// </summary>
		public List<BTNodeBase> executeNodesLog = new List<BTNodeBase>();

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
			while (executeNodes.Count > 0)
			{
				//��ȡ��ǰ����ִ�еĽڵ�
				BTNodeBase currentNode = executeNodes[executeNodes.Count - 1];
				//Debug.Log("����ִ�еĽڵ� ��" + currentNode.nodeName);
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
						//���õ�ǰ�ڵ�����ȼ� ����ִ�е��Ⱥ�˳����ȷ�� ���ȼ�Խ��Խ��ִ��
						nodeResult.targetNode.nodePriority = executeNodesLog.Count;
						Debug.Log(nodeResult.targetNode.nodeName + "������ִ��ջ");
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
					Debug.Log(currentNode.nodeName + "�Ƴ���ִ��ջ");
					executeNodes.RemoveAt(executeNodes.Count - 1);
				}
			}
			Restart();
			LastTick = Time.time;
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
	}
}