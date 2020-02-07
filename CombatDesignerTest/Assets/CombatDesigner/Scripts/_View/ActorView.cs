using UnityEngine;

namespace CombatDesigner
{
    public class ActorView : MonoBehaviour
    {
        /// <summary>
        ///  Core data of each character 
        /// </summary>
        ActorModel model;

        /// <summary>
        /// Animation Object Reference
        /// </summary>
        BehaviorView_Animation animationView;

        /// <summary>
        /// BehaviorAttack Object Reference
        /// </summary>
        BehaviorView_Attack atkView;


        // Initialization
        void Awake()
        {
            model = GetComponent<ActorController>().model;
            animationView = new BehaviorView_Animation();
            atkView = new BehaviorView_Attack();

        }

        // Register Events
        void OnEnable()
        {
            model.EnterBehaviorEvent += animationView.CrossFadeAnimationInFixedTime;
            model.UpdateBehaviorEvent += atkView.UpdateAttackInfos;
        }

        // Unregister Events
        void OnDisable()
        {
            model.EnterBehaviorEvent -= animationView.CrossFadeAnimationInFixedTime;
            model.UpdateBehaviorEvent -= atkView.UpdateAttackInfos;
        }

        
        void Update()
        {
            animationView.UpdateAnimation(model);
        }
    }


}
