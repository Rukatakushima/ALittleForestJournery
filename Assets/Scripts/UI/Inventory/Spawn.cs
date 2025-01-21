using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject item;
    [SerializeField] private Transform player;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public void SpawnDroppedItem()
    {
        Vector2 playerPos = new Vector2(player.position.x + 1, player.position.y); //смещение появляющегося объекта
        Instantiate(item, playerPos, Quaternion.identity);//какой предмет, где, вращение объекта 
    }
}
