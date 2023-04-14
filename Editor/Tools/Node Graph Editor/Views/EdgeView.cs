using Konfus.Systems.Node_Graph;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Konfus.Tools.NodeGraphEditor
{
    public class EdgeView : Edge
    {
        public PortView InputPort => input as PortView;
        public PortView OutputPort => output as PortView;

        public bool isConnected = false;

        public SerializableEdge serializedEdge => userData as SerializableEdge;

        private readonly string edgeStyle = "GraphProcessorStyles/EdgeView";

        protected GraphView owner => ((input ?? output) as PortView).owner.owner;

        public EdgeView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(edgeStyle));
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public override void OnPortChanged(bool isInput)
        {
            base.OnPortChanged(isInput);
            UpdateEdgeSize();
        }

        public void UpdateEdgeSize()
        {
            if (input == null && output == null)
                return;

            PortData inputPortData = (input as PortView)?.portData;
            PortData outputPortData = (output as PortView)?.portData;

            for (int i = 1; i < 20; i++)
                RemoveFromClassList($"edge_{i}");
            int maxPortSize = Mathf.Max(inputPortData?.sizeInPixel ?? 0, outputPortData?.sizeInPixel ?? 0);
            if (maxPortSize > 0)
                AddToClassList($"edge_{Mathf.Max(1, maxPortSize - 6)}");
        }

        protected override void OnCustomStyleResolved(ICustomStyle styles)
        {
            base.OnCustomStyleResolved(styles);

            UpdateEdgeControl();
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            if (e.clickCount == 2)
            {
                // Empirical offset:
                Vector2 position = e.mousePosition;
                position += new Vector2(-10f, -28);
                Vector2 mousePos = owner.ChangeCoordinatesTo(owner.contentViewContainer, position);

                owner.AddRelayNode(input as PortView, output as PortView, mousePos);
            }
        }

        public override bool UpdateEdgeControl()
        {
            if (InputPort != null)
            {
                bool isInputPortVertical = InputPort.portData.vertical;
                edgeControl.inputOrientation = isInputPortVertical ? Orientation.Vertical : Orientation.Horizontal;
            }

            if (OutputPort != null)
            {
                bool isOutputPortVertical = OutputPort.portData.vertical;
                edgeControl.outputOrientation = isOutputPortVertical ? Orientation.Vertical : Orientation.Horizontal;
            }

            return base.UpdateEdgeControl();
        }
    }
}