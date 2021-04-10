using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectiles : MonoBehaviour
{
    public Rigidbody projectile;
    public Transform spawnPoint;    
    bool isShooting = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        //draw the ray for debuging purposes (will only show up in scene view)
        Debug.DrawRay(spawnPoint.transform.position, spawnPoint.transform.forward, Color.green);

        //cast a ray from the spawnpoint in the direction of its forward vector
        if (Physics.Raycast(spawnPoint.transform.position, spawnPoint.transform.forward, out hit, 1000))
        {
            if (!isShooting)
            {
                StartCoroutine(Shoot());
            }               

        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        GameObject bullet = Instantiate(Resources.Load("Bullet", typeof(GameObject))) as GameObject;
        //Get the bullet's rigid body component and set its position and rotation equal to that of the spawnPoint
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet.transform.rotation = spawnPoint.transform.rotation;
        bullet.transform.position = spawnPoint.transform.position;        

        //add force to the bullet in the direction of the spawnPoint's forward vector
        rb.AddForce(-spawnPoint.transform.forward * 1000f);

        //destroy the bullet after 1 second
        Destroy(bullet, 1);
        //wait for 1 second and set isShooting to false so we can shoot again
        yield return new WaitForSeconds(1f);
        isShooting = false;
    }
}
