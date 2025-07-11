namespace MK.Pool
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class GameObjectPool : MonoBehaviour
    {
        private GameObject cachedOriginal;

        private readonly List<GameObject> activeObjects   = new();
        private readonly List<GameObject> inactiveObjects = new();

        public void Create(GameObject original, int initialAmount)
        {
            this.cachedOriginal = original;
            for (var i = 0; i < initialAmount; i++)
            {
                var newObj = Instantiate(this.cachedOriginal);
                newObj.SetActive(false);
                this.inactiveObjects.Add(newObj);
            }
        }

        public bool Contains(GameObject element)
        {
            return this.activeObjects.Contains(element);
        }

        public GameObject Spawn()
        {
            GameObject obj;
            if (this.inactiveObjects.Count > 0)
            {
                obj = this.inactiveObjects[0];
                this.inactiveObjects.RemoveAt(0);
            }
            else
            {
                obj = Instantiate(this.cachedOriginal);
            }

            obj.SetActive(true);
            this.activeObjects.Add(obj);

            return obj;
        }

        public void Recycle(GameObject element)
        {
            if (!this.activeObjects.Remove(element))
            {
                throw new Exception($"Couldn't recycle object: {element.name}");
            }

            this.inactiveObjects.Add(element);
            element.SetActive(false);
        }

        public void Cleanup()
        {
            while (this.inactiveObjects.Count > 0)
            {
                Destroy(this.inactiveObjects[0]);
                this.inactiveObjects.RemoveAt(0);
            }

            while (this.activeObjects.Count > 0)
            {
                Destroy(this.activeObjects[0]);
                this.activeObjects.RemoveAt(0);
            }

            this.inactiveObjects.Clear();
            this.activeObjects.Clear();
        }
    }
}