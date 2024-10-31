using Photon.Pun;
using GameManagement;
using ModIO.UI;
using XLMultiMapVote.Data;
using XLMultiMapVote.Map;

namespace XLMultiMapVote.State
{
	public class PopUpState : GameState
	{
        
		public override void OnEnter(GameState prevState)
		{
          
        }
		public override void OnExit(GameState nextState)
		{
           
        }
		public override void OnUpdate()
		{

		}


		public void SetAvailableTransitions()
		{
			// Initialize the transitions array with the desired states directly
			GameState[] transitions = new GameState[]
			{
				GameStateMachine.Instance.PlayObject.GetComponent<GameState>(),
				GameStateMachine.Instance.PauseObject.GetComponent<GameState>(),
				GameStateMachine.Instance.MultiplayerMenuObject.GetComponent<GameState>()
			};

			availableTransitions = transitions;
		}
	}
}
