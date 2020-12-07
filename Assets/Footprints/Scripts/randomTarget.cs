using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class randomTarget : MonoBehaviour {
    private List<GameObject> targets;
    // Use this for initialization
    void Start () {
        targets = new List<GameObject>();
        targets.AddRange(GameObject.FindGameObjectsWithTag("Target"));
    }
	
	// Update is called once per frame
	void Update () {
        float distance = Vector3.Distance(gameObject.GetComponent<AICharacterControl>().target.transform.position, transform.position);
        if (distance <= 10.0f)
        {
            int randomTarget = Random.Range(0, targets.Count - 1);
            gameObject.GetComponent<AICharacterControl>().target = targets[randomTarget].transform;
        }
	}
}
