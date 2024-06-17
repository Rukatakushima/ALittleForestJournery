using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public string CharacterName;
    /*
    public List<BodyPart> CharacterBody;
    
    [System.Serializable]
    public class BodyPart
    {
        public string part;
        public Sprite sprite;
        public bool isChangable;
        public bool isChildOfPreviousPart;
    }
*/
    void Start()
    {
        LoadCharacter();
    }

    void Update()
    {
        
    }

    public void LoadCharacter()
    {
        // Создание объектов для частей тела
        GameObject body = new GameObject("Body");
        GameObject head = new GameObject("Head");
        GameObject hair = new GameObject("Hair");
        GameObject armLeft = new GameObject("ArmLeft");
        GameObject armRight = new GameObject("ArmRight");
        GameObject legLeft = new GameObject("LegLeft");
        GameObject legRight = new GameObject("LegRight");

        // Расположение объектов в иерархии
        body.transform.SetParent(transform); 
        head.transform.SetParent(body.transform); 
        hair.transform.SetParent(head.transform); 
        armLeft.transform.SetParent(body.transform); 
        armRight.transform.SetParent(body.transform); 
        legLeft.transform.SetParent(transform); 
        legRight.transform.SetParent(transform); 
        
        // Добавление SpriteRenderer - > Назначение спрайта -> Загрузка спрайта из папки Resources
        body.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(CharacterName+"/body");
        head.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(CharacterName+"/head");
        hair.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(CharacterName+"/hair");
        armLeft.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(CharacterName+"/arm");
        armRight.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(CharacterName+"/arm");
        legLeft.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(CharacterName+"/leg");
        legRight.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(CharacterName+"/leg");

        // Настройка порядка частей тела
        body.GetComponent<SpriteRenderer>().sortingOrder = -5; 
        head.GetComponent<SpriteRenderer>().sortingOrder = -4; 
        hair.GetComponent<SpriteRenderer>().sortingOrder = -3; 
        armLeft.GetComponent<SpriteRenderer>().sortingOrder = -6; 
        armRight.GetComponent<SpriteRenderer>().sortingOrder = -6; 
        legLeft.GetComponent<SpriteRenderer>().sortingOrder = -6; 
        legRight.GetComponent<SpriteRenderer>().sortingOrder = -6; 

        // Настройка позиций частей тела
        body.transform.localPosition = new Vector3(0, 0, 0);
        head.transform.localPosition = new Vector3(-0.015f, 0.44f, 0);
        hair.transform.localPosition = new Vector3(0.06f, 0.185f, 0);
        armLeft.transform.localPosition = new Vector3(0.19f, -0.07f, 0);
        armRight.transform.localPosition = new Vector3(-0.18f, -0.07f, 0);
        armRight.transform.localScale = new Vector3(-1f, 1f, 0);
        legLeft.transform.localPosition = new Vector3(0.03f, -0.42f, 0);
        legRight.transform.localPosition = new Vector3(-0.11f, -0.42f, 0);

        // // Добавление SpriteRenderer объектам
        // SpriteRenderer spriteRenderer = body.AddComponent<SpriteRenderer>();
        // // Загрузка спрайт из папки Resources
        // Sprite sprite = Resources.Load<Sprite>(CharacterName+"/body");
        // // Назначение спрайтов
        // spriteRenderer.sprite = sprite;
    }
}
