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
		public Dirction dirction { get; protected set; } = Dirction.Normal;

		/// <summary>
		/// �жϲ���������
		/// </summary>
		public AbortType abortType { get; protected set; } = AbortType.Noone;

		/// <summary>
		/// ����direction�õ���һ���ӽڵ�
		/// </summary>
		protected bool GetNextChild()
		{
			if (currentChild == null)
			{
				currentChild = childNodes[childNodeIndex];
				return false;
			}
			bool finish = false;
			switch (dirction)
			{
				case Dirction.Normal:
					childNodeIndex++;
					if (childNodeIndex == childNodes.Count)
					{
						childNodeIndex = 0;
						finish = true;
					}
					break;

				case Dirction.Reverse:
					childNodeIndex--;
					if (childNodeIndex < 0)
					{
						childNodeIndex = childNodes.Count - 1;
						finish = true;
					}
					break;
				//TODO:��¼����Щ�ڵ��Ѿ���ִ�� �´β��ᱻ�����
				case Dirction.Random:
					Random.InitState(childNodeIndex);
					childNodeIndex = Random.Range(0, childNodes.Count - 1);
					break;
			}
			currentChild = childNodes[childNodeIndex];
			return finish;
		}

		/// <summary>
		/// �����ӽڵ�
		/// </summary>
		protected void ResetChild()
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
			currentChild = null;
		}
	}
}