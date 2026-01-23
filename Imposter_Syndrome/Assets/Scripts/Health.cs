using UnityEngine;

public class Health : MonoBehaviour
{
    public int playerHealth = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameObject.CompareTag("Hider"))
        {
            playerHealth = 100;
        }
        if (gameObject.CompareTag("Seeker"))
        {
            playerHealth = 1000;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
