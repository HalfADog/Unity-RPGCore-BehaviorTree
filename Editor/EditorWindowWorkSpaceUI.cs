using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorWindowWorkSpaceUI : TwoPaneSplitView
{
	public new class UxmlFactory : UxmlFactory<EditorWindowWorkSpaceUI, UxmlTraits>
	{ }

	private VisualElement leftPane;
	private VisualElement rightPane;
	private TwoPaneSplitView leftPaneSplitView;

	private VisualElement inspectorContainer;
	private ScrollView inspector;
	private VisualElement blackboardContainer;
	private ScrollView blackboard;

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

		inspectorContainer = new VisualElement();
		inspectorContainer.name = "InspectorContainer";
		inspectorContainer.style.minHeight = 128.0f;
		leftPaneSplitView.Add(inspectorContainer);
		inspector = new ScrollView();
		inspector.name = "Inspector";
		Label inspectorTitle = new Label("Inspector");
		inspectorTitle.name = "inspectorTitle";
		inspectorContainer.Add(inspectorTitle);
		inspectorContainer.Add(inspector);

		blackboardContainer = new VisualElement();
		blackboardContainer.name = "BlackboardContainer";
		blackboardContainer.style.minHeight = 128.0f;
		leftPaneSplitView.Add(blackboardContainer);
		blackboard = new ScrollView();
		blackboard.name = "Blackboard";
		Label blackboardTitle = new Label("Blackboard");
		blackboardTitle.name = "blackboardTitle";
		blackboardContainer.Add(blackboardTitle);
		blackboardContainer.Add(blackboard);

		leftPaneSplitView.orientation = TwoPaneSplitViewOrientation.Vertical;
		leftPane.Add(leftPaneSplitView);

		this.orientation = TwoPaneSplitViewOrientation.Horizontal;
	}
}