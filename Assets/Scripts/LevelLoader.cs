using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    // Method to load a level by name
    public void LoadLevel(string levelName)
    {
        // Ensure the scene is in the Build Settings
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {

            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogError($"Level '{levelName}' not found in Build Settings!");
        }
    }

    // Method to quit the game
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
#else
            Application.Quit(); // Quit the game in a build
#endif
    }
}
