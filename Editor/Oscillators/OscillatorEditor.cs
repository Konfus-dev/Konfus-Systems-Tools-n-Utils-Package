using UnityEditor;
using UnityEngine;

namespace Konfus.Editor.Oscillators
{
    /// <summary>
    ///     Custom Unity inspector for Oscillator.cs.
    /// </summary>
    [CustomEditor(typeof(Oscillator), true)]
    public class OscillatorEditor : UnityEditor.Editor
    {
        /// <summary>
        ///     Draw the default inspector, with a clamped Vector3 on the forceScale.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var oscillator = (Oscillator) target;
            for (int i = 0; i < 3; i++) oscillator.forceScale[i] = (int) Mathf.Clamp01(oscillator.forceScale[i]);
        }
    }
}