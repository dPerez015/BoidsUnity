using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish3D : MonoBehaviour
{
    [HideInInspector]
    public FishSettings3D settings;

    Vector3 force;
    Vector3 velocity;
    float speed;

    [HideInInspector]
    public int id;

    [HideInInspector]
    public Vector3 avoidanceDirection;
    [HideInInspector]
    public Vector3 flockCenter;
    [HideInInspector]
    public Vector3 flockDirection;
    [HideInInspector]
    public Vector3 obstacleAvoidDir;
    [HideInInspector]
    public int numFlockMates;

    List<Vector3> collCheckDir;
    LayerMask obsMask;

#if UNITY_EDITOR
    public bool drawGizmo = false;
#endif

    /*
     * Places the fish at a a random position and orientation. Also sets the velocity
     * */
    public void Init(List<Vector3> directions, float spawnSize)
    {
        //we save a reference to the vector of directions
        collCheckDir = directions;
        
        //we get the obstacle mask 
        obsMask = LayerMask.GetMask("Obstacles");

        //new forward direction for the fish
        Vector3 newOrinetation = Random.onUnitSphere;

        //We change the rotation of the fish to face the new direction
        float angleBetweenVec = Mathf.Acos(Vector3.Dot(transform.forward, newOrinetation));
        Vector3 rotationAxis = Vector3.Cross(transform.forward, newOrinetation);
        transform.Rotate(rotationAxis, Mathf.Rad2Deg*angleBetweenVec);

        speed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = transform.up * speed;

        transform.Translate(Random.insideUnitSphere* spawnSize, Space.World);

        GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.blue, new Color(0.6f, 0.6f, 0.9f, 1.0f), Random.Range(0.0f, 1.0f));

        speed = (settings.minSpeed + settings.maxSpeed) / 2.0f;

    }

    // Update is called once per frame
    public void UpdateFish()
    {

        //reseting forces
        force = Vector3.zero;

        if (numFlockMates > 0)
        {
            //Steer away from other fishes
            avoidanceDirection /= numFlockMates;
            force += settings.avoidanceWeight * SteerForceTowards(avoidanceDirection);

            //go to the same direction as other fishes on the flock
            flockDirection /= numFlockMates;
            force += settings.alignWeight * SteerForceTowards(flockDirection);

            //stay close to the center of the flock
            flockCenter /= numFlockMates;
            flockCenter = flockCenter - transform.position;
            force += settings.cohesionWeight * SteerForceTowards(flockCenter);
           
        }

        //avoid obstacles
        if (isHeadingToCollider())
        {
            obstacleAvoidDir = calculateAvoidanceDir();
            force += settings.obstacleAvoidanceWeight * SteerForceTowards(obstacleAvoidDir);
        }
        //calculate new velocity and speed;
        velocity += force * Time.deltaTime;
        speed = Mathf.Clamp(velocity.magnitude, settings.minSpeed, settings.maxSpeed);
        transform.forward = velocity.normalized;
        velocity = speed * transform.forward;

        //movement
        transform.position = transform.position + transform.forward * speed * Time.deltaTime;

    }

    bool isHeadingToCollider()
    {
        RaycastHit hit;
        Vector3 dir = transform.TransformDirection(collCheckDir[0]);
        return Physics.SphereCast(transform.position, 0.1f, dir, out hit, settings.obsAvoidRadius);
    }

    Vector3 calculateAvoidanceDir()
    {
        Vector3 retDir=transform.forward;
        float maxDist = 0;
        RaycastHit hit;

        for(int i = 1; i< collCheckDir.Count; i++)
        {//transform the original directions to match the current orientation of the fish
            Vector3 dir = transform.TransformDirection(collCheckDir[i]);
            if(Physics.SphereCast(transform.position,0.1f, dir,out hit, settings.obsAvoidRadius))
            {
                if (hit.distance > maxDist)
                {
                    //Debug.DrawLine(transform.position, transform.position + retDir,Color.red);
                    retDir = dir;
                    maxDist = hit.distance;
                }
            }
            else
            {
                //Debug.DrawLine(transform.position, transform.position + retDir, Color.green);
                return dir;
            }
        }
        return retDir;
    }


    /*
    * Computes the force necesary to get to the desired direction
    */
    public Vector3 SteerForceTowards(Vector3 vec)
    {
        Vector3 force = vec.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude(force, settings.maxSteerForce);
    }
}
