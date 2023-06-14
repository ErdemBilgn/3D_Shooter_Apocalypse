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
    public AnimationCurve JumpCurve = new AnimationCurve();

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


        if(navAgent.isOnOffMeshLink)
        {
            StartCoroutine(Jump(1));
            return;
        }

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

    IEnumerator Jump(float duration)
    {
        OffMeshLinkData data = navAgent.currentOffMeshLinkData;
        Vector3 startPos = navAgent.transform.position;
        Vector3 endPos = data.endPos + (navAgent.baseOffset * Vector3.up);

        float time = 0;
        while (time <= duration)
        {
            float t = time / duration;
            navAgent.transform.position = Vector3.Lerp(startPos, endPos, t) + (JumpCurve.Evaluate(t) * Vector3.up);
            time += Time.deltaTime;
            yield return null;
        }

        navAgent.CompleteOffMeshLink();
    }
}
