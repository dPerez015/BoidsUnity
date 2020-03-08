using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banco3D : MonoBehaviour
{
    public FishSettings3D settings;
    List<Fish3D> componentes;

    public GameObject fishPrefab;
    public int numFishes;

    static List<Vector3> collCheckDir;
    static int numDirections = 32;
    public int numCheckedDir = 0;

    public float spawnSize = 2;

    void Start()
    {
        componentes = new List<Fish3D>(numFishes);
        //we generate the lookup directions for the collision checks once
        if (collCheckDir == null)
        {
            collCheckDir = new List<Vector3>(numDirections);
            for (int i = 0; i < numDirections; i++)
            {
                float phi = Mathf.Acos(1.0f - 2.0f * (float)i / numDirections);
                float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * i;
                collCheckDir.Add(new Vector3(Mathf.Cos(theta) * Mathf.Sin(phi), Mathf.Sin(theta) * Mathf.Sin(phi), Mathf.Cos(phi)));
            }
        }

        //we generate the fishes
        for (int i = 0; i<numFishes; i++)
        {
            GameObject fish = Instantiate(fishPrefab,transform);

            Fish3D fishComp = fish.GetComponent<Fish3D>();
            fishComp.id = componentes.Count;
            fishComp.settings = settings;
            fishComp.Init(collCheckDir, spawnSize);
            componentes.Add(fishComp);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, spawnSize);
     
        Gizmos.color = Color.green;
        for (int i = 0; i < numCheckedDir && i < numDirections; i++)
        {
            Gizmos.DrawLine(transform.position, transform.position + collCheckDir[i]);
        }
        Gizmos.color = Color.red;
        if (collCheckDir != null)
        {
            for (int i = numCheckedDir; i < numDirections; i++)
            {
                Gizmos.DrawLine(transform.position, transform.position + collCheckDir[i]);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Fish3D fish in componentes)
        {
            CalculateFishData(fish);
            fish.UpdateFish();
        }
    }

    void CalculateFishData(Fish3D aFish)
    {
        //RESET
        aFish.flockCenter = Vector3.zero;
        aFish.flockDirection = Vector3.zero;
        aFish.avoidanceDirection = Vector3.zero;
        aFish.numFlockMates = 0;

        foreach (Fish3D otherFish in componentes)
        {
            if (aFish.id != otherFish.id)
            {
                //position checking
                Vector3 otherFishPos = otherFish.transform.position;
                Vector3 diference = aFish.transform.position - otherFishPos;

                float distance = diference.magnitude;

                //data calculation
                if (distance < settings.sightRadius)
                {

                    aFish.flockCenter += -otherFishPos;
                    aFish.flockDirection += otherFish.transform.forward;

                    if (distance < settings.avoidRadius)
                        aFish.avoidanceDirection += diference / distance;
                    aFish.numFlockMates++;

                    #if UNITY_EDITOR
                    if (aFish.drawGizmo)
                        Debug.DrawLine(aFish.transform.position, otherFishPos);
                    #endif
                }
            }
        }
    }
}
