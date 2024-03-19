using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using GameManagement;
using Rewired;

namespace XLMultiMapVote.Data
{
	public class VoteState : GameState
	{
		public override void OnEnter(GameState prevState)
		{
			Main.uiController.EnterVoteUI();
		}

		public override void OnExit(GameState nextState)
		{
			Main.uiController.ExitVoteUI();
		}

		public override void OnUpdate()
		{
			CheckForInput();
		}

		private void CheckForInput()
        {
			if (RewiredInput.PrimaryPlayer.GetButtonDown("B"))
			{
				GameStateMachine.Instance.RequestTransitionTo(GameStateMachine.Instance.LastState);

				/*
				GameStateMachine.Instance.RequestPlayState();

                if (GameStateMachine.Instance.CurrentState.GetType() != typeof(PlayState))
                {
					GameStateMachine.Instance.RequestTransitionTo(typeof(PlayState));
                }
				*/
			}
		}
	}
}
