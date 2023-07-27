using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
		}
	}
}