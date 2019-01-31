using UnityEngine;
using RPG.Weapons;

namespace RPG.Characters
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] AnimatorOverrideController animOverride;
        private Animator animator = null;

        [SerializeField] Weapon currentWeapon;
        [SerializeField] Transform weaponHand;

        // Start is called before the first frame update
        void Start() {
            var weapon = Instantiate(currentWeapon.WeaponPrefab, weaponHand);
            weapon.transform.localPosition = currentWeapon.Grip.position;
            weapon.transform.localRotation = currentWeapon.Grip.rotation;

            animator = GetComponentInChildren<Animator>();
            SetAttackAnimation();
        }

        private void SetAttackAnimation() {
            animator.runtimeAnimatorController = animOverride;
            animOverride["DEFAULT ATTACK"] = currentWeapon.AnimClip;
        }

        public void Attack() {
            animator.SetTrigger("Attack");
        }
    }
}