using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float playerSpeed;
    private Rigidbody rigidbody;
    public bool isPlayerAlive;
    public float boost = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        isPlayerAlive = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        rigidbody.velocity = new Vector3((horizontal * playerSpeed), 0.0f, (vertical * playerSpeed));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactive"))
        {
            other.gameObject.SetActive(false);
            playerSpeed += boost;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("EndPoint"))
        {
            FindObjectOfType<GameManager>().EndGame();
        }
            
    }
}
