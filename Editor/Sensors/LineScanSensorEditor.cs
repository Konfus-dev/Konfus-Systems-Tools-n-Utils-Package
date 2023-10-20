using System.Linq;
using Konfus.Systems.Sensor_Toolkit;
using UnityEditor;
using UnityEngine;

namespace Konfus.Editor.Sensors
{
    [CustomEditor(typeof(LineScanSensor))]
    public class LineScanSensorEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.Active)]
        private static void DrawGizmos(LineScanSensor sensor, GizmoType gizmoType)
        {
            sensor.Scan();
            DrawSensor(sensor);
        }

        private static void DrawSensor(LineScanSensor sensor)
        {
            Gizmos.color = SensorColors.NoHitColor;
            if (sensor.isTriggered) Gizmos.color = SensorColors.HitColor;

            // transform the gizmo
            Gizmos.matrix *= Matrix4x4.TRS(sensor.transform.position, sensor.transform.rotation, Vector3.one);

            float length = sensor.sensorLength;

            if (sensor.isTriggered)
                length = Vector3.Distance(sensor.transform.position, sensor.hits.First().point);

            Gizmos.DrawLine(Vector3.zero, Vector3.forward * length);

            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.02f, 0.02f, 0.02f));
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.forward * length, new Vector3(0.02f, 0.02f, 0.02f));
        }
    }
}
