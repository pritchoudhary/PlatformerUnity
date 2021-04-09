using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class Node : PrefabPlacer
{
    #region Prefab Options
    //If creating new prefab deletes the instantiated prefab
    public bool addPrefab = false;
    //Lock prefab axis
    public bool xLock, zLock;
    public colliderMenu shape = colliderMenu.Box;
    #endregion

    #region Variables
    public enum colliderMenu
    {
        Box,
        Sphere,
    }

    //parent prefab will be spawned here
    public GameObject prefabParent;
    public bool isChild = false;
    #endregion

    //Draw sphere and cube in inspector
    private void OnDrawGizmos()
    {
        if(!isChild)
        {
            Gizmos.color = new Color(.5f, 1.0f, 1.0f, .5f);
            switch(shape)
            {
                case colliderMenu.Box:
                    Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.localRotation, transform.localScale);
                    Gizmos.DrawCube(Vector3.zero, Vector3.one);

                    Gizmos.color = new Color(0, 0, 0, .75f);
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    Gizmos.matrix = Matrix4x4.identity;
                    break;

                case colliderMenu.Sphere:
                    Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.localRotation, transform.localScale);
                    Gizmos.DrawSphere(Vector3.zero, 1);

                    Gizmos.color = new Color(0, 0, 0, .75f);
                    Gizmos.DrawWireSphere(Vector3.zero, 1);
                    Gizmos.matrix = Matrix4x4.identity;
                    break;
            }
        }
    }

    //Quick delete of all prefabs
    public void DeletePrefabs()
    {
        DestroyImmediate(prefabParent);
    }

    //Spawn prefabs in the node assigned
    public void StartPrefabSpawn(bool child)
    {
        isChild = child;

        if (!addPrefab && !isChild)
            DeletePrefabs();

        if (!prefabParent && !isChild)
            prefabParent = new GameObject("PrefabParent - " + gameObject.name);

        CheckNegativeScale();

        if(prefabList.Count != 0)
        {
            try
            {
                SpawnPrefabs(child);
            }                
            catch(System.Exception exc)
            {
                Debug.LogErrorFormat("Exception in starting prefab creation. Message: {0}", exc.Message);
            }                
           
        }
        else
        {
            Debug.LogWarning("Node has no prefabs in the list");
            return;
        }
    }

    //Check negative scale of a node and revert it if true
    void CheckNegativeScale()
    {
        Vector3 temp = transform.localScale;

        if (transform.localScale.x < 0)
            temp.x = System.Math.Abs(temp.x);

        if (transform.localScale.y < 0)
            temp.y = System.Math.Abs(temp.y);

        if (transform.localScale.z < 0)
            temp.z = System.Math.Abs(temp.z);

        transform.localScale = temp;
    }

    //Instantiate Prefabs
    void SpawnPrefabs(bool child)
    {
        if (numberOfPrefabsToSpawn < 1)
            numberOfPrefabsToSpawn = 1;

        Vector3 spawnPos = new Vector3();
        float posMultiplier = 1;

        for(int i =0; i<numberOfPrefabsToSpawn;++i)
        {
            GetSpawnPosition(ref spawnPos, ref posMultiplier);

            if (xLock)
                spawnPos.x = 0;
            if (zLock)
                spawnPos.z = 0;

            GameObject parent = child ? gameObject : prefabParent;
            float distance = child ? distanceBetPrefab : 1;

            if(isChild)
            {
                Vector3 temp = spawnPos;
                temp.y += 2;
                spawnPos = temp;
            }
            IntantiatePrefabs(spawnPos, posMultiplier, distance, parent.transform);
        }
    }

    void GetSpawnPosition(ref Vector3 spawnPos, ref float posMult)
    {
        switch(shape)
        {
            case colliderMenu.Box:
                spawnPos = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f));
                posMult = .45f;
                break;

            case colliderMenu.Sphere:
                spawnPos = Random.insideUnitSphere;
                spawnPos.y = 1;
                posMult = .95f;
                break;
        }
    }   
}
#endif
