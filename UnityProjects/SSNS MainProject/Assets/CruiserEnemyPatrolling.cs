using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiserEnemyPatrolling : StateMachineBehaviour
{
	private CruiserEnemy _cruiserEnemy;
	private Transform _transform;
	private Transform _player;

	private List<Vector3> _waypoints;

	private int _targetPointIndex = 0;

	private bool _changingState;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_transform = animator.gameObject.transform;
		_player = GameObject.FindGameObjectWithTag("Player").transform;
		_cruiserEnemy = _transform.GetComponent<CruiserEnemy>();
		_waypoints = _cruiserEnemy.Waypoints;
		_targetPointIndex = 0;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (Vector3.Distance(_waypoints[_targetPointIndex], _cruiserEnemy.gameObject.transform.position) < _cruiserEnemy.TargetDistanceToPoint)
		{
			_targetPointIndex = (_targetPointIndex + 1) % (_waypoints.Count - 1);
		}
		else
		{
			_cruiserEnemy.Move(_waypoints[_targetPointIndex]);
		}

		if (Vector3.Distance(_transform.position, _player.position) < _cruiserEnemy.AggroRange)
		{
			animator.SetTrigger("Attacking");
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

	}
}
