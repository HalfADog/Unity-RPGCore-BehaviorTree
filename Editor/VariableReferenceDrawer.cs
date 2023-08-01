using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPGCore.BehaviorTree.Editor
{
	[CustomPropertyDrawer(typeof(Variable.BlackboardReference), true)]
	public class VariableReferenceDrawer : PropertyDrawer
	{
		private GUIStyle constVarGUIStyle = new GUIStyle("MiniButton");

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			EditorGUI.BeginChangeCheck();
			position.height = 18f;
			//获取一些关键属性
			SerializedProperty keyProperty = property.FindPropertyRelative("key");
			SerializedProperty blackboardProperty = property.FindPropertyRelative("blackboard");
			SerializedProperty isConstantProperty = property.FindPropertyRelative("isConstant");
			//targetObject指的是当前property显示在哪个object的inspector上
			MonoBehaviour inspectedComponent = property.serializedObject.targetObject as MonoBehaviour;
			if (inspectedComponent != null)
			{
				//拿到blackboard
				Blackboard.BehaviorTreeBlackboard blackboard = inspectedComponent.GetComponent<Blackboard.BehaviorTreeBlackboard>();
				if (blackboard != null)
				{
					//如果当前的reference是常值和引用值都可以 那么就显示切换按钮
					if (property.FindPropertyRelative("useConstant").boolValue)
					{
						Rect togglePosition = position;
						togglePosition.width = 12;
						togglePosition.height = 16;
						isConstantProperty.boolValue = EditorGUI.Toggle(togglePosition, isConstantProperty.boolValue, constVarGUIStyle);
						position.xMin += 16;//xMin指的是x位置的最小值 这里加16就指的是绘制位置向后移动16px
					}
					//如果当前使用的是常量值 显示这个常量值
					if (isConstantProperty.boolValue)
					{
						EditorGUI.PropertyField(position, property.FindPropertyRelative("constantValue"), label);
					}
					//否则就是使用引用值
					else
					{
						//拿到所有blackboard variable
						System.Type desiredVariableType = fieldInfo.FieldType.BaseType.GetGenericArguments()[0];
						Variable.BlackboardVariable[] variables = blackboard.GetAllVariables();
						//构造下拉选项菜单
						List<string> keys = new List<string>();
						keys.Add("None");
						for (int i = 0; i < variables.Length; i++)
						{
							Variable.BlackboardVariable bv = variables[i];
							//只有reference类型相同的variable才能被加入下拉菜单中
							if (bv.GetType() == desiredVariableType)
							{
								keys.Add(bv.key);
							}
						}
						//检测当前选中的项
						int selected = keys.IndexOf(keyProperty.stringValue);
						if (selected < 0)
						{
							selected = 0;
							// If key is not empty it means variable was deleted and missing
							if (!System.String.IsNullOrEmpty(keyProperty.stringValue))
							{
								keys[0] = "Missing";
							}
						}
						//构造下拉菜单 并尝试给当前property（BlackboardReference）的key字段赋值
						//BlackboardReference通过key来在blackboard中查找对应的值
						int result = EditorGUI.Popup(position, label.text, selected, keys.ToArray());
						if (result > 0)
						{
							keyProperty.stringValue = keys[result];
							blackboardProperty.objectReferenceValue = blackboard;
						}
						else
						{
							keyProperty.stringValue = "";
							blackboardProperty.objectReferenceValue = null;
						}
					}
				}
			}
			if (EditorGUI.EndChangeCheck())
			{
				//保存修改值
				property.serializedObject.ApplyModifiedProperties();
			}
			EditorGUI.EndProperty();
		}
	}
}