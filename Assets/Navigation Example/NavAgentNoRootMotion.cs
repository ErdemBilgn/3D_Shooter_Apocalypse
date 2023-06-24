using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentNoRootMotion : MonoBehaviour
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
    Animator animator = null;
    float originalMaxSpeed = 0;

    void Start()
    {
        // Cache NavMeshAgent Reference.
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if(navAgent)
        {
            originalMaxSpeed = navAgent.speed;
        }
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

        int turnOnSpot;

        HasPath = navAgent.hasPath;
        PathPending = navAgent.pathPending;
        PathStale = navAgent.isPathStale;
        PathStatus = navAgent.pathStatus;

        Vector3 cross = Vector3.Cross(transform.forward, navAgent.desiredVelocity.normalized);
        float horizontal = (cross.y < 0) ? -cross.magnitude : cross.magnitude;
        horizontal = Mathf.Clamp(horizontal* 2.32f, -2.32f, 2.32f);

        if(navAgent.desiredVelocity.magnitude < 1f && Vector3.Angle(transform.forward, navAgent.desiredVelocity) > 10 || !HasPath)
        {
            navAgent.speed = 0.1f;
            turnOnSpot = (int)Mathf.Sign(horizontal);
        }
        else
        {
            navAgent.speed = originalMaxSpeed;
            turnOnSpot = 0;
        }
        animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", navAgent.desiredVelocity.magnitude, 0.1f, Time.deltaTime);
        animator.SetInteger("TurnOnSpot", turnOnSpot);

        /*if(navAgent.isOnOffMeshLink)
        {
            StartCoroutine(Jump(1));
            return;
        }*/

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
