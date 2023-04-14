using UnityEngine;
using UnityEngine.UIElements;

namespace Konfus.Tools.NodeGraphEditor
{
    internal class NodeSettingsView : VisualElement
    {
        public NodeSettingsView()
        {
            pickingMode = PickingMode.Ignore;
            styleSheets.Add(Resources.Load<StyleSheet>("GraphProcessorStyles/NodeSettings"));
            var uxml = Resources.Load<VisualTreeAsset>("UXML/NodeSettings");
            uxml.CloneTree(this);

            // Get the element we want to use as content container
            contentContainer = this.Q("contentContainer");
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            evt.StopPropagation();
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            evt.StopPropagation();
        }

        public override VisualElement contentContainer { get; }
    }
}