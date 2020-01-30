﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SNSSTypes;
using UnityEngine.InputSystem;

/// <summary>
/// Handles switching from states
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool debug;

    [HideInInspector] public testShipController shipController;

    [SerializeField] private GameState gameState = GameState.NODE_TRANSITION;

    [SerializeField] private int seed;

    private System.Random random;

    /// <summary>
    /// Switch to provided state
    /// </summary>
    /// <param name="gameState">The game state to switch to</param>
    public void SwitchState(GameState gameState)
    {
        this.gameState = gameState;

        UpdateState();
    }

    /// <summary>
    /// Transition to new state
    /// </summary>
    private void UpdateState()
    {
        switch (gameState)
        {
            case GameState.BATTLE:
                break;
            case GameState.BATTLE_END:
                AreaManager.Instance.EndArea();
                break;
            case GameState.PAUSE:
                break;
            case GameState.NODE_SELECTION:
                shipController.StopThrust = true;
                NodeManager.Instance.BeginNodeSelection();
                break;
            case GameState.NODE_TRANSITION:
                shipController.StopThrust = false;
                AreaManager.Instance.LoadNewArea(NodeManager.Instance.CurrentNode.NodeInfo);
                break;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;

        shipController = GameObject.FindGameObjectWithTag("Player").GetComponent<testShipController>();

        random = new System.Random(seed);
    }

    private void Start()
    {
        UpdateState();
    }

    public GameState GameState { get { return gameState; } }

    public System.Random Random { get { return random; } }
}
