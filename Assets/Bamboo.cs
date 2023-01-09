using System.Collections.Generic;
using UnityEngine;

public class Bamboo : MonoBehaviour
{
    [Header("Parameters")] 
    public float NewSegmentStartsAfterTime;
    public AnimationCurve HeightOverTime;
    public AnimationCurve RadiusOverTime;
    public int MaxSubSegments;
    public float MaxAngle;
    public float MinRandomTimeScale;
    public float MaxRandomTimeScale;
    public AnimationCurve LeafScaleOverSegmentIndex;
    public AnimationCurve LeafScaleOverSegmentLifetime;
    public AnimationCurve BambooGrowthSpeedOverLifetime;
    public int MaxSegments;

    [Header("Prefabs")] 
    public GameObject BambooSegmentOld;
    public GameObject BambooSegmentYoung;
    public GameObject BambooLeaf;

    private readonly List<BambooSegment> _segments = new (20);
    private float _bambooCreated;

    private void Start()
    {
        _bambooCreated = Time.time;
        AddNewSegmentFromParent(0, null);   
    }
    
    private void Update()
    {
        var bambooLifetime = Time.time - _bambooCreated;
        var growthSpeed = BambooGrowthSpeedOverLifetime.Evaluate(bambooLifetime);
        
        for (var i = 0; i < _segments.Count; i++)
        {
            var segment = _segments[i];
            UpdateSegment(i, segment, growthSpeed);
        }
    }

    private void AddNewSegmentFromParent(int parentIndexFromBottom, BambooSegment parent)
    {
        var segmentObject = Instantiate(BambooSegmentYoung, transform);
        var initialRotation = new Vector3(
            Random.Range(-MaxAngle, MaxAngle),    
            0,
            Random.Range(-MaxAngle, MaxAngle)
        );
        var leaf = Instantiate(BambooLeaf, Vector3.zero, Quaternion.Euler(0, Random.Range(-180, 180), 0));
        leaf.transform.SetParent(segmentObject.transform, false);
        
        segmentObject.transform.Rotate(initialRotation);

        var segment = new BambooSegment
        {
            Parent = parent,
            Transform = segmentObject.transform,
            GrowProgress = 0f,
            Started = Time.time,
            TimeScale = Random.Range(MinRandomTimeScale, MaxRandomTimeScale),
            LeafTransform = leaf.transform
        };
        UpdateSegment(parentIndexFromBottom + 1, segment, 1f);
        _segments.Add(segment);
    }

    private void UpdateSegment(int segmentIndexFromBottom, BambooSegment segment, float bambooGrowthSpeed)
    {
        // Positioning
        var pivotPosition = Vector3.zero;
        if (segment.Parent != null)
            pivotPosition = SegmentEndPosition(segment.Parent);
        segment.Transform.localPosition = pivotPosition;

        var lifetime = (Time.time - segment.Started) * segment.TimeScale; // * bambooGrowthSpeed; ???

        // Size growth
        var segmentHeight = HeightOverTime.Evaluate(lifetime);
        var segmentRadius = RadiusOverTime.Evaluate(lifetime);
        segment.Transform.localScale = new Vector3(segmentRadius, segmentHeight, segmentRadius);
        
        // Leaf growth
        var leafSize = LeafScaleOverSegmentIndex.Evaluate(segmentIndexFromBottom) *
                       LeafScaleOverSegmentLifetime.Evaluate(lifetime);
        segment.LeafTransform.localScale = new Vector3(leafSize, leafSize, leafSize);

        // Spawn segment if needed
        if (segment.NSubSegments < MaxSubSegments && lifetime > NewSegmentStartsAfterTime && _segments.Count < MaxSegments)
        {
            if (segment.NSubSegments == 0)
            {
                var replacementObject = Instantiate(
                    BambooSegmentOld, 
                    segment.Transform.position,
                    segment.Transform.rotation, 
                    transform);
                replacementObject.transform.localScale = segment.Transform.localScale;
                segment.LeafTransform.SetParent(replacementObject.transform);
                
                Destroy(segment.Transform.gameObject);
                segment.Transform = replacementObject.transform;
            }
            
            segment.NSubSegments++;
            AddNewSegmentFromParent(segmentIndexFromBottom + 1, segment);
        }
    }

    private Vector3 SegmentEndPosition(BambooSegment segment)
    {
        var topPosition = segment.Transform.TransformPoint(Vector3.up);
        return transform.InverseTransformPoint(topPosition);
    }

    public void CutAt(int segmentIndex)
    {
        for (var i = _segments.Count - 1; i >= segmentIndex; i--)
        {
            var segment = _segments[i];
            segment.Transform.SetParent(null);
            var segmentObject = segment.Transform.gameObject;
            segmentObject.AddComponent<CapsuleCollider>();
            segmentObject.AddComponent<Rigidbody>();
            
            //Destroy(segment.Transform.gameObject);
            _segments.RemoveAt(i);
        }
    }
}
