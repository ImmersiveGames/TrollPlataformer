using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaypointPlatform))]
public class WaypointPlatformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WaypointPlatform platform = (WaypointPlatform)target;

        if (GUILayout.Button("Adicionar Waypoint"))
        {
            GameObject wp = new GameObject("Waypoint_" + platform.waypoints.Count);

            // Posição: se houver um anterior, copia; senão, usa a posição da plataforma
            Vector3 spawnPos = platform.waypoints.Count > 0
                ? platform.waypoints[platform.waypoints.Count - 1].position
                : platform.transform.position;

            wp.transform.position = spawnPos;

            // Coloca como filho do holder, se existir
            Transform holder = platform.transform.Find("WaypointsHolder");
            wp.transform.parent = holder != null ? holder : platform.transform;

            // Adiciona na lista e marca como sujo
            platform.waypoints.Add(wp.transform);
            EditorUtility.SetDirty(platform);
            
            // Seleciona o novo waypoint
            Selection.activeGameObject = wp;
        }

        if (GUILayout.Button("Limpar Waypoints"))
        {
            platform.waypoints.Clear();
            EditorUtility.SetDirty(platform);
        }
    }
}