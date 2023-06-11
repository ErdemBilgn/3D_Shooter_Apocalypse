using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

[CustomEditor(typeof(AIWaypointNetwork))]
public class AIWaypointNetworkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AIWaypointNetwork network = (AIWaypointNetwork)target;
        network.DisplayMode = (PathDisplayMode)EditorGUILayout.EnumPopup("Display Mode", network.DisplayMode);
        if (network.DisplayMode == PathDisplayMode.Paths)
        {
            network.UIStart = EditorGUILayout.IntSlider("Waypoints Start", network.UIStart, 0, network.WayPoints.Count - 1);
            network.UIEnd = EditorGUILayout.IntSlider("Waypoints End", network.UIEnd, 0, network.WayPoints.Count - 1);
        }
        DrawDefaultInspector();
    }


    void OnSceneGUI()
    {
        AIWaypointNetwork network = (AIWaypointNetwork)target;
        
        for(int i = 0; i < network.WayPoints.Count; i++) 
        {
            if (network.WayPoints[i] != null)
            {
                Handles.Label(network.WayPoints[i].position, "Waypoint " + i.ToString());
            }         
        }


        if(network.DisplayMode == PathDisplayMode.Connections)
        {
            Vector3[] linePoints = new Vector3[network.WayPoints.Count + 1];

            for (int i = 0; i <= network.WayPoints.Count; ++i)
            {
                int index = i != network.WayPoints.Count ? i : 0;
                if (network.WayPoints[index] != null)
                {
                    linePoints[i] = network.WayPoints[index].position;
                }
                else
                    linePoints[i] = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
            }
            Handles.color = Color.cyan;
            Handles.DrawPolyLine(linePoints);
        }
        else if (network.DisplayMode == PathDisplayMode.Paths) 
        {
            NavMeshPath path = new NavMeshPath();

            if (network.WayPoints[network.UIStart] != null && network.WayPoints[network.UIEnd] != null)
            {
                Vector3 from = network.WayPoints[network.UIStart].position;
                Vector3 to = network.WayPoints[network.UIEnd].position;

                NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path);

                Handles.color = Color.yellow;
                Handles.DrawPolyLine(path.corners);
            }
        }

    }

}
