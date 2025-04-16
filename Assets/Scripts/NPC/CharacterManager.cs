using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    private bool isInitialized;
    public bool createCharacterInInspector = false;
    public bool dublicateCharacter = false;
    public BodyPart body = new BodyPart("Body", "Body", "", -5, new Vector2(0, 0), false, false, false, false, false);
    public BodyPart head = new BodyPart("Head", "Head", "", -4, new Vector2(-0.015f, 0.44f), false, false, true, false, false);
    public BodyPart hair = new BodyPart("Hair", "Hair", "", -3, new Vector2(0.06f, 0.185f), false, false, false, true, false);
    public BodyPart armLeft = new BodyPart("Left Hand", "Hand", "", -6, new Vector2(0.19f, -0.07f), false, false, true, false, false);
    public BodyPart armRight = new BodyPart("Right Hand", "Hand", "", -6, new Vector2(-0.18f, -0.07f), true, false, true, false, false);
    public BodyPart legLeft = new BodyPart("Left Leg", "Leg", "", -6, new Vector2(0.03f, -0.42f), false, false, false, false, false);
    public BodyPart legRight = new BodyPart("Right Leg", "Leg", "", -6, new Vector2(-0.11f, -0.42f), false, false, false, false, false);
    public List<BodyPart> CharacterBody;
    [System.Serializable]
    public class BodyPart
    {
        public string partName, partSpriteName;
        public int orderInLayer;
        public Vector2 position;
        public bool reflectXByScale, isChildOfPreviousPart, isChildOfBody, isChildOfHead;
        public string parentName;
        public bool isChangable;
        public BodyPart(string partName, string partSpriteName, string parentName, int orderInLayer, Vector2 position, bool reflectXByScale, bool isChildOfPreviousPart, bool isChildOfBody, bool isChildOfHead, bool isChangable)
        {
            this.partName = partName;
            this.partSpriteName = partSpriteName;
            this.parentName = parentName;
            this.orderInLayer = orderInLayer;
            this.position = position;
            this.reflectXByScale = reflectXByScale;
            this.isChildOfPreviousPart = isChildOfPreviousPart;
            this.isChildOfBody = isChildOfBody;
            this.isChildOfHead = isChildOfHead;
            this.isChangable = isChangable;
        }
        public void ChooseParentPart()
        {
            if (isChildOfBody)
            {
                parentName = "Body";
            }
            else if (isChildOfHead)
            {
                parentName = "Head";
            }
        }
    }
#if UNITY_EDITOR
    void OnValidate()
    {
        if(createCharacterInInspector)
        {
            InitializationBaseOfCharacter();
            LoadCharacter();
        }
        
    }
#endif
    // void Awake()
    // {
    //     InitializationBaseOfCharacter();
    //     LoadCharacter();
    // }
    void Update() { }
    public void InitializationBaseOfCharacter()
    {
        if (isInitialized)
        {
            return;
        }
        else
        {
            // Установка parentName
            body.ChooseParentPart();
            head.ChooseParentPart();
            hair.ChooseParentPart();
            armLeft.ChooseParentPart();
            armRight.ChooseParentPart();
            legLeft.ChooseParentPart();
            legRight.ChooseParentPart();
            // Добавление базовых частей тела в начало списка
            CharacterBody.Insert(0, body);
            CharacterBody.Insert(1, head);
            CharacterBody.Insert(2, hair);
            CharacterBody.Insert(3, armLeft);
            CharacterBody.Insert(4, armRight);
            CharacterBody.Insert(5, legLeft);
            CharacterBody.Insert(6, legRight);
            isInitialized = true;
        }

    }
    public void LoadCharacter()
    {
        GameObject previousPart = null; // Сохранение ссылок на предыдущие объекты
        // Создание объектов из списка CharacterBody типа BodyPart(класс)
        foreach (BodyPart bodyPart in CharacterBody)
        {
            if (GameObject.Find(bodyPart.partName) == null || (GameObject.Find(bodyPart.partName) != null && dublicateCharacter)) // Проверка на существование объекта
            {
                GameObject part = new GameObject(bodyPart.partName); // Создание объекта
                if (bodyPart.parentName != "")
                {
                    part.transform.SetParent(FindParent(transform, bodyPart.parentName).transform);
                }
                else if (bodyPart.isChildOfPreviousPart && previousPart != null)
                {
                    part.transform.SetParent(previousPart.transform);
                }
                else
                {
                    part.transform.SetParent(transform);
                }
                part.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Characters/" + gameObject.name + "/" + bodyPart.partSpriteName);
                Debug.Log(gameObject.name + "/" + bodyPart.partSpriteName);
                part.GetComponent<SpriteRenderer>().sortingOrder = bodyPart.orderInLayer;
                part.transform.localPosition = bodyPart.position;
                if (bodyPart.reflectXByScale) part.transform.localScale = new Vector2(-1f, 1f);
                previousPart = part;
            }
        }
    }
    public GameObject FindParent(Transform parent, string searchParent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).name == searchParent)
            {
                return parent.GetChild(i).gameObject;
            }
            else
            {
                GameObject result = FindParent(parent.GetChild(i), searchParent);
                if (result != null)
                {
                    return result;
                }
            }
        }
        return parent.gameObject;
    }
}
