using MSA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MSA
{
    public class CharacterSoundEffectManager : MonoBehaviour
    {
        private AudioSource audioSource;

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayRollSoundFX()
        {
            audioSource.PlayOneShot(WorldSFXManager.instance.rollSFX);
        }
    }
}
