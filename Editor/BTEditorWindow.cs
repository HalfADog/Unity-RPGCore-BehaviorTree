using RPGCore.BehaviorTree.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGCore.BehaviorTree.Editor
{
	public class BTEditorWindow : EditorWindow
	{
		public static BTEditorWindow window;
		public BTNodeGraphView nodeGraphView;
		public VisualElement nodeInspector;
		public VisualElement treeList;

		/// <summary>
		/// ��ǰѡ�е���btexecute��gameobject
		/// </summary>
		public static GameObject targetGameObject;

		/// <summary>
		/// ��nodegraphview����ʾ��ǰѡ�е�behaviortree
		/// </summary>
		public BehaviorTree targetBehaviorTree;

		[MenuItem("Window/AI/BehaviorTree Editor")]
		public static void OpenEditorWindow()
		{
			window = EditorWindow.GetWindow<BTEditorWindow>();
			window.titleContent = new GUIContent("BehaviorTree Editor");
			window.minSize = new Vector2(540, 480);
		}

		private void OnEnable()
		{
			//����������
			VisualTreeAsset editorViewTree = Resources.Load<VisualTreeAsset>("BehaviorTreeEditorWindow");
			TemplateContainer editorInstance = editorViewTree.CloneTree();
			editorInstance.StretchToParentSize();
			rootVisualElement.Add(editorInstance);

			//���ؽڵ���ͼ
			nodeGraphView = new BTNodeGraphView(this);
			nodeGraphView.StretchToParentSize();
			rootVisualElement.Q<VisualElement>("RightPane").Add(nodeGraphView);

			//��ȡ�ڵ���������
			nodeInspector = rootVisualElement.Q<VisualElement>("Inspector");

			//ֻ�е�ǰѡ�е���������Ϊ��������ܼ���༭
			rootVisualElement.SetEnabled(false);
			GameObject selected = Selection.activeGameObject;
			if (selected != null)
			{
				//�����ǰѡ�е���������Ϊ�����
				if (selected.GetComponent<BehaviorTreeExecutor>() != null)
				{
					//����༭ �����ɽڵ���ͼ
					targetGameObject = selected;
					rootVisualElement.SetEnabled(true);
					GenerateNodeGraphView();
				}
			}
		}

		//��ѡ�е�����ı�����
		private void OnSelectionChange()
		{
			GameObject selected = Selection.activeGameObject;
			if (selected != null)
			{
				if (selected.GetComponent<BehaviorTreeExecutor>() != null)
				{
					targetGameObject = selected;
					rootVisualElement.SetEnabled(true);
					//���˵��ǰ�Ľڵ���ͼ���нڵ�
					//Ҳ����˵��ǰ�Ǽ���༭״̬ �򱣴浱ǰ����Ϊ��
					if (nodeGraphView.nodes.Count() != 0) UpdateMonoNodes();
					//���¸��ݵ�ǰѡ�е��������е���Ϊ���������ɽڵ���ͼ
					RegenerateGraphNodesView();
				}
			}
		}

		/// <summary>
		/// ����һ��mono�ڵ�
		/// </summary>
		/// <param name="nodeType"></param>
		/// <returns></returns>
		public BTNodeBase CreateMonoNode(Type nodeType)
		{
			BTNodeBase node = (BTNodeBase)targetGameObject.AddComponent(nodeType);
			//���ű���inspector�����ز���ʾ
			//node.hideFlags = HideFlags.HideInInspector;
			node.targetTree = targetGameObject.GetComponent<BehaviorTree>();
			node.targetTree.treeNodes.Add(node);
			return node;
		}

		/// <summary>
		/// �����ڵ���ͼ
		/// </summary>
		private void GenerateNodeGraphView()
		{
			//����Node �����ݼ�¼��position��Ϣ����λ��
			if (targetGameObject == null) return;
			targetGameObject.GetComponent<BehaviorTree>().treeNodes.ForEach((node) =>
			{
				nodeGraphView.MakeNode(node);
			});
			//����childNodes�����Ӹ����ڵ�
			nodeGraphView.nodes.ForEach((node) =>
			{
				BTNodeGraph onode = (BTNodeGraph)node;
				BTNodeGraph inode = null;
				onode.monoNode.childNodes?.ForEach((mnode) =>
				{
					inode = (BTNodeGraph)nodeGraphView.nodes.ToList().Find(gnode => ((BTNodeGraph)gnode).monoNode == mnode);
					if (inode != null)
					{
						nodeGraphView.MakeEdge(onode, inode);
					}
				});
			});
		}

		/// <summary>
		/// ���ݵ�ǰ�ڵ���ͼ���еĽڵ�ȥ����runtime�ڵ� runtime�ж�����Ľڵ��ɾ��
		/// </summary>
		public void UpdateMonoNodes()
		{
			if (targetGameObject != null)
			{
				//���ȸ��½ڵ�����
				List<Node> graphNodes = nodeGraphView.nodes.ToList();
				List<BTNodeBase> monoNodes = targetGameObject.GetComponent<BehaviorTree>().treeNodes;
				monoNodes.ForEach(mnode =>
				{
					//�����ǰruntime�ڵ��ڽڵ���ͼ�в�������ɾ��
					if (!graphNodes.Exists(gnode => ((BTNodeGraph)gnode).monoNode == mnode))
					{
						mnode.DeleteNode();
					}
				});
				//�Ƴ�������ɾ���ڵ�
				monoNodes.RemoveAll(node => node == null);
				//�ٴθ��½ڵ���ͼ�ĵĽڵ�λ����Ϣ��runtime�ڵ�
				graphNodes.ForEach(gnode =>
				{
					((BTNodeGraph)gnode).monoNode.graphNodePosition = gnode.GetPosition().position;
				});
				//֮���ٸ��½ڵ�֮������ӹ�ϵ ��Edges
				monoNodes.ForEach(mnode => { mnode.childNodes?.Clear(); });//����� ���¼���
				nodeGraphView.edges.ForEach(edge =>
				{
					BTNodeBase fnode = ((BTNodeGraph)edge.output.node).monoNode;
					BTNodeBase cnode = ((BTNodeGraph)edge.input.node).monoNode;
					fnode.childNodes?.Add(cnode);
					cnode.parentNode = fnode;
				});
				//�����ӽڵ�������� ����node��view���е�yλ��
				monoNodes.ForEach((node) =>
				{
					node.childNodes?.Sort();
				});
			}
		}

		/// <summary>
		/// �������ɽڵ���ͼ
		/// </summary>
		public void RegenerateGraphNodesView()
		{
			//�����
			nodeGraphView.ClearNodeGraph();
			//������
			GenerateNodeGraphView();
		}

		private void OnDisable()
		{
			UpdateMonoNodes();
			targetGameObject = null;
			rootVisualElement.Q<VisualElement>("RightPane").Remove(nodeGraphView);
			AssetDatabase.Refresh();
		}
	}
}