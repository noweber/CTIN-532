using System.Collections.Generic;
using UnityEngine;

namespace Assets._Script.Game
{
    public enum GameState
    {
        Menu,
        Credits,
        District,
    }

    public class GameStateMachine : MonoBehaviour
    {
        public GameMenuState MenuState;

        public GameCreditsState CreditsState;

        public GameDistrictState DistrictState;

        private Dictionary<GameState, IGameState> gameStates;

        private GameState currentState;

        void Awake()
        {
            gameStates = new();
            gameStates.Add(GameState.Menu, MenuState);
            gameStates.Add(GameState.Credits, CreditsState);
            gameStates.Add(GameState.District, DistrictState);
        }

        private void Start()
        {
            ChangeState(GameState.Menu, false);
        }

        public void ChangeState(GameState nextState, bool exitCurrentState = true)
        {
            if (!gameStates.ContainsKey(nextState))
            {
                Debug.LogError("Next state is invalid.");
                return;
            }

            if (exitCurrentState && gameStates.ContainsKey(currentState))
            {
                gameStates[currentState].OnExit();
            }

            currentState = nextState;
            gameStates[currentState].OnEnter();
        }
    }
}
