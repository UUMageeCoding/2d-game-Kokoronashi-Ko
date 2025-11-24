
using UnityEngine;
using TMPro; // For TextMeshPro

public class RingCount: MonoBehaviour
{
    public static int totalCoins = 0;

    [Header("UI Settings")]
    [SerializeField] private TMP_Text ringText; // Assign your TextMeshPro UI element in Inspector

    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Initialize UI if assigned
        if (ringText != null)
        {
            ringText.text = "Rings: " + totalCoins;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            totalCoins++;
            Debug.Log("Rings Collected: " + totalCoins);

            // Update UI if assigned
            if (ringText != null)
            {
                ringText.text = "Rings: " + totalCoins;
            }

            Destroy(gameObject);
        }
    }
}
