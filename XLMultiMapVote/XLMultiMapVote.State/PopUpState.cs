using Photon.Pun;
using GameManagement;
using ModIO.UI;
using XLMultiMapVote.Data;
using XLMultiMapVote.Map;

namespace XLMultiMapVote.State
{
	public class PopUpState : GameState
	{
		private GameState[] _availableTransitions;
		public new GameState[] availableTransitions
		{
			get
			{
				if (_availableTransitions == null)
				{
					_availableTransitions = new GameState[]
					{
					GameStateMachine.Instance.PlayObject.GetComponent<GameState>(),
					GameStateMachine.Instance.PauseObject.GetComponent<GameState>(),
					GameStateMachine.Instance.MultiplayerMenuObject.GetComponent<GameState>()
					};
				}
				return _availableTransitions;
			}
		}

		public override void OnEnter(GameState prevState)
		{
        }
		public override void OnExit(GameState nextState)
		{       
        }
		public override void OnUpdate()
		{
		}
	}
}
