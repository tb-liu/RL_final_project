using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.AI;
public class ShootingCompeteAgent : Agent
{
    
    public float moveForce = 0.5f;
    public float rotationSpeed = 100f; // Speed at which the agent rotates
    public Vector3 startPos = new Vector3(0, 2, 0);
    public Transform firePoint;
    private Rigidbody rBody;

    public int MaxHit = 4; // hit four times then end the episode
    public int MaxMiss = 10; // missed 10 times then end

    public GameObject bulletPrefab;      // Reference to the bullet prefab
    public GameObject respawnArea;       // Respawn area
    public float fireRate = 0.1f;        // Time between shots
    private float nextFireTime = 0f;     // Controls cooldown between shots

    public Transform opponent;
    public float neutralRange = 20f;
    private float previousDistanceToOpponent = float.MaxValue;
    private float episodeTime = 0f; // Timer for the episode
    public float maxEpisodeTime = 60f; // Max allowed time for an episode
    public override void Initialize()
    {
        base.Initialize();
        rBody = GetComponent<Rigidbody>();
    }
    public override void OnEpisodeBegin()
    {
        // Reset agent's position and rotation at the start of each episode
        MoveTargetToRandomPosition();
        rBody.angularVelocity = Vector3.zero;
        rBody.linearVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localRotation);
        //sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Get the continuous actions for movement and rotation
        float moveForward = actions.ContinuousActions[0];   // Forward/backward movement
        float moveSideways = actions.ContinuousActions[1];  // Left/right movement
        float rotate = actions.ContinuousActions[2];        // Rotation
        bool shoot = actions.DiscreteActions[0] == 1;

        // Apply movement forces based on the input
        Vector3 force = transform.forward * moveForward * moveForce + transform.right * moveSideways * moveForce;
        rBody.AddForce(force, ForceMode.VelocityChange);

        // Apply rotation
        transform.Rotate(0, rotate * rotationSpeed * Time.deltaTime, 0);

        // Handle shooting
        if (shoot && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }


        // Calculate distance to the opponent
        float distanceToOpponent = CalculatePathDistance(transform.position, opponent.position);

        // Check if agent is within the neutral zone
        if (distanceToOpponent <= neutralRange)
        {
            // Neutral range: Encourage staying in this range with a small reward
            AddReward(0.01f);
        }
        else
        {
            // Outside neutral range: Encourage getting closer
            if (distanceToOpponent < previousDistanceToOpponent)
            {
                AddReward(0.02f); // Larger reward for moving closer to the neutral zone
            }
            else
            {
                AddReward(-0.01f); // Small penalty for moving further away
            }
        }

        // Update previous distance
        previousDistanceToOpponent = distanceToOpponent;

        // Check for episode timeout (existing logic)
        episodeTime += Time.deltaTime;
        if (episodeTime >= maxEpisodeTime)
        {
            SetReward(-5.0f); // Penalize timeout
            EndEpisode();
        }
    }

    private void Shoot()
    {
        // Instantiate the bullet at the agent's position and rotation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation);

        // Get the bullet script component and initialize it with the forward direction
        BulletCompete bulletScript = bullet.GetComponent<BulletCompete>();
        bulletScript.Initialize(transform.forward, RewardOnHit, PenalizeOnMiss);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        // Use arrow keys for testing: forward/backward and left/right for rotation
        continuousActions[0] = Input.GetAxis("Vertical");  // Forward/Backward movement
        continuousActions[1] = Input.GetAxis("Horizontal"); // Left/Right movement
        continuousActions[2] = Input.GetKey(KeyCode.Q) ? -1 : (Input.GetKey(KeyCode.E) ? 1 : 0); // Rotation with Q and E keys


        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.F) ? 1 : 0; // Shoot
    }
    public void RewardOnHit(float reward)
    {
        AddReward(reward);
        EndEpisode();
    }

    public void PenalizeOnMiss(float reward) 
    {
        AddReward(reward);
    }

    void MoveTargetToRandomPosition()
    {
        BoxCollider area = respawnArea.GetComponent<BoxCollider>();
        Vector3 point = new Vector3(
            Random.Range(-area.size.x / 2, area.size.x / 2),
            0.5f,
            Random.Range(-area.size.z / 2, area.size.z / 2)
        ) + area.center + respawnArea.transform.position;

        transform.position = point;
    }

    public float CalculatePathDistance(Vector3 startPosition, Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();

        // Calculate the path between two points on the NavMesh
        if (NavMesh.CalculatePath(startPosition, targetPosition, NavMesh.AllAreas, path))
        {
            // Sum up the distances between path corners
            float distance = 0f;
            for (int i = 1; i < path.corners.Length; i++)
            {
                distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
            return distance;
        }

        // Return a large value if no path exists
        return float.MaxValue;
    }
}
