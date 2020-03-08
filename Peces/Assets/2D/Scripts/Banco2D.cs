using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banco2D : MonoBehaviour
{
    public Fish2DSettings settings;

    public GameObject fishPrefab;
    List<Fish2D> componentes;
    public int numFishes;

    /*
    *Fish creation and initialization
    */
    void Start()
    {
        componentes = new List<Fish2D>(numFishes);
        for (int i = 0; i < numFishes; i++)
        {
            GameObject newFish = Instantiate(fishPrefab);

            Fish2D fishComp = newFish.GetComponent<Fish2D>();
            fishComp.id = componentes.Count;
            fishComp.settings = settings;
            fishComp.Init();
            componentes.Add(fishComp);

            newFish.transform.parent = gameObject.transform;
        }
    }

    /*
     * Calls the fishes update function
     */
    private void Update()
    {
        foreach (Fish2D fish in componentes)
        {
            CalculateFishData(fish);
            fish.UpdateFish();
        }
    }

    void CalculateFishData(Fish2D aFish)
    {
        //RESET
        aFish.flockCenter = Vector3.zero;
        aFish.flockDirection = Vector3.zero;
        aFish.avoidanceDirection = Vector3.zero;
        aFish.numFlockMates = 0;

        foreach (Fish2D otherFish in componentes)
        {
            if (aFish.id != otherFish.id)
            {
                //position checking
                Vector3 otherFishPos = otherFish.transform.position;
                Vector3 diference = aFish.transform.position - otherFishPos;

                if (diference.x > 20.0f-settings.sightRadius)
                {
                    if (otherFishPos.x > aFish.transform.position.x)
                        otherFishPos.x -= 20.0f;
                    else
                        otherFishPos.x += 20.0f;
                }
                if(diference.z > 10.0f - settings.sightRadius)
                {
                    if (otherFishPos.z > aFish.transform.position.z)
                        otherFishPos.z -= 10.0f;
                    else
                        otherFishPos.z += 10.0f;
                }

                diference = aFish.transform.position - otherFishPos;
                float distance = diference.magnitude;

                //data calculation
                if (distance < settings.sightRadius)
                {
                   
                    aFish.flockCenter += -otherFishPos;
                    aFish.flockDirection += otherFish.forward;

                    if(distance < settings.avoidRadius)
                        aFish.avoidanceDirection += diference/distance;
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
