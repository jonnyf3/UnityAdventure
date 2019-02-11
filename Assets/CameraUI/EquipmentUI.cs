using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Weapons;
using RPG.Magic;

namespace RPG.CameraUI
{
    public class EquipmentUI : MonoBehaviour
    {
        [SerializeField] Image weaponIcon = null;
        [SerializeField] Image magicIcon  = null;
        
        //The inital delegate events from PlayerCombat may be missed on Start so sprites would not be set on Start
        void Awake() {
            var player = FindObjectOfType<Player>();
            Assert.IsNotNull(player, "Could not find player in the scene!");

            player.GetComponent<PlayerCombat>().onChangedWeapon += OnChangedWeapon;
            player.GetComponent<SpecialCombat>().onChangedMagic += OnChangedMagic;
        }

        void OnChangedWeapon(Weapon weapon) {
            weaponIcon.sprite = weapon.Sprite;
        }
        void OnChangedMagic(MagicData ability) {
            magicIcon.sprite = ability.Sprite;
        }
    }
}