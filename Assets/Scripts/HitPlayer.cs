using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPlayer : MonoBehaviour
{
    PlayerScript player;
   
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerScript>();   
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player") && player.isPlayerAlive)
        {
            collision.gameObject.SetActive(false);
            player.isPlayerAlive = false;
            FindObjectOfType<GameManager>().PlayerDead();
        }

    }

}
