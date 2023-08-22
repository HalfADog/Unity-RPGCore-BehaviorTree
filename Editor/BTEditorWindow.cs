using RPGCore.BehaviorTree.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		public VisualElement blackboardView;

		private ToolbarButton saveButton;
		private ToolbarButton newButton;
		private ToolbarButton deleteTreeButton;
		private TextField newBtName;
		private DropdownField editorTreeSelector;

		private ListView variableList;
		private Button addVariableButton;
		private TextField addVariableName;
		private ToolbarButton deleteVariableButton;
		private DropdownField addVariableTypes;
		private List<Type> variableTypes = new List<Type>();
		private VisualTreeAsset variableViewTree;

		private bool deleteTree = false;

		/// <summary>
		/// ��ǰѡ�е���btexecute��gameobject
		/// </summary>
		public static GameObject targetGameObject;

		/// <summary>
		/// ��ǰѡ�е���Ϸ�����ϵ�BehaviorTreeExecutor���
		/// </summary>
		public BehaviorTreeExecutor treeExecutor;

		/// <summary>
		/// ��ǰ�����ϵ�blackboard��� һ��executorֻ����һ��blackboard
		/// </summary>
		public Blackboard.BehaviorTreeBlackboard treeBlackboard;

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
			//����blackboard��ͼ
			blackboardView = rootVisualElement.Q<VisualElement>("Blackboard");
			VisualTreeAsset blackboardViewTree = Resources.Load<VisualTreeAsset>("BlackboardEditorWindow");
			TemplateContainer blackboardViewInstance = blackboardViewTree.CloneTree();
			blackboardView.Add(blackboardViewInstance);
			//����variable��ͼ
			variableViewTree = Resources.Load<VisualTreeAsset>("BlackboardVariableView");

			//��ȡUI
			nodeInspector = rootVisualElement.Q<VisualElement>("Inspector");
			saveButton = rootVisualElement.Q<ToolbarButton>("save");
			newButton = rootVisualElement.Q<ToolbarButton>("new");
			deleteTreeButton = rootVisualElement.Q<ToolbarButton>("delete");
			newBtName = rootVisualElement.Q<TextField>("newbtname");
			editorTreeSelector = rootVisualElement.Q<DropdownField>("btTarget");
			editorTreeSelector.choices = new List<string>();

			addVariableName = blackboardView.Q<TextField>("variableName");
			addVariableButton = blackboardView.Q<Button>("add");
			deleteVariableButton = blackboardView.Q<ToolbarButton>("delete");
			addVariableTypes = blackboardView.Q<DropdownField>("variableType");
			variableList = blackboardView.Q<ListView>("variables");
			addVariableTypes.choices = new List<string>();
			//ע��UI�¼�
			saveButton.RegisterCallback<ClickEvent>(callback => { UpdateMonoNodes(); });
			newButton.RegisterCallback<ClickEvent>(callback =>
			{
				if (Application.isPlaying) return;
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
			deleteTreeButton.RegisterCallback<ClickEvent>(callback =>
			{
				if (Application.isPlaying) return;
				nodeGraphView.currentSelectedNode = null;
				DeleteCurrentEditorTree();
			});
			editorTreeSelector.RegisterValueChangedCallback(callback =>
			{
				//ɾ���������������˵��Ӷ�����ChangeCurrentEditorTree
				//��ᵼ������ ��������deleteTree��־������Ƿ�ִ�и���
				if (!deleteTree) ChangeCurrentEditorTree(editorTreeSelector.text);
				deleteTree = false;
			});
			addVariableButton.RegisterCallback<ClickEvent>(callback => { AddVariable(); });
			deleteVariableButton.RegisterCallback<ClickEvent>(callback => { DeleteVariable(); });
			//�������ʼ������
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
					treeBlackboard = targetGameObject.GetComponent<Blackboard.BehaviorTreeBlackboard>();
					rootVisualElement.SetEnabled(true);
					treeExecutor.currentEditorTree = treeExecutor.currentExecuteTree;
					GenerateNodeGraphView();
					UpdateEditorTreeSelector();

					GenerateVariableListView();
					UpdateVariableTypeSelector();
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

		private void OnInspectorUpdate()
		{
			nodeGraphView.UpdateNodesStateView();
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
			if (Application.isPlaying) return;
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
		/// ���µ�ͬʱ�ᴥ��ChangeCurrentEditorTree
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
		public void ChangeCurrentEditorTree(string treeName, bool saveCurrentTree = true)
		{
			//Debug.Log(treeExecutor.currentEditorTree.treeName + " " + treeName);
			if (treeName == "") return;
			if (saveCurrentTree) UpdateMonoNodes();
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
					//������ͼ
					RegenerateGraphNodesView();
				}
				else
				{
					//�ı䵱ǰ�༭����Ϊ�� Ĭ���޸�Ϊ��һ����Ϊ��
					int cindex = editorTreeSelector.choices.FindIndex(tree => tree == treeExecutor.currentEditorTree.treeName);
					if (cindex != 0)
					{
						ChangeCurrentEditorTree(editorTreeSelector.choices[0], false);
					}
					else
					{
						ChangeCurrentEditorTree(editorTreeSelector.choices[1], false);
					}
					if (treeExecutor.currentExecuteTree == editortree)
					{
						treeExecutor.currentExecuteTree = treeExecutor.currentEditorTree;
					}
				}
				//ɾ����Ϊ��
				editortree.DeleteBehaviorTree();
				//���������˵�
				deleteTree = true;//��ֹUpdateEditorTreeSelector ���� ChangeCurrentEditorTree
				UpdateEditorTreeSelector();
			}
		}

		/// <summary>
		/// ���½ڵ����� ��graphview��ѡ��һ���ڵ�ʱ����
		/// </summary>
		public void UpdateNodeInspectorView(BTNodeBase selectNode)
		{
			if (selectNode == null)
			{
				nodeInspector.Clear();
				return;
			}
			//��ȡ����ǰ�ڵ����������л�����
			SerializedObject serializedNode = new SerializedObject(selectNode);
			SerializedProperty nodeProperty = serializedNode.GetIterator();
			nodeProperty.Next(true);
			//�����������л�����
			while (nodeProperty.NextVisible(false))
			{
				if (nodeProperty.name == "m_Script") continue;
				//����һ��PropertyField������ʾ
				PropertyField field = new PropertyField(nodeProperty, "");
				field.name = "field";
				field.SetEnabled(nodeProperty.name != "nodeType");
				//��ʵ�ʵĽڵ����ݰ�
				field.Bind(serializedNode);
				//���༭��ʾ������ʱ����
				field.RegisterValueChangeCallback(callback => { });
				//������ʾ�ֶε����� ���������в���
				Label fieldName = new Label(nodeProperty.displayName);
				fieldName.name = "fieldName";
				//���������ֶ� ���ֶ�����
				VisualElement fieldContainer = new VisualElement();
				fieldContainer.Add(fieldName);
				fieldContainer.Add(field);
				//���ݵ�ǰ�ֶ��Ƿ���variable reference�����ò���
				if (nodeProperty.type.Contains("Reference"))
				{
					fieldContainer.name = "fieldContainer_ref";
					fieldContainer.style.flexDirection = FlexDirection.Column;
				}
				else
				{
					fieldContainer.name = "fieldContainer";
					fieldContainer.style.flexDirection = FlexDirection.Row;
				}
				nodeInspector.Add(fieldContainer);
			}
		}

		/// <summary>
		/// ����blackboard��variable list
		/// </summary>
		private void GenerateVariableListView()
		{
			variableList.makeItem = () =>
			{
				TemplateContainer variableViewInstance = variableViewTree.CloneTree();
				return variableViewInstance;
			};
			variableList.bindItem = (item, index) =>
			{
				item.Q<Label>("variableName").text = treeBlackboard.variables[index].key;
				SerializedObject serializedObject = new SerializedObject(treeBlackboard.variables[index]);
				SerializedProperty property = serializedObject.FindProperty("val");
				item.Q<PropertyField>("field").label = "";
				item.Q<PropertyField>("field").BindProperty(property);
				item.Q<PropertyField>("field").Bind(serializedObject);
			};
			variableList.itemsSource = treeBlackboard.variables;
		}

		/// <summary>
		/// ���variable��ˢ��variableList
		/// </summary>
		private void AddVariable()
		{
			if (Application.isPlaying) return;
			treeBlackboard.CreateVariable(addVariableName.text, variableTypes[addVariableTypes.index]);
			variableList.Rebuild();
		}

		/// <summary>
		/// ɾ��variable��ˢ��variableList
		/// </summary>
		private void DeleteVariable()
		{
			if (Application.isPlaying) return;
			if (variableList.selectedItem == null) return;
			var variable = treeBlackboard.variables.Find(variable => variable.key == (variableList.selectedItem as Variable.BlackboardVariable).key);
			if (variable != null)
			{
				treeBlackboard.DeleteVariable(variable.key);
				variableList.Rebuild();
			}
		}

		/// <summary>
		/// �������variable��type�����˵�
		/// </summary>
		private void UpdateVariableTypeSelector()
		{
			variableTypes.Clear();
			addVariableTypes.choices.Clear();
			//�����������򼯲������м̳���BlackboardVariable�� ���ҵ�����variable����
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				IEnumerable<Type> enumerable = assembly
					.GetTypes()
					.Where(
						myType =>
							myType.IsClass
							&& !myType.IsAbstract
							&& myType.IsSubclassOf(typeof(Variable.BlackboardVariable))
					);
				foreach (Type type in enumerable)
				{
					variableTypes.Add(type);
					addVariableTypes.choices.Add(type.Name);
				}
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