using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class NavigationScript : MonoBehaviour
{
    // Components
    public NavMeshAgent agent;
    public TextMeshProUGUI linearVelocityText;
    public TextMeshProUGUI angularSpeedText;
    public TextMeshProUGUI directionToNextWaypointText;

    // Navigation variables
    private List<Transform> pointsOfInterest = new List<Transform>(); // Stores dynamically found POIs
    private int currentPOIIndex = 0;
    private bool waiting = false;
    private float waitTimer = 5.0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        DiscoverPointsOfInterest();
        if (pointsOfInterest.Count > 0)
            MoveToNextPOI();
    }

    void Update()
    {
        CheckMovementStatus();
        UpdateNavigationInfo();
        LogVelocityAndAngle();
    }

    private void DiscoverPointsOfInterest()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("NavTarget");
        foreach (GameObject target in targets)
        {
            pointsOfInterest.Add(target.transform);
        }
    }

    private void CheckMovementStatus()
    {
        if (!waiting)
        {
            if (agent.velocity.magnitude < 0.1f && agent.remainingDistance < 0.5f)
            {
                StartWaiting();
            }
        }
        else
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                waiting = false;
                waitTimer = 5.0f;
                MoveToNextPOI();
            }
        }
    }

    private void StartWaiting()
    {
        waiting = true;
        agent.isStopped = true;
    }

    private void MoveToNextPOI()
    {
        if (pointsOfInterest.Count == 0) return;
        currentPOIIndex = (currentPOIIndex + 1) % pointsOfInterest.Count;
        agent.SetDestination(pointsOfInterest[currentPOIIndex].position);
        agent.isStopped = false;
    }

    private void UpdateNavigationInfo()
    {
        if (linearVelocityText)
            linearVelocityText.text = "Linear Velocity: " + agent.velocity.ToString("F2");
        
        if (angularSpeedText)
            angularSpeedText.text = "Angular Speed: " + agent.angularSpeed.ToString("F2");
        
        if (directionToNextWaypointText && agent.hasPath)
        {
            Vector3 direction = (agent.steeringTarget - transform.position).normalized;
            directionToNextWaypointText.text = "Direction to Next Waypoint: " + direction.ToString("F2");
        }
    }

    private void LogVelocityAndAngle()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        Vector3 projectedAxis = new Vector3(localVelocity.x, 0, localVelocity.z);
        float diffAngle = Vector3.SignedAngle(Vector3.forward, projectedAxis, Vector3.up);

        Debug.Log($"Velocity: {localVelocity}, Angular Speed: {agent.angularSpeed}, " +
                  $"Direction to Next Waypoint: {(agent.steeringTarget - transform.position).normalized}, " +
                  $"Yaw Angle: {diffAngle} degrees");
    }
}
