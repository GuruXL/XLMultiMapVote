using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using GameManagement;
using Rewired;
using System.Collections;
using ModIO.UI;

namespace XLMultiMapVote.Data
{
	public class VoteState : GameState
	{
		public override void OnEnter(GameState prevState)
		{
            if (!PhotonNetwork.IsMasterClient)
            {
				MessageSystem.QueueMessage(MessageDisplayData.Type.Error, "Only room host can set up voting", 2.5f);
				GameStateMachine.Instance.RequestTransitionTo(GameStateMachine.Instance.LastState);
				return;
			}
			else if (Main.multiMapVote.isMapchanging)
            {
				MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, "Please wait until current Vote is Complete", 2.5f);
				GameStateMachine.Instance.RequestTransitionTo(GameStateMachine.Instance.LastState);
				return;
			}
			//base.OnEnter(prevState);
			//Time.timeScale = 0f;
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
