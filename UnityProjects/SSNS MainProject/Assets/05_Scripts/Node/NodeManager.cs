using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;

/// <summary>
/// Handles switching from different nodes
/// </summary>
public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance;

    #region Constant Values

    private const float MAX_SELECT_TIME = 30f;
    private const float CONFIRM_TIME = 5f;
    private const float SELECT_DELAY = 0.5f;

    #endregion

    #region Serialized Variables

    [Space(5)]
    [SerializeField] private NodeManagerUI nodeUI;

    [Space(5)]
    [SerializeField] private GameObject portalPrefab;

    [SerializeField] private Node startNode;

    [SerializeField] private bool timedChoice = true;

    [SerializeField] private int maxPortalDistance = 350;

    [Header("Confirm Values")]
    [SerializeField] bool lockedP1;

    [SerializeField] bool lockedP2;

    #endregion

    #region Node Storage

    private Node currentNode;
    private Node lastNode;

    #endregion

    private System.Random random;

    #region Portal Stuff

    private GameObject[] portals;

    private bool rotateToPortal;

    #endregion

    #region Player Selection Variables

    private int selectedIndexP1;
    private int selectedIndexP2;

    private bool selectingNode;
    private bool nodeSelected;

    #endregion

    #region Time Variables

    private float timeBeforeSelection;

    private float timeSinceChangeP1;
    private float timeSinceChangeP2;

    #endregion

    // TODO: Add later
    #region Depth Selection

    //private Node[] depthNodes;

    //private int selectedDepth;

    //private bool mapPreview;

    #endregion

    /// <summary>
    /// Begins next selection for next node
    /// </summary>
    public void BeginNodeSelection()
    {
        if (Choices.Length == 0) // If there is no choices left then game is over
        {
            GameManager.Instance.SwitchState(GameState.GAME_END);
        }
        else
        {
            selectingNode = true;
            rotateToPortal = true;

            //selectedDepth = currentNode.Depth + 1;
            //depthNodes = FindNodes(selectedDepth);

            // Changes the time based on if there is more then 1 choice
            if (Choices.Length > 1)
            {
                timeBeforeSelection = MAX_SELECT_TIME;
            }
            else
            {
                timeBeforeSelection = CONFIRM_TIME;

                selectedIndexP1 = selectedIndexP2 = 0;
            }

            // Spawn the portals and update the information for the nodes
            SpawnPortals();

            // Selects the middle choice as default
            NodeUpdate(Choices.Length / 2);

            // Enable all node UI
            nodeUI.FadeGroups(true, true, true);
        }
    }

    /// <summary>
    /// Ends node selection and switches to node transition
    /// </summary>
    /// <param name="node">Next node to travel to</param>
    public void TravelToNode(Node node)
    {
        // Resets all boolean variables
        selectingNode = false;
        nodeSelected = false;
        lockedP1 = false;
        lockedP2 = false;

        // Stores the last node
        lastNode = currentNode;

        currentNode = node;

        // Destroys the portal parent
        if (portals != null)
        {
            Destroy(portals[0].transform.parent.gameObject);
        }

        lastNode.ResetColor();

        currentNode.ActiveColor();

        // Hides all UI except the gunner
        nodeUI.FadeGroups(false, false, true);


        // Switches state to node transition to load next area
        GameManager.Instance.SwitchState(GameState.NODE_TRANSITION);
    }

    /// <summary>
    /// Hides all UI that is tied to nodes
    /// </summary>
    public void HideAllNodeUI()
    {
        // Disable all node UI
        nodeUI.FadeGroups(false, false, false);

        // Resets node color
        if (lastNode != null)
            lastNode.ResetColor();
    }

    #region Player Selection

    /// <summary>
    /// Select the node in the provided direction for the player role
    /// </summary>
    /// <param name="role">The player's role</param>
    /// <param name="direction">The direction of the selection</param>
    public void SelectNodeChoice(PlayerRole role, int direction)
    {
        if (selectingNode)
        {
            direction = Mathf.Clamp(direction, -1, 1);

            // Checks if the player hasn't confirmed for each role
            if ((role == PlayerRole.Pilot && timeSinceChangeP1 == 0 && !lockedP1) || (role == PlayerRole.Gunner && timeSinceChangeP2 == 0 && !lockedP2))
            {
                NodeUpdate(role, selectedIndexP2 + direction);
            }
        }
    }

    /// <summary>
    /// Select the depth
    /// </summary>
    /// <param name="direction">The direction of the selection</param>
    //public void SelectNodeDepth(int direction)
    //{
    //    if (selectingNode && mapPreview)
    //    {
    //        direction = Mathf.Clamp(direction, -1, 1);

    //        if (GameManager.Instance.debug)
    //        {
    //            selectedDepth = Mathf.Clamp(selectedDepth + direction, startNode.Depth, startNode.MaxDepth);
    //        }
    //        else
    //        {
    //            selectedDepth = Mathf.Clamp(selectedDepth + direction, currentNode.Depth, startNode.MaxDepth);
    //        }

    //        if (selectedDepth == currentNode.Depth + 1)
    //            depthNodes = null;
    //        else
    //            depthNodes = FindNodes(selectedDepth);
    //    }
    //}

    /// <summary>
    /// Locks node choice for the player role
    /// </summary>
    /// <param name="role">The player's role</param>
    /// <param name="locked">Whether to confirm choice</param>
    public void LockChoice(PlayerRole role, bool locked)
    {
        // Checks if both player's haven't confirmed
        if (selectingNode && !(lockedP1 && lockedP2))
        {
            if (role == PlayerRole.Pilot)
            {
                lockedP1 = locked;
            }
            else if (role == PlayerRole.Gunner)
            {
                if (GameManager.Instance.debug)
                {
                }
                else
                    lockedP2 = locked;
            }
        }
    }

    /// <summary>
    /// Updates the information for the node for the provided role
    /// </summary>
    /// <param name="role">The role to update</param>
    /// <param name="newNodeIndex">The node to select</param>
    private void NodeUpdate(PlayerRole role, int newNodeIndex)
    {
        if (role == PlayerRole.Pilot)
        {
            newNodeIndex = Mathf.Clamp(newNodeIndex, 0, Choices.Length - 1);

            timeSinceChangeP1 = SELECT_DELAY;
            selectedIndexP1 = newNodeIndex;
            rotateToPortal = true;

            // Updates the node UI for the pilot
            #region UI 

            NodeInfo selectedNode = Choices[selectedIndexP1].NodeInfo;

            NodeInfo leftNode = null;
            NodeInfo rightNode = null;

            int leftNodeP1 = selectedIndexP1 - 1;
            int rightNodeP1 = selectedIndexP1 + 1;

            if (leftNodeP1 >= 0)
                leftNode = Choices[leftNodeP1].NodeInfo;

            if (rightNodeP1 < Choices.Length)
                rightNode = Choices[rightNodeP1].NodeInfo;

            nodeUI.UpdatePilot(selectedNode, leftNode, rightNode);

            #endregion
        }
        else if (role == PlayerRole.Gunner)
        {
            timeSinceChangeP2 = SELECT_DELAY;
            Choices[selectedIndexP2].ResetColor();
            selectedIndexP2 = newNodeIndex;

            // Updates the node UI for the gunner
            #region UI

            currentNode.ActiveColor();

            Choices[selectedIndexP2].DestinationColor();

            nodeUI.UpdateGunner(Choices[selectedIndexP2].NodeInfo, currentNode.NodeInfo);

            #endregion
        }
    }

    /// <summary>
    /// Updates the information for the node for both players
    /// </summary>
    /// <param name="newNodeIndex">The node to select</param>
    private void NodeUpdate(int newNodeIndex)
    {
        NodeUpdate(PlayerRole.Pilot, newNodeIndex);
        NodeUpdate(PlayerRole.Gunner, newNodeIndex);
    }

    #endregion

    #region Node Depth Finder

    /// <summary>
    /// Find all nodes at depth
    /// </summary>
    /// <param name="depth">The depth to find nodes at</param>
    /// <returns></returns>
    public Node[] FindNodes(int depth)
    {
        List<Node> nodes = new List<Node>();

        FindNodes(ref nodes, depth, startNode);

        return nodes.ToArray();
    }

    /// <summary>
    /// Recursion method for finding nodes at depth
    /// </summary>
    /// <param name="nodes">Stores all nodes at depth</param>
    /// <param name="depth">The depth to find it at</param>
    /// <param name="node">Current node</param>
    private void FindNodes(ref List<Node> nodes, int depth, Node node)
    {
        if (node == startNode)
            nodes.Clear();

        if (!nodes.Contains(node))
        {
            if (node == null) return;

            if (node.Depth == depth)
            {
                nodes.Add(node);
            }
            else
            {
                foreach (Node n in node.Children)
                {
                    FindNodes(ref nodes, depth, n);
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// Spawns portals in-front of the player
    /// </summary>
    private void SpawnPortals()
    {
        portals = new GameObject[Choices.Length];

        // Create a parent for the portals
        Transform parent = new GameObject("Portals").transform;

        // Grabs the ship transform
        Transform ship = GameManager.Instance.shipController.transform;

        // Vector the ship's forward direction
        Vector3 forward = ship.forward * 300;

        // Finds the far left side of the ship for placing the portals
        Vector3 farLeft = -ship.right * maxPortalDistance / 2.0f;

        // Loops through all choices and creates portals
        for (int i = 0; i < Choices.Length; i++)
        {
            // Creates portal
            GameObject portal = Instantiate<GameObject>(portalPrefab, parent);
            portal.name = "[Portal] Choice: " + (i + 1);

            Vector3 position = ship.right;

            if (Choices.Length == 1)
                position *= DistanceBetweenPortals;
            else
                position *= DistanceBetweenPortals * i;

            // Sets the portal position
            portal.transform.position = ship.position + forward + farLeft + position;

            portals[i] = portal;
        }
    }

    /// <summary>
    /// Rotates the player ship to the portal
    /// TODO: This probably should be handled in the player controller
    /// </summary>
    private void RotateToPortal()
    {
        if (portals == null || !rotateToPortal) return;

        // Finds the direction to the portal
        Vector3 portalDir = (portals[selectedIndexP1].transform.position - GameManager.Instance.shipController.transform.position).normalized;

        // Finds the angle between
        float angle = Vector3.SignedAngle(GameManager.Instance.shipController.transform.forward, portalDir, GameManager.Instance.shipController.transform.up);

        Vector2 dir = Vector2.zero;

        // Find a direction based on the angle
        if (Mathf.Abs(angle) > 0.5f)
        {
            float t = Mathf.Abs(angle) / DistanceBetweenPortals;

            dir = new Vector2(Mathf.Lerp(0.3f, 3f, t), 0);

            if (angle < 0)
            {
                dir *= Vector2.left;
            }
        }
        else
        {
            rotateToPortal = false;
        }

        // Steers the ship to the direction
        GameManager.Instance.shipController.SteerShip(dir);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;

        timeBeforeSelection = MAX_SELECT_TIME;

        currentNode = startNode;
        lastNode = null;
    }

    private void Start()
    {
        random = GameManager.Instance.Random;

        // Generates all nodes
        foreach (Node n in startNode)
        {
            n.GenerateNode();
        }

        AreaManager.Instance.AreaLoaded += HideAllNodeUI;
    }

    private void Update()
    {
        if (selectingNode)
        {
            if (timedChoice || timeBeforeSelection <= CONFIRM_TIME)
                timeBeforeSelection -= Time.deltaTime;

            // If timer hits 0 select node based on player selections
            if (timeBeforeSelection <= 0) 
            {
                timeBeforeSelection = 0;

                int selectedIndex = selectedIndexP1;

                if (selectedIndexP1 != selectedIndexP2)
                {
                    switch (random.Next(2))
                    {
                        case 1:
                            selectedIndex = selectedIndexP2;
                            break;
                    }
                }

                NodeUpdate(selectedIndex);

                selectingNode = false;
                nodeSelected = true;

            }
            // If timer is below confirm time and neither have choosen randomize player choices
            else if (timeBeforeSelection <= CONFIRM_TIME)
            {
                if (!lockedP1 || !lockedP2)
                {
                    lockedP1 = true;
                    lockedP2 = true;

                    // Updates the selected nodes with random node choice
                    NodeUpdate(PlayerRole.Pilot, random.Next(Choices.Length));
                    NodeUpdate(PlayerRole.Gunner, random.Next(Choices.Length));
                }
            }
            // If both players have confirmed lower time to confirm time
            else if (lockedP1 && lockedP2)
            {
                timeBeforeSelection = CONFIRM_TIME;
            }

            timeSinceChangeP1 -= Time.deltaTime;
            if (timeSinceChangeP1 <= 0)
            {
                timeSinceChangeP1 = 0;
            }

            timeSinceChangeP2 -= Time.deltaTime;
            if (timeSinceChangeP2 <= 0)
            {
                timeSinceChangeP2 = 0;
            }

            // Update selection UI
            nodeUI.UpdateTimer((int)timeBeforeSelection);
            nodeUI.UpdateConfirm(lockedP1, lockedP2);
        }

        // Rotates to portal
        RotateToPortal();

        // Once the pilot has stopped moving and a node is selected ends node selection
        if (nodeSelected && !rotateToPortal)
        {
            nodeSelected = false;
            TravelToNode(Choices[selectedIndexP1]);
        }
    }

    public Node[] Choices { get { return currentNode.Children; } }
    public Node StartNode { get { return startNode; } }
    public Node CurrentNode { get { return currentNode; } }

    //public bool MapPreview { get { return mapPreview; } set { mapPreview = value; } }

    private float DistanceBetweenPortals { get { return maxPortalDistance / (Choices.Length == 1 ? 2 : (Choices.Length - 1)); } }
}
