using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
public class BackToMenu : MonoBehaviour
{
    public Agent[] agents;
    public void GoToMainMenu()
    {
        // Load the Main Menu scene
        SceneManager.LoadScene("MainMenu"); 
    }

    public void ReloadLevel()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Restart episodes for all agents
        foreach (var agent in agents)
        {
            if (agent != null)
            {
                agent.EndEpisode();
            }
        }
    }
}