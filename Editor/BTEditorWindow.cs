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
		/// 当前选中的有btexecute的gameobject
		/// </summary>
		public static GameObject targetGameObject;

		/// <summary>
		/// 在nodegraphview中显示当前选中的behaviortree
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
			//加载主界面
			VisualTreeAsset editorViewTree = Resources.Load<VisualTreeAsset>("BehaviorTreeEditorWindow");
			TemplateContainer editorInstance = editorViewTree.CloneTree();
			editorInstance.StretchToParentSize();
			rootVisualElement.Add(editorInstance);

			//加载节点视图
			nodeGraphView = new BTNodeGraphView(this);
			nodeGraphView.StretchToParentSize();
			rootVisualElement.Q<VisualElement>("RightPane").Add(nodeGraphView);

			//获取节点检查器容器
			nodeInspector = rootVisualElement.Q<VisualElement>("Inspector");

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
					rootVisualElement.SetEnabled(true);
					GenerateNodeGraphView();
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
					//如果说当前的节点视图中有节点
					//也就是说当前是激活编辑状态 则保存当前的行为树
					if (nodeGraphView.nodes.Count() != 0) UpdateMonoNodes();
					//从新根据当前选中的物体所有的行为树从新生成节点视图
					RegenerateGraphNodesView();
				}
			}
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
			node.targetTree = targetGameObject.GetComponent<BehaviorTree>();
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
			targetGameObject.GetComponent<BehaviorTree>().treeNodes.ForEach((node) =>
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
			if (targetGameObject != null)
			{
				//首先更新节点数量
				List<Node> graphNodes = nodeGraphView.nodes.ToList();
				List<BTNodeBase> monoNodes = targetGameObject.GetComponent<BehaviorTree>().treeNodes;
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

		private void OnDisable()
		{
			UpdateMonoNodes();
			targetGameObject = null;
			rootVisualElement.Q<VisualElement>("RightPane").Remove(nodeGraphView);
			AssetDatabase.Refresh();
		}
	}
}