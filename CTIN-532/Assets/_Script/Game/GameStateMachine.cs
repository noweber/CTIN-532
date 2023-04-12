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

        private Dictionary<GameState, GameObject> gameObjects;

        [SerializeField]
        public GameState CurrentState { get; private set; }

        void Awake()
        {
            gameStates = new();
            gameStates.Add(GameState.Menu, MenuState);
            gameStates.Add(GameState.Credits, CreditsState);
            gameStates.Add(GameState.District, DistrictState);
            gameObjects = new();
            gameObjects.Add(GameState.Menu, MenuState.gameObject);
            gameObjects.Add(GameState.Credits, CreditsState.gameObject);
            gameObjects.Add(GameState.District, DistrictState.gameObject);
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

            if (exitCurrentState && gameStates.ContainsKey(CurrentState))
            {
                gameStates[CurrentState].OnExit();
                gameObjects[CurrentState].SetActive(false);
            }

            CurrentState = nextState;
            gameStates[CurrentState].OnEnter();
            gameObjects[CurrentState].SetActive(true);
        }
    }
}
