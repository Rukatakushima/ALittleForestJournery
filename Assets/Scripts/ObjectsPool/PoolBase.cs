using System;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectsPool
{
    public class PoolBase<T>
    {
        private readonly Func<T> PreloadFunc;
        private readonly Action<T> GetAction;
        private readonly Action<T> ReturnAction;
        private Queue<T> pool = new Queue<T>();
        private List<T> active = new List<T>();

        public PoolBase(Func<T> preloadFunc, Action<T> getAction, Action<T> returnAction, int preloadCount)
        {
            PreloadFunc = preloadFunc;
            GetAction = getAction;
            ReturnAction = returnAction;

            if (preloadFunc == null)
            {
                Debug.LogError("Preload function is null");
                return;
            }

            for (int i = 0; i < preloadCount; i++)
                ReturnToPool(PreloadFunc());
        }

        public T GetFromPool()
        {
            T item = pool.Count > 0 ? pool.Dequeue() : PreloadFunc();
            GetAction(item);
            active.Add(item);
            return item;
        }

        public void ReturnToPool(T item)
        {
            ReturnAction(item);
            pool.Enqueue(item);
            active.Remove(item);
        }

        public void ReturnAllToPool()
        {
            foreach (T item in active.ToArray())
                ReturnToPool(item);
        }
    }
}