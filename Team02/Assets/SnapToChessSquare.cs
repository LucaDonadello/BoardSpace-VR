using UnityEngine;

public class SnapToChessSquare : MonoBehaviour
{
    private ChessboardSnapManager snapManager;

    void Start()
    {
        snapManager = GetComponentInParent<ChessboardSnapManager>();
    }

    public void SnapToClosest()
    {
        if (snapManager == null) return;

        Transform snapPoint = snapManager.GetClosestSnapPoint(transform.position);
        if (snapPoint != null)
        {
            transform.position = snapPoint.position;
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }
    }
}
