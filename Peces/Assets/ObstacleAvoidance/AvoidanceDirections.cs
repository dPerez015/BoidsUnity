using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidanceDirections : MonoBehaviour
{
    public int numPoints = 100;
    public int numChecks=0;
    List<Vector3> points;
    public float multiplier = (1 + Mathf.Sqrt(5));
    float timeLastStep =0.0f;
    float timebetweensteps = 0.066f;
    public bool drawLines = true;
    
    // Start is called before the first frame update
    void Start()
    {     
        
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < numPoints; i++)
        {
            float phi = Mathf.Acos(1.0f - 2.0f * (float) i / numPoints);
            float theta = Mathf.PI * multiplier * i;
            points.Add(new Vector3(Mathf.Cos(theta)*Mathf.Sin(phi), Mathf.Sin(theta)*Mathf.Sin(phi), Mathf.Cos(phi)));
        }
        if (drawLines)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < numChecks && i < numPoints; i++)
            {
                Gizmos.DrawLine(transform.position, transform.position + points[i]);
            }
            Gizmos.color = Color.red;
            for (int i = numChecks; i < numPoints; i++)
            {
                Gizmos.DrawLine(transform.position, transform.position + points[i]);
            }
        }
        else
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < numPoints; i++)
                Gizmos.DrawSphere(transform.position + points[i], 0.02f);
        }
        points.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        timeLastStep += Time.deltaTime;
        if (timeLastStep > timebetweensteps)
        {
            timeLastStep -= timebetweensteps;
            multiplier += 0.0002f;
        }
    }
}
