using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject BambooPrefab;
    public Ground Ground;
    public float CutRadius;

    private readonly Collider[] _overlaps = new Collider[10];

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        var currentPositionOnTheGround = GetCurrentPositionOnTheGround();
        
        if (Input.GetMouseButton(0))
        {
            Instantiate(BambooPrefab, currentPositionOnTheGround, Quaternion.identity);
        }

        if (Input.GetMouseButton(1))
        {
            CutBambooAtPosition(currentPositionOnTheGround);
        }
    }

    private Vector3 GetCurrentPositionOnTheGround()
    {
        return Ground.LastPointerWorldPosition;
    }

    private void CutBambooAtPosition(Vector3 currentPositionOnTheGround)
    {
        var nOverlaps = Physics.OverlapSphereNonAlloc(currentPositionOnTheGround, CutRadius, _overlaps);
        for (var i = 0; i < nOverlaps; i++)
        {
            var bamboo = _overlaps[i].GetComponent<Bamboo>();
            if (bamboo != null)
            {
                bamboo.CutAt(1, Vector3.zero, Vector3.zero);
            }
        }
    }
}
