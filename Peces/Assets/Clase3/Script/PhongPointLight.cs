using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhongPointLight : MonoBehaviour
{

    public Material phongMaterial;
    int positionID;
    // Start is called before the first frame update
    void Start()
    {
        positionID = Shader.PropertyToID("Position");
    }

    // Update is called once per frame
    void Update()
    {
        phongMaterial.SetVector(positionID, transform.position);
    }
}
