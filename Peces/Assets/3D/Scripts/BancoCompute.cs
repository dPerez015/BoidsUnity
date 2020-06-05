using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BancoCompute : MonoBehaviour
{
    public FishSettings3D settings;
    List<Fish3D> componentes;

    public GameObject fishPrefab;
    public int numFishes;

    static List<Vector3> collCheckDir;
    static int numDirections = 32;

    public float spawnSize = 2;

    public ComputeShader compute;
    const int threadGroupSize = 128;

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
        for (int i = 0; i < numFishes; i++)
        {
            GameObject fish = Instantiate(fishPrefab, transform);

            Fish3D fishComp = fish.GetComponent<Fish3D>();
            fishComp.id = componentes.Count;
            fishComp.settings = settings;
            fishComp.Init(collCheckDir, spawnSize);
            componentes.Add(fishComp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateFishData();
    }

    void CalculateFishData()
    {
        //we generate an array to pass the data from the cpu to the gpu
        int numFishes = componentes.Count;
        BoidData[] data = new BoidData[numFishes];
        //filling the array with actual framedata
        for(int i = 0; i < numFishes; i++)
        {
            data[i].position = componentes[i].transform.position;
            data[i].direction = componentes[i].transform.forward;
        }
        //we create the buffer to pass the data
        ComputeBuffer dataBuffer = new ComputeBuffer(numFishes, BoidData.Size);
        dataBuffer.SetData(data);
        //we pass the values to the GPU, setBuffer gets a kernelIndex (0 since we only have one, other wise should use FindKernel with the name of the function)
        compute.SetBuffer(0, "boids", dataBuffer);
        compute.SetInt("numFishes", numFishes);
        compute.SetFloat("viewRadius", settings.sightRadius);
        compute.SetFloat("avoidRadius", settings.avoidRadius);

        int threadGroups = Mathf.CeilToInt(numFishes / (float)threadGroupSize);
        compute.Dispatch(0, threadGroups, 1, 1);

        dataBuffer.GetData(data);
        for(int i = 0; i<numFishes; i++)
        {
            componentes[i].avoidanceDirection = data[i].avoidanceDirection;
            componentes[i].flockCenter = data[i].flockCenter;
            componentes[i].flockDirection = data[i].flockHeading;
            componentes[i].numFlockMates = data[i].numFlockmates;
            
            componentes[i].UpdateFish();
        }
        //Debug.Log(data[10].numFlockmates);

        dataBuffer.Release();
        
    }

    public void OnDestroy()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

}




public struct BoidData
{
    public Vector3 position;
    public Vector3 direction;

    public Vector3 flockHeading;
    public Vector3 flockCenter;
    public Vector3 avoidanceDirection;
    public int numFlockmates;
    public float debug;

    public static int Size
    {
        get
        {
            return sizeof(float) * 3 * 5 + sizeof(int) + sizeof(float);
        }
    }
}
