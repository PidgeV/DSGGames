using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiserEnemyAttacking : StateMachineBehaviour
{
	private CruiserEnemy _cruiserEnemy;
	private Transform _transform;
	private Transform _player;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_transform = animator.gameObject.transform;
		_cruiserEnemy = animator.gameObject.GetComponent<CruiserEnemy>();
		_player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_cruiserEnemy.Move(_player.transform.position);

		if (Vector3.Distance(_transform.position, _player.position) > _cruiserEnemy.EscapeRange)
		{
			animator.SetTrigger("Patrolling");
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

	}
}
