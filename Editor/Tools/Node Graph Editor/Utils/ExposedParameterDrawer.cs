using Konfus.Systems.Node_Graph;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Konfus.Tools.NodeGraphEditor
{
    [CustomPropertyDrawer(typeof(ExposedParameter))]
    public class ExposedParameterDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Create property container element.
            var container = new VisualElement();

            container.Add(CreateValProperty(property));

            return container;
        }

        protected VisualElement CreateValProperty(SerializedProperty property, string displayName = null)
        {
            if (displayName == null)
                displayName = GetNameProperty(property).stringValue;

            var p = new PropertyField(GetValProperty(property), displayName);

            p.RegisterValueChangeCallback(e => { ApplyModifiedProperties(property); });

            return p;
        }

        protected SerializedProperty GetSettingsProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative(nameof(ExposedParameter.settings));
        }

        protected SerializedProperty GetValProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative("val");
        }

        protected SerializedProperty GetNameProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative(nameof(ExposedParameter.name));
        }

        protected void ApplyModifiedProperties(SerializedProperty property)
        {
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
    }

    [CustomPropertyDrawer(typeof(FloatParameter))]
    public class FloatParameterDrawer : ExposedParameterDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            SerializedProperty val = GetValProperty(property);
            SerializedProperty name = GetNameProperty(property);

            SerializedProperty settings = GetSettingsProperty(property);
            SerializedProperty mode = settings.FindPropertyRelative(nameof(FloatParameter.FloatSettings.mode));
            SerializedProperty min = settings.FindPropertyRelative(nameof(FloatParameter.FloatSettings.min));
            SerializedProperty max = settings.FindPropertyRelative(nameof(FloatParameter.FloatSettings.max));
            container.Add(new IMGUIContainer(() =>
            {
                float newValue;
                EditorGUIUtility.labelWidth = 150;
                if ((FloatParameter.FloatMode) mode.intValue == FloatParameter.FloatMode.Slider)
                {
                    newValue = EditorGUILayout.Slider(name.stringValue, val.floatValue, min.floatValue, max.floatValue);
                    newValue = Mathf.Clamp(newValue, min.floatValue, max.floatValue);
                }
                else
                {
                    newValue = EditorGUILayout.FloatField(name.stringValue, val.floatValue);
                }

                if (newValue != val.floatValue)
                {
                    val.floatValue = newValue;
                    ApplyModifiedProperties(property);
                }

                EditorGUIUtility.labelWidth = 0;
            }));

            return container;
        }
    }

    [CustomPropertyDrawer(typeof(IntParameter))]
    public class IntParameterDrawer : ExposedParameterDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            SerializedProperty val = GetValProperty(property);
            SerializedProperty name = GetNameProperty(property);

            SerializedProperty settings = GetSettingsProperty(property);
            SerializedProperty mode = settings.FindPropertyRelative(nameof(IntParameter.IntSettings.mode));
            SerializedProperty min = settings.FindPropertyRelative(nameof(IntParameter.IntSettings.min));
            SerializedProperty max = settings.FindPropertyRelative(nameof(IntParameter.IntSettings.max));
            container.Add(new IMGUIContainer(() =>
            {
                int newValue;
                EditorGUIUtility.labelWidth = 150;
                if ((IntParameter.IntMode) mode.intValue == IntParameter.IntMode.Slider)
                {
                    newValue = EditorGUILayout.IntSlider(name.stringValue, val.intValue, min.intValue, max.intValue);
                    newValue = Mathf.Clamp(newValue, min.intValue, max.intValue);
                }
                else
                {
                    newValue = EditorGUILayout.IntField(name.stringValue, val.intValue);
                }

                if (newValue != val.intValue)
                {
                    val.intValue = newValue;
                    ApplyModifiedProperties(property);
                }

                EditorGUIUtility.labelWidth = 0;
            }));

            return container;
        }
    }

    [CustomPropertyDrawer(typeof(Vector2Parameter))]
    public class Vector2ParameterDrawer : ExposedParameterDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            SerializedProperty val = GetValProperty(property);
            SerializedProperty name = GetNameProperty(property);

            SerializedProperty settings = GetSettingsProperty(property);
            SerializedProperty mode = settings.FindPropertyRelative(nameof(Vector2Parameter.Vector2Settings.mode));
            SerializedProperty min = settings.FindPropertyRelative(nameof(Vector2Parameter.Vector2Settings.min));
            SerializedProperty max = settings.FindPropertyRelative(nameof(Vector2Parameter.Vector2Settings.max));
            container.Add(new IMGUIContainer(() =>
            {
                EditorGUIUtility.labelWidth = 150;
                EditorGUI.BeginChangeCheck();
                if ((Vector2Parameter.Vector2Mode) mode.intValue == Vector2Parameter.Vector2Mode.MinMaxSlider)
                {
                    float x = val.vector2Value.x;
                    float y = val.vector2Value.y;
                    EditorGUILayout.MinMaxSlider(name.stringValue, ref x, ref y, min.floatValue, max.floatValue);
                    val.vector2Value = new Vector2(x, y);
                }
                else
                {
                    val.vector2Value = EditorGUILayout.Vector2Field(name.stringValue, val.vector2Value);
                }

                if (EditorGUI.EndChangeCheck())
                    ApplyModifiedProperties(property);
                EditorGUIUtility.labelWidth = 0;
            }));

            return container;
        }
    }

    [CustomPropertyDrawer(typeof(GradientParameter))]
    public class GradientParameterDrawer : ExposedParameterDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty name = GetNameProperty(property);
            SerializedProperty settings = GetSettingsProperty(property);
            var mode = (GradientParameter.GradientColorMode) settings
                .FindPropertyRelative(nameof(GradientParameter.GradientSettings.mode)).intValue;
            if (mode == GradientParameter.GradientColorMode.HDR)
                return new PropertyField(property.FindPropertyRelative("hdrVal"), name.stringValue);
            return new PropertyField(property.FindPropertyRelative("val"), name.stringValue);
        }
    }

    [CustomPropertyDrawer(typeof(ColorParameter))]
    public class ColorParameterDrawer : ExposedParameterDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty name = GetNameProperty(property);
            SerializedProperty settings = GetSettingsProperty(property);
            SerializedProperty val = GetValProperty(property);
            var mode = (ColorParameter.ColorMode) settings
                .FindPropertyRelative(nameof(ColorParameter.ColorSettings.mode)).intValue;

            var colorField = new ColorField(name.stringValue)
                {value = val.colorValue, hdr = mode == ColorParameter.ColorMode.HDR};
            colorField.RegisterValueChangedCallback(e =>
            {
                val.colorValue = e.newValue;
                ApplyModifiedProperties(property);
            });
            return colorField;
        }
    }

    [CustomPropertyDrawer(typeof(ExposedParameter.Settings))]
    public class ExposedParameterSettingsDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return CreateHideInInspectorField(property);
        }

        protected VisualElement CreateHideInInspectorField(SerializedProperty settingsProperty)
        {
            SerializedProperty isHidden =
                settingsProperty.FindPropertyRelative(nameof(ExposedParameter.Settings.isHidden));
            Graph graph = GetGraph(settingsProperty);
            ExposedParameter param = GetParameter(settingsProperty);
            var p = new PropertyField(isHidden, "Hide in Inspector");

            p.RegisterValueChangeCallback(e =>
            {
                settingsProperty.serializedObject.ApplyModifiedProperties();
                graph.NotifyExposedParameterChanged(param);
            });

            return p;
        }

        protected static Graph GetGraph(SerializedProperty property)
        {
            return property.serializedObject.FindProperty("graph").objectReferenceValue as Graph;
        }

        protected static ExposedParameter GetParameter(SerializedProperty settingsProperty)
        {
            string guid = settingsProperty.FindPropertyRelative(nameof(ExposedParameter.Settings.guid)).stringValue;
            return GetGraph(settingsProperty).GetExposedParameterFromGUID(guid);
        }

        protected static PropertyField CreateSettingsField(SerializedProperty settingsProperty, string fieldName,
            string displayName = null)
        {
            SerializedProperty prop = settingsProperty.FindPropertyRelative(fieldName);
            ExposedParameter param = GetParameter(settingsProperty);
            Graph graph = GetGraph(settingsProperty);

            if (displayName == null)
                displayName = ObjectNames.NicifyVariableName(fieldName);

            var p = new PropertyField(prop, displayName);
            p.Bind(settingsProperty.serializedObject);
            p.RegisterValueChangeCallback(e =>
            {
                settingsProperty.serializedObject.ApplyModifiedProperties();
                graph.NotifyExposedParameterChanged(param);
            });

            return p;
        }
    }

    [CustomPropertyDrawer(typeof(ColorParameter.ColorSettings))]
    public class ExposedColorSettingsDrawer : ExposedParameterSettingsDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty settingsProperty)
        {
            var settings = new VisualElement();

            settings.Add(CreateHideInInspectorField(settingsProperty));
            settings.Add(CreateSettingsField(settingsProperty, nameof(ColorParameter.ColorSettings.mode), "Mode"));

            return settings;
        }
    }

    [CustomPropertyDrawer(typeof(FloatParameter.FloatSettings))]
    public class FloatSettingsDrawer : ExposedParameterSettingsDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty settingsProperty)
        {
            var settings = new VisualElement();
            settings.Bind(settingsProperty.serializedObject);

            settings.Add(CreateHideInInspectorField(settingsProperty));
            PropertyField mode =
                CreateSettingsField(settingsProperty, nameof(FloatParameter.FloatSettings.mode), "Mode");
            PropertyField min = CreateSettingsField(settingsProperty, nameof(FloatParameter.FloatSettings.min), "Min");
            PropertyField max = CreateSettingsField(settingsProperty, nameof(FloatParameter.FloatSettings.max), "Max");

            mode.RegisterValueChangeCallback(e => UpdateVisibility(e.changedProperty));
            UpdateVisibility(settingsProperty.FindPropertyRelative(nameof(FloatParameter.FloatSettings.mode)));

            void UpdateVisibility(SerializedProperty property)
            {
                if (property == null)
                    return;
                var newValue = (FloatParameter.FloatMode) property.intValue;

                if (newValue == FloatParameter.FloatMode.Slider)
                    min.style.display = max.style.display = DisplayStyle.Flex;
                else
                    min.style.display = max.style.display = DisplayStyle.None;
            }

            settings.Add(mode);
            settings.Add(min);
            settings.Add(max);

            return settings;
        }
    }

    [CustomPropertyDrawer(typeof(IntParameter.IntSettings))]
    public class IntSettingsDrawer : ExposedParameterSettingsDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty settingsProperty)
        {
            var settings = new VisualElement();
            settings.Bind(settingsProperty.serializedObject);

            settings.Add(CreateHideInInspectorField(settingsProperty));
            PropertyField mode = CreateSettingsField(settingsProperty, nameof(IntParameter.IntSettings.mode), "Mode");
            PropertyField min = CreateSettingsField(settingsProperty, nameof(IntParameter.IntSettings.min), "Min");
            PropertyField max = CreateSettingsField(settingsProperty, nameof(IntParameter.IntSettings.max), "Max");

            mode.RegisterValueChangeCallback(e => UpdateVisibility(e.changedProperty));
            UpdateVisibility(settingsProperty.FindPropertyRelative(nameof(IntParameter.IntSettings.mode)));

            void UpdateVisibility(SerializedProperty property)
            {
                if (property == null)
                    return;
                var newValue = (IntParameter.IntMode) property.intValue;

                if (newValue == IntParameter.IntMode.Slider)
                    min.style.display = max.style.display = DisplayStyle.Flex;
                else
                    min.style.display = max.style.display = DisplayStyle.None;
            }

            settings.Add(mode);
            settings.Add(min);
            settings.Add(max);

            return settings;
        }
    }

    [CustomPropertyDrawer(typeof(Vector2Parameter.Vector2Settings))]
    public class Vector2SettingsDrawer : ExposedParameterSettingsDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty settingsProperty)
        {
            var settings = new VisualElement();

            settings.Add(CreateHideInInspectorField(settingsProperty));
            settings.Add(CreateSettingsField(settingsProperty, nameof(Vector2Parameter.Vector2Settings.mode), "Mode"));
            settings.Add(CreateSettingsField(settingsProperty, nameof(Vector2Parameter.Vector2Settings.min), "Min"));
            settings.Add(CreateSettingsField(settingsProperty, nameof(Vector2Parameter.Vector2Settings.max), "Max"));

            return settings;
        }
    }

    [CustomPropertyDrawer(typeof(GradientParameter.GradientSettings))]
    public class GradientSettingsDrawer : ExposedParameterSettingsDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty settingsProperty)
        {
            var settings = new VisualElement();

            settings.Add(CreateHideInInspectorField(settingsProperty));
            settings.Add(CreateSettingsField(settingsProperty, nameof(GradientParameter.GradientSettings.mode),
                "Mode"));

            return settings;
        }
    }
}