using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCore.BehaviorTree.Variable;

namespace RPGCore.BehaviorTree.Blackboard
{
	public class BehaviorTreeBlackboard : MonoBehaviour
	{
		public List<Variable.BlackboardVariable> variables = new List<Variable.BlackboardVariable>();
		private Dictionary<string, Variable.BlackboardVariable> dictionary = new Dictionary<string, Variable.BlackboardVariable>();

		private void Awake()
		{
			dictionary.Clear();
			for (int i = 0; i < variables.Count; i++)
			{
				Variable.BlackboardVariable var = variables[i];
				dictionary.Add(var.key, var);
			}
		}

		public Variable.BlackboardVariable[] GetAllVariables()
		{
			return variables.ToArray();
		}

		public T GetVariable<T>(string key) where T : Variable.BlackboardVariable
		{
			return (dictionary.TryGetValue(key, out Variable.BlackboardVariable val)) ? (T)val : null;
		}
	}
}