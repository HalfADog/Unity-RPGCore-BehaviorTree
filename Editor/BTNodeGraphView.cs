using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
		private BehaviorTreeNodeBase selectedNode;

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
		}
	}
}