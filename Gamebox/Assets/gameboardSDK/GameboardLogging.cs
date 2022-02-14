using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameboard
{
    public static class GameboardLogging
    {
        public enum MessageTypes { Log, Warning, Error, Verbose }

        public static void LogMessage(string inString, MessageTypes inType)
        {
            switch (inType)
            {
                case MessageTypes.Log: Debug.Log($"Gameboard Log: {inString}"); break;
                case MessageTypes.Error: Debug.LogError($"Gameboard Error: {inString}"); break;
                case MessageTypes.Warning: Debug.LogWarning($"Gameboard Warning: {inString}"); break;
                case MessageTypes.Verbose:

#if GAMEBOARD_VERBOSE_LOGGING
                Debug.Log("Gameboard Verbose: " + inString);        
#endif
                break;
            }
        }
    }
}