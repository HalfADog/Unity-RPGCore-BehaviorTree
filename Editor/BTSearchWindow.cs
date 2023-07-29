using RPGCore.BehaviorTree.Editor;
using RPGCore.BehaviorTree.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPGCore.BehaviorTree.Editor
{
	public class BTSearchWindow : ScriptableObject, ISearchWindowProvider
	{
		private BTEditorWindow editorWindow;
		private BTNodeGraphView graphView;

		//��ʼ��
		public void Init(BTEditorWindow window, BTNodeGraphView view)
		{
			editorWindow = window;
			graphView = view;
		}

		//����һ�����νṹ�Ľڵ㴴����
		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>();
			searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("Behavior Tree Nodes"), 0));
			//ͨ����������ҵ����м̳���BTNodeBase���� Ҳ�����ҵ����нڵ���
			List<Type> types = new List<Type>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				List<Type> result = assembly.GetTypes().Where(type =>
				{
					return type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(BTNodeBase));
				}).ToList();
				types.AddRange(result);
			}
			//ͨ���ڵ��������õ�·��������������һ�����νṹ�ڵ����
			List<SearchWindowMenuItem> mainMenu = new List<SearchWindowMenuItem>();
			foreach (Type type in types)
			{
				//��ȡ�ڵ����Ե�NodePath
				string nodePath = type.GetCustomAttribute<BTNodeAttribute>()?.NodePath;
				if (nodePath == null) continue;
				//��·����ÿһ��ָ�
				string[] menus = nodePath.Split('/');
				//�����ָ��ÿһ�������
				List<SearchWindowMenuItem> currentFloor = mainMenu;
				for (int i = 0; i < menus.Length; i++)
				{
					string currentName = menus[i];
					bool exist = false;
					//���������һ��˵����ǰ��ǲ˵���
					bool lastFloor = (i == (menus.Length - 1));
					//�����ǰ���ܹ��ڵ�ǰ�����ҵ�˵����ǰ���Ѿ�����
					SearchWindowMenuItem temp = currentFloor.Find(item => item.Name == currentName);
					if (temp != null)
					{
						exist = true;
						//����ǰ���µ�������Ϊ��һ��
						currentFloor = temp.ChildItems;
					}
					//��ǰ����� �͹��쵱ǰ����뵽��ǰ�㼶��
					if (!exist)
					{
						SearchWindowMenuItem item = new SearchWindowMenuItem() { Name = currentName, IsNode = lastFloor };
						currentFloor.Add(item);
						//�����ǰ��ǽڵ� ��û����һ��
						if (!item.IsNode && item.ChildItems == null)
						{
							//�����µ��Ӽ���
							item.ChildItems = new List<SearchWindowMenuItem>();
						}
						if (item.IsNode) item.type = type;
						currentFloor = item.ChildItems;
					}
				}
			}
			MakeSearchTree(mainMenu, 1, ref searchTreeEntries);
			return searchTreeEntries;
		}

		//���ݹ���Ľڵ�Ŀ¼�ṹ�������յĽڵ㴴��Ŀ¼
		private void MakeSearchTree(List<SearchWindowMenuItem> floor, int floorIndex, ref List<SearchTreeEntry> treeEntries)
		{
			foreach (var item in floor)
			{
				//��ǰ��ǽڵ�
				if (!item.IsNode)
				{
					//����һ��
					SearchTreeEntry entry = new SearchTreeGroupEntry(new GUIContent(item.Name))
					{
						level = floorIndex,
					};
					treeEntries.Add(entry);
					//���뵱ǰ�����һ���������
					MakeSearchTree(item.ChildItems, floorIndex + 1, ref treeEntries);
				}
				//��ǰ���ǽڵ�
				else
				{
					//����ڵ��� �ص����� ��������
					SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(item.Name))
					{
						userData = item.type,
						level = floorIndex
					};
					treeEntries.Add(entry);
				}
			}
		}

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
			//��ȡ����ǰ����λ��
			var worldMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(
				editorWindow.rootVisualElement.parent,
				context.screenMousePosition - editorWindow.position.position
			);
			var localMousePosition = graphView.contentViewContainer.WorldToLocal(worldMousePosition);
			Type type = (Type)SearchTreeEntry.userData;
			graphView.MakeNode(editorWindow.CreateMonoNode(type), localMousePosition);
			return true;
		}
	}

	//����SearchWindowʱ �����洢�ڵ�Ŀ¼�Ľṹ
	public class SearchWindowMenuItem
	{
		//Ŀ¼�������
		public string Name { get; set; }

		//��ǰĿ¼���Ƿ��ǽڵ�
		public bool IsNode { get; set; }

		public Type type;
		public List<SearchWindowMenuItem> ChildItems { get; set; }
	}
}