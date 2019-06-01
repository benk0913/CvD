using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CVDFerrTerrainCollider : Editor
{
    const string LOG_TAG = "Fit Collider To Terrain - ";
    const float OFFSET_Y = 0.12f;
    const float OFFSET_X = 0;

    [MenuItem("CvD/Fit Collider To Terrain")]
    static void FitCollider()
    {
        if(Selection.activeObject == null)
        {
            Debug.LogError(LOG_TAG+"NOTHING SELECTED");
            return;
        }

        PolygonCollider2D pCollider = Selection.activeGameObject.GetComponent<PolygonCollider2D>();

        if (pCollider == null)
        {
            Debug.LogError(LOG_TAG + "NO PolygonCollider2D ON SELECTION");
            return;
        }

        Ferr2DT_PathTerrain terrain = pCollider.transform.parent.GetComponent<Ferr2DT_PathTerrain>();

        if (terrain == null)
        {
            Debug.LogError(LOG_TAG + "PARENT OF COLLIDER IS NOT A Ferr2DT_PathTerrain");
            return;
        }

        Vector2[] arrayPoints = terrain.PathData.GetPoints(0).ToArray();

        pCollider.points = arrayPoints;

        pCollider.offset = new Vector2(OFFSET_X, OFFSET_Y);
    }

}