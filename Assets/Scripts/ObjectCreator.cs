using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ObjectCreator : MonoBehaviour
{
    private void OnEnable()
    {
        // Создаем новый объект
        GameObject newObject = new GameObject("NewObject");
        // Присваиваем ему позицию от объекта, на котором находится скрипт
        newObject.transform.position = transform.position;
        // Делаем новый объект дочерним к объекту, на котором находится скрипт
        newObject.transform.parent = transform;
    }
}
