using RPGCore.BehaviorTree.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
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

		private ToolbarButton saveButton;
		private ToolbarButton newButton;
		private ToolbarButton deleteButton;
		private TextField newBtName;
		private DropdownField editorTreeSelector;

		/// <summary>
		/// ��ǰѡ�е���btexecute��gameobject
		/// </summary>
		public static GameObject targetGameObject;

		/// <summary>
		/// ��ǰѡ�е���Ϸ�����ϵ�BehaviorTreeExecutor���
		/// </summary>
		public BehaviorTreeExecutor treeExecutor;

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

			//��ȡUI
			nodeInspector = rootVisualElement.Q<VisualElement>("Inspector");
			saveButton = rootVisualElement.Q<ToolbarButton>("save");
			newButton = rootVisualElement.Q<ToolbarButton>("new");
			deleteButton = rootVisualElement.Q<ToolbarButton>("delete");
			newBtName = rootVisualElement.Q<TextField>("newbtname");
			editorTreeSelector = rootVisualElement.Q<DropdownField>("btTarget");
			editorTreeSelector.choices = new List<string>();
			//ע��UI�¼�
			saveButton.RegisterCallback<ClickEvent>(callback => { UpdateMonoNodes(); });
			newButton.RegisterCallback<ClickEvent>(callback =>
			{
				//����Ϊ�����Ʋ�Ϊ�� �Ҳ�����
				if (newBtName.text != "" && treeExecutor.behaviorTrees.FindIndex(tree => tree.treeName == newBtName.text) == -1)
				{
					treeExecutor.AddBehaviorTree(newBtName.text, true);
					//����ǰ����ʾ����Ϊ���ı�Ϊ��������Ϊ��ͼ
					UpdateMonoNodes();
					RegenerateGraphNodesView();
					UpdateEditorTreeSelector();
				}
			});
			deleteButton.RegisterCallback<ClickEvent>(callback => { DeleteCurrentEditorTree(); });
			editorTreeSelector.RegisterValueChangedCallback(callback =>
			{
				ChangeCurrentEditorTree(editorTreeSelector.text);
			});

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
					treeExecutor = targetGameObject.GetComponent<BehaviorTreeExecutor>();
					rootVisualElement.SetEnabled(true);
					treeExecutor.currentEditorTree = treeExecutor.currentExecuteTree;
					GenerateNodeGraphView();
					UpdateEditorTreeSelector();
				}
				else
				{
					rootVisualElement.Q<VisualElement>("nobt").style.visibility = Visibility.Hidden;
					rootVisualElement.Q<VisualElement>("nogameobject").style.visibility = Visibility.Visible;
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
					rootVisualElement.Q<VisualElement>("nogameobject").style.visibility = Visibility.Hidden;
					//���˵��ǰ�Ľڵ���ͼ���нڵ�
					//Ҳ����˵��ǰ�Ǽ���༭״̬ �򱣴浱ǰ����Ϊ��
					UpdateMonoNodes();
					//���¸��ݵ�ǰѡ�е��������е���Ϊ���������ɽڵ���ͼ
					RegenerateGraphNodesView();
					UpdateEditorTreeSelector();
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
			node.targetTree = treeExecutor.currentEditorTree;
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
			//���һ����Ϊ��Ҳû��
			if (treeExecutor.behaviorTrees.Count == 0 || treeExecutor.currentEditorTree == null)
			{
				rootVisualElement.Q<VisualElement>("nobt").style.visibility = Visibility.Visible;
				rootVisualElement.Q<VisualElement>("editorspace").SetEnabled(false);
				return;
			}
			else
			{
				rootVisualElement.Q<VisualElement>("nobt").style.visibility = Visibility.Hidden;
				rootVisualElement.Q<VisualElement>("editorspace").SetEnabled(true);
			}
			treeExecutor.currentEditorTree.treeNodes.ForEach((node) =>
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
			if (targetGameObject != null && treeExecutor.currentEditorTree != null && nodeGraphView.nodes.Count() != 0)
			{
				//���ȸ��½ڵ�����
				List<Node> graphNodes = nodeGraphView.nodes.ToList();
				List<BTNodeBase> monoNodes = treeExecutor.currentEditorTree.treeNodes;
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

		/// <summary>
		/// ���ݵ�ǰ�༭����Ϊ��������ͼ���Ϸ��ĵ�ǰ�༭����Ϊ��ѡ������UI
		/// </summary>
		private void UpdateEditorTreeSelector()
		{
			//�ĸ���Ϸ����
			editorTreeSelector.Q<Label>().text = targetGameObject.name;
			//����ѡ�������ڵ�ѡ��
			int selectIndex = -1;
			editorTreeSelector.choices.Clear();
			for (int i = 0; i < treeExecutor.behaviorTrees.Count; i++)
			{
				editorTreeSelector.choices.Add(treeExecutor.behaviorTrees[i].treeName);
				if (treeExecutor.behaviorTrees[i].treeName == treeExecutor.currentEditorTree.treeName)
				{
					selectIndex = i;
				}
			}
			if (selectIndex != -1)
			{
				editorTreeSelector.index = selectIndex;
			}
			else
			{
				editorTreeSelector.choices.Add("Noone");
				editorTreeSelector.index = 0;
			}
		}

		/// <summary>
		/// �ı䵱ǰ�༭���� һ����editorTreeSelectorѡ��ı�ʱ����
		/// </summary>
		public void ChangeCurrentEditorTree(string treeName)
		{
			//Debug.Log(treeExecutor.currentEditorTree.treeName + " " + treeName);
			if (treeName == "") return;
			UpdateMonoNodes();
			var ctree = treeExecutor.behaviorTrees.Find(tree => tree.treeName == treeName);
			if (ctree != null)
			{
				treeExecutor.currentEditorTree = ctree;
			}
			RegenerateGraphNodesView();
		}

		/// <summary>
		/// ɾ����ǰ���ڱ༭����
		/// </summary>
		public void DeleteCurrentEditorTree()
		{
			if (treeExecutor.currentEditorTree != null)
			{
				BehaviorTree editortree = treeExecutor.currentEditorTree;
				//ֻ��һ����Ϊ�� ɾ�˾�û����
				if (treeExecutor.behaviorTrees.Count == 1)
				{
					treeExecutor.currentEditorTree = null;
					treeExecutor.currentExecuteTree = null;
				}
				else
				{
					//�ı䵱ǰ�༭����Ϊ�� Ĭ���޸�Ϊ��һ����Ϊ��
					int cindex = treeExecutor.behaviorTrees.FindIndex(tree => tree.treeName == editortree.treeName);
					if (cindex != 0)
					{
						treeExecutor.currentEditorTree = treeExecutor.behaviorTrees[0];
					}
					else
					{
						treeExecutor.currentEditorTree = treeExecutor.behaviorTrees[1];
					}
					if (treeExecutor.currentExecuteTree == editortree)
					{
						treeExecutor.currentExecuteTree = treeExecutor.currentEditorTree;
					}
				}
				//ɾ����Ϊ��
				editortree.DeleteBehaviorTree();
				//������ͼ
				RegenerateGraphNodesView();
				//���������˵�
				UpdateEditorTreeSelector();
			}
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