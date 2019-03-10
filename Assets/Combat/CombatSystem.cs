using UnityEngine;
using UnityEngine.Assertions;
using RPG.Characters;

namespace RPG.Combat
{
    [RequireComponent(typeof(WeaponSystem))]
    public class CombatSystem : MonoBehaviour
    {
        [SerializeField] float attacksPerSecond = 0.5f;

        private WeaponSystem weapons;
        private WeaponData currentWeapon;
        public float AttackRange => currentWeapon.AttackRange;

        private float lastAttackTime;
        private float attackRandomnessFactor;
        private float AttackPeriod {
            get {
                if ((GetComponent<Character>() as Player)) { return currentWeapon.AnimClip.length; }
                return (1f / attacksPerSecond) * attackRandomnessFactor;
            }
        }

        private const string ANIMATOR_ATTACK_PARAM = "onAttack";
        
        void Start() {
            weapons = GetComponent<WeaponSystem>();
            weapons.onChangedWeapon += (newWeapon) => currentWeapon = newWeapon;

            lastAttackTime = 0;
            attackRandomnessFactor = Random.Range(0.6f, 1.4f);
        }

        public void Attack() {
            if (Time.time - lastAttackTime >= AttackPeriod) {
                DoAttackAnimation();
                weapons.DoWeaponBehaviour();

                lastAttackTime = Time.time;
                attackRandomnessFactor = Random.Range(0.6f, 1.4f);
            }
        }

        private void DoAttackAnimation() {
            var animator = GetComponent<Animator>();
            var animOverride = animator.runtimeAnimatorController as AnimatorOverrideController;
            Assert.IsNotNull(animOverride, gameObject + " has no animator override controller to set custom animation!");
            animOverride["DEFAULT ATTACK"] = currentWeapon.AnimClip;

            animator.SetFloat("AttackSpeedMultiplier", currentWeapon.AnimationSpeed);
            animator.SetTrigger(ANIMATOR_ATTACK_PARAM);
        }
    }
}