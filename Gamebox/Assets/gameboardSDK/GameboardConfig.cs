﻿using System;
using UnityEngine;

namespace Gameboard
{
    public class GameboardConfig : IGameboardConfig
    {
        public Version SDKVersion { get; }
        public int gameboardConnectPort { get; }
        public string companionServerUri { get; }
        public string companionServerUriNamespace { get; }
        public DataTypes.OperatingMode operatingMode { get; }
        public Vector2 deviceResolution { get; }

        public string gameboardId { get; }

        /// <summary>
        /// How long events will wait before timing out, in Milliseconds
        /// </summary>
        public int eventTimeoutLength { get; }

        /// <summary>
        /// How many times an event will be retried after timing out from eventTimeoutLength before being considered a fatal failure.
        /// </summary>
        public int eventRetryCount { get; }

        public AndroidJavaClass mGameboardSettings { get; }
        public AndroidJavaClass drawerHelper { get; }

        public AndroidApplicationContext androidApplicationContext { get; }

        public GameboardConfig(string mockGameboardId = "")
        {
            SDKVersion = new Version(0, 0, 1066);
            gameboardConnectPort = 3333;
            deviceResolution = new Vector2(1920, 1920); //TODO: dynamically figure out resolution, but in our case the GB-1 is always 1920x1920         

            androidApplicationContext = new AndroidApplicationContext();

#if UNITY_ANDROID
            mGameboardSettings = new AndroidJavaClass("com.lastgameboard.gameboard_settings_sdk.GameboardSettings");
            drawerHelper = new AndroidJavaClass("com.lastgameboard.gameboardservice.client.drawer.DrawerHelper");
#endif

            companionServerUri = GetBoardServiceWebSocketUrl(mGameboardSettings);
            companionServerUriNamespace = "";
            operatingMode = DataTypes.OperatingMode.Production;

            gameboardId = (Application.platform == RuntimePlatform.Android) ? GetBoardId(mGameboardSettings) : mockGameboardId;
            Debug.Log("BOARD ID: " + gameboardId);

            eventTimeoutLength = 3000;
            eventRetryCount = 5;
        }

        ///
        /// Retrives the board ID for the gameboard this game is running on. 
        /// This API is only available while running on the hardware so if 
        /// running on the editor no Board ID will be obtained.
        ///
        private string GetBoardId(AndroidJavaClass inSettings)
        {
#if UNITY_ANDROID
            return inSettings.CallStatic<string>("getBoardId", androidApplicationContext.GetNativeContext());
#else
            Debug.Log("Cannot acquire BoardID because you are not running on the Gameboard device.");
            return "NullID";
#endif
        }
        
        ///
        /// This method returns the WebSocket URL as configured in the board.
        /// This is a full URL including the protocol and port. i.e. wss://api.lastgameboard.com:443
        /// In the event the board is configured to use the service running local on the board 
        /// that address might be of the form: ws://localhost:8000
        ///
        private string GetBoardServiceWebSocketUrl(AndroidJavaClass inSettings)
        {
            string defaultURL = "ws://api.lastgameboard.com";

#if UNITY_EDITOR
            return defaultURL;
#elif UNITY_ANDROID
            try
            {
                return inSettings.CallStatic<string>("getBoardServiceWebSocketUrl", androidApplicationContext.GetNativeContext());
            }
            catch(Exception e)
            {
                Debug.Log($"Cannot acquire Board Service URL due to the exception {e.Message}. Using default URL of {defaultURL}.");
                return defaultURL;
            }
#else
            Debug.Log($"Cannot acquire Board Service URL because you are not running on the Gameboard device. Using default URL of {defaultURL}.");
            return defaultURL;
#endif
        }
    }
}
