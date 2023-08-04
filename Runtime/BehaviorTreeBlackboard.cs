using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore.BehaviorTree.Variable;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using System;

namespace RPGCore.BehaviorTree.Blackboard
{
	public class BehaviorTreeBlackboard : MonoBehaviour
	{
		/// <summary>
		/// �洢������blackboard variable
		/// </summary>
		public List<Variable.BlackboardVariable> variables = new List<Variable.BlackboardVariable>();

		/// <summary>
		/// variable key �� variable���ɵ��ֵ�
		/// </summary>
		private Dictionary<string, Variable.BlackboardVariable> dictionary = new Dictionary<string, Variable.BlackboardVariable>();

		/// <summary>
		/// Ŀ��executor
		/// </summary>
		public BehaviorTreeExecutor targetExecutor;

		private void Awake()
		{
			dictionary.Clear();
			for (int i = 0; i < variables.Count; i++)
			{
				Variable.BlackboardVariable var = variables[i];
				dictionary.Add(var.key, var);
			}
		}

		/// <summary>
		/// ��ȡȫ��variable
		/// </summary>
		/// <returns></returns>
		public Variable.BlackboardVariable[] GetAllVariables()
		{
			return variables.ToArray();
		}

		/// <summary>
		/// ��ȡkey��Ӧ��variable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public T GetVariable<T>(string key) where T : Variable.BlackboardVariable
		{
			return (dictionary.TryGetValue(key, out Variable.BlackboardVariable val)) ? (T)val : null;
		}

		/// <summary>
		/// ����һ���µ�variable
		/// </summary>
		public void CreateVariable(string newVariableKey, Type variableType)
		{
			if (string.IsNullOrEmpty(newVariableKey) || newVariableKey.Equals("None"))
			{
				return;
			}
			string k = new string(
				newVariableKey.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray()
			);
			// Check for key duplicates
			for (int i = 0; i < variables.Count; i++)
			{
				if (variables[i].key == k)
				{
					Debug.LogWarning("Variable '" + k + "' already exists.");
					return;
				}
			}
			Variable.BlackboardVariable var = gameObject.AddComponent(variableType) as Variable.BlackboardVariable;
			var.key = k;
			variables.Add(var);
		}

		/// <summary>
		/// ɾ��ָ��key��variable
		/// </summary>
		/// <param name="key"></param>
		public void DeleteVariable(string key)
		{
			DestroyImmediate(variables.Find(v => v.key == key));
			variables.RemoveAll(v => v == null);
		}
	}
}