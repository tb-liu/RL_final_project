using UnityEngine;
public class BulletHitRespawn : MonoBehaviour
{
    public GameObject respawnArea;
    private void Start()
    {
        MoveTargetToRandomPosition();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet")) 
        {
            MoveTargetToRandomPosition();
        }
    }

    void MoveTargetToRandomPosition()
    {
        BoxCollider area = respawnArea.GetComponent<BoxCollider>();
        Vector3 point = new Vector3(
            Random.Range(-area.size.x / 2, area.size.x / 2),
            0.25f,
            Random.Range(-area.size.z / 2, area.size.z / 2)
        ) + area.center + respawnArea.transform.position;

        transform.position = point;
    }

}
