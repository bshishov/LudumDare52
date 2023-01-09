using System.Linq;
using UnityEngine;

public class Level : MonoBehaviour
{
    private Bamboo[] _bambooInstances;
    private Shuriken[] _shurikenInstances;

    private void Start()
    {
        _bambooInstances = FindObjectsOfType<Bamboo>();
        _shurikenInstances = FindObjectsOfType<Shuriken>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            ResetAll();
        }

        if (AllShurikenOutOfBounds())
        {
            ResetAll();
        }

        if (AllBambooIsCut())
        {
            ResetAll();
        }
    }

    private bool AllShurikenOutOfBounds()
    {
        for (var index = 0; index < _shurikenInstances.Length; index++)
        {
            var shuriken = _shurikenInstances[index];
            if (shuriken.State != ShurikenState.OutOfBounds) 
                return false;
        }

        return true;
    }
    
    private bool AllBambooIsCut()
    {
        return _bambooInstances.All(bamboo => bamboo.IsCut);
    }

    private void ResetAll()
    {
        foreach (var bamboo in _bambooInstances)
        {
            bamboo.ResetToStart();
        }
            
        foreach (var shuriken in _shurikenInstances)
        {
            shuriken.ResetToStart();
        }
    }
}
