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
    
}
