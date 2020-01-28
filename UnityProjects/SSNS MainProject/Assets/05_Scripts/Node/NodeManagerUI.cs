using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SNSSTypes;
using System.Reflection;

/// <summary>
/// Struct that manages all node UI elements
/// </summary>
[System.Serializable]
public struct NodeManagerUI
{
    // Structs that contain UI elements
    [SerializeField] private SelectionInfoUI selectionUI;
    [SerializeField] private PilotNodeUI pilotNodeUI;
    [SerializeField] private GunnerNodeUI gunnerNodeUI;

    /// <summary>
    /// Update the pilot's node information
    /// </summary>
    /// <param name="selectedNode">The pilot's currently selected node</param>
    /// <param name="leftNode">The node that is on the left side</param>
    /// <param name="rightNode">The node that is on the right side</param>
    public void UpdatePilot(NodeInfo selectedNode, NodeInfo leftNode, NodeInfo rightNode)
    {
        #region Left + Right Node Info

        // Hides the left or right node info if there is none
        pilotNodeUI.leftNodeName.gameObject.SetActive(leftNode != null);
        pilotNodeUI.rightNodeName.gameObject.SetActive(rightNode != null);

        // Updates the left and right node info
        if (leftNode != null)
            pilotNodeUI.leftNodeName.text = leftNode.name;

        if (rightNode != null)
            pilotNodeUI.rightNodeName.text = rightNode.name;

        #endregion

        // Updates the selected node info
        #region Node Selection Info

        pilotNodeUI.nodeName.text = selectedNode.name;
        pilotNodeUI.nodeType.text = "Type: " + System.Enum.GetName(typeof(NodeType), selectedNode.type);
        
        if (selectedNode.reward)
            pilotNodeUI.nodeReward.text = selectedNode.reward.rewardName;

        #endregion
    }

    /// <summary>
    /// Update the gunner's node information
    /// </summary>
    /// <param name="selectedNode">The gunner's currently selected node</param>
    /// <param name="currentNode">The current node that the ship is on</param>
    public void UpdateGunner(NodeInfo selectedNode, NodeInfo currentNode)
    {
        // Updates the current node info
        #region Current Node

        gunnerNodeUI.c_nodeName.text = currentNode.name;
        gunnerNodeUI.c_nodeType.text = "Type: " + System.Enum.GetName(typeof(NodeType), currentNode.type);

        if (currentNode.reward)
            gunnerNodeUI.c_nodeReward.text = selectedNode.reward.rewardName;

        #endregion

        // Updates the selected node info
        #region Selected Node

        gunnerNodeUI.s_nodeName.text = selectedNode.name;
        gunnerNodeUI.s_nodeType.text = "Type: " + System.Enum.GetName(typeof(NodeType), selectedNode.type);

        if (selectedNode.reward)
            gunnerNodeUI.s_nodeReward.text = selectedNode.reward.rewardName;

        #endregion
    }

    /// <summary>
    /// Updates the selection timer
    /// </summary>
    /// <param name="time">The time that the selection is at</param>
    public void UpdateTimer(int time)
    {
        selectionUI.confirmTime.text = time.ToString("00"); // Formats it to double digits
    }

    /// <summary>
    /// Updates the confirm icons
    /// --TODO
    /// - Probably will be changed as for now just changes the text color
    /// </summary>
    /// <param name="confirm1">Whether the pilot confirmed</param>
    /// <param name="confirm2">Whether the gunner confirmed</param>
    public void UpdateConfirm(bool confirm1, bool confirm2)
    {
        if (confirm1)
        {
            selectionUI.pilotConfirm.color = Color.green;
        }
        else
        {
            selectionUI.pilotConfirm.color = Color.red;
        }

        if (confirm2)
        {
            selectionUI.gunnerConfirm.color = Color.green;
        }
        else
        {
            selectionUI.gunnerConfirm.color = Color.red;
        }
    }

    /// <summary>
    /// Shows certain UI elements
    /// </summary>
    /// <param name="selection">Whether to show the selection UI</param>
    /// <param name="pilot">Whether to show the pilot UI</param>
    /// <param name="gunner">Whether to show the gunner UI</param>
    public void FadeGroups(bool selection, bool pilot, bool gunner)
    {
        selectionUI.groupFade.FadeActive(selection);
        pilotNodeUI.groupFade.FadeActive(pilot);
        gunnerNodeUI.groupFade.FadeActive(gunner);
    }
}

/// <summary>
/// Struct for containing the selection info UI
/// </summary>
[System.Serializable]
public struct SelectionInfoUI
{
    public Fade groupFade;

    public Text confirmTime;
    public Text pilotConfirm;
    public Text gunnerConfirm;
}

/// <summary>
/// Struct for containing the pilot node UI
/// </summary>
[System.Serializable]
public struct PilotNodeUI
{
    public Fade groupFade;

    [Header("Selected Node UI")]
    public Text nodeName;
    public Text nodeType;
    public Text nodeDifficulty;
    public Text nodeReward;

    [Header("Other Nodes")]
    public Text leftNodeName;
    public Text rightNodeName;
}

/// <summary>
/// Struct for containing the gunner node UI
/// </summary>
[System.Serializable]
public struct GunnerNodeUI
{
    public Fade groupFade;

    [Header("Current Node UI")]
    public Text c_nodeName;
    public Text c_nodeType;
    public Text c_nodeDifficulty;
    public Text c_nodeReward;

    [Header("Selected Node UI")]
    public Text s_nodeName;
    public Text s_nodeType;
    public Text s_nodeDifficulty;
    public Text s_nodeReward;
}
