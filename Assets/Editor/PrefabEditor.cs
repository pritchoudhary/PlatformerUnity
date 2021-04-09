using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class PrefabEditor : MonoBehaviour
{
    static private GameObject node;

    [MenuItem("PrefabPlacer/CreateNode")]
    static public void CreateNode()
    {
        //get prefab path
        node = AssetDatabase.LoadAssetAtPath("Assets/PrefabPlacer/PrefabNode.prefab", typeof(GameObject)) as GameObject;
        Object clone = Instantiate(node, Vector3.zero, Quaternion.identity);
        clone.name = node.name;
    }

    public class LayerMasker : Editor
    {
        //Allows the editor to display all the layermasks present
        public static LayerMask LayerMaskField(string label, string tootTip, LayerMask layerMask)
        {
            List<int> layerNums = new List<int>();
            var layers = InternalEditorUtility.layers;

            layerNums.Clear();

            for (int i = 0; i < layers.Length; i++)
                layerNums.Add(LayerMask.NameToLayer(layers[i]));

            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNums.Count; i++)
            {
                if (((1 << layerNums[i]) & layerMask.value) > 0)
                    maskWithoutEmpty |= (1 << i);
            }

            maskWithoutEmpty = EditorGUILayout.MaskField(new GUIContent(label, tootTip), maskWithoutEmpty, layers);

            int mask = 0;
            for (int i = 0; i < layerNums.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << 1)) > 0)
                    mask |= (1 << layerNums[i]);
            }
            layerMask.value = mask;
            return layerMask;
        }
    }
    

    [CustomEditor(typeof(Node)),CanEditMultipleObjects]
    public class NodeInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            #region Buttons
            Node nodeScript = (Node)target;
            EditorGUILayout.Separator();

            if(!nodeScript.isChild)
            {
                if (GUILayout.Button("Spawn Prefabs"))
                    nodeScript.StartPrefabSpawn(false);

                EditorGUILayout.Separator();

                if (GUILayout.Button("Delete Prefabs"))
                    nodeScript.DeletePrefabs();
            }
            #endregion

            #region Base Variables
            EditorGUILayout.Separator();

            nodeScript.debug = EditorGUILayout.Toggle(new GUIContent("Enable Debug Mode", "Toggles PrefbaPlacer DebugLogs \nWARNING! Slow performance while enabled"), nodeScript.debug);

            if (!nodeScript.isChild)
                nodeScript.shape = (Node.colliderMenu)EditorGUILayout.EnumPopup(new GUIContent("Shape of prefab node"), nodeScript.shape);
            nodeScript.prefabLayerMask = LayerMasker.LayerMaskField("Prefab Mask","Selected layermasks will be ignored by prefab placers raycast",nodeScript.prefabLayerMask);
            nodeScript.numberOfPrefabsToSpawn = EditorGUILayout.IntField("Number of prefabs to spawn", nodeScript.numberOfPrefabsToSpawn);
            nodeScript.distanceBetPrefab = EditorGUILayout.FloatField(new GUIContent("Distance from parent"), nodeScript.distanceBetPrefab);

            EditorGUILayout.Separator();
            #endregion

            #region Prefab Lists
            //Initialise lists if they havent already been
            if (nodeScript.prefabList == null)
                nodeScript.prefabList = new List<GameObject>();
            if (nodeScript.prefabValues == null)
                nodeScript.prefabValues = new List<float>();

            serializedObject.Update();
            SerializedProperty prefabList = serializedObject.FindProperty("prefabList");
            EditorGUILayout.PropertyField(prefabList);
            SerializedProperty size = prefabList.FindPropertyRelative("Array.size");
            EditorGUILayout.PropertyField(size);
            serializedObject.ApplyModifiedProperties();

            //keep prefab value list the same size as prefab list
            while (nodeScript.prefabValues.Count < nodeScript.prefabList.Count)
                nodeScript.prefabValues.Add(1f);
            while (nodeScript.prefabValues.Count > nodeScript.prefabList.Count)
                nodeScript.prefabValues.RemoveAt(nodeScript.prefabValues.Count - 1);

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            SerializedProperty valueList = serializedObject.FindProperty("prefabValues");

            if (size.hasMultipleDifferentValues)
                EditorGUILayout.HelpBox("not showing lists with different sizes", MessageType.Warning);

            if(prefabList.isExpanded && !size.hasMultipleDifferentValues)
            {
                for(int i = 0; i<prefabList.arraySize;++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(prefabList.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(150));
                    EditorGUILayout.LabelField("Prefab Value", GUILayout.Width(60));
                    EditorGUILayout.PropertyField(valueList.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(30));
                    EditorGUILayout.EndHorizontal();
                }
            }
            serializedObject.ApplyModifiedProperties();
            #endregion

            #region Prefab Variables
            EditorGUILayout.Separator();

            nodeScript.overlapPrefabs = EditorGUILayout.Toggle(new GUIContent("Prefab Overlap", "Allows prefabs to spawn inside another object"),nodeScript.overlapPrefabs);

            if (!nodeScript.isChild)
                nodeScript.addPrefab = EditorGUILayout.Toggle(new GUIContent("Add prefabs on prefabs node only"), nodeScript.addPrefab);
            nodeScript.surfaceNormal = EditorGUILayout.Toggle(new GUIContent("Rotate prefab to normal"), nodeScript.surfaceNormal);
            nodeScript.maxSurfaceAngle = EditorGUILayout.Slider(new GUIContent("Surface angle max in degrees"), nodeScript.maxSurfaceAngle,0,89);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            #endregion

            #region Position and Transform Variables
            nodeScript.xLock = EditorGUILayout.Toggle(new GUIContent("Lock X Axis"), nodeScript.xLock);
            nodeScript.zLock = EditorGUILayout.Toggle(new GUIContent("Lock Z Axus"), nodeScript.zLock);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            nodeScript.posOffset = EditorGUILayout.Toggle(new GUIContent("Position offset"), nodeScript.posOffset);
            if (nodeScript.posOffset)
                nodeScript.isMesh = EditorGUILayout.Toggle(new GUIContent("Use mesh scaling"), nodeScript.isMesh);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            #region Rotation Data
            if(nodeScript.rotationOverRide == Vector3.zero)
            {
                EditorGUILayout.LabelField(new GUIContent("Random rotation, prefab will randomly rotate between min and max range"));

                //X Axis
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("X Rotation", GUILayout.Width(80));
                EditorGUILayout.LabelField("Min Value", GUILayout.Width(30));
                nodeScript.xRot.x = EditorGUILayout.FloatField(nodeScript.xRot.x, GUILayout.Width(50));
                EditorGUILayout.LabelField("Max Value", GUILayout.Width(30));
                nodeScript.xRot.y = EditorGUILayout.FloatField(nodeScript.xRot.y, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();

                //Y Axis
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Y Rotation", GUILayout.Width(80));
                EditorGUILayout.LabelField("Min Value", GUILayout.Width(30));
                nodeScript.yRot.x = EditorGUILayout.FloatField(nodeScript.yRot.x, GUILayout.Width(50));
                EditorGUILayout.LabelField("Max Value", GUILayout.Width(30));
                nodeScript.yRot.y = EditorGUILayout.FloatField(nodeScript.yRot.y, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();

                //Z Axis
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Z Rotation", GUILayout.Width(80));
                EditorGUILayout.LabelField("Min Value", GUILayout.Width(30));
                nodeScript.zRot.x = EditorGUILayout.FloatField(nodeScript.zRot.x, GUILayout.Width(50));
                EditorGUILayout.LabelField("Max Value", GUILayout.Width(30));
                nodeScript.zRot.y = EditorGUILayout.FloatField(nodeScript.zRot.y, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Separator();
            }
            nodeScript.rotationOverRide = EditorGUILayout.Vector3Field(new GUIContent("Rotation Overide"), nodeScript.rotationOverRide);
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            #endregion

            #region Scaling Data
            if(nodeScript.scalerOverRide == Vector3.zero)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Scale prefabs bet min an max value"), GUILayout.Width(90));
                EditorGUILayout.LabelField("Min Scale", GUILayout.Width(30));
                nodeScript.randomScaler.x = EditorGUILayout.FloatField(nodeScript.randomScaler.x);
                EditorGUILayout.LabelField("Max Scale", GUILayout.Width(30));
                nodeScript.randomScaler.y= EditorGUILayout.FloatField(nodeScript.randomScaler.y);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();

            }
            #endregion

            #endregion
            serializedObject.ApplyModifiedProperties();
            //base.OnInspectorGUI();
        }
    }

}
