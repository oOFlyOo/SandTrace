using UnityEngine;
using System.Collections.Generic;

// The object pool is a list of already instantiated game objects of the same type.
public class ObjectPool
{
    //the list of objects.
    private List<GameObject> pooledObjects;

    //sample of the actual object to store. Used if we need to grow the pool.
    private GameObject pooledObj;

    //maximum number of objects to have in pool.
    private int maxPoolSize;

    //initial and default number of objects in pool.
    private int initialPoolSize;

    // Constructor
    public ObjectPool(GameObject obj, int initialPoolSize, int maxPoolSize)
    {
        //instantiate a new list of game objects to store our pooled objects in.
        pooledObjects = new List<GameObject>();

        //create and add an object based on initial size.
        for (int i = 0; i < initialPoolSize; i++)
        {
            //instantiate and create a game object with base attributes.
            GameObject nObj = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;

            //make sure the object isn't active.
            nObj.SetActive(false);

            //add the object too our list.
            pooledObjects.Add(nObj);

            //Don't destroy on load, it remains accessible.
            GameObject.DontDestroyOnLoad(nObj);
        }

        //store other settings that are useful.
        this.maxPoolSize = maxPoolSize;
        this.pooledObj = obj;
        this.initialPoolSize = initialPoolSize;
    }

    // Return an active object from the object pool
    public GameObject GetObject()
    {
        //iterate through all pooled objects.
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            //look for the first one that is inactive.
            if (pooledObjects[i].activeSelf == false)
            {
                //set the object to active.
                pooledObjects[i].SetActive(true);
                //return the object we found
                return pooledObjects[i];
            }
        }
        // No inactive objects so try and grow the pool
        if (this.maxPoolSize > this.pooledObjects.Count)
        {
            //Instantiate a new object
            GameObject nObj = GameObject.Instantiate(pooledObj, Vector3.zero, Quaternion.identity) as GameObject;
            //set object to active
            nObj.SetActive(true);
            //add it to the pool of objects for use
            pooledObjects.Add(nObj);

            //Don't destroy on load, objects remain accessible.
            GameObject.DontDestroyOnLoad(nObj);

            //return the object to the requestor.
            return nObj;
        }
        // No inactive objects and couldn't grow the pool
        return null;
    }
}