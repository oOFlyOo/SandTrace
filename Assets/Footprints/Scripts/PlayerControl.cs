using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
   
    // This monitors for a Right Click of the Mouse for Zooming
    public bool isAiming()
    {
        if (Input.GetButton("Fire2"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
