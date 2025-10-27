// Damage Script
using UnityEngine;

public class Damage : MonoBehaviour
{
    public int damage = 10;
    public PlayerHealth Health;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Health.TakeDamage(damage);
            Debug.Log("Player took " + damage + " damage.");
        }
    }


    void Update()
    {

    }
}