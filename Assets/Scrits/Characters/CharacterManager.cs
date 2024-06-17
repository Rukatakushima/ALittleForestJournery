using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    
    public string[] partNames = new string[]{"Body", "Head", "Hair", "Arm", "Leg"};
    public int[] ordersInLayer = new int[] {-5, -4, -3, -6};
    public Vector3 bodyPosition = new Vector3(0, 0, 0);
    public Vector3 headPosition = new Vector3(-0.015f, 0.44f, 0);
    public Vector3 hairPosition = new Vector3(0.06f, 0.185f, 0);
    public Vector3 armLeftPosition = new Vector3(0.19f, -0.07f, 0);
    public Vector3 armRightPosition = new Vector3(-0.18f, -0.07f, 0);
    public Vector3 legLeftPosition = new Vector3(0.03f, -0.42f, 0);
    public Vector3 legRightPosition = new Vector3(-0.11f, -0.42f, 0);
    public List<BodyPart> CharacterBody;
    [System.Serializable]
    public class BodyPart
    {
        public string partName;
        public int orderInLayer;
        public Vector3 position;
        public bool reflectXByScale, isChildOfPreviousPart, isChildOfBody, isChangable;
    }
    void Start()
    {
        LoadBaseOfCharacter();
        LoadCharacter();
    }
    void Update()
    {
        
    }
    public void LoadBaseOfCharacter()
    {
        // Body:
        CharacterBody.Add(new BodyPart() 
        {
            partName = partNames[0],
            orderInLayer = ordersInLayer[0],
            position = bodyPosition,
        });
        // Head:
        CharacterBody.Add(new BodyPart() 
        {
            partName = partNames[1],
            orderInLayer = ordersInLayer[1],
            position = headPosition,
            isChildOfPreviousPart = true,
        });
        // Hair:
        CharacterBody.Add(new BodyPart() 
        {
            partName = partNames[2],
            orderInLayer = ordersInLayer[2],
            position = hairPosition,
            isChildOfPreviousPart = true,
        });
        // ArmLeft:
        CharacterBody.Add(new BodyPart() 
        {
            partName = partNames[3],
            orderInLayer = ordersInLayer[3],
            position = armLeftPosition,
            isChildOfBody = true,
        });
        // ArmRight:
        CharacterBody.Add(new BodyPart() 
        {
            partName = partNames[3],
            orderInLayer = ordersInLayer[3],
            position = armRightPosition,
            reflectXByScale = true,
            isChildOfBody = true,
        });
        // LegLeft:
        CharacterBody.Add(new BodyPart() 
        {
            partName = partNames[4],
            orderInLayer = ordersInLayer[3],
            position = legLeftPosition,
        });
        // LegRight:
        CharacterBody.Add(new BodyPart() 
        {
            partName = partNames[4],
            orderInLayer = ordersInLayer[3],
            position = legRightPosition,
        });
    }

    public void LoadCharacter()
    {        
        /*
        // Создание объектов для частей тела
        GameObject body = new GameObject("Body");
        GameObject head = new GameObject("Head");
        GameObject hair = new GameObject("Hair");
        GameObject armLeft = new GameObject("Arm");
        GameObject armRight = new GameObject("Arm");
        GameObject legLeft = new GameObject("Leg");
        GameObject legRight = new GameObject("Leg");

        // Расположение объектов в иерархии
        body.transform.SetParent(transform); 
        head.transform.SetParent(body.transform); 
        hair.transform.SetParent(head.transform); 
        armLeft.transform.SetParent(body.transform); 
        armRight.transform.SetParent(body.transform); 
        legLeft.transform.SetParent(transform); 
        legRight.transform.SetParent(transform); 

        AddSpriteRender(transform);

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
        */
        
        /*
        for (int i = 0; i < CharacterBody.Count; i++)
        {
            GameObject part = new GameObject(CharacterBody[i].partName);
            if (CharacterBody[i].isChildOfPreviousPart)
            {
                part.transform.SetParent(CharacterBody[i-1].transform);
            }
            else
            {
                part.transform.SetParent(transform); 
            }
            part.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(gameObject.name+"/"+part.name);
            part.GetComponent<SpriteRenderer>().sortingOrder = CharacterBody[i].orderInLayer;
            part.transform.localPosition = CharacterBody[i].position;
            if (CharacterBody[i].reflectXByScale) part.transform.localScale = new Vector3(-1f, 1f, 0);
        }*/
        // Создание объектов из списка CharacterBody типа BodyPart(класс)
        GameObject previousPart = null;
        GameObject body = null;
        foreach (BodyPart bodyPart in CharacterBody)
        {
            GameObject part = new GameObject(bodyPart.partName);
            if (bodyPart.isChildOfPreviousPart && previousPart != null)
            {
                part.transform.SetParent(previousPart.transform);
            }
            else if (bodyPart.isChildOfBody)
            {
                part.transform.SetParent(body.transform);
            }
            else
            {
                part.transform.SetParent(transform); 
            }
            part.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(gameObject.name+"/"+part.name);
            part.GetComponent<SpriteRenderer>().sortingOrder = bodyPart.orderInLayer;
            part.transform.localPosition = bodyPart.position;
            if (bodyPart.reflectXByScale) part.transform.localScale = new Vector3(-1f, 1f, 0);
            previousPart = part;
            if (bodyPart.partName == "Body") body = part;
        }
    }
/*
    public void AddSpriteRender(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            // Добавление SpriteRenderer -> Загрузка спрайта из папки Resources
            parent.GetChild(i).gameObject.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(gameObject.name+"/"+parent.GetChild(i).gameObject.name);
            AddSpriteRender(parent.GetChild(i));
        }
    }*/
}
