using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class AgentScript : Agent
{
    public float moveForce = 0.5f;
    public float rotationSpeed = 100f; // Speed at which the agent rotates
    public Vector3 startPos = new Vector3(0, 2, 0);

    private Rigidbody rBody;
    private void OnTriggerEnter(Collider other)
    {
        // Check if the agent has collided with the target
        if (other.gameObject.CompareTag("Target"))  // Ensure the target has the "Target" tag
        {
            SetReward(1.0f);  // Reward for reaching the target
            EndEpisode();  // End the episode
        }
    }
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
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the agent's position and the relative position to the target
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localRotation);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Get the continuous actions for movement and rotation
        float moveForward = actions.ContinuousActions[0];   // Forward/backward movement
        float moveSideways = actions.ContinuousActions[1];  // Left/right movement
        float rotate = actions.ContinuousActions[2];        // Rotation

        // Apply movement forces based on the input
        Vector3 force = transform.forward * moveForward * moveForce + transform.right * moveSideways * moveForce;
        rBody.AddForce(force, ForceMode.VelocityChange);

        // Apply rotation
        transform.Rotate(0, rotate * rotationSpeed * Time.deltaTime, 0);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;

        // Use arrow keys for testing: forward/backward and left/right for rotation
        continuousActions[0] = Input.GetAxis("Vertical");  // Forward/Backward movement
        continuousActions[1] = Input.GetAxis("Horizontal"); // Left/Right movement
        continuousActions[2] = Input.GetKey(KeyCode.Q) ? -1 : (Input.GetKey(KeyCode.E) ? 1 : 0); // Rotation with Q and E keys

    }

}
