using RPGCore.BehaviorTree.Nodes;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGCore.BehaviorTree.Editor
{
	public class BTNodeGraphView : GraphView
	{
		private BTSearchWindow searchWindow;
		private BTEditorWindow editorWindow;
		public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
		private BTNodeBase selectedNode;

		/// <summary>
		/// ��ǰ�ڽڵ���ͼ��ѡ��Ľڵ� setʱ����UpdateNodeInspectorView
		/// </summary>
		public BTNodeBase currentSelectedNode
		{
			get { return selectedNode; }
			set
			{
				if (selectedNode != value)
				{
					selectedNode = value;
					//Debug.Log(selectedNode.nodeName + " Selected!");
				}
				editorWindow.UpdateNodeInspectorView(selectedNode);
			}
		}

		public BTNodeGraphView(BTEditorWindow window)
		{
			editorWindow = window;
			styleSheets.Add(Resources.Load<StyleSheet>("NodeGraphGridBackground"));

			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			var grid = new GridBackground();
			Insert(0, grid);

			AddSearchWindow(editorWindow);
		}

		//�ڽڵ�ͼ����ӽڵ���������
		private void AddSearchWindow(BTEditorWindow editorWindow)
		{
			searchWindow = ScriptableObject.CreateInstance<BTSearchWindow>();
			searchWindow.Init(editorWindow, this);
			//��Ӧ�ڵ㴴���¼� �򿪽ڵ㴴����������
			nodeCreationRequest = context =>
			{
				if (BTEditorWindow.targetGameObject != null)
					SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
			};
		}

		//���������ڵ�ʱ���� ��ȡ����ǰ�ڵ�˿��ܹ����ӵ�������ڵ�˿�
		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			List<Port> compatiblePorts = new List<Port>();
			ports.ForEach(
				(port) =>
				{
					if (startPort.node != port.node && //�����Լ������Լ�
						startPort.direction != port.direction)//����input��input output��output
					{
						compatiblePorts.Add(port);
					}
				}
			);
			return compatiblePorts;
		}

		/// <summary>
		/// ���ݴ���Ľڵ㴴������ӻ��ڵ�
		/// </summary>
		/// <param name="monoNode">ʵ�ʽڵ�</param>
		/// <returns></returns>
		public BTNodeGraph MakeNode(BTNodeBase monoNode)
		{
			return MakeNode(monoNode, monoNode.graphNodePosition);
		}

		public BTNodeGraph MakeNode(BTNodeBase monoNode, Vector2 position)
		{
			BTNodeGraph graphNode = new BTNodeGraph(monoNode, this);
			graphNode.SetPosition(new Rect(position, graphNode.GetPosition().size));
			AddElement(graphNode);
			return graphNode;
		}

		/// <summary>
		/// ���ݴ���Ľڵ��˿ڴ������߲�����
		/// </summary>
		/// <param name="oput"></param>
		/// <param name="iput"></param>
		/// <returns></returns>
		public Edge MakeEdge(Port oput, Port iput)
		{
			var edge = new Edge { output = oput, input = iput };
			edge?.input.Connect(edge);
			edge?.output.Connect(edge);
			AddElement(edge);
			return edge;
		}

		public Edge MakeEdge(BTNodeGraph output, BTNodeGraph input)
		{
			return MakeEdge(output.output, input.input);
		}

		/// <summary>
		/// ������ӻ��ڵ�ͼ
		/// </summary>
		public void ClearNodeGraph()
		{
			foreach (var node in nodes)
			{
				//Remove Edges
				edges.ToList()
					 .Where(x => x.input.node == node)
					 .ToList()
					 .ForEach(edge => RemoveElement(edge));

				//Remove Node
				RemoveElement(node);
			}
		}

		public void UpdateNodesStateView()
		{
			for (int i = 0; i < nodes.Count(); i++)
			{
				BTNodeGraph node = nodes.ToArray()[i] as BTNodeGraph;
				node.UpdateNodeStateView();
			}
		}
	}
}