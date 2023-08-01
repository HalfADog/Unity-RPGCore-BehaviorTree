using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace RPGCore.BehaviorTree.Editor
{
	[CustomEditor(typeof(BehaviorTreeExecutor))]
	public class BTExecutorInspectorWindow : UnityEditor.Editor
	{
		private int selectTreeIndex;

		public override void OnInspectorGUI()
		{
			BehaviorTreeExecutor treeExecutor = (BehaviorTreeExecutor)target;

			if (GUILayout.Button("Start"))
			{
				treeExecutor.treeExecutePause = false;
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Pause"))
			{
				treeExecutor.treeExecutePause = true;
			}
			if (GUILayout.Button("Restart"))
			{
				treeExecutor.treeExecutePause = false;
				treeExecutor.treeRestart = true;
			}
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Open Editor"))
			{
				BTEditorWindow.OpenEditorWindow();
				BTEditorWindow.targetGameObject = target.GameObject();
			}
			GUILayout.Space(16);
			string executorState = "Executor State : " + (treeExecutor.treeExecutePause ? "Pause" : "Running");
			if (!Application.isPlaying) executorState = "Executor State : " + "[Game is not playing]";
			GUILayout.Label(executorState, GUILayout.ExpandWidth(true));
			//DrawDefaultInspector();
			//��ȡ���е���Ϊ�����Ʋ�����һ������ѡ����
			List<string> options = new List<string>();
			for (int i = 0; i < treeExecutor.behaviorTrees.Count; i++)
			{
				options.Add(treeExecutor.behaviorTrees[i].treeName);
			}
			//����1˵��������һ����Ϊ��
			if (options.Count > 0)
			{
				//�ҵ���ǰ����Ϊ����ѡ���е�λ��
				int preIndex = -1;
				if (treeExecutor.currentExecuteTree != null)
				{
					preIndex = options.IndexOf(treeExecutor.currentExecuteTree.treeName);
				}
				//��ǰ����Ϊ����Ϊ�� �ҵ�ǰ����Ϊ������
				if (preIndex != -1)
				{
					if (preIndex != selectTreeIndex)
					{
						selectTreeIndex = preIndex;
					}
				}
			}
			else
			{
				options.Add("Noone");
				selectTreeIndex = 0;
			}
			selectTreeIndex = EditorGUILayout.Popup("Execute Tree", selectTreeIndex, options.ToArray(), GUILayout.ExpandWidth(true));
			treeExecutor.currentExecuteTree = treeExecutor.behaviorTrees.Find(tree => tree.treeName == options[selectTreeIndex]);

			GUILayout.Space(16);
			DrawDefaultInspector();
		}
	}
}