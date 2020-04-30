
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;


namespace CombatDesigner
{
    public enum LogDomain
    {
        None, All, Project_Setup, ActorData, ActorControl, ActorBehavior, ActorFSM, BehaviorAcrion, FrameInfo,
        BehaviorRequirement,
        BT_Conditional,
        DataSerialization,
        Input
    }

    public class CombatDebugger : GameManager<CombatDebugger>
    {
        [ShowInInspector] public static LogDomain logDomain;

        /// <summary>
        /// Log Messages
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        public static void Log(string msg, LogDomain domain)
        {
            if (logDomain == LogDomain.None)
            {
                return;
            }
            else if (logDomain == LogDomain.All)
            {
                UnityEngine.Debug.Log("<b><color=#002fff>[CombatDebugger]: </color></b>" + msg);
            }
            else if (domain == logDomain)
            {
                UnityEngine.Debug.Log("<b><color=#002fff>[CombatDebugger]: </color></b>" + msg);
            }
        }

        /// <summary>
        /// Log Warnings
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        public static void LogWarning(string msg, LogDomain domain)
        {
            if (logDomain == LogDomain.None)
            {
                return;
            }
            else if (logDomain == LogDomain.All)
            {
                UnityEngine.Debug.LogWarning("<b><color=#002fff>[CombatDebugger]: </color></b>" + msg);
            }
            else if (domain == logDomain)
            {
                UnityEngine.Debug.LogWarning("<b><color=#002fff>[CombatDebugger]: </color></b>" + msg);
            }
        }

        /// <summary>
        /// Log Errors
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        public static void LogError(string msg, LogDomain domain)
        {
            if (logDomain == LogDomain.None)
            {
                return;
            }
            else if (logDomain == LogDomain.All)
            {
                UnityEngine.Debug.LogError("<b><color=#002fff>[CombatDebugger]: </color></b>" + msg);
            }
            else if (domain == logDomain)
            {
                UnityEngine.Debug.LogError("<b><color=#002fff>[CombatDebugger]: </color></b>" + msg);
            }
        }
    }

}