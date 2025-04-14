using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform[] waypoints;

    [Header("Detection Settings")]
    public Transform player;
    public GameEnding gameEnding;
    public AudioSource rearDetectAudio;
    public float rearDetectionDistance = 2.0f;
    [Range(-1f, 0f)]
    public float rearDotThreshold = -0.7f;

    [Header("Turning Settings")]
    public float turnSpeed = 180f;

    private enum GhostState { Patrolling, Turning, Caught }
    private GhostState currentState = GhostState.Patrolling;

    int m_CurrentWaypointIndex;
    private Vector3 directionToPlayerOnCatch;

    void Start()
    {
        if (navMeshAgent == null) Debug.LogError($"NavMeshAgent not assigned on {gameObject.name}");
        if (player == null) Debug.LogError($"Player Transform not assigned in WaypointPatrol on {gameObject.name}");
        if (gameEnding == null) Debug.LogError($"GameEnding reference not assigned in WaypointPatrol on {gameObject.name}");
        if (rearDetectAudio == null) Debug.LogWarning($"RearDetectAudio Source not assigned in WaypointPatrol on {gameObject.name}. Sound won't play.");
        if (waypoints == null || waypoints.Length == 0) Debug.LogWarning($"No waypoints assigned in WaypointPatrol on {gameObject.name}. Ghost won't patrol.");

        // --- Initialization ---
        currentState = GhostState.Patrolling;
        if (waypoints != null && waypoints.Length > 0 && navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(waypoints[0].position);
            navMeshAgent.isStopped = false;
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case GhostState.Patrolling:
                HandlePatrollingState();
                CheckForRearDetection();
                break;

            case GhostState.Turning:
                HandleTurningState();
                break;

            case GhostState.Caught:
                break;
        }
    }

    void HandlePatrollingState()
    {
        if (navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isOnNavMesh && waypoints.Length > 0)
        {
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
                navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
            }
        }
    }

    void CheckForRearDetection()
    {
        if (player == null || gameEnding == null) return;

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceSqr = directionToPlayer.sqrMagnitude;

        // Check if player is within detection distance
        if (distanceSqr < rearDetectionDistance * rearDetectionDistance)
        {
            // Calculate dot product relative to ghost's forward direction
            // Normalize the direction vector just for the dot product
            float dotProduct = Vector3.Dot(transform.forward, directionToPlayer.normalized);

            if (dotProduct < rearDotThreshold)
            {
                // --- Player Detected From Behind ---
                TransitionToTurningState(directionToPlayer.normalized);
            }
        }
    }

    void TransitionToTurningState(Vector3 direction)
    {
        currentState = GhostState.Turning;
        directionToPlayerOnCatch = direction;

        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
        }

        if (rearDetectAudio != null)
        {
            rearDetectAudio.Play();
        }
    }

    void HandleTurningState()
    {
        if (gameEnding == null) return;
        if (directionToPlayerOnCatch == Vector3.zero) return;

        // Calculate the target rotation to look directly at the stored direction
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayerOnCatch);

        // Smoothly rotate the ghost towards the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        // Check if the ghost is now facing the direction where the player was detected
        float angleDifference = Vector3.Angle(transform.forward, directionToPlayerOnCatch);

        // If facing angle is small enough, consider the turn complete and catch the player
        if (angleDifference < 5.0f)
        {
            currentState = GhostState.Caught;
            gameEnding.CaughtPlayer();
        }
    }
}