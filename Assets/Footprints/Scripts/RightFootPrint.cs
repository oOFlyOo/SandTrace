using UnityEngine;
using System.Collections;
public class RightFootPrint : MonoBehaviour
{
    public bool footPrintsEnabled;
    public GameObject dustStep;
    public bool groundContact;
    public float groundDistance; // Lowest distance (from player) to draw footprints. For jumping.
    public GameObject footprintR; // Right footprint object.
     public int decayRate; // Time at which footprints disappear.
    public Vector3 offset; // Offset footprint positions.
    public AudioClip[] footSounds;
    private Transform mytransform;
    private Quaternion reflected;
    private float ry;
    public PlayerControl player;
    public Transform playerTransform;
    private GameObject dustParticle;
    void Awake()
    {
        mytransform = transform;
        reflected = new Quaternion();
    }
    // Use this for initialization
    void Start () {
        
        footPrintsEnabled = true;
        decayRate = 10;
        groundDistance = 5.0f;
    }
	
    void OnTriggerEnter(Collider grounder)
    {
        if (grounder.tag == "Terrain")
        {
            groundContact = true;
            GetComponent<AudioSource>().clip = footSounds[1];
            GetComponent<AudioSource>().Play();
        }
    }

    void OnTriggerExit(Collider grounder)
    {
        if (grounder.tag == "Terrain")
        {
            groundContact = false;
            RaycastHit hit;
            if (Physics.Raycast(mytransform.position, -Vector3.up, out hit, groundDistance) && hit.collider.CompareTag("Terrain"))
            {
                ry = playerTransform.eulerAngles.y + 180.0f;
                StartCoroutine(startFootPrint((hit.point + offset), ry, reflected));
                // Use Pool for a small dust puff on step
                StartCoroutine(startFootParticles(hit.point, ry, dustStep.transform.rotation));
            }
        }
    }
    IEnumerator startFootPrint(Vector3 position,float ry, Quaternion rotation)
    {
        GameObject footPrint = ObjectPoolManager.Instance.GetObject("Footstep");
        if (footPrint)
        {
            footPrint.transform.position = position;
            footPrint.transform.rotation = rotation;
            footPrint.transform.Rotate(0, ry, 0, Space.Self);
            yield return new WaitForSeconds(5.0f);
            footPrint.SetActive(false);
        }
    }
    IEnumerator startFootParticles(Vector3 position, float ry, Quaternion rotation)
    {
        GameObject footPrint = ObjectPoolManager.Instance.GetObject("DustStep");
        if (footPrint)
        {
            footPrint.transform.position = position;
            footPrint.transform.rotation = rotation;
            footPrint.GetComponent<ParticleSystem>().Play();
            yield return new WaitForSeconds(5.0f);
            footPrint.GetComponent<ParticleSystem>().Stop();
            footPrint.SetActive(false);
        }
    }
}
