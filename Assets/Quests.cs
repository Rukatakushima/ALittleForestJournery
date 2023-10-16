using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quests : MonoBehaviour
{
    public int questNumber;
    public int[] items; //id предметов
    public GameObject[] clouds;//массив объектов с облаками для запроса предметов
    public GameObject barrier; //барьер, что не будет давать пройти
    public GameObject sprayCan;

    public int idPickedUp;
    
    //какой предмет для квеста мы отдаем? 
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag != "Player")
        {
            idPickedUp = other.gameObject.GetComponent<Pickup>().id;
            //Debug.Log(other.gameObject.GetComponent<Pickup>().id);
            
            //подобранный предмет = не игрок; у предмета есть компонент Pickup и проверяем его id
            if(other.tag != "Player" && other.gameObject.GetComponent<Pickup>().id == items[questNumber])
            {
                questNumber++;//прибавляем номер квеста, т.е. произошло взаимодействие
                Destroy(other.gameObject); //а сам предмет уничтожается, словно NPC забрал объект
                //DialogManager.StartDialogue(0);
            }
        }
    }
}
