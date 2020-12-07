using UnityEngine;
using System.Collections.Generic;
using System;

public class ObjectPoolManager
{
    // Assign instance before it can be accessed
    private static volatile ObjectPoolManager instance;

    // Allows lookup of collection of multiple pools
    private Dictionary<String, ObjectPool> objectPools;

    //object for locking
    private static object syncRoot = new System.Object();

    // Constructor
    private ObjectPoolManager()
    {
        //Ensure object pools exists.
        this.objectPools = new Dictionary<String, ObjectPool>();
    }

    // Retrieve singleton.
    public static ObjectPoolManager Instance
    {
        get
        {
            //check to see if it is null
            if (instance == null)
            {
                //lock access, if it is already locked, wait for lock to release.
                lock (syncRoot)
                {
                    //the instance could have been made between checking and waiting for a lock to release.
                    if (instance == null)
                    {
                        //create new instance
                        instance = new ObjectPoolManager();
                    }
                }
            }
            //return either the new instance or the existing one.
            return instance;
        }
    }

    // Create a new object pool
    public bool CreatePool(GameObject objToPool, int initialPoolSize, int maxPoolSize)
    {
        //Check to see if the pool already exists by searching the object pool
        if (ObjectPoolManager.Instance.objectPools.ContainsKey(objToPool.name))
        {
            //let the caller know it already exists, just use the pool that already exists.
            return false;
        }
        else
        {
            //create a new pool using user settings
            ObjectPool nPool = new ObjectPool(objToPool, initialPoolSize, maxPoolSize);
            //Add the pool to the dictionary of pools to manage using the object name as the key and the pool as the value.
            ObjectPoolManager.Instance.objectPools.Add(objToPool.name, nPool);
            // Return newly created pool
            return true;
        }
    }

    // Get an object from the pool.
    public GameObject GetObject(string objName)
    {
        //Find the right pool and search for an object.
        return ObjectPoolManager.Instance.objectPools[objName].GetObject();
    }
}