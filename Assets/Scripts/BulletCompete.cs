using UnityEngine;
using System;
public class BulletCompete : MonoBehaviour
{
    public float force = 1000f;
    public float maxLifetime = 2f;     // Time after which the bullet is destroyed if it doesn't hit anything

    private Vector3 direction;
    private Rigidbody rb;
    private Action<float> onHit;    // Delegate to store the reward function
    private Action<float> onMiss;
    // Method to initialize the bullet's direction and reward function 
    private ShootingCompeteAgent hitAgent;
    public void Initialize(Vector3 initialDirection, Action<float> rewardFunction, Action<float> penalizeFunction)
    {
        rb = GetComponent<Rigidbody>();
        direction = initialDirection.normalized;
        // Store the reference to the reward function
        this.onHit = rewardFunction;
        this.onMiss = penalizeFunction;
        rb.AddForce(direction * force);
        Invoke(nameof(Miss), maxLifetime);
    }
    private void Miss()
    {
        // Notify the agent that the bullet missed
        onMiss!.Invoke(-0.25f);
        Destroy(gameObject);
    }


    void OnCollisionEnter(Collision collision)
    {
        // Log the collision for debugging
        Debug.Log("Bullet collided with " + collision.collider.name);
        if (collision.gameObject.CompareTag("Agent"))
        {
            hitAgent = collision.gameObject.GetComponent<ShootingCompeteAgent>();
            if (hitAgent) 
            {
                hitAgent.SetReward(-4f);
                hitAgent.EndEpisode();
            }
            onHit!.Invoke(4.0f);
        }
        else 
        {
            onMiss!.Invoke(-0.25f);
        }
        Destroy(gameObject);
    }
}
