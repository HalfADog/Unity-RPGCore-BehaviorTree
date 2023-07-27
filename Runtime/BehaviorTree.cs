using RPGCore.BehaviorTree.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree
{
	/// <summary>
	/// 行为树 负责处理节点执行流和节点中断
	/// </summary>
	//[RequireComponent()]
	public class BehaviorTree : MonoBehaviour
	{
		/// <summary>
		/// 当前树的所有节点
		/// </summary>
		public List<BTNodeBase> treeNodes = new List<BTNodeBase>();

		/// <summary>
		/// 当前树的根节点
		/// </summary>
		public BTNodeBase rootNode = null;

		/// <summary>
		/// 当前节点执行流中的节点 该List最后一项表示正在执行的节点
		/// </summary>
		public List<BTNodeBase> executeNodes = new List<BTNodeBase>();
	}
}