using UnityEngine;

public class HPTest : MonoBehaviour
{
    public PlayerHealth playerHealth;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerHealth.TakeDamage(1);
        }
    }
}