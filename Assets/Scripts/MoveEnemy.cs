using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
    //Variables
    public float enemyMoveSpeed = 2f;
    public float enemyObstacleDetect = 4f;
    
    // Update is called once per frame
    void Update()
    {
        //Move game object forward regardless of turns
        transform.Translate(0, 0, enemyMoveSpeed * Time.deltaTime);

        //Ray at object position that points in the direction of that object
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;
        //Raycasting with a circumfrence around the ray 
        if(Physics.SphereCast(ray,.75f,out hit))
        {
            if(hit.distance < enemyObstacleDetect)
            {
                //If close to obstacle, turn in a random direction and continue moving
                float angle = Random.Range(-110, 110);
                transform.Rotate(0, angle, 0);
            }
        }
    }
}
