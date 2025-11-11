using System.Numerics;
using UnityEngine;

public class MinionScript : MonoBehaviour

{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform Playerposition;
    UnityEngine.Vector2 directiontoplayer;
    int EnemySpeed;
    void Start()
    {
        Playerposition = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Playerposition != null)
        {
            directiontoplayer = (Playerposition.position - transform.position).normalized;
            Debug.Log(directiontoplayer);
        }
        if (Playerposition.position - transform.position != UnityEngine.Vector3.zero) MoveToPlayer(EnemySpeed);

    }
    public void MoveToPlayer(float speed)
    {
        
    }
}
