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
		/// ͼ�νڵ㵱ǰλ�ڵĽڵ���ͼ
		/// </summary>
		public BTNodeGraphView graphView;

		private VisualElement descriptionContainer;

		/// <summary>
		/// ��ǰͼ�νڵ��Ӧ��ʵ�ʽڵ�
		/// </summary>
		public BTNodeBase monoNode;

		public BTNodeGraph(BTNodeBase node, BTNodeGraphView graphView) : base(
			AssetDatabase.GetAssetPath(Resources.Load<VisualTreeAsset>("BehaviorTreeNodeView")))
		{
			this.graphView = graphView;
			monoNode = node;
			descriptionContainer = this.Q<VisualElement>("description");
			//Ĭ�ϲ���ʾ����
			descriptionContainer.style.display = DisplayStyle.None;
			GenerateNode();
		}

		/// <summary>
		/// ���ɶ�Ӧ�ڵ����������˿�
		/// </summary>
		private void GenerateNode()
		{
			title = monoNode.nodeName;
			//�Ƿ�����ж������
			bool multiConnection = true;
			//�Ƿ�������˿� Ҷ�ӽڵ�û��
			bool outputPort = true;
			//��ȡ�������õĽڵ�����
			descriptionContainer.Q<Label>().text = monoNode.GetType().GetCustomAttribute<BTNodeAttribute>().NodeDescription;
			//���ݽڵ��������ýڵ�
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
		/// ���Input�˿�
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
		/// ���Output�˿� ���ݽڵ����� ���ö˿�����
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