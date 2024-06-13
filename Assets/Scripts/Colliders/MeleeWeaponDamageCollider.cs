using MSA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MSA
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")]
        public CharacterManager characterCausingDamage; //  (When calculating damage this is used to check for attackers damage modifiers, effects ect)
    }
}
