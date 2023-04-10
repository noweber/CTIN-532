using Assets._Script.Districts;
using Assets._Script.Game;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Script
{
    public enum DistrictState
    {
        Start,
        Play,
        Victory,
        Defeat,
    }

    public class DistrictStateMachine : MonoBehaviour
    {
        public DistrictStartState StartState;

        public DistrictPlayState PlayState;

        public DistrictVictoryState VictoryState;

        public DistrictDefeatState DefeatState;

        private Dictionary<DistrictState, IGameState> states;

        public DistrictState CurrentState { get; private set; }

        private void Awake()
        {
            states = new();
            states.Add(DistrictState.Start, StartState);
            states.Add(DistrictState.Play, PlayState);
            states.Add(DistrictState.Victory, VictoryState);
            states.Add(DistrictState.Defeat, DefeatState);
        }

        public void ChangeState(DistrictState nextState, bool exitCurrentState = true)
        {
            if (!states.ContainsKey(nextState))
            {
                Debug.LogError("Next district state is invalid.");
                return;
            }

            if (exitCurrentState && states.ContainsKey(CurrentState))
            {
                states[CurrentState].OnExit();
            }

            CurrentState = nextState;
            states[CurrentState].OnEnter();
        }
    }
}
