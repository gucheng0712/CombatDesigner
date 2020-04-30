using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace CombatDesigner
{
    /// <summary>
    /// ActorModel is a ScriptableObject Class that represents a character's core data
    /// </summary>
    //  [CreateAssetMenu(fileName = "NewActorModel", menuName = "ACT-System/Model")]

    public class ActorModel:MonoBehaviour
    {
        #region Editor Fields
        /*================ Editor VFX ===============*/
#if UNITY_EDITOR
        [HideInInspector] public GameObject vfxGO;
        [HideInInspector] public float e_vfxInitialTime;
        [HideInInspector] public float e_currentStateTime;

        [DisableIf("@behaviorDir==\"\"")]
        [Settings]
        [ButtonGroup("Settings/A"), Button("Load Behaviors", ButtonSizes.Large)]
        public void LoadBehaviorBtn()
        {
            if (!string.IsNullOrEmpty(behaviorDir))
            {
                string path = behaviorDir.Substring(behaviorDir.LastIndexOf("Resources/") + 10);
                behaviors = Resources.LoadAll<ActorBehavior>(path).ToList();
            }
            Debug.Log("Load all behaviors");
        }

        [DisableIf("@anim==null")]
        [Settings]
        [ButtonGroup("Settings/A"), Button("Setup Behaviors in Animator", ButtonSizes.Large)]
        public void SetupBehaviorInAnimatorBtn()
        {
            if (anim == null)
            {
                Debug.LogError("Require Animator Component");
                return;
            }

            AnimatorController controller = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(anim.runtimeAnimatorController));
            if (controller == null)
            {
                Debug.LogErrorFormat("AnimatorController must not be null.");
                return;
            }

            AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
            var states = rootStateMachine.states;

            foreach (var b in behaviors)
            {
                if (!string.IsNullOrEmpty(b.name))
                {
                    if (!states.Any(x => x.state.name == b.name))
                    {
                        var newState = rootStateMachine.AddState(b.name);
                        if (b.animClip)
                        {
                            newState.motion = b.animClip;
                        }
                        Debug.Log("Added a New State called " + b.name + " in Animator");
                    }
                }
            }
        }


        [PropertySpace(10)]
        [Settings]
        public bool showReadOnly;
#endif

        #endregion

        #region Public Fields

        /// <summary>
        ///  Actor control Type
        /// </summary>
        [ModelBasic]
        public ActorType actorType = ActorType.PLAYER;

        /// <summary>
        /// Character Stats Object
        /// </summary>
        [Tooltip("Character Stats Object")]
        [ModelBasic]
        public ActorStats actorStats;

        /// <summary>
        /// The Hurt VFX Prefab
        /// </summary>
        [Tooltip("The Hurt VFX Prefab")]
        [ModelBasic]
        public GameObject hurtVFX;

        /// <summary>
        ///  The floating text prefab
        /// </summary>
        [Tooltip("The floating text prefab")]
        [ModelBasic]
        public GameObject floatingText;


        /// <summary>
        ///  Character GameObject
        /// </summary>
        [Tooltip("Character GameObject")]
        [ModelComponentReadOnly]
        public GameObject character;

        /// <summary>
        /// CharacterController Component
        /// </summary>
        [Tooltip("CharacterController Component")]
        [ModelComponentReadOnly]
        public CharacterController cc;

        //  public Rigidbody rb;

        /// <summary>
        /// Character Animator Component
        /// </summary>
        [Tooltip("Character Animaator Component")]
        [ModelComponentReadOnly]
        public Animator anim;


        /// <summary>
        /// Character AudioSource Component
        /// </summary>
        [Tooltip("Character AudioSource Component")]
        [ModelComponentReadOnly]
        public AudioSource audioSource;

        /// <summary>
        /// Cinemachine GameObject
        /// </summary>
        [Tooltip("Cinemachine GameObject Component")]
        [ModelComponentReadOnly]
        public GameObject cmGO;

        /// <summary>
        /// Character Hitbox Object
        /// </summary>
        [ModelComponentReadOnly]
        public HitBox hitBox;

        /// <summary>
        /// Character HurtBox Object
        /// </summary>
        [ModelComponentReadOnly]
        public HurtBox hurtBox;

        ///// <summary>
        ///// The domain timescale object
        ///// </summary>
        //[Tooltip("Domain Timescale Object")]
        //[ModelComponentReadOnly]
        //public Timeline time;

        [HideInInspector] public MaterialPropertyBlock props;

        [HideInInspector] public Renderer[] renderers;

        /*================ Actor Status ===============*/
        /// <summary>
        /// Character Finite State Machine Object
        /// </summary>
        [ModelStatusReadOnly]
        public ActorFSM fsm { get; set; }

        public ActorBuffSystem buffSystem;

        /// <summary>
        /// The Input Direction for movement
        /// </summary>
        [ModelStatusReadOnly]
        public Vector2 moveInputDir;

        /// <summary>
        /// The model's local timescale
        /// </summary>
        [ModelStatusReadOnly]
        public float objectTimeScale { get; set; }


        /// <summary>
        /// Layer mask for checking if is groundable
        /// </summary>
        [ModelStatusReadOnly]
        public LayerMask groundLayer;

        /// <summary>
        /// Is character dead
        /// </summary>
        [ModelStatusReadOnly]
        public bool isDead { get; set; }

        /// <summary>
        /// Is character in the air
        /// </summary>
        [ModelStatusReadOnly]
        public bool isAerial { get; set; }

        /// <summary>
        /// The current target GameObject
        /// </summary>
        [ModelStatusReadOnly]
        public ActorModel target;
        /// <summary>
        /// Has lock-on target?
        /// </summary>
        [ModelStatusReadOnly]
        public bool hasLockdeTarget { get; set; }

        // Attack Properties
        [ModelStatusReadOnly]
        public bool CanCancel { get; set; }

        [ModelStatusReadOnly]
        public bool HitConfirm { get; set; }

        [ModelStatusReadOnly]
        public bool IsInvincible { get; set; }

        [ModelStatusReadOnly]
        public bool IsCloaking { get; set; }

        [ModelStatusReadOnly]
        public float CurrentHitPauseFrames { get; set; }

        [ModelStatusReadOnly]
        public int CurrentAtkIndex { get; set; }

        [ModelStatusReadOnly]
        public float HitRecoverFrames { get; set; }
        // JumpProperties
        [ModelStatusReadOnly]
        public float aerialTimer { get; set; }

        // animation
        [ModelStatusReadOnly] public float animAir { get; set; }
        [ModelStatusReadOnly] public float animFallSpeed { get; set; }
        [ModelStatusReadOnly] public float animMoveSpeed { get; set; }
        [ModelStatusReadOnly] public Vector2 animHit { get; set; }
        [ModelStatusReadOnly] public float animSpeed { get; set; }


        /// <summary>
        /// A collection of behaviors that character has
        /// </summary>
        [PropertyTooltip("All behaviors of the current character model")]
        [ModelBehaviorReadOnly]
        public List<ActorBehavior> behaviors;

        /// <summary>
        /// The Current Behavior
        /// </summary>
        [ModelBehaviorReadOnly]
        public ActorBehavior currentBehavior { get; set; }

        /// <summary>
        /// Current frame of current behavior
        /// </summary>
        [ModelBehaviorReadOnly]
        public int currentFrame { get; set; }

        /// <summary>
        /// Previous frame of current behavior
        /// </summary>
        [ModelBehaviorReadOnly]
        public int previousFrame { get; set; }

        /// <summary>
        ///  a Collection of ChainBehavior
        /// </summary>
        [ModelBehaviorReadOnly]
        public List<ChainBehavior> chainBehaviors;

        /// <summary>
        /// The current chain behavior index in the current behavior
        /// </summary>
        [ModelBehaviorReadOnly]
        public int currentChainIndex { get; set; }

        [FolderPath]
        [Settings]
        public string behaviorDir { get; set; }

        /*================ Physical Properties ===============*/

        [ModelPhysics]
        public Vector3 friction = new Vector3(0.5f, 0.99f, 0.5f);

        [ModelPhysics]
        public float gravity { get; set; } = -20f;

        [ModelPhysicsReadOnly]
        public Vector3 velocity;
        public float moveSpeedModifier = 1;


        // The event targets on when entering 
        public delegate void EnterBehaviorEventHandler();
        public EnterBehaviorEventHandler EnterBehaviorEvent;

        public delegate void DectectedTargetEventHandler();
        public DectectedTargetEventHandler DetectEnemyEvent;

        public delegate void LostTargetEventHandler();
        public LostTargetEventHandler LostTargetEvent;


        #endregion

        /// <summary>
        ///  Initialization
        /// </summary>
        /// <param name="go"></param>
        public void Init(GameObject go)
        {
            if (behaviors.Count == 0 || behaviors == null)
            {
                Debug.LogError("The current Actor doesn't have any behaviors data, please add behaviors into ActorModel's behaviors list");
            }
            groundLayer = ~LayerMask.NameToLayer("Groundable"); // only disable grounable layer
            fsm = new ActorFSM(this); // init FSM
            buffSystem = new ActorBuffSystem();
            props = new MaterialPropertyBlock();

            character = go;
            if (character != null)
            {
                cc = character.GetComponent<CharacterController>();
                audioSource = character.GetComponent<AudioSource>();
                anim = character.GetComponentInChildren<Animator>();
                hitBox = character.GetComponentInChildren<HitBox>(); // todo maybe add sub hitboxes for different shape of attack abilities
                hurtBox = character.GetComponentInChildren<HurtBox>();
                renderers = character.GetComponentsInChildren<Renderer>();
            }

            actorStats?.Init();

            if (hitBox != null)
            {
                hitBox.SetActive(false);
                hitBox.Init();
            }
            if (hurtBox != null)
            {
                hurtBox.Init();
                hurtBox.SetActive(true);
            }

            isDead = false;
            hasLockdeTarget = false;
            IsInvincible = false;
            IsCloaking = false;
            target = null;
            velocity = Vector3.zero;
            animAir = 0;
            animFallSpeed = 0;
            animMoveSpeed = 0;
            animSpeed = 0;
            animHit = Vector3.zero;
            objectTimeScale = 1;
            // Initial behavior
            currentBehavior = GetBehavior("Default");
            StartBehavior(currentBehavior);

            fsm?.InitBehaviorActions();
        }


        /// <summary>
        ///  Return a boolean to determine if current character is in hit pause frames.
        /// </summary>
        /// <returns></returns>
        public bool IsInHitPauseFrames()
        {
            if (CurrentHitPauseFrames > 0)
            {
                CurrentHitPauseFrames -= 1;
            }

            return CurrentHitPauseFrames > 0;
        }

        /// <summary>
        /// Increase the energy at power meter of the character
        /// </summary>
        /// <param name="healthPoint"></param>
        public void IncreaseHealth(int healthPoint)
        {
            actorStats?.IncreaseHealth(healthPoint);
        }
        /// <summary>
        /// Increase the energy at power meter of the character
        /// </summary>
        /// <param name="healthPoint"></param>
        public void DecreaseHealth(int healthPoint)
        {
            actorStats?.ReduceHealth(healthPoint);
        }

        /// <summary>
        /// Increase the energy at power meter of the character
        /// </summary>
        /// <param name="power"></param>
        public void IncreaseEnergy(int power)
        {
            actorStats?.IncreaseEnergy(power);
        }

        /// <summary>
        /// Decrease the energy at power meter of the character
        /// </summary>
        /// <param name="power"></param>
        public void DecreaseEnergy(int power)
        {
            actorStats?.ReduceEnergy(power);
        }

        /// <summary>
        ///  Reset the Initial Air Jump Point
        /// </summary>
        public void RefillAirJumpPoint()
        {
            actorStats?.RefillAirJumpPoint();
        }
        public void ReduceAirJumpPoint(int reducedPoints)
        {
            actorStats?.ReduceJumpPoint(reducedPoints);
        }

        /// <summary>
        ///  Change the character's position
        /// </summary>
        /// <param name="pos"></param>
        public void SetActorPosition(Vector3 pos)
        {
            cc.enabled = false;
            character.transform.position = pos;
            cc.enabled = true;
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
            if (isDead)
                return;
            fsm.StartBehavior(newBehavior);
            if (newBehavior != null)
            {
                DecreaseEnergy(newBehavior.energyPointCost);
                ReduceAirJumpPoint(newBehavior.airJumpPointCost);
            }
        }

        /// <summary>
        /// Loop the behavior by reset the frame
        /// </summary>        
        public void LoopBehavior()
        {
            fsm.LoopBehavior();
        }

        /// <summary>
        /// End the Behavior, automaticly go back to the Default Behavior
        /// </summary>
        public void EndBehavior()
        {
            fsm.EndBehavior();
        }

        /// <summary>
        ///  Get the current Behavior attack
        /// </summary>
        /// <returns></returns>
        public BehaviorAttack GetCurrentAttack()
        {
            List<BehaviorAttack> atkInfos = currentBehavior.attackInfos;

            return atkInfos.IsNullOrEmpty() ? null : atkInfos[CurrentAtkIndex];
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
