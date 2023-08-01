using RPGCore.BehaviorTree.Nodes;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGCore.BehaviorTree.Editor
{
	public class BTNodeGraph : Node
	{
		public Port input;
		public Port output;

		/// <summary>
		/// 图形节点当前位于的节点视图
		/// </summary>
		public BTNodeGraphView graphView;

		private VisualElement descriptionContainer;

		/// <summary>
		/// 当前图形节点对应的实际节点
		/// </summary>
		public BTNodeBase monoNode;

		public BTNodeGraph(BTNodeBase node, BTNodeGraphView graphView) : base(
			AssetDatabase.GetAssetPath(Resources.Load<VisualTreeAsset>("BehaviorTreeNodeView")))
		{
			this.graphView = graphView;
			monoNode = node;
			descriptionContainer = this.Q<VisualElement>("description");
			//默认不显示描述
			descriptionContainer.style.display = DisplayStyle.None;
			GenerateNode();
		}

		/// <summary>
		/// 生成对应节点的输入输出端口
		/// </summary>
		private void GenerateNode()
		{
			title = monoNode.nodeName;
			//是否可以有多个连接
			bool multiConnection = true;
			//是否有输出端口 叶子节点没有
			bool outputPort = true;
			//获取属性设置的节点描述
			descriptionContainer.Q<Label>().text = monoNode.GetType().GetCustomAttribute<BTNodeAttribute>().NodeDescription;
			//依据节点类型配置节点
			switch (monoNode.nodeType)
			{
				case Nodes.BTNodeType.Sequence:
				case Nodes.BTNodeType.Select:
				case Nodes.BTNodeType.Parallel:
					outputPort = true;
					AddToClassList("composite");
					break;

				case Nodes.BTNodeType.Decorator:
					multiConnection = false;
					outputPort = true;
					AddToClassList("decorator");
					break;

				case Nodes.BTNodeType.Action:
					outputPort = false;
					AddToClassList("action");
					break;

				case Nodes.BTNodeType.Condition:
					outputPort = false;
					AddToClassList("condition");
					break;
			}
			CreateInputPorts();
			if (outputPort)
			{
				CreateOutputPorts(multiConnection);
			}
			RefreshExpandedState();
			RefreshPorts();
		}

		/// <summary>
		/// 添加Input端口
		/// </summary>
		private void CreateInputPorts()
		{
			input = InstantiatePort(Orientation.Vertical, Direction.Input,
									Port.Capacity.Single, typeof(Node));
			if (input == null) return;
			input.portName = "";
			input.name = "input-port";
			inputContainer.Add(input);
		}

		/// <summary>
		/// 添加Output端口 根据节点类型 设置端口类型
		/// </summary>
		private void CreateOutputPorts(bool multiConnection)
		{
			output = InstantiatePort(Orientation.Vertical, Direction.Output,
				multiConnection ? Port.Capacity.Multi : Port.Capacity.Single, typeof(Node));
			if (output == null) return;
			output.portName = "";
			output.name = "output-port";
			outputContainer.Add(output);
		}

		public override void OnSelected()
		{
			base.OnSelected();
			if (descriptionContainer.Q<Label>().text != "")
			{
				descriptionContainer.style.display = DisplayStyle.Flex;
			}
			graphView.currentSelectedNode = null;
			graphView.currentSelectedNode = monoNode;
		}

		public override void OnUnselected()
		{
			base.OnUnselected();
			descriptionContainer.style.display = DisplayStyle.None;
		}
	}
}