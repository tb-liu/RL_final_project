using UnityEngine;
using System.Collections;
public class Shooting : MonoBehaviour
{
    public Transform firePoint;            // Starting point of the laser
    public float laserRange = 100f;        // Maximum range of the laser
    public float laserSpeed = 50f;         // Speed at which the laser extends
    public LayerMask targetLayer;          // Layer of objects the laser can hit
    public LineRenderer laserLine;         // LineRenderer component for the laser visual
    public float laserFadeDuration = 0.1f; // Duration for which the laser fades out after hitting the target

    private float nextFireTime = 0f;
    public float fireRate = 0.5f; // Time between shots
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            StartCoroutine(ShootLaser());
            nextFireTime = Time.time + fireRate;
        }
    }

    IEnumerator ShootLaser()
    {
        // Start from zero-length laser
        laserLine.enabled = true;
        laserLine.SetPosition(0, firePoint.position);
        laserLine.SetPosition(1, firePoint.position);

        // Fire the laser raycast to check if it hits anything
        RaycastHit hit;
        Vector3 targetPosition = firePoint.position + firePoint.forward * laserRange;

        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, laserRange, targetLayer))
        {
            targetPosition = hit.point;

            
        }

        // Extend the laser from the starting point to the target at a given speed
        float distanceToTarget = Vector3.Distance(firePoint.position, targetPosition);
        float currentDistance = 0f;

        while (currentDistance < distanceToTarget)
        {
            currentDistance += laserSpeed * Time.deltaTime;
            Vector3 newEndPosition = Vector3.Lerp(firePoint.position, targetPosition, currentDistance / distanceToTarget);
            laserLine.SetPosition(1, newEndPosition);
            yield return null;
        }

        // Set the final position to ensure it fully reaches the target
        laserLine.SetPosition(1, targetPosition);

        // Fade out the laser line after reaching the target
        yield return new WaitForSeconds(laserFadeDuration);
        laserLine.enabled = false;
    }
}
