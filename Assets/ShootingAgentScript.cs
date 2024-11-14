using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class ShootingAgent : Agent
{
    
    public float moveForce = 0.5f;
    public float rotationSpeed = 100f; // Speed at which the agent rotates
    public Vector3 startPos = new Vector3(0, 2, 0);
    public Transform firePoint;
    private Rigidbody rBody;

    public int MaxHit = 4; // hit four times then end the episode
    private int hitCount = 0, missCount = 0;
    public int MaxMiss = 10; // missed 10 times then end

    public GameObject bulletPrefab;      // Reference to the bullet prefab
    public float fireRate = 0.1f;        // Time between shots
    private float nextFireTime = 0f;     // Controls cooldown between shots
    //private void OnTriggerEnter(Collider other)
    //{
    //    // Check if the agent has collided with the target
    //    if (other.gameObject.CompareTag("Target"))  // Ensure the target has the "Target" tag
    //    {
    //        SetReward(1.0f);  // Reward for reaching the target
    //        EndEpisode();  // End the episode
    //    }
    //}
    public override void Initialize()
    {
        base.Initialize();
        rBody = GetComponent<Rigidbody>();
    }
    public override void OnEpisodeBegin()
    {
        // Reset agent's position and rotation at the start of each episode
        transform.localPosition = startPos;
        rBody.angularVelocity = Vector3.zero;
        rBody.linearVelocity = Vector3.zero;
        hitCount = 0; missCount = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // moved since the last observation
        //Vector3 moved = transform.localPosition - previousPosition;
        //sensor.AddObservation(moved);
        sensor.AddObservation(transform.localRotation);
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

    }

    private void Shoot()
    {
        // Instantiate the bullet at the agent's position and rotation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation);

        // Get the bullet script component and initialize it with the forward direction
        Bullet bulletScript = bullet.GetComponent<Bullet>();
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
    public void RewardOnHit()
    {
        ++hitCount;
        AddReward(1.0f); // Give a reward for hitting the target
        if (hitCount >= MaxHit) EndEpisode();
    }

    public void PenalizeOnMiss() 
    {
        ++missCount;
        AddReward(-0.5f);
        if (missCount >= MaxMiss) EndEpisode();
    }
}
