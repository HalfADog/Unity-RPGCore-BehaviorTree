using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorWindowWorkSpaceUI : TwoPaneSplitView
{
	public new class UxmlFactory : UxmlFactory<EditorWindowWorkSpaceUI, UxmlTraits>
	{ }

	private VisualElement leftPane;
	private VisualElement rightPane;
	private TwoPaneSplitView leftPaneSplitView;

	private VisualElement inspector;
	private VisualElement behaviorTreesList;

	public EditorWindowWorkSpaceUI()
	{
		leftPane = new VisualElement();
		leftPane.name = "LeftPane";
		leftPane.style.minWidth = 192.0f;
		this.Add(leftPane);
		rightPane = new VisualElement();
		rightPane.name = "RightPane";
		rightPane.style.minWidth = 128.0f;
		this.Add(rightPane);

		leftPaneSplitView = new TwoPaneSplitView();
		leftPaneSplitView.name = "LeftPaneSplitView";

		inspector = new VisualElement();
		inspector.name = "Inspector";
		inspector.style.minHeight = 128.0f;
		leftPaneSplitView.Add(inspector);
		behaviorTreesList = new VisualElement();
		behaviorTreesList.name = "BTsList";
		behaviorTreesList.style.minHeight = 128.0f;
		leftPaneSplitView.Add(behaviorTreesList);

		leftPaneSplitView.orientation = TwoPaneSplitViewOrientation.Vertical;
		leftPane.Add(leftPaneSplitView);

		this.orientation = TwoPaneSplitViewOrientation.Horizontal;
	}
}