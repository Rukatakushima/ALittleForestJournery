using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    private Inventory inventory;
    public int i;
    public GameObject player;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if(transform.childCount <= 0) //childCount - объекты внутри нашего слота
        {
            inventory.isFull[i] = false;//если их нет, то слот пустой
        }

        if (inventory.isFull[3] == true) 
        {
            player.GetComponent<PlayerController>().speed = 6;
            player.GetComponent<PlayerController>().jumpForce = 6;
        }
        else 
        {
            player.GetComponent<PlayerController>().speed = 3;
            player.GetComponent<PlayerController>().jumpForce = 4;
        }
    }

    public void DropItem()
    {
        foreach(Transform child in transform)
        {
            child.GetComponent<Spawn>().SpawnDroppedItem();//возват объекта на локацию
            GameObject.Destroy(child.gameObject);
        }
    }

    public void PlaceInNextSlot()
    {
        if(inventory.isFull[i] == true)
        {
            i++;
            if (inventory.isFull[i] == false)
            {
                foreach(Transform child in transform)
                {
                        Instantiate(child, inventory.slots[i].transform); // объект появился в слоте
                        GameObject.Destroy(child.gameObject);
                        inventory.isFull[i] = true;
                        i--;
                        inventory.isFull[i] = false;
                        i++;
                        break;
                }
            }
            i--;
        }
    }
}
