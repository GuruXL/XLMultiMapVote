using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using GameManagement;
using Rewired;
using System.Collections;
using ModIO.UI;
using MapEditor;

namespace XLMultiMapVote.Data
{
	public class VoteState : GameState
	{
		public override void OnEnter(GameState prevState)
		{
            if (!PhotonNetwork.IsMasterClient)
            {
				MessageSystem.QueueMessage(MessageDisplayData.Type.Error, Labels.hostError, 2.5f);
				GameStateMachine.Instance.RequestTransitionTo(GameStateMachine.Instance.LastState);
				return;
			}
			else if (Main.multiMapVote.isMapchanging)
            {
				MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, Labels.voteInProgressError, 2.5f);
				GameStateMachine.Instance.RequestTransitionTo(GameStateMachine.Instance.LastState);
				return;
			}

            Main.uiController.EnterVoteUI();

            base.OnEnter(prevState);
        }

		public override void OnExit(GameState nextState)
		{
            Main.uiController.ExitVoteUI();

            base.OnExit(nextState);
        }

		public override void OnUpdate()
		{
			CheckForInput();
		}

		private void CheckForInput()
        {
            GameState currentState = GameStateMachine.Instance.CurrentState;
            if (currentState == null || !(currentState is VoteState))
                return;

			if (RewiredInput.PrimaryPlayer.GetButtonDown("Start"))
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
