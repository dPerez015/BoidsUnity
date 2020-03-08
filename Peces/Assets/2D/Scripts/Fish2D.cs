using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish2D : MonoBehaviour
{
    [HideInInspector]
    public Fish2DSettings settings;

    public Vector3 forward;
    Vector3 gizmoPos;

    Vector3 force;
    Vector3 velocity;
    float speed;

   [HideInInspector]
    public int id;

    [HideInInspector]
    public Vector3 avoidanceDirection;
    public Vector3 flockCenter;
    public Vector3 flockDirection;
    public int numFlockMates;

#if UNITY_EDITOR
    public bool drawGizmo=false;
#endif
    /*
     * Places the fish at a random position and orientation. Also sets the velocity
     * */
    public void Init()
    {
        float rotation = Random.Range(-0.0f, 360.0f);
        forward = Quaternion.Euler(0, rotation, 0) * -Vector3.right;
        transform.right = forward;
        speed = (settings.minSpeed + settings.maxSpeed) / 2;
        velocity = forward * speed;

        transform.Translate(new Vector3(Random.Range(-10.0f, 10.0f), 0.0f, Random.Range(-5.0f, 5.0f)),Space.World);
        
        GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.blue, new Color(0.6f,0.6f,0.9f,1.0f), Random.Range(0.0f, 1.0f));

        speed = (settings.minSpeed + settings.maxSpeed)/ 2.0f;

     }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        gizmoPos = transform.position + (forward * 0.7f);
        //Gizmos.DrawLine(transform.position, gizmoPos);
        //Radius of the fish sense of sight
        Gizmos.DrawWireSphere(transform.position, settings.sightRadius);

        
        if (numFlockMates>0)
        {
            Gizmos.color = Color.green;
            //desired direction to avoid other flock mates
            Gizmos.DrawLine(transform.position, transform.position + avoidanceDirection.normalized);
        }
        Gizmos.color = Color.yellow;
        if (force!=Vector3.zero)
            //force direction to steer towards the disired direction
            Gizmos.DrawLine(transform.position, transform.position+force.normalized);
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
            force += settings.avoidanceWeight*SteerForceTowards(avoidanceDirection);

            //go to the same direction as other fishes on the flock
            flockDirection /= numFlockMates;
            force += settings.alignWeight * SteerForceTowards(flockDirection);

            //stay close to the center of the flock
            flockCenter /= numFlockMates;
            flockCenter = flockCenter - transform.position;
            force += settings.cohesionWeight * SteerForceTowards(flockCenter);

            //calculate new velocity and speed;
            velocity += force * Time.deltaTime;
            speed = Mathf.Clamp(velocity.magnitude, settings.minSpeed, settings.maxSpeed);
            forward = velocity.normalized;
            velocity = speed * forward;
        }
        //movement
        transform.position = transform.position + forward * speed * Time.deltaTime;
        //rotation
        transform.right = forward;

        //position swap on screen edge
        if (transform.position.x < -10 || transform.position.x > 10)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x * -1,-10,10), transform.position.y, transform.position.z);
        }
        if (transform.position.z < -5 || transform.position.z > 5)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z * -1, -5, 5));
        }
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
