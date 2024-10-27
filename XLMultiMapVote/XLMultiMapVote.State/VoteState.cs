using Photon.Pun;
using GameManagement;
using ModIO.UI;
using XLMultiMapVote.Data;

namespace XLMultiMapVote.State
{
	public class VoteState : GameState
	{
		public override void OnEnter(GameState prevState)
		{
            if (!PhotonNetwork.IsMasterClient)
            {
				MessageSystem.QueueMessage(MessageDisplayData.Type.Error, Labels.hostError, 2.5f);
				//GameStateMachine.Instance.RequestTransitionTo(GameStateMachine.Instance.LastState);
				RequestTransitionBack();
				return;
			}
			else if (Main.multiMapVote.isMapchanging)
            {
				MessageSystem.QueueMessage(MessageDisplayData.Type.Warning, Labels.voteInProgressError, 2.5f);
				//GameStateMachine.Instance.RequestTransitionTo(GameStateMachine.Instance.LastState);
                RequestTransitionBack();
                return;
			}

            Main.uiController.EnterVoteUI();

            //base.OnEnter(prevState);
        }

		public override void OnExit(GameState nextState)
		{
            Main.uiController.ExitVoteUI();

            //base.OnExit(nextState);
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
                //GameStateMachine.Instance.RequestPlayState();
                //GameStateMachine.Instance.RequestTransitionTo(GameStateMachine.Instance.LastState);
                RequestTransitionBack();
			}
			else if (RewiredInput.PrimaryPlayer.GetButtonDown("B"))
			{
				if (Main.uiController.uiDropDownList.IsExpanded || Main.uiController.filterMapsInput.isFocused)
				{
					return;
                }
				else
				{
                    RequestTransitionBack();
                }
            }
		}
	}
}
