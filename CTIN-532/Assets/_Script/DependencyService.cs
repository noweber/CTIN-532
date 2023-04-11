using Assets._Script.Districts;
using Assets._Script.Game;
using Assets._Script.Units;
using UnityEngine;

namespace Assets._Script
{
    public class DependencyService : Singleton<DependencyService>
    {
        [SerializeField]
        private GameStateMachine gameStateMachine;

        [SerializeField]
        private DistrictController districtController;

        [SerializeField]
        private DistrictStateMachine districtStateMachine;

        [SerializeField]
        private PlayerSelection playerSelection;

        public GameStateMachine GameFsm()
        {
            if (gameStateMachine == null)
            {
                gameStateMachine = FindObjectOfType<GameStateMachine>();
            }
            return gameStateMachine;
        }

        public DistrictStateMachine DistrictFsm()
        {
            if (districtStateMachine == null)
            {
                districtStateMachine = FindObjectOfType<DistrictStateMachine>();
            }
            return districtStateMachine;
        }

        public DistrictController DistrictController()
        {
            if (districtController == null)
            {
                districtController = FindObjectOfType<DistrictController>();
            }
            return districtController;
        }

        public PlayerSelection Player()
        {
            if (playerSelection == null)
            {
                playerSelection = FindObjectOfType<PlayerSelection>();
            }
            return playerSelection;
        }

        private GameManager gameManager;
        public GameManager Game()
        {
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }
            return gameManager;
        }
    }
}
