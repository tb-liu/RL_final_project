using UnityEngine;
using Unity.MLAgents.Policies;
using TMPro;
public class PlayMode : MonoBehaviour
{
    public BehaviorParameters behaviorParameters; // Reference to the agent's BehaviorParameters
    private bool isInHeuristicMode = false;       // Track the current mode
    public TMP_Text buttonText;
    public void SwitchMode()
    {
        if (behaviorParameters == null)
        {
            Debug.LogError("BehaviorParameters is not assigned!");
            return;
        }

        // Toggle between HeuristicOnly and InferenceOnly modes
        isInHeuristicMode = !isInHeuristicMode;
        // Update the button text
        buttonText.text = isInHeuristicMode ? "Never Mind" : "Let Me Play!!!";
        behaviorParameters.BehaviorType = isInHeuristicMode ? BehaviorType.HeuristicOnly : BehaviorType.InferenceOnly;

        Debug.Log($"Switched to {(isInHeuristicMode ? "Heuristic" : "Inference")} mode.");
    }

}
