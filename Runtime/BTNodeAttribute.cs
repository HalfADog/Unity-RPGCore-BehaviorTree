using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree
{
	[AttributeUsage(AttributeTargets.Class)]
	public class BTNodeAttribute : Attribute
	{
		//�ڵ����·��
		public string NodePath { get; set; }

		//�ڵ�����
		public string NodeDescription { get; set; } = "";

		public BTNodeAttribute(string nodepath)
		{
			NodePath = nodepath;
		}

		public BTNodeAttribute(string nodepath, string description)
		{
			NodePath = nodepath;
			NodeDescription = description;
		}
	}
}