using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BTSearchWindow : ScriptableObject, ISearchWindowProvider
{
	public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
	{
		throw new System.NotImplementedException();
	}

	public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
	{
		throw new System.NotImplementedException();
	}
}