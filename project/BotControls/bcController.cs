using BotControls.Functions;
using BotControls.Utilities;
using Comfort.Common;
using EFT;
using EFT.Bots;
using EFT.Communications;
using EFT.UI;
using spawnscene = EFT.WavesSpawnScenario;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BotControls
{
    public class bcController : MonoBehaviour
    {
        // "bunker_2" is the name of the scene for Hideout.
        // "----APPLICATION----" is the name of the primary GameObject for Hideout.

        //private static BotControllerClass botController;
        //private static WavesSettings wavesSettings;
        //private static WaveInfo waveInfo;

        private static GameObject input;
        private static GameWorld gameWorld;

        private static Type gameDateTime;
        private static MethodInfo CalculateTime;

        private static DateTime currentDateTime;
        private static Rect windowRect = new Rect(100, 50, 500, 250);
        private static bool GUIStatus = false;
        private static bool DebugBots = false;
        private static string botdebugtex;

        private static int _aiAmount;
        private static int _aiAmountSlide;
        private static int _aiDifficulty;
        private static int _aiDifficultySlide;
        private static string _aidiftex;
        private static int _aiWaves;
        private static int _aiWavesSlide;
        private static int _aiInterval;
        private static int _aiIntervalSlide;
        private static int _aiDelay;
        private static int _aiDelaySlide;
        private static int _aiDuration;
        private static int _aiDurationSlide;

        public BotZone bzController { get; private set; }
        private bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return parameters.Length == 1
                && parameters[0].Name == "gameDateTime";
        }
        public bool Debugger { get; private set; }



        void Start()
        {
            // Add console command to duplicate functionality of toggle key
            GameConsole.AddCommand("bc", match =>
            {
                // Obtain current gameworld instance
                gameWorld = Singleton<GameWorld>.Instance;

                // If gameworld == null, player is not in raid.
                if (gameWorld == null)
                {
                    string log = "You must be in a raid to use Bot Controls.";
                    Notifier.DisplayWarningNotification(log, ENotificationDurationType.Long);
                    PreloaderUI.Instance.Console.AddLog("    " + log, "[BotControls]:");
                }
                else
                {
                    // Find instance of Hideout by searching for a gameobject that only exists in Hideout
                    if (GameObject.Find("----APPLICATION----"))
                    {
                        string log = "Bot Controls only works in raid. Hideout does not count soldier.";
                        Notifier.DisplayWarningNotification(log, ENotificationDurationType.Long);
                        PreloaderUI.Instance.Console.AddLog("    " + log, "[BotControls]:");
                    }
                    else
                    {
                        // Caching input manager GameObject which script is responsible for reading the player inputs
                        if (input == null)
                        {
                            input = GameObject.Find("___Input");
                        }

                        // Getting type responsible for time in the current world for later use
                        gameDateTime = gameWorld.GameDateTime.GetType();
                        CalculateTime = gameDateTime.GetMethod("Calculate", BindingFlags.Public | BindingFlags.Instance);

                        GUIStatus = !GUIStatus;
                        Cursor.visible = GUIStatus;
                        if (GUIStatus)
                        {
                            // Changing the default windows cursor to an EFT-style one and playing a sound when the menu appears
                            CursorSettings.SetCursor(ECursorType.Idle);
                            Cursor.lockState = CursorLockMode.None;
                            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuContextMenu);
                        }
                        else
                        {
                            // Hiding cursor and playing a sound when the menu disappears
                            CursorSettings.SetCursor(ECursorType.Invisible);
                            Cursor.lockState = CursorLockMode.Locked;
                            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuDropdown);
                        }
                        //Disabling the input manager so the player won't move
                        input.SetActive(!GUIStatus);
                    }
                }
            });
        }


        public void Update()
        {
            if (Input.GetKeyDown(bcPlugin.TogglePanel.Value.MainKey))
            {
                // Obtain current gameworld instance
                gameWorld = Singleton<GameWorld>.Instance;

                // If gameworld == null, player is not in raid.
                if (gameWorld == null)
                {
                    if (GameObject.Find("ErrorScreen"))
                        PreloaderUI.Instance.CloseErrorScreen();
                    PreloaderUI.Instance.ShowErrorScreen("Bot Controls Error", "You must be in a raid to use Bot Controls.");
                }
                else
                {
                    // Find instance of Hideout by searching for a gameobject that only exists in Hideout
                    if (GameObject.Find("----APPLICATION----"))
                    {
                        string log = "Bot Controls only works in raid. Hideout does not count soldier.";
                        Notifier.DisplayWarningNotification(log, ENotificationDurationType.Long);
                        PreloaderUI.Instance.Console.AddLog("    " + log, "[BotControls]:");
                    }
                    else
                    {
                        // Caching input manager GameObject which script is responsible for reading the player inputs
                        if (input == null)
                        {
                            input = GameObject.Find("___Input");
                        }

                        // Getting type responsible for time in the current world for later use
                        gameDateTime = gameWorld.GameDateTime.GetType();
                        CalculateTime = gameDateTime.GetMethod("Calculate", BindingFlags.Public | BindingFlags.Instance);

                        GUIStatus = !GUIStatus;
                        Cursor.visible = GUIStatus;
                        if (GUIStatus)
                        {
                            // Changing the default windows cursor to an EFT-style one and playing a sound when the menu appears
                            CursorSettings.SetCursor(ECursorType.Idle);
                            Cursor.lockState = CursorLockMode.None;
                            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuContextMenu);
                        }
                        else
                        {
                            // Hiding cursor and playing a sound when the menu disappears
                            CursorSettings.SetCursor(ECursorType.Invisible);
                            Cursor.lockState = CursorLockMode.Locked;
                            Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuDropdown);
                        }
                        //Disabling the input manager so the player won't move
                        input.SetActive(!GUIStatus);
                    }
                }
            }
        }


        public void OnGUI()
        {
            // Draws the main outline of the Bot Controls window with title
            if (GUIStatus)
                windowRect = GUI.Window(0, windowRect, WindowFunction, "Bot Controls v2.3.1 by jbs4bmx");
        }

        void WindowFunction(int BGCWindowID)
        {
            


            // Not sure if this step even matters. See DebugSelector below for drawing object
            if (DebugBots)
                botdebugtex = "ON";
            else
                botdebugtex = "OFF";

            //Caching BotController script for the first time
            if (bzController == null)
            {
                bzController = GameObject.Find("GAME").GetComponent<BotZone>();
            }


            //----Draw GUI - Labels, sliders, and button to activate changes----------------------------\\

            //----ALLOW WINDOW DRAG---------------------------------------------------------------------\\
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            //------------------------------------------------------------------------------------------\\


            //----CURRENT TIME--------------------------------------------------------------------------\\
            currentDateTime = (DateTime)CalculateTime.Invoke(gameWorld.GameDateTime, null);
            GUI.Box(new Rect(30, 30, 440, 30), "Current in-game time: " + currentDateTime.ToString("HH:mm:ss"));
            //------------------------------------------------------------------------------------------\\


            //----COLUMN 1 SLIDERS----------------------------------------------------------------------\\
            GUILayout.BeginArea(new Rect(15, 70, 230, 125));
            GUILayout.BeginVertical();

            // How many bots in the game... WARNING! Too many bots can melt you CPU
            GUILayout.Box("AI Amount (Bot Count): " + _aiAmount.ToString());
            _aiAmountSlide = Mathf.RoundToInt(GUILayout.HorizontalSlider(_aiAmount, 5, 30));
            _aiAmount = _aiAmountSlide;

            // Spawn Waves (1..10)
            GUILayout.Box("Spawn Waves: " + _aiWaves.ToString());
            _aiWavesSlide = Mathf.RoundToInt(GUILayout.HorizontalSlider(_aiWaves, 1, 10));
            _aiWaves = _aiWavesSlide;

            // Delay before spawn wave is added to map
            GUILayout.Box("Wave Delay (Seconds): " + _aiDelay.ToString());
            _aiDelaySlide = Mathf.RoundToInt(GUILayout.HorizontalSlider(_aiDelay, 0, 60));
            _aiDelay = _aiDelaySlide;

            GUILayout.EndVertical();
            GUILayout.EndArea();
            //------------------------------------------------------------------------------------------\\


            //----COLUMN 2 SLIDERS----------------------------------------------------------------------\\
            GUILayout.BeginArea(new Rect(255, 70, 230, 125));
            GUILayout.BeginVertical();

            // Bot difficulty level
            GUILayout.Box("AI Difficulty: " + _aiDifficulty.ToString() + " (" + _aidiftex + ")");
            _aiDifficultySlide = Mathf.RoundToInt(GUILayout.HorizontalSlider(_aiDifficultySlide, 0, 5));
            switch (_aiDifficultySlide)
            {
                case 0:
                    _aiDifficulty = 0x00000000;
                    _aidiftex = "AsOnline";
                    break;
                case 1:
                    _aiDifficulty = 0x00000001;
                    _aidiftex = "Easy";
                    break;
                case 2:
                    _aiDifficulty = 0x00000002;
                    _aidiftex = "Normal";
                    break;
                case 3:
                    _aiDifficulty = 0x00000003;
                    _aidiftex = "Hard";
                    break;
                case 4:
                    _aiDifficulty = 0x00000004;
                    _aidiftex = "Impossible";
                    break;
                case 5:
                    _aiDifficulty = 0x00000005;
                    _aidiftex = "Random";
                    break;
            }
            _aiDifficulty = _aiDifficultySlide;

            // Time in Minutes between wave spawns
            GUILayout.Box("Wave Intervals (Minutes): " + _aiInterval.ToString());
            _aiIntervalSlide = Mathf.RoundToInt(GUILayout.HorizontalSlider(_aiInterval, 0, 60));
            _aiInterval = _aiIntervalSlide;

            // Still working this one out... is it even possible?
            // Time until dead bodies are removed from the map to aid in game performance
            // Note... bodies remain... they are just no longer rendered. You can still loot them, but not having them rendered should help
            // performance as there would be less game objects loaded.
            GUILayout.Box("Dead Duration (Minutes): " + _aiDuration.ToString());
            _aiDurationSlide = Mathf.RoundToInt(GUILayout.HorizontalSlider(_aiDuration, 0, 120));
            _aiDuration = _aiDurationSlide;

            GUILayout.EndVertical();
            GUILayout.EndArea();
            //------------------------------------------------------------------------------------------\\


            //----DEBUG SELECTOR------------------------------------------------------------------------\\
            // Draws a stupid shadow box around the sliders unless the debug option is turned on... may be pointless but it's here none the less
            DebugBots = GUI.Toggle(new Rect(15, 210, 75, 25), DebugBots, "AI Debug:");
            GUI.Label(new Rect(95, 210, 30, 25), botdebugtex);
            if (!DebugBots)
            {
                Debugger = false;
                GUI.Box(new Rect(10, 65, 480, 135), "");
            }
            else
            {
                Debugger = true;
            }
            //------------------------------------------------------------------------------------------\\


            //----BUTTON--------------------------------------------------------------------------------\\
            // The button. This is the logic behind the desired settings being applied or not.
            if ( GUI.Button(new Rect(140, 205, 50, 30), "Set") )
            {
                if (Debugger)
                {
                    //BotControllerClass.AllTypes.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(x => x.FieldType == typeof(BotControllerClass));
                    //DebugBotData.AllTypes.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(x => x.FieldType == typeof(DebugBotData));

                    // Was trying a few things here... wtf?!
                    // TODO: Add logic here to find changable fields?
                    var bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;



                    var ignore = typeof(BossLocationSpawn).GetField("IgnoreMaxBots", bindingFlags);
                    var botNum1 = typeof(WildSpawnWave).GetField("slots_min", bindingFlags);
                    var botNum2 = typeof(WildSpawnWave).GetField("slots_max", bindingFlags);
                    var botDif = typeof(WildSpawnWave).GetField("_botDifficulty", bindingFlags);
                    var botwav = typeof(WildSpawnWave).GetField("wavesSettings", bindingFlags);
                    var botInt = typeof(WildSpawnWave).GetField("respawnCount", bindingFlags);
                    var botDel = typeof(Class697).GetField("__Delay__", bindingFlags);
                    var botDur = typeof(WildSpawnWave).GetField("__Duration__", bindingFlags);

                    //var botNum = typeof(DebugBotData).GetField("MaxBotsCount", bindingFlags);
                    //var botDif = typeof(DebugBotData).GetField("BotDifficulty", bindingFlags);
                    //var botwav = typeof(DebugBotData).GetField("wavesSettings", bindingFlags);
                    //var botInt = typeof(DebugBotData).GetField("respawnCount", bindingFlags);
                    //var botDel = typeof(DebugBotData).GetField("__Delay__", bindingFlags);
                    //var botDur = typeof(DebugBotData).GetField("__Duration__", bindingFlags);

                    // Notify player of the changes (currently the only part of this section that works.)
                    Notifier.DisplayMessageNotification("Bot count set to: " + _aiAmount);
                    Notifier.DisplayMessageNotification("Bot difficulty set to: " + _aiDifficulty + " (" + _aidiftex + ")");
                    Notifier.DisplayMessageNotification("Spawn waves set to: " + _aiWaves);
                    Notifier.DisplayMessageNotification("Wave intervals set to: " + _aiInterval);
                    Notifier.DisplayMessageNotification("AI delay set to: " + _aiDelay);
                    Notifier.DisplayMessageNotification("Duration to keep bodies set to: " + _aiDuration);

                    // Set the changes ???
                    // TODO:: Add logic here to set all changes.
                    ignore.SetValue(null, true);
                    botNum1.SetValue(null, _aiAmount);
                    botNum2.SetValue(null, _aiAmount);
                    botDif.SetValue(null, _aidiftex) ;
                    botwav.SetValue(null, _aiWaves);
                    botInt.SetValue(null, _aiInterval);
                    botDel.SetValue(null, _aiDelay);
                    botDur.SetValue(null, _aiDuration);

                    Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.MenuInspectorWindowClose);
                }
            }
            //------------------------------------------------------------------------------------------\\
        }
    }
}
