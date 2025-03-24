using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public int id { get; private set; }
    private SceneItem sceneItem;
    [SerializeField] private const float MAX_SPRITE_SIZE = 0.4f;

    public void SetParameters(int id, SceneItem sceneItem, Sprite sprite)
    {
        this.id = id;
        this.sceneItem = sceneItem;

        GetComponent<Image>().sprite = sprite;
        GetComponent<RectTransform>().localScale = sprite.bounds.size * MAX_SPRITE_SIZE;
    }

    public void Show() => gameObject.SetActive(true);

    public void Hide()
    {
        gameObject.SetActive(false);
        transform.SetParent(null);
    }

    public void Drop()
    {
        Hide();
        sceneItem.Spawn();
    }
}