using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class RayPerceptionVisualizer : MonoBehaviour
{
    public float fieldOfView = 90f; // Field of view in degrees
    public float range = 20f;      // Maximum range of the frustum
    public int resolution = 10;    // Number of lines for frustum edges
    private LineRenderer lineRenderer;

    void Start()
    {
        // Initialize the LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.loop = true; // Close the frustum
        lineRenderer.widthMultiplier = 0.2f; // Thickness of the lines
    }

    void Update()
    {
        DrawFrustumEdges();
    }

    private void DrawFrustumEdges()
    {
        Vector3[] points = new Vector3[resolution + 2]; // +2: First point and closing point

        float halfFOV = fieldOfView / 2f;
        float angleStep = fieldOfView / resolution;

        // Generate points in local space
        points[0] = transform.position; // Origin (agent's position)
        for (int i = 1; i <= resolution + 1; i++)
        {
            float angle = -halfFOV + (i - 1) * angleStep;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward; // Local forward direction
            points[i] = transform.position + transform.TransformDirection(direction * range);
        }

        // Update LineRenderer
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }
}
