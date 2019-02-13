using UnityEngine;
using UnityEngine.AI;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class AICharacter : Character
    {
        protected NavMeshAgent agent;
        [Header("NavMesh")]
        [SerializeField] float radius = 0.3f;
        [SerializeField] float height = 1.7f;
        [SerializeField] float speed = 1f;
        [SerializeField] float acceleration = 100f;
        [SerializeField] float stoppingDistance = 1.2f;

        protected CharacterUI ui;
        private Viewer viewer;

        protected override void Awake() {
            base.Awake();

            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        protected override void Start() {
            base.Start();

            SetupNavMeshAgent();

            ui = GetComponentInChildren<CharacterUI>();
            DeactivateUI();
            viewer = Camera.main.GetComponent<Viewer>();
            viewer.onChangedFocus += DeactivateUI;
        }

        protected void MoveTowards(Vector3 destination) {
            agent.SetDestination(destination);

            bool arrivedAtTarget = (agent.remainingDistance <= agent.stoppingDistance);
            if (arrivedAtTarget) {
                movement.Move(Vector3.zero, false);
            }
            else {
                movement.Move(agent.desiredVelocity, false);
            }
        }

        private void SetupNavMeshAgent() {
            agent.radius = radius;
            agent.height = height;
            agent.speed = speed;
            agent.acceleration = acceleration;
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = true;
            agent.updatePosition = true;
        }

        public virtual void ActivateUI() {
            ui.gameObject.SetActive(true);
        }
        protected virtual void DeactivateUI() {
            ui.gameObject.SetActive(false);
        }

        private void OnDestroy() {
            viewer.onChangedFocus -= DeactivateUI;
        }
    }
}