using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject item;
    private Transform player;   

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; 
    }

    public void SpawnDroppedItem()
    {
        Vector2 playerPos = new Vector2(player.position.x + 1, player.position.y); //смещение появляющегося объекта
        Instantiate(item, playerPos, Quaternion.identity);//какой предмет, где, вращение объекта 
    }
}
