// Health Script
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int Health = 100;
    public bool hasDied;

    public GameManagerScript gameManager;

    void Start()
    {
        Health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
    }

    void Update()
    {
        if (Health <= 0 && !hasDied)
        {
            hasDied = true;
            gameObject.SetActive(false);
            gameManager.gameOver();
            Debug.Log("You Died");
        }
    }
}

public class GameManagerScript
{
    public void gameOver()
    {
        Debug.Log("Game Over Triggered");
    }
}