using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Level Set")]
public class LevelSet : ScriptableObject
{
    [SerializeField] private string[] LevelNames;

    private static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    private string GetNextLevelName()
    {
        var currentLevelIndex = Array.IndexOf(LevelNames, GetCurrentSceneName());
        if (currentLevelIndex < 0)
        {
            return null;
        }

        if (currentLevelIndex >= LevelNames.Length - 1)
        {
            return null;
        }

        return LevelNames[currentLevelIndex + 1];
    }

    public void LoadNextLevel()
    {
        var nextLevelName = GetNextLevelName();
        if (nextLevelName != null)
        {
            SceneManager.LoadScene(nextLevelName);
        }
    }
}