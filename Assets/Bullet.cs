using UnityEngine;
using System;
public class Bullet : MonoBehaviour
{
    public float force = 1000f;
    public float maxLifetime = 2f;     // Time after which the bullet is destroyed if it doesn't hit anything

    private Vector3 direction;
    private Rigidbody rb;
    private Action onHit;    // Delegate to store the reward function
    private Action onMiss;
    // Method to initialize the bullet's direction and reward function 
    public void Initialize(Vector3 initialDirection, Action rewardFunction, Action penalizeFunction)
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
        onMiss?.Invoke();
        Destroy(gameObject);
    }


    void OnCollisionEnter(Collision collision)
    {
        // Log the collision for debugging
        Debug.Log("Bullet collided with " + collision.collider.name);
        if (collision.gameObject.CompareTag("Target"))
        {
            onHit!.Invoke();
        }
        else 
        {
            onMiss!.Invoke();
        }
        Destroy(gameObject);
    }
}
