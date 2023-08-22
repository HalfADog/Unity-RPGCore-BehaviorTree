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
		/// 当前选中的有btexecute的gameobject
		/// </summary>
		public static GameObject targetGameObject;

		/// <summary>
		/// 当前选中的游戏物体上的BehaviorTreeExecutor组件
		/// </summary>
		public BehaviorTreeExecutor treeExecutor;

		/// <summary>
		/// 当前物体上的blackboard组件 一个executor只能有一个blackboard
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
			//加载主界面
			VisualTreeAsset editorViewTree = Resources.Load<VisualTreeAsset>("BehaviorTreeEditorWindow");
			TemplateContainer editorInstance = editorViewTree.CloneTree();
			editorInstance.StretchToParentSize();
			rootVisualElement.Add(editorInstance);

			//加载节点视图
			nodeGraphView = new BTNodeGraphView(this);
			nodeGraphView.StretchToParentSize();
			rootVisualElement.Q<VisualElement>("RightPane").Add(nodeGraphView);
			//加载blackboard视图
			blackboardView = rootVisualElement.Q<VisualElement>("Blackboard");
			VisualTreeAsset blackboardViewTree = Resources.Load<VisualTreeAsset>("BlackboardEditorWindow");
			TemplateContainer blackboardViewInstance = blackboardViewTree.CloneTree();
			blackboardView.Add(blackboardViewInstance);
			//加载variable视图
			variableViewTree = Resources.Load<VisualTreeAsset>("BlackboardVariableView");

			//获取UI
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
			//注册UI事件
			saveButton.RegisterCallback<ClickEvent>(callback => { UpdateMonoNodes(); });
			newButton.RegisterCallback<ClickEvent>(callback =>
			{
				if (Application.isPlaying) return;
				//新行为树名称不为空 且不存在
				if (newBtName.text != "" && treeExecutor.behaviorTrees.FindIndex(tree => tree.treeName == newBtName.text) == -1)
				{
					treeExecutor.AddBehaviorTree(newBtName.text, true);
					//将当前的显示的行为树改变为新增的行为树图
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
				//删除树后会更新下拉菜单从而触发ChangeCurrentEditorTree
				//这会导致问题 所以依靠deleteTree标志来检测是否执行更新
				if (!deleteTree) ChangeCurrentEditorTree(editorTreeSelector.text);
				deleteTree = false;
			});
			addVariableButton.RegisterCallback<ClickEvent>(callback => { AddVariable(); });
			deleteVariableButton.RegisterCallback<ClickEvent>(callback => { DeleteVariable(); });
			//激活与初始化界面
			//只有当前选中的物体有行为树组件才能激活编辑
			rootVisualElement.SetEnabled(false);
			GameObject selected = Selection.activeGameObject;
			if (selected != null)
			{
				//如果当前选中的物体有行为树组件
				if (selected.GetComponent<BehaviorTreeExecutor>() != null)
				{
					//激活编辑 并生成节点视图
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

		//当选中的物体改变后调用
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
					//如果说当前的节点视图中有节点
					//也就是说当前是激活编辑状态 则保存当前的行为树
					UpdateMonoNodes();
					//从新根据当前选中的物体所有的行为树从新生成节点视图
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
		/// 创建一个mono节点
		/// </summary>
		/// <param name="nodeType"></param>
		/// <returns></returns>
		public BTNodeBase CreateMonoNode(Type nodeType)
		{
			BTNodeBase node = (BTNodeBase)targetGameObject.AddComponent(nodeType);
			//将脚本在inspector中隐藏不显示
			//node.hideFlags = HideFlags.HideInInspector;
			node.targetTree = treeExecutor.currentEditorTree;
			node.targetTree.treeNodes.Add(node);
			return node;
		}

		/// <summary>
		/// 创建节点视图
		/// </summary>
		private void GenerateNodeGraphView()
		{
			//生成Node 并根据记录的position信息设置位置
			if (targetGameObject == null) return;
			//如果一个行为树也没有
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
			//根据childNodes来连接各个节点
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
		/// 根据当前节点视图上有的节点去更新runtime节点 runtime中多出来的节点就删除
		/// </summary>
		public void UpdateMonoNodes()
		{
			if (Application.isPlaying) return;
			if (targetGameObject != null && treeExecutor.currentEditorTree != null && nodeGraphView.nodes.Count() != 0)
			{
				//首先更新节点数量
				List<Node> graphNodes = nodeGraphView.nodes.ToList();
				List<BTNodeBase> monoNodes = treeExecutor.currentEditorTree.treeNodes;
				monoNodes.ForEach(mnode =>
				{
					//如果当前runtime节点在节点视图中不存在则删除
					if (!graphNodes.Exists(gnode => ((BTNodeGraph)gnode).monoNode == mnode))
					{
						mnode.DeleteNode();
					}
				});
				//移除所有已删除节点
				monoNodes.RemoveAll(node => node == null);
				//再次更新节点视图的的节点位置信息到runtime节点
				graphNodes.ForEach(gnode =>
				{
					((BTNodeGraph)gnode).monoNode.graphNodePosition = gnode.GetPosition().position;
				});
				//之后再更新节点之间的连接关系 即Edges
				monoNodes.ForEach(mnode => { mnode.childNodes?.Clear(); });//先清空 重新计算
				nodeGraphView.edges.ForEach(edge =>
				{
					BTNodeBase fnode = ((BTNodeGraph)edge.output.node).monoNode;
					BTNodeBase cnode = ((BTNodeGraph)edge.input.node).monoNode;
					fnode.childNodes?.Add(cnode);
					cnode.parentNode = fnode;
				});
				//对其子节点进行排序 依据node在view当中的y位置
				monoNodes.ForEach((node) =>
				{
					node.childNodes?.Sort();
				});
			}
		}

		/// <summary>
		/// 重新生成节点视图
		/// </summary>
		public void RegenerateGraphNodesView()
		{
			//先清除
			nodeGraphView.ClearNodeGraph();
			//再生成
			GenerateNodeGraphView();
		}

		/// <summary>
		/// 根据当前编辑的行为树更新视图正上方的当前编辑的行为树选择下拉UI
		/// 更新的同时会触发ChangeCurrentEditorTree
		/// </summary>
		private void UpdateEditorTreeSelector()
		{
			//哪个游戏物体
			editorTreeSelector.Q<Label>().text = targetGameObject.name;
			//构造选择下拉节点选项
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
		/// 改变当前编辑的树 一般由editorTreeSelector选项改变时调用
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
		/// 删除当前正在编辑的树
		/// </summary>
		public void DeleteCurrentEditorTree()
		{
			if (treeExecutor.currentEditorTree != null)
			{
				BehaviorTree editortree = treeExecutor.currentEditorTree;
				//只有一个行为树 删了就没有了
				if (treeExecutor.behaviorTrees.Count == 1)
				{
					treeExecutor.currentEditorTree = null;
					treeExecutor.currentExecuteTree = null;
					//更新视图
					RegenerateGraphNodesView();
				}
				else
				{
					//改变当前编辑的行为树 默认修改为第一个行为树
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
				//删除行为树
				editortree.DeleteBehaviorTree();
				//更新下拉菜单
				deleteTree = true;//防止UpdateEditorTreeSelector 触发 ChangeCurrentEditorTree
				UpdateEditorTreeSelector();
			}
		}

		/// <summary>
		/// 更新节点检查器 在graphview中选中一个节点时调用
		/// </summary>
		public void UpdateNodeInspectorView(BTNodeBase selectNode)
		{
			if (selectNode == null)
			{
				nodeInspector.Clear();
				return;
			}
			//获取到当前节点中所有序列化数据
			SerializedObject serializedNode = new SerializedObject(selectNode);
			SerializedProperty nodeProperty = serializedNode.GetIterator();
			nodeProperty.Next(true);
			//遍历所有序列化数据
			while (nodeProperty.NextVisible(false))
			{
				if (nodeProperty.name == "m_Script") continue;
				//构造一个PropertyField用于显示
				PropertyField field = new PropertyField(nodeProperty, "");
				field.name = "field";
				field.SetEnabled(nodeProperty.name != "nodeType");
				//与实际的节点数据绑定
				field.Bind(serializedNode);
				//当编辑显示的数据时调用
				field.RegisterValueChangeCallback(callback => { });
				//单独显示字段的名称 方便后面进行布局
				Label fieldName = new Label(nodeProperty.displayName);
				fieldName.name = "fieldName";
				//用来放置字段 和字段名称
				VisualElement fieldContainer = new VisualElement();
				fieldContainer.Add(fieldName);
				fieldContainer.Add(field);
				//根据当前字段是否是variable reference来设置布局
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
		/// 生成blackboard的variable list
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
		/// 添加variable并刷新variableList
		/// </summary>
		private void AddVariable()
		{
			if (Application.isPlaying) return;
			treeBlackboard.CreateVariable(addVariableName.text, variableTypes[addVariableTypes.index]);
			variableList.Rebuild();
		}

		/// <summary>
		/// 删除variable并刷新variableList
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
		/// 更新添加variable的type下拉菜单
		/// </summary>
		private void UpdateVariableTypeSelector()
		{
			variableTypes.Clear();
			addVariableTypes.choices.Clear();
			//遍历整个程序集查找所有继承自BlackboardVariable类 即找到所有variable类型
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