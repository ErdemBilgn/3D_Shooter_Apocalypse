using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshExample : MonoBehaviour
{
    // Inspector assigned variable.
    public AIWaypointNetwork WaypointNetwork = null;
    public int CurrentIndex = 0;
    public bool HasPath = false;
    public bool PathPending = false;
    public bool PathStale = false;
    public NavMeshPathStatus PathStatus = NavMeshPathStatus.PathInvalid;

    // private Members.
    NavMeshAgent navAgent = null;


    void Start()
    {
        // Cache NavMeshAgent Reference.
        navAgent = GetComponent<NavMeshAgent>();
        //navAgent.updatePosition = false;
        //navAgent.updateRotation = false;

        if (WaypointNetwork == null) return;

        SetNextDestination(false);
    }

    void SetNextDestination(bool increment)
    {
        if (!WaypointNetwork) return;

        int incStep = increment ? 1 : 0;

        int nextWaypoint = (CurrentIndex + incStep >= WaypointNetwork.WayPoints.Count) ? 0 : CurrentIndex + incStep;
        Transform nextWaypointTransform = WaypointNetwork.WayPoints[nextWaypoint];

        if (nextWaypointTransform != null)
        {
            CurrentIndex = nextWaypoint;
            navAgent.SetDestination(nextWaypointTransform.position);
            return;
        }

        CurrentIndex++;
        
    }

    void Update()
    {
        HasPath = navAgent.hasPath;
        PathPending = navAgent.pathPending;
        PathStale = navAgent.isPathStale;
        PathStatus = navAgent.pathStatus;


        if((!HasPath && !PathPending) || PathStatus == NavMeshPathStatus.PathInvalid /*|| PathStatus == NavMeshPathStatus.PathPartial*/)
        {
            SetNextDestination(true);
        }
        else
        {
            if(navAgent.isPathStale)
            {
                SetNextDestination(false);
            }
        }
    }
}
