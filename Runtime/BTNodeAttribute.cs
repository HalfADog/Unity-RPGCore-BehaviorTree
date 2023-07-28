using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree
{
	[AttributeUsage(AttributeTargets.Class)]
	public class BTNodeAttribute : Attribute
	{
		//节点分类路径
		public string NodePath { get; set; }

		//节点描述
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