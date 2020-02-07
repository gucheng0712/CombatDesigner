using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace CombatDesigner
{
    /// <summary>
    /// ActorModel is a ScriptableObject Class that represents a character's core data
    /// </summary>
    [CreateAssetMenu(fileName = "NewActorModel", menuName = "ACT-System/Model")]
    public class ActorModel : ScriptableObject
    {
        #region Editor Fields
        /*================ Editor VFX ===============*/
#if UNITY_EDITOR
        [HideInInspector] public GameObject vfxGO;
        [HideInInspector] public bool e_playVFX;
        [HideInInspector] public float e_vfxInitialTime;
        [HideInInspector] public float e_currentStateTime;
#endif

        #endregion

        #region Public Fields

        /// <summary>
        ///  Actor control Type
        /// </summary>
        [HideInInspector] public ActorType actorType = ActorType.PLAYER;

        /// <summary>
        /// Layer mask for checking if is groundable
        /// </summary>
        [HideInInspector] public LayerMask groundLayer;


        /*================ Components ===============*/

        /// <summary>
        ///  Character GameObject
        /// </summary>
        [Tooltip("Character GameObject")]
        public GameObject character;
        
        /// <summary>
        /// Character Animator Component
        /// </summary>
        [HideInInspector] public Animator anim;
        /// <summary>
        /// CharacterController Component
        /// </summary>
        [HideInInspector] public CharacterController cc;
        /// <summary>
        /// Character AudioSource Component
        /// </summary>
        [HideInInspector] public AudioSource audioSource;


        /// <summary>
        /// Cinemachine GameObject
        /// </summary>
        [HideInInspector] public GameObject cmGO;

        /// <summary>
        /// The current target GameObject
        /// </summary>
        [HideInInspector] public GameObject target;

        /*================ Actor Status ===============*/
        /// <summary>
        ///  a Collection of ChainBehavior
        /// </summary>
        [HideInInspector]public List<ChainBehavior> chainBehaviors;

        /// <summary>
        /// The current chain behavior index in the current behavior
        /// </summary>
        public int currentChainIndex { get; set; }

        /// <summary>
        /// Character Stats Object
        /// </summary>
        [HideInInspector] public ActorStats actorStats;

        [HideInInspector] public int energy; // todo move to ActorStats Class

        /// <summary>
        /// Character Finite State Machine Object
        /// </summary>
        [HideInInspector] public ActorFSM fsm;

        /// <summary>
        /// Is character dead
        /// </summary>
        [HideInInspector] public bool isDead { get; set; }

        /// <summary>
        /// Is character in the air
        /// </summary>
        [HideInInspector] public bool isAerial { get; set; }

        /// <summary>
        /// Has lock-on target?
        /// </summary>
        [HideInInspector] public bool hasLockdeTarget { get; set; }

        /// <summary>
        /// A collection of behaviors that character has
        /// </summary>
        [Tooltip("All behaviors of the current character model")]
        public List<ActorBehavior> behaviors;

        /// <summary>
        /// The Current Behavior
        /// </summary>
        public ActorBehavior currentBehavior { get; set; }

        /// <summary>
        /// Current frame of current behavior
        /// </summary>
        public float currentFrame { get; set; }

        /// <summary>
        /// Previous frame of current behavior
        /// </summary>
        public int previousFrame { get; set; }

        /// <summary>
        /// The character's local time scale
        /// </summary>
        public float objectTimeScale { get; set; } = 1;

        /*================ Physical Properties ===============*/
        /// <summary>
        /// Character Hitbox Object
        /// </summary>
        [HideInInspector] public HitBox hitBox;

        [HideInInspector] public Vector3 velocity;
        public float gravity = -20f;
         public Vector3 friction = new Vector3(0.5f, 0.99f, 0.5f);
        public float CurrentSpeed { get; set; }
        [HideInInspector] public Vector2 moveInputDir;

        // Attack Properties
        public bool CanCancel { get; set; }
        public bool HitConfirm { get; set; }
         public float CurrentHitPauseFrames { get; set; }
        public bool IsHitBoxActive { get; set; }
        public int CurrentAtkIndex { get; set; }
        public float HitRecoverFrames { get; set; }

        // JumpProperties
        [HideInInspector] public float aerialTimer;
        [HideInInspector] public int currentAirJumpPoint;
         public int maxAirJumpPoint = 2;

        // animation
        [HideInInspector] public float animAir;
        [HideInInspector] public float animFallSpeed;
        [HideInInspector] public float animMoveSpeed;
        [HideInInspector] public Vector2 animHit;
        [HideInInspector] public float animSpeed;

        // The event targets on when entering 
        public delegate void EnterBehaviorEventHandler(ActorModel model);
        public EnterBehaviorEventHandler EnterBehaviorEvent;

        // The event targets on when updating behavior
        public delegate void UpdateBehaviorEventHandler(ActorModel model);
        public UpdateBehaviorEventHandler UpdateBehaviorEvent;

        // The event targets on when getting hit
        public delegate void GetHitEventHandler(ActorModel model);
        public GetHitEventHandler GetHitEvent;
        #endregion

        /// <summary>
        ///  Init model
        /// </summary>
        /// <param name="go"></param>
        public void Init(GameObject go)
        {
            if (behaviors.Count == 0 || behaviors == null)
            {
                Debug.LogError("The current Actor doesn't have any behaviors data, please add behaviors into ActorModel's behaviors list");
            }
            groundLayer = ~(1 << 9); // only disable grounable layer
            fsm = new ActorFSM(this); // init FSM

            character = go;
            cc = go.GetComponent<CharacterController>();
            audioSource = go.GetComponent<AudioSource>();
            anim = go.GetComponentInChildren<Animator>();
            hitBox = go.GetComponentInChildren<HitBox>(); // todo maybe add sub hitboxes for different shape of attack abilities

            if (hitBox != null)
            {
                hitBox.SetActive(false);
            }

            energy = 0;
            hasLockdeTarget = false;
            target = null;
            velocity = Vector3.zero;
            RefillAirJumpPoint();

            // Initial behavior
            StartBehavior(GetBehavior("Neutral"));

            fsm.InitBehaviorActions();
        }

        /// <summary>
        /// Increase the energy at power meter of the character
        /// </summary>
        /// <param name="power"></param>
        public void IncreaseEnergy(int power)
        {
            energy += power;
            Mathf.Clamp(energy, 0, 100);
        }

        /// <summary>
        ///  Reset the Initial Air Jump Point
        /// </summary>
        public void RefillAirJumpPoint()
        {
            currentAirJumpPoint = maxAirJumpPoint;
        }


        /// <summary>
        /// Get Behavior by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActorBehavior GetBehavior(string name)
        {
            return fsm.GetBehavior(name);
        }

        /// <summary>
        /// Get Behavior by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActorBehavior GetBehavior(int index)
        {
            return fsm.GetBehavior(index);
        }

        /// <summary>
        /// Start a new Behavior
        /// </summary>
        /// <param name="newBehavior"></param>
        public void StartBehavior(ActorBehavior newBehavior)
        {
            fsm.StartBehavior(newBehavior);
        }

        /// <summary>
        /// Loop the behavior by reset the frame
        /// </summary>        
        public void LoopBehavior()
        {
            fsm.LoopBehavior();
        }

        /// <summary>
        /// End the Behavior, automaticly go back to the Neutral Behavior
        /// </summary>
        public void EndBehavior()
        {
            fsm.EndBehavior();
        }

        /// <summary>
        ///  Get beahvior names
        /// </summary>
        /// <returns></returns>
        public string[] GetBehaviorNames()
        {
            return fsm.GetBehaviorNames();
        }

        /// <summary>
        /// Get Chain Behavior Names
        /// </summary>
        /// <returns></returns>
        public string[] GetChainBehaviorNames()
        {
            return fsm.GetChainBehaviorNames();
        }

        /// <summary>
        /// Get Behavior Action names
        /// </summary>
        /// <returns></returns>
        public string[] GetBehaviorActionNames()
        {
            return fsm.GetBehaviorActionNames();
        }
    }
}
