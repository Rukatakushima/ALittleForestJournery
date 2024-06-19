using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CharacterManager : MonoBehaviour
{
    public BodyPart body = new BodyPart("Body", "body", "", -5, new Vector2(0, 0), false, false, false, false, false, false);
    public BodyPart head = new BodyPart("Head", "head", "", -4, new Vector2(-0.015f, 0.44f), false, false, true, false, false, false);
    public BodyPart hair = new BodyPart("Hair", "hair", "", -3, new Vector2(0.06f, 0.185f), false, false, false, true, false, false);
    public BodyPart armLeft = new BodyPart("Arm Left", "arm", "", -6, new Vector2(0.19f, -0.07f), false, false, true, false, false, false);
    public BodyPart armRight = new BodyPart("Arm Right", "arm", "", -6, new Vector2(-0.18f, -0.07f), true, false, true, false, false, false);
    public BodyPart legLeft = new BodyPart("Leg Left", "leg", "", -6, new Vector2(0.03f, -0.42f), false, false, false, false, false, false);
    public BodyPart legRight = new BodyPart("Leg Right", "leg", "", -6, new Vector2(-0.11f, -0.42f), false, false, false, false, false, false);
    public List<BodyPart> CharacterBody;
    [System.Serializable]
    public class BodyPart
    {
        public string partName, partSpriteName; 
        public int orderInLayer;
        public Vector2 position;
        public bool reflectXByScale, isChildOfPreviousPart, isChildOfBody, isChildOfHead, isChildOfOtherPart;
        public string parentName;
        public bool isChangable;
        public BodyPart(string partName, string partSpriteName, string parentName, int orderInLayer, Vector2 position, bool reflectXByScale, bool isChildOfPreviousPart, bool isChildOfBody, bool isChildOfHead, bool isChildOfOtherPart, bool isChangable)
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
            this.isChildOfOtherPart = isChildOfOtherPart;
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
    void Awake()
    {
        InitializationBaseOfCharacter();
        LoadCharacter();
    }
    void Update(){    }    
    public void InitializationBaseOfCharacter()
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
    }
    public void LoadCharacter()
    {
        GameObject previousPart = null; // Сохранение ссылок на предыдущие объекты,
        // GameObject partBody = null;
        // GameObject partHead = null;
        // GameObject prevPart = null;
        // Создание объектов из списка CharacterBody типа BodyPart(класс)
        foreach (BodyPart bodyPart in CharacterBody)
        {
            if (GameObject.Find(bodyPart.partName) == null) // Проверка на существование объекта
            {
                GameObject part = new GameObject(bodyPart.partName); // Создание объекта
                // if (bodyPart.isChildOfOtherPart || bodyPart.isChildOfBody || bodyPart.isChildOfHead)
                // {
                //     prevPart = FindParent(transform, bodyPart.parentName);
                //     part.transform.SetParent(prevPart.transform);
                // }
                if (bodyPart.isChildOfOtherPart || bodyPart.isChildOfBody || bodyPart.isChildOfHead)
                {
                    //prevPart = FindParent(transform, bodyPart.parentName);
                    //prevPart = transform.Find(bodyPart.parentName).gameObject;
                    part.transform.SetParent(FindParent(transform, bodyPart.parentName).transform);
                }
                // else if (bodyPart.isChildOfBody)
                // {
                //     part.transform.SetParent(partBody.transform);
                // }
                // else if (bodyPart.isChildOfHead)
                // {
                //     part.transform.SetParent(partHead.transform);
                // }
                else if (bodyPart.isChildOfPreviousPart && previousPart != null)
                {
                    part.transform.SetParent(previousPart.transform);
                }
                else
                {
                    part.transform.SetParent(transform);
                }
                part.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(gameObject.name + "/" + bodyPart.partSpriteName);
                part.GetComponent<SpriteRenderer>().sortingOrder = bodyPart.orderInLayer;
                part.transform.localPosition = bodyPart.position;
                if (bodyPart.reflectXByScale) part.transform.localScale = new Vector2(-1f, 1f);
                previousPart = part;
                // if (bodyPart.partName == "Body") partBody = part;
                // if (bodyPart.partName == "Head") partHead = part;
            }
        }
    }
    // public GameObject FindParent(Transform parent, string searchParent)
    // {
    //     foreach (Transform child in parent)
    //     {
    //         if (child.name == searchParent)
    //         {
    //             return child;
    //         }
    //         else
    //         {
    //             FindParent(child);
    //         }
    //     }
    // }
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
        return null;
    }
}
