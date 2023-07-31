using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace RPGCore.BehaviorTree.Editor
{
	//ATTENTION:�ݶ��Ȳ�Ϊÿ��behavior tree��Ӷ�����Ϣ��ʾ
	//[CustomEditor(typeof(BehaviorTree))]
	public class BTInspectorWindow : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Editor"))
			{
				BTEditorWindow.OpenEditorWindow();
				BTEditorWindow.targetGameObject = target.GameObject();
			}
			base.OnInspectorGUI();
		}
	}
}