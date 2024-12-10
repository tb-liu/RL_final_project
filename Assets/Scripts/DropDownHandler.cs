using UnityEngine;
using TMPro;
using Unity.MLAgents.Policies;
using Unity.Sentis;

public class ModelDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown modelDropdown;         // Reference to the dropdown menu
    public BehaviorParameters agentBehavior; // Reference to the agent's BehaviorParameters
    public ModelAsset[] models;              // Array of NNModel assets

    void Start()
    {
        // Add a listener to handle dropdown value changes
        modelDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Set the first model as the default
        if (models.Length > 0 && agentBehavior != null)
        {
            agentBehavior.Model = models[0];
        }
    }

    // Method to handle dropdown selection changes
    public void OnDropdownValueChanged(int index)
    {
        if (index >= 0 && index < models.Length)
        {
            // Assign the selected model to the agent
            agentBehavior.Model = models[index];
            Debug.Log($"Switched to model: {models[index].name}");
        }
        else
        {
            Debug.LogError("Invalid model index selected!");
        }
    }
}
