using UnityEngine;

public class ItemOnScene : MonoBehaviour
{
    private Inventory inventory;
    public GameObject itemInInventory;
    public int id;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ItemsManager.Instance.SpawnInInventory(itemInInventory, this.gameObject, id);
        }
    }
}
