using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGCore.BehaviorTree.Blackboard
{
	public class BehaviorTreeBlackboard : MonoBehaviour
	{
		public List<BlackboardVariable> variables = new List<BlackboardVariable>();
		private Dictionary<string, BlackboardVariable> dictionary = new Dictionary<string, BlackboardVariable>();

		private void Awake()
		{
			dictionary.Clear();
			for (int i = 0; i < variables.Count; i++)
			{
				BlackboardVariable var = variables[i];
				dictionary.Add(var.key, var);
			}
		}

		public BlackboardVariable[] GetAllVariables()
		{
			return variables.ToArray();
		}

		public T GetVariable<T>(string key) where T : BlackboardVariable
		{
			return (dictionary.TryGetValue(key, out BlackboardVariable val)) ? (T)val : null;
		}
	}
}