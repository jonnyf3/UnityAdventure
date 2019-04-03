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
        private Weapon currentWeapon;
        public float AttackRange => currentWeapon.range;

        private float lastAttackTime;
        private float attackRandomnessFactor;
        private float AttackPeriod {
            get {
                if ((GetComponent<Character>() as Player)) { return currentWeapon.AnimClip.length; }
                return (1f / attacksPerSecond) * attackRandomnessFactor;
            }
        }

        private const string ANIMATOR_ATTACK_PARAM = "onAttack";

        private void Awake() {
            weapons = GetComponent<WeaponSystem>();
            weapons.onChangedWeapon += (newWeapon) => currentWeapon = newWeapon;
        }

        void Start() {
            lastAttackTime = 0;
            attackRandomnessFactor = Random.Range(0.6f, 1.4f);
        }

        public void Attack(Transform target = null) {
            if (Time.time - lastAttackTime >= AttackPeriod) {
                DoAttackAnimation();
                GetComponentInChildren<WeaponBehaviour>().Attack(target);

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