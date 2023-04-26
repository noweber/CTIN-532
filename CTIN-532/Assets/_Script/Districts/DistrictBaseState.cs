using Assets._Script.Game;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Script.Districts
{
    public abstract class DistrictBaseState : MonoBehaviour, IGameState
    {
        [SerializeField] protected List<GameObject> DistrictStateGameObjects;

        [SerializeField] protected AudioSource OnEnterSoundEffect;

        [SerializeField] protected AudioSource OnExitSoundEffect;

        public virtual void OnEnter()
        {
            SetDistrictStateGameObjectsActiveState(true);
            PlaySoundEffect(OnEnterSoundEffect);
        }

        public virtual void OnExit()
        {
            PlaySoundEffect(OnExitSoundEffect);
            SetDistrictStateGameObjectsActiveState(false);
        }

        protected virtual void SetDistrictStateGameObjectsActiveState(bool state)
        {
            if (DistrictStateGameObjects != null)
            {
                foreach (var gameObject in DistrictStateGameObjects)
                {
                    gameObject.SetActive(state);
                }
            }
        }

        protected virtual void PlaySoundEffect(AudioSource source)
        {
            if(source != null)
            {
                source.Play();
            }
        }
    }
}
