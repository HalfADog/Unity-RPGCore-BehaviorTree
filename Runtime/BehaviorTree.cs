using RPGCore.BehaviorTree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree
{
	/// <summary>
	/// ��Ϊ�� ������ڵ�ִ�����ͽڵ��ж�
	/// </summary>
	//[RequireComponent()]
	public class BehaviorTree : MonoBehaviour
	{
		/// <summary>
		/// ��ǰ�������нڵ�
		/// </summary>
		public List<BTNodeBase> treeNodes = new List<BTNodeBase>();

		/// <summary>
		/// ��ǰ���ĸ��ڵ�
		/// </summary>
		public BTNodeBase rootNode = null;

		/// <summary>
		/// ��ǰ�ڵ�ִ�����еĽڵ� ��List���һ���ʾ����ִ�еĽڵ�
		/// </summary>
		public List<BTNodeBase> executeNodes = new List<BTNodeBase>();
	}
}