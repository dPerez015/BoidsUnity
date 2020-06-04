using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhongPointLightDirecto : MonoBehaviour
{
    public Material phongIlumMat;
    int propId;

    Renderer rend;
    Material thisMat;


    // Start is called before the first frame update
    void Start()
    {
        propId = Shader.PropertyToID("Position");
        rend = GetComponent<Renderer>();
        thisMat = rend.material;
    }

    // Update is called once per frame
    void Update()
    {
        phongIlumMat.SetVector(propId, transform.position);
    }


}
