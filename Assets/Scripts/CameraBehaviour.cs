using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Top down camera that would follow the player with an offset at all times
public class CameraBehaviour : MonoBehaviour
{
    //Variables    
    public Vector3 targetOffset;
    public float moveSpeed = 1.5f;
    private Transform playerTarget;
    private Transform cameraTransform;

    private void Awake()
    {
        playerTarget = FindObjectOfType<PlayerScript>().transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = transform;
    }
    
    //Using fixed update to smoothen the camera movement and hence using fixedDeltaTime instead of deltaTime
    void FixedUpdate()
    {
        //Check if there's are target to follow and then transform camera position to the player
        if(playerTarget != null)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, playerTarget.position + targetOffset, moveSpeed*Time.fixedDeltaTime);
        }
        else
        {
            Debug.LogError("Attach Player object to the camera");
        }
    }

    public void SetPlayerTarget(Transform playerTransform)
    {
        playerTarget = playerTransform;
    }
}
