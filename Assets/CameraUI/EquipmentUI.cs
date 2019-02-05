using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using RPG.Characters;
using RPG.Weapons;

namespace RPG.CameraUI
{
    public class EquipmentUI : MonoBehaviour
    {
        [SerializeField] Image weaponIcon;
        [SerializeField] Image magicIcon;
        
        // Start is called before the first frame update
        void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            Assert.IsNotNull(player, "Could not find player in the scene!");
            var playerCombat = player.GetComponent<PlayerCombat>();
            playerCombat.onChangedWeapon += OnChangedWeapon;
            playerCombat.onChangedMagic += OnChangedMagic;
        }

        void OnChangedWeapon(Weapon weapon) {
            weaponIcon.sprite = weapon.Sprite;
        }

        void OnChangedMagic(MagicData ability) {
            magicIcon.sprite = ability.Sprite;
        }
    }
}