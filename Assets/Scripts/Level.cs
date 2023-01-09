using System.Linq;
using TSUtils.Sounds;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private LevelSet LevelSet;
    [SerializeField] private SoundAsset CompleteSound;
    
    private Bamboo[] _bambooInstances;
    private Shuriken[] _shurikenInstances;
    private bool _levelCompleted;

    private void Start()
    {
        _levelCompleted = false;
        _bambooInstances = FindObjectsOfType<Bamboo>();
        _shurikenInstances = FindObjectsOfType<Shuriken>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            ResetAll();
            _levelCompleted = false;
        }

        if (!_levelCompleted)
        {
            if (AllShurikenOutOfBounds())
            {
                ResetAll();
            }
            if (AllBambooIsCut())
            {
                _levelCompleted = true;
                SoundManager.Instance.Play(CompleteSound);
                LevelSet.LoadNextLevel();
            }
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
        _levelCompleted = false;
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
