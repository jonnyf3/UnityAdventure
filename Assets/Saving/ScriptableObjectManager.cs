using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Actions;

namespace RPG.Saving
{
    public class ScriptableObjectManager : MonoBehaviour
    {
        /* This object should contain references to all ScriptableObjects used
          in the game, so that other objects can serialize/save them by name and
          restore/load them via lookup to this persistent object */

        [Header("Weapons")]
        [SerializeField] List<Weapon> weapons;
        public Weapon GetWeapon(string name) {
            return weapons.Find((w) => w.name == name);
        }

        [Header("Special Abilities")]
        [SerializeField] List<AbilityData> abilities;
        public AbilityData GetAbility(string name) {
            return abilities.Find((a) => a.name == name);
        }
    }
}