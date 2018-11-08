using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace Atlanticide
{
    public class Pool<T>
            where T : Component
    {
        // The initial size of the pool.
        private int _poolSize;

        // The prefab from which all objects in the pool are instantiated.
        private T _objectPrefab;

        // When the pool runs out of objects, should the pool grow or just
        // return null.
        private bool _shouldGrow;

        // The list containing all the objects in this pool.
        private List<T> _pool;

        private Action<T> _initMethod;

        public Pool( int poolSize, bool shouldGrow, T prefab )
        {
            _poolSize = poolSize;
            _shouldGrow = shouldGrow;
            _objectPrefab = prefab;
            // Initialize the pool by adding '_poolSize' amount of objects to the pool.
            _pool = new List<T>(_poolSize);

            for(int i = 0; i < _poolSize; ++i)
            {
                AddObject();
            }
        }

        public Pool(int poolSize, bool shouldGrow, T[] prefabArray)
        {
            _poolSize = poolSize;
            _shouldGrow = shouldGrow;
            // Initialize the pool by adding '_poolSize' amount of objects to the pool.
            _pool = new List<T>(_poolSize);

            int i = 0;
            while (i < _poolSize)
            {
                foreach(T t in prefabArray)
                {
                    _objectPrefab = t;
                    if (i < _poolSize)
                    {
                        AddObject();
                        i++;
                    }
                }
            }
        }

        public Pool( int poolSize, bool shouldGrow, T prefab, Action<T> initMethod )
            : this(poolSize, shouldGrow, prefab)
        {
            _initMethod = initMethod;

            foreach(var item in _pool)
            {
                _initMethod(item);
            }
        }

        /// <summary>
        /// Adds an object to the pool.
        /// </summary>
        /// <param name="isActive">Should the object be active when it is added to the pool or not.</param>
        /// <returns>The object added to the pool.</returns>
        private T AddObject( bool isActive = false )
        {
            // Instantiate pooled objects under this parent.
            T go = Object.Instantiate(_objectPrefab);

            if(isActive)
            {
                Activate(go);
            }
            else
            {
                Deactivate(go);
            }

            _pool.Add(go);

            return go;
        }

        /// <summary>
        /// Called when the object is returned to the pool. Deactivates the object.
        /// </summary>
        /// <param name="component">Object to deactivate</param>
        protected virtual void Deactivate( T component )
        {
            component.gameObject.SetActive(false);
        }

        public void DeactivateAllObjects()
        {
            foreach(T component in _pool)
            {
                component.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Called when the object is fetched from the pool. Activates the object.
        /// </summary>
        /// <param name="component">Object to activate</param>
        protected virtual void Activate( T component )
        {
            component.gameObject.SetActive(true);
        }

        /// <summary>
        /// Fetches the object form the pool.
        /// </summary>
        /// <returns>An object from the pool or if all objects are already in use and pool cannot grow, returns null</returns>
        public T GetPooledObject()
        {
            T result = null;
            for(int i = 0; i < _pool.Count; i++)
            {
                if(_pool[i].gameObject.activeSelf == false)
                {
                    result = _pool[i];
                    break; // Jumps out from the loop.
                }
            }

            // If there were no inactive GameObjects in the pool and the pool should
            // grow, then let's add a new object to the pool.
            if(result == null && _shouldGrow)
            {
                result = AddObject();
            }

            // If we found an incative object let's activate it.
            if(result != null)
            {
                Activate(result);
            }

            return result;
        }

        /// <summary>
        /// Returns an object back to the pool.
        /// </summary>
        /// <param name="component">The object which should be returned to the pool.</param>
        /// <returns>Could the object be returned back to the pool.</returns>
        public bool ReturnObject( T component )
        {
            bool result = false;

            foreach(var pooledObject in _pool)
            {
                if(pooledObject == component)
                {
                    Deactivate(component);
                    result = true;
                    break;
                }
            }

            if(!result)
            {
                Debug.LogError("Tried to return an object which doesn't belong to this pool!");
            }

            return result;
        }
    }
}
