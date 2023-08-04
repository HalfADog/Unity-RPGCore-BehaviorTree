using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Nodes
{
	/// <summary>
	/// ���ƽڵ�ִ��˳��
	/// </summary>
	public enum Dirction
	{
		Normal,//bottom to top
		Reverse,//top to bottom
		Random
	}

	/// <summary>
	/// �жϲ�������
	/// </summary>
	public enum AbortType
	{
		Self,
		LowPriority,
		Both,
		Noone
	}

	public abstract class BTNodeControl : BTNodeBase
	{
		/// <summary>
		/// �Ƿ��ǵ�ǰ���ĸ��ڵ�
		/// </summary>
		public bool isRootNode = false;

		/// <summary>
		/// ��ǰ�ӽڵ��±�
		/// </summary>
		protected int childNodeIndex { get; set; } = 0;

		/// <summary>
		/// ��ǰ�ӽڵ�
		/// </summary>
		public BTNodeBase currentChild { get; set; } = null;

		/// <summary>
		/// �ӽڵ�ִ��˳��
		/// </summary>
		public Dirction dirction = Dirction.Normal;

		/// <summary>
		/// �жϲ���������
		/// </summary>
		public AbortType abortType = AbortType.Noone;

		/// <summary>
		/// �Ƿ�����һ���ӽڵ�
		/// </summary>
		protected bool existNextChild = true;

		/// <summary>
		/// ����Direction��ȡ��ǰ�ӽڵ����һ���ӽڵ�
		/// </summary>
		/// <returns>�Ƿ��Ѿ���ȡ�����һ���ڵ�</returns>
		protected void GetNextChild()
		{
			switch (dirction)
			{
				case Dirction.Normal:
					childNodeIndex++;
					if (childNodeIndex == childNodes.Count)
					{
						childNodeIndex = 0;
						existNextChild = false;
					}
					break;

				case Dirction.Reverse:
					childNodeIndex--;
					if (childNodeIndex < 0)
					{
						childNodeIndex = childNodes.Count - 1;
						existNextChild = false;
					}
					break;
				//TODO:��¼����Щ�ڵ��Ѿ���ִ�� �´β��ᱻ�����
				case Dirction.Random:
					Random.InitState(childNodeIndex);
					childNodeIndex = Random.Range(0, childNodes.Count - 1);
					break;
			}
			currentChild = childNodes[childNodeIndex];
		}

		/// <summary>
		/// �����ӽڵ�
		/// </summary>
		protected void ResetCurrentChild()
		{
			switch (dirction)
			{
				case Dirction.Normal:
					childNodeIndex = 0;
					break;

				case Dirction.Reverse:
					childNodeIndex = childNodes.Count - 1;
					break;
			}
			currentChild = childNodes[childNodeIndex];
			existNextChild = true;
		}

		public int GetCurrentChildNodeIndex()
		{
			return childNodeIndex;
		}
	}
}