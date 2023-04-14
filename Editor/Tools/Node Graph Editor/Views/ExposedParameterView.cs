using System;
using System.Collections.Generic;
using System.Linq;
using Konfus.Systems.Node_Graph;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Konfus.Tools.NodeGraphEditor
{
    public class ExposedParameterView : PinnedElementView
    {
        protected GraphView graphView;

        private new const string title = "Parameters";

        private readonly string exposedParameterViewStyle = "GraphProcessorStyles/ExposedParameterView";

        private List<Rect> blackboardLayouts = new();

        public ExposedParameterView()
        {
            var style = Resources.Load<StyleSheet>(exposedParameterViewStyle);
            if (style != null)
                styleSheets.Add(style);
        }

        protected virtual void OnAddClicked()
        {
            var parameterType = new GenericMenu();

            foreach (Type paramType in GetExposedParameterTypes())
                parameterType.AddItem(new GUIContent(GetNiceNameFromType(paramType)), false, () =>
                {
                    string uniqueName = "New " + GetNiceNameFromType(paramType);

                    uniqueName = GetUniqueExposedPropertyName(uniqueName);
                    graphView.graph.AddExposedParameter(uniqueName, paramType);
                });

            parameterType.ShowAsContext();
        }

        protected string GetNiceNameFromType(Type type)
        {
            string name = type.Name;

            // Remove parameter in the name of the type if it exists
            name = name.Replace("Parameter", "");

            return ObjectNames.NicifyVariableName(name);
        }

        protected string GetUniqueExposedPropertyName(string name)
        {
            // Generate unique name
            string uniqueName = name;
            int i = 0;
            while (graphView.graph.exposedParameters.Any(e => e.name == name))
                name = uniqueName + " " + i++;
            return name;
        }

        protected virtual IEnumerable<Type> GetExposedParameterTypes()
        {
            foreach (Type type in TypeCache.GetTypesDerivedFrom<ExposedParameter>())
            {
                if (type.IsGenericType)
                    continue;

                yield return type;
            }
        }

        protected virtual void UpdateParameterList()
        {
            content.Clear();

            foreach (ExposedParameter param in graphView.graph.exposedParameters)
            {
                var row = new BlackboardRow(new ExposedParameterFieldView(graphView, param),
                    new ExposedParameterPropertyView(graphView, param));
                row.expanded = param.settings.expanded;
                row.RegisterCallback<GeometryChangedEvent>(e => { param.settings.expanded = row.expanded; });

                content.Add(row);
            }
        }

        protected override void Initialize(GraphView graphView)
        {
            this.graphView = graphView;
            base.title = title;
            scrollable = true;

            graphView.onExposedParameterListChanged += UpdateParameterList;
            graphView.initialized += UpdateParameterList;
            Undo.undoRedoPerformed += UpdateParameterList;

            RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
            RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
            RegisterCallback<MouseDownEvent>(OnMouseDownEvent, TrickleDown.TrickleDown);
            RegisterCallback<DetachFromPanelEvent>(OnViewClosed);

            UpdateParameterList();

            // Add exposed parameter button
            header.Add(new Button(OnAddClicked)
            {
                text = "+"
            });
        }

        private void OnViewClosed(DetachFromPanelEvent evt)
        {
            Undo.undoRedoPerformed -= UpdateParameterList;
        }

        private void OnMouseDownEvent(MouseDownEvent evt)
        {
            blackboardLayouts = content.Children().Select(c => c.layout).ToList();
        }

        private int GetInsertIndexFromMousePosition(Vector2 pos)
        {
            pos = content.WorldToLocal(pos);
            // We only need to look for y axis;
            float mousePos = pos.y;

            if (mousePos < 0)
                return 0;

            int index = 0;
            foreach (Rect layout in blackboardLayouts)
            {
                if (mousePos > layout.yMin && mousePos < layout.yMax)
                    return index + 1;
                index++;
            }

            return content.childCount;
        }

        private void OnDragUpdatedEvent(DragUpdatedEvent evt)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            int newIndex = GetInsertIndexFromMousePosition(evt.mousePosition);
            object graphSelectionDragData = DragAndDrop.GetGenericData("DragSelection");

            if (graphSelectionDragData == null)
                return;

            foreach (ISelectable obj in graphSelectionDragData as List<ISelectable>)
                if (obj is ExposedParameterFieldView view)
                {
                    VisualElement blackBoardRow = view.parent.parent.parent.parent.parent.parent;
                    int oldIndex = content.Children().ToList().FindIndex(c => c == blackBoardRow);
                    // Try to find the blackboard row
                    content.Remove(blackBoardRow);

                    if (newIndex > oldIndex)
                        newIndex--;

                    content.Insert(newIndex, blackBoardRow);
                }
        }

        private void OnDragPerformEvent(DragPerformEvent evt)
        {
            bool updateList = false;

            int newIndex = GetInsertIndexFromMousePosition(evt.mousePosition);
            foreach (ISelectable obj in DragAndDrop.GetGenericData("DragSelection") as List<ISelectable>)
                if (obj is ExposedParameterFieldView view)
                {
                    if (!updateList)
                        graphView.RegisterCompleteObjectUndo("Moved parameters");

                    int oldIndex = graphView.graph.exposedParameters.FindIndex(e => e == view.parameter);
                    ExposedParameter parameter = graphView.graph.exposedParameters[oldIndex];
                    graphView.graph.exposedParameters.RemoveAt(oldIndex);

                    // Patch new index after the remove operation:
                    if (newIndex > oldIndex)
                        newIndex--;

                    graphView.graph.exposedParameters.Insert(newIndex, parameter);

                    updateList = true;
                }

            if (updateList)
            {
                graphView.graph.NotifyExposedParameterListChanged();
                evt.StopImmediatePropagation();
                UpdateParameterList();
            }
        }
    }
}