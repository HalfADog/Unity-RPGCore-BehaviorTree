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

		//初始化
		public void Init(BTEditorWindow window, BTNodeGraphView view)
		{
			editorWindow = window;
			graphView = view;
		}

		//构造一个树形结构的节点创建表
		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
			List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>();
			searchTreeEntries.Add(new SearchTreeGroupEntry(new GUIContent("Behavior Tree Nodes"), 0));
			//通过反射程序集找到所有继承自BTNodeBase的类 也就是找到所有节点类
			List<Type> types = new List<Type>();
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				List<Type> result = assembly.GetTypes().Where(type =>
				{
					return type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(BTNodeBase));
				}).ToList();
				types.AddRange(result);
			}
			//通过节点属性设置的路径和名称来构造一个树形结构节点分类
			List<SearchWindowMenuItem> mainMenu = new List<SearchWindowMenuItem>();
			foreach (Type type in types)
			{
				//获取节点属性的NodePath
				string nodePath = type.GetCustomAttribute<BTNodeAttribute>()?.NodePath;
				if (nodePath == null) continue;
				//将路径中每一项分割
				string[] menus = nodePath.Split('/');
				//遍历分割的每一项的名称
				List<SearchWindowMenuItem> currentFloor = mainMenu;
				for (int i = 0; i < menus.Length; i++)
				{
					string currentName = menus[i];
					bool exist = false;
					//还不是最后一项说明当前项还是菜单项
					bool lastFloor = (i == (menus.Length - 1));
					//如果当前项能够在当前层中找到说明当前项已经存在
					SearchWindowMenuItem temp = currentFloor.Find(item => item.Name == currentName);
					if (temp != null)
					{
						exist = true;
						//将当前项下的子项作为下一层
						currentFloor = temp.ChildItems;
					}
					//当前项不存在 就构造当前项并加入到当前层级中
					if (!exist)
					{
						SearchWindowMenuItem item = new SearchWindowMenuItem() { Name = currentName, IsNode = lastFloor };
						currentFloor.Add(item);
						//如果当前项不是节点 且没有下一层
						if (!item.IsNode && item.ChildItems == null)
						{
							//构造新的子级层
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

		//根据构造的节点目录结构构造最终的节点创建目录
		private void MakeSearchTree(List<SearchWindowMenuItem> floor, int floorIndex, ref List<SearchTreeEntry> treeEntries)
		{
			foreach (var item in floor)
			{
				//当前项不是节点
				if (!item.IsNode)
				{
					//构造一层
					SearchTreeEntry entry = new SearchTreeGroupEntry(new GUIContent(item.Name))
					{
						level = floorIndex,
					};
					treeEntries.Add(entry);
					//进入当前项的下一层继续构造
					MakeSearchTree(item.ChildItems, floorIndex + 1, ref treeEntries);
				}
				//当前项是节点
				else
				{
					//构造节点项 回到顶层 继续构造
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
			//获取到当前鼠标的位置
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

	//构造SearchWindow时 用来存储节点目录的结构
	public class SearchWindowMenuItem
	{
		//目录项的名称
		public string Name { get; set; }

		//当前目录项是否是节点
		public bool IsNode { get; set; }

		public Type type;
		public List<SearchWindowMenuItem> ChildItems { get; set; }
	}
}