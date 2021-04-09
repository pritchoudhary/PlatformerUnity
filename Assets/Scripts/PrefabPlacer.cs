using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
[ExecuteInEditMode]
public class PrefabPlacer : MonoBehaviour
{
    //Prefab variable options
    #region Prefab Data
    public bool debug = false;
    //Allow prefabs to spawn inside each other
    public bool overlapPrefabs = false;
    //Normal of the surface
    public bool surfaceNormal = false;
    public float maxSurfaceAngle = 45f;
    //Layer to be added for the prefabs to instantiate in the scene
    public LayerMask prefabLayerMask;
    public int numberOfPrefabsToSpawn = 1;
    public float distanceBetPrefab = 1f;

    //Get the mesh size to create casts
    public bool isMesh = false;
    public bool posOffset;

    #region Position/Transform variables
    //Range of each rotational value of the prefab
    public Vector2 xRot, yRot, zRot;
    public Vector3 rotationOverRide;

    //Scake randomiser;
    public Vector2 randomScaler;
    public Vector3 scalerOverRide;
    #endregion

    #region List Of Prefabs
    public List<GameObject> prefabList;

    //Values assigned to each prefab so as to populate the highest value more
    public List<float> prefabValues;
    #endregion

    GameObject prefabSpawnInstance;
    #endregion


    //Instantiating objects
    protected void IntantiatePrefabs(Vector3 location, float nodeMultiplier, float distance, Transform parentObject)
    {
        #region Prefab SetUp
        //Get Random Prefabs
        int spawnRandomPrefabs = RandomObject();

        //Gets Local transform into world space and modiy it
        Vector3 newLocation = transform.TransformPoint(location * nodeMultiplier * distance);
        prefabSpawnInstance = Instantiate(prefabList[spawnRandomPrefabs], new Vector3(), Quaternion.identity) as GameObject;

        //Modify prefabs values
        SetPrefabRotation();
        SetPrefabScale();

        prefabSpawnInstance.transform.parent = parentObject;
        prefabSpawnInstance.name = prefabList[spawnRandomPrefabs].name;
        #endregion

        #region Rigidbody and Mesh Info
        //get collider info
        Mesh mesh = prefabSpawnInstance.GetComponent<MeshFilter>().sharedMesh;
        Rigidbody rigidbody = prefabSpawnInstance.GetComponent<Rigidbody>();
        #endregion

        #region Raaycast Info
        RaycastHit hit;
        bool cast;

        //Use RbSweep is there is a rigidbody and no overlap
        if (rigidbody != null && !overlapPrefabs)
        {
            if (debug)
                Debug.Log("Using rigidbody Sweep");

            //Move the prefab object so raycast can happen
            prefabSpawnInstance.transform.position = newLocation;
            cast = rigidbody.SweepTest(-transform.up, out hit, Mathf.Infinity);
        }

        //if overlap exists, spherecast
        else if (mesh != null)
        {
            if (debug)
                Debug.Log("Casting is happening");

            //Modify size of spherecast to that of the largest mesh 
            float spherSize;

            if (isMesh)
            {
                if (mesh.bounds.extents.x > mesh.bounds.extents.z)
                    spherSize = mesh.bounds.extents.x;
                else
                    spherSize = mesh.bounds.extents.z;
            }
            else
            {
                if (prefabSpawnInstance.transform.lossyScale.x > prefabSpawnInstance.transform.lossyScale.z)
                    spherSize = prefabSpawnInstance.transform.lossyScale.x;
                else
                    spherSize = prefabSpawnInstance.transform.lossyScale.z;
            }

            if (overlapPrefabs)
            {
                //ignore the chosen layermask if exists
                cast = Physics.SphereCast(newLocation, spherSize, -transform.up, out hit, Mathf.Infinity, ~prefabLayerMask);

            }
            else
                cast = Physics.SphereCast(newLocation, spherSize, -transform.up, out hit, Mathf.Infinity);
        }
        else
        {
            Debug.LogWarning("could not find a rigidbody or a mesh filter on: " + prefabSpawnInstance.name);
            return;
        }
        #endregion

        #region Move the Prefab to the raycast point
        //if raycast doesn't hit anything, break
        if(cast)
        {
            float angle = Vector3.Angle(-transform.up, hit.normal);
            //Calculates if an object will spawn depending on the angle of the collider below it
            if(angle < (180 - maxSurfaceAngle))
            {
                if (debug)
                    Debug.Log("Angle is sharper than " + maxSurfaceAngle + ". Object " + prefabSpawnInstance + " not instantiated");
                DestroyImmediate(prefabSpawnInstance);
                return;
            }
            //condition for checking if collider hits the hit point
            if((prefabLayerMask.value & (1 << hit.collider.gameObject.layer)) != 0 && !overlapPrefabs)
            {
                if (debug)
                    Debug.Log("Prefab with selected layer mask is in the way. Object " + prefabSpawnInstance.name + " not isntantiated");
                DestroyImmediate(prefabSpawnInstance);
                return;
            }

            if (posOffset)
            {
                if (isMesh)
                {
                    if (mesh == null)
                    {
                        if (debug)
                            Debug.LogWarning("Cannot use mesh scaling without a meshFilter. Swappng to gloabl transform scaling.");
                        isMesh = false;
                    }
                    else
                        prefabSpawnInstance.transform.position = hit.point + (hit.normal * (mesh.bounds.extents.y));
                }
                else
                    prefabSpawnInstance.transform.position = hit.point + (hit.normal * (prefabSpawnInstance.transform.lossyScale.y * .5f));
            }
            else
                prefabSpawnInstance.transform.position = hit.point;

            if (surfaceNormal)
                prefabSpawnInstance.transform.rotation = Quaternion.FromToRotation(prefabSpawnInstance.transform.up, hit.normal);
        }
        else
        {
            if (debug)
                Debug.Log("Collider not foind in bounds of node. Object " + prefabSpawnInstance.name + " Not instantiated");
            DestroyImmediate(prefabSpawnInstance);
            return;
        }
        #endregion

        #region Start Child nodes
        //if the child has a prefab child script on it, then instantiate more
        Node child = prefabSpawnInstance.GetComponent<Node>();
        if (child != null)
            child.StartPrefabSpawn(true);
        #endregion
    }

    //Setting the rotation of the spawned prefabs
    void SetPrefabRotation()
    {
        Vector3 newRot = new Vector3(0, 0, 0);

        //Override user input if present
        if(rotationOverRide != Vector3.zero)
        {
            //x - use random value if no override
            if (rotationOverRide.x == 0 && xRot != Vector2.zero)
                newRot.x = Random.Range(xRot.x, xRot.y);
            else if (rotationOverRide.x != 0)
                newRot.x = rotationOverRide.x;

            //y
            if (rotationOverRide.y == 0 && yRot != Vector2.zero)
                newRot.y = Random.Range(yRot.x, yRot.y);
            else if (rotationOverRide.y != 0)
                newRot.y = rotationOverRide.y;

            //z
            if (rotationOverRide.y == 0 && zRot != Vector2.zero)
                newRot.y = Random.Range(zRot.x, zRot.y);
            else if (rotationOverRide.z != 0)
                newRot.z = rotationOverRide.z;
        }
        else
        {
            //if override is 0, use random bools
            if (xRot != Vector2.zero)
                newRot.x = Random.Range(xRot.x, xRot.y);

            if (yRot != Vector2.zero)
                newRot.x = Random.Range(yRot.x, yRot.y);

            if (zRot != Vector2.zero)
                newRot.x = Random.Range(zRot.x, zRot.y);
        }
        prefabSpawnInstance.transform.rotation = Quaternion.Euler(newRot);
    }

    //Setting the scale of the spawned prefabs
    void SetPrefabScale()
    {
        Vector3 newScale = new Vector3(0, 0, 0);
        Vector3 currScale = prefabSpawnInstance.transform.localScale;

        if(scalerOverRide == Vector3.zero && randomScaler != Vector2.zero)
        {
            float random = Random.Range(randomScaler.x, randomScaler.y);
            newScale = currScale * random;
        }
        else
        {
            if (scalerOverRide.x != 0)
                newScale.x = scalerOverRide.x;
            else
                newScale.x = currScale.x;

            if (scalerOverRide.y != 0)
                newScale.y = scalerOverRide.y;
            else
                newScale.y = currScale.y;

            if (scalerOverRide.z != 0)
                newScale.x = scalerOverRide.z;
            else
                newScale.z = currScale.z;
        }
    }

    //Generate a random number and return the index of the gameobject
    int RandomObject()
    {
        float currCount = 0, totalWeight = 0;

        foreach (float weight in prefabValues)
            totalWeight += weight;

        float random = Random.Range(0, totalWeight);

        for(int i=0;i<prefabValues.Count;++i)
        {
            currCount += prefabValues[i];
            if (currCount > random)
                return i;
        }
        Debug.LogWarning("Function shouldn't have returned here. ");
        //return a random prefab count
        return Random.Range(0, prefabList.Count - 1);
    }
    
}
#endif