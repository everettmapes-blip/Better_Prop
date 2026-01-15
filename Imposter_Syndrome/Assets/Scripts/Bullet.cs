using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnCollisionEnter(Collision collision)
    {   
        
        if (collision.gameObject.CompareTag("Target") || collision.gameObject.CompareTag("Wall") ||collision.gameObject.CompareTag("Roof") || collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
    }
}
