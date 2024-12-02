using UnityEngine;
using UnityEngine.AI;

public class PathDistanceDebugger : MonoBehaviour
{
    public Transform agent1; // Reference to the first agent
    public Transform agent2; // Reference to the second agent

    private NavMeshAgent navMeshAgent; // For path calculation
    private float pathDistance; // Store the calculated path distance

    void Start()
    {
        // Add or get a NavMeshAgent component (can be on any object)
        navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        navMeshAgent.updatePosition = false; // Prevent automatic movement
        navMeshAgent.updateRotation = false; // Prevent automatic rotation
        navMeshAgent.enabled = true;
    }

    void Update()
    {
        // Calculate the path distance between agent1 and agent2
        pathDistance = CalculatePathDistance(agent1.position, agent2.position);

        // Print debug info
        // Debug.Log($"Path distance between {agent1.name} and {agent2.name}: {pathDistance} units");
    }

    private float CalculatePathDistance(Vector3 startPosition, Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();

        // Calculate the path between the two positions
        if (navMeshAgent.CalculatePath(targetPosition, path))
        {
            float distance = 0f;
            // Sum up distances between path corners
            for (int i = 1; i < path.corners.Length; i++)
            {
                distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            return distance;
        }

        // Return a high value if no path exists
        return float.MaxValue;
    }

    void OnDrawGizmos()
    {
        if (agent1 == null || agent2 == null) return;

        NavMeshPath path = new NavMeshPath();

        // Calculate the path between the two agents
        if (NavMesh.CalculatePath(agent1.position, agent2.position, NavMesh.AllAreas, path))
        {
            Gizmos.color = Color.red;

            // Draw the path using Gizmos
            for (int i = 1; i < path.corners.Length; i++)
            {
                Gizmos.DrawLine(path.corners[i - 1], path.corners[i]);
            }
        }
    }

    void OnGUI()
    {
        // Display the path distance on the screen
        GUI.Label(new Rect(10, 10, 300, 20), $"Path Distance: {pathDistance:F2} units");
    }
}
