using System;
using Konfus.Systems.Node_Graph;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Konfus.Tools.NodeGraphEditor
{
    public abstract class PinnedElementView : GraphElement
    {
        protected PinnedElement pinnedElement;
        protected VisualElement root;
        protected VisualElement content;
        protected VisualElement header;

        protected event Action onResized;

        private readonly VisualElement main;
        private readonly Label titleLabel;
        private bool _scrollable;
        private readonly ScrollView scrollView;

        private static readonly string pinnedElementStyle = "GraphProcessorStyles/PinnedElementView";
        private static readonly string pinnedElementTree = "GraphProcessorElements/PinnedElement";

        public override string title
        {
            get => titleLabel.text;
            set => titleLabel.text = value;
        }

        protected bool scrollable
        {
            get => _scrollable;
            set
            {
                if (_scrollable == value)
                    return;

                _scrollable = value;

                style.position = Position.Absolute;
                if (_scrollable)
                {
                    content.RemoveFromHierarchy();
                    root.Add(scrollView);
                    scrollView.Add(content);
                    AddToClassList("scrollable");
                }
                else
                {
                    scrollView.RemoveFromHierarchy();
                    content.RemoveFromHierarchy();
                    root.Add(content);
                    RemoveFromClassList("scrollable");
                }
            }
        }

        public PinnedElementView()
        {
            var tpl = Resources.Load<VisualTreeAsset>(pinnedElementTree);
            styleSheets.Add(Resources.Load<StyleSheet>(pinnedElementStyle));

            main = tpl.CloneTree();
            main.AddToClassList("mainContainer");
            scrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);

            root = main.Q("content");

            header = main.Q("header");

            titleLabel = main.Q<Label>("titleLabel");
            content = main.Q<VisualElement>("contentContainer");

            hierarchy.Add(main);

            capabilities |= Capabilities.Movable | Capabilities.Resizable;
            style.overflow = Overflow.Hidden;

            ClearClassList();
            AddToClassList("pinnedElement");

            this.AddManipulator(new Dragger {clampToParentEdges = true});

            scrollable = false;

            hierarchy.Add(new Resizer(() => onResized?.Invoke()));

            RegisterCallback<DragUpdatedEvent>(e => { e.StopPropagation(); });

            title = "PinnedElementView";
        }

        public void InitializeGraphView(PinnedElement pinnedElement, GraphView graphView)
        {
            this.pinnedElement = pinnedElement;
            SetPosition(pinnedElement.position);

            onResized += () => { pinnedElement.position.size = layout.size; };

            RegisterCallback<MouseUpEvent>(e => { pinnedElement.position.position = layout.position; });

            Initialize(graphView);
        }

        public void ResetPosition()
        {
            pinnedElement.position = new Rect(Vector2.zero, PinnedElement.defaultSize);
            SetPosition(pinnedElement.position);
        }

        protected abstract void Initialize(GraphView graphView);

        ~PinnedElementView()
        {
            Destroy();
        }

        protected virtual void Destroy()
        {
        }
    }
}