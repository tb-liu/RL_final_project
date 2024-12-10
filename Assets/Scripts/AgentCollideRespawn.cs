using UnityEngine;
public class AgentCollideRespawn : MonoBehaviour
{
    public GameObject respawnArea;
    private void Start()
    {
        MoveTargetToRandomPosition();
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Agent")) 
    //    {
    //        MoveTargetToRandomPosition();
    //    }
    //}

    public void MoveTargetToRandomPosition()
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
