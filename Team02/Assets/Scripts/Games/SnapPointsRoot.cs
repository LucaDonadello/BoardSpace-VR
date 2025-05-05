using UnityEngine;

public class ChessboardSnapManager : MonoBehaviour
{
    public int gridSize = 10;
    public float cellSize = 0.06f;
    public Vector3 originOffset; //Offset from board center to corner
    public Transform[,] gridPoints;

    void Awake()
    {
        gridPoints = new Transform[gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                GameObject snapPoint = new GameObject($"Snap_{x}_{z}");
                snapPoint.transform.parent = transform;
                snapPoint.transform.localPosition = originOffset + new Vector3(
                                                    (x + 0.5f) * cellSize, 0.01f, 
                                                    (z + 0.5f) * cellSize);
                gridPoints[x, z] = snapPoint.transform;
            }
        }
    }

    public Transform GetClosestSnapPoint(Vector3 worldPosition)
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (Transform t in gridPoints)
        {
            float dist = Vector3.Distance(worldPosition, t.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = t;
            }
        }

        return closest;
    }

    //draw spheres on snap points in scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (gridPoints == null) return;

        foreach (Transform point in gridPoints)
        {
            if (point != null)
                Gizmos.DrawSphere(point.position, 0.01f); // adjust radius as needed
        }
    }

}
