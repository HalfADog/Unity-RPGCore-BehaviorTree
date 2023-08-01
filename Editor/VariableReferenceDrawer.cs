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
			//��ȡһЩ�ؼ�����
			SerializedProperty keyProperty = property.FindPropertyRelative("key");
			SerializedProperty blackboardProperty = property.FindPropertyRelative("blackboard");
			SerializedProperty isConstantProperty = property.FindPropertyRelative("isConstant");
			//targetObjectָ���ǵ�ǰproperty��ʾ���ĸ�object��inspector��
			MonoBehaviour inspectedComponent = property.serializedObject.targetObject as MonoBehaviour;
			if (inspectedComponent != null)
			{
				//�õ�blackboard
				Blackboard.BehaviorTreeBlackboard blackboard = inspectedComponent.GetComponent<Blackboard.BehaviorTreeBlackboard>();
				if (blackboard != null)
				{
					//�����ǰ��reference�ǳ�ֵ������ֵ������ ��ô����ʾ�л���ť
					if (property.FindPropertyRelative("useConstant").boolValue)
					{
						Rect togglePosition = position;
						togglePosition.width = 12;
						togglePosition.height = 16;
						isConstantProperty.boolValue = EditorGUI.Toggle(togglePosition, isConstantProperty.boolValue, constVarGUIStyle);
						position.xMin += 16;//xMinָ����xλ�õ���Сֵ �����16��ָ���ǻ���λ������ƶ�16px
					}
					//�����ǰʹ�õ��ǳ���ֵ ��ʾ�������ֵ
					if (isConstantProperty.boolValue)
					{
						EditorGUI.PropertyField(position, property.FindPropertyRelative("constantValue"), label);
					}
					//�������ʹ������ֵ
					else
					{
						//�õ�����blackboard variable
						System.Type desiredVariableType = fieldInfo.FieldType.BaseType.GetGenericArguments()[0];
						Variable.BlackboardVariable[] variables = blackboard.GetAllVariables();
						//��������ѡ��˵�
						List<string> keys = new List<string>();
						keys.Add("None");
						for (int i = 0; i < variables.Length; i++)
						{
							Variable.BlackboardVariable bv = variables[i];
							//ֻ��reference������ͬ��variable���ܱ����������˵���
							if (bv.GetType() == desiredVariableType)
							{
								keys.Add(bv.key);
							}
						}
						//��⵱ǰѡ�е���
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
						//���������˵� �����Ը���ǰproperty��BlackboardReference����key�ֶθ�ֵ
						//BlackboardReferenceͨ��key����blackboard�в��Ҷ�Ӧ��ֵ
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
				//�����޸�ֵ
				property.serializedObject.ApplyModifiedProperties();
			}
			EditorGUI.EndProperty();
		}
	}
}