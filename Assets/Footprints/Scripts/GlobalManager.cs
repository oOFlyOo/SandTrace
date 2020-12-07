using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour {
    public GameObject footPrint;
    public GameObject dogPrint;
    public GameObject footParticles;
    // Use this for initialization
    void Start () {
        QualitySettings.vSyncCount = 0;

        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "DustyDemo")
        {
            RenderSettings.fogColor = new Color(130.0f / 255.0F, 112.0f / 255.0F, 81.0f / 255.0F, 255.0f / 255.0F);
        }
        else
        {
            RenderSettings.fogColor = new Color(255.0f / 255.0F, 255.0f / 255.0F, 255.0f / 255.0F, 255.0f / 255.0F);
        }

        CreateObjectPools();
    }
    private void CreateObjectPools()
    {
        ObjectPoolManager.Instance.CreatePool(this.footPrint, 1, 200);
        ObjectPoolManager.Instance.CreatePool(this.dogPrint, 1, 200);
        ObjectPoolManager.Instance.CreatePool(this.footParticles, 1, 200);
    }
}
