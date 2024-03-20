using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using GameManagement;
using Rewired;
using System.Collections;

namespace XLMultiMapVote.Data
{
	public class VoteState : GameState
	{
		public override void OnEnter(GameState prevState)
		{
			//base.OnEnter(prevState);

			Main.uiController.EnterVoteUI();
			EventSystem.current.SetSelectedGameObject(null);
			EventSystem.current.firstSelectedGameObject = Main.uiController.uiDropDownList.gameObject;
			EventSystem.current.SetSelectedGameObject(Main.uiController.uiDropDownList.gameObject);
		}

		public override void OnExit(GameState nextState)
		{
			//base.OnExit(nextState);

			Main.uiController.ExitVoteUI();
			EventSystem.current.SetSelectedGameObject(null);
		}

		public override void OnUpdate()
		{
			CheckForInput();
		}

		private void CheckForInput()
        {
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
