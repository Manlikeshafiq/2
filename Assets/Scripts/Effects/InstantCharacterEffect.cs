using MSA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MSA
{
    public class InstantCharacterEffect : ScriptableObject
    {
        [Header("Effect ID")]
        public int instantEffectID;

        public virtual void ProcessEffect(CharacterManager character)
        {

        }
    }
}
