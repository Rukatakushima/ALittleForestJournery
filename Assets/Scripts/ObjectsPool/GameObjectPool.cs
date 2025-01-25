using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ObjectsPool
{
    public class GameObjectPool : PoolBase<GameObject>
    {
        public GameObjectPool(GameObject prefab, int preloadCount) 
        : base (preloadFunc:() => Preload(prefab), GetAction, ReturnAction, preloadCount) {}

        public static GameObject Preload(GameObject prefab) => Object.Instantiate(prefab);
        public static void GetAction(GameObject @object) => @object.SetActive(true);
        public static void ReturnAction(GameObject @object) => @object.SetActive(false);
    }
}