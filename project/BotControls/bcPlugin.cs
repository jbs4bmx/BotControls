using BepInEx;
using BepInEx.Configuration;
using DrakiaXYZ.VersionChecker;
using System;
using UnityEngine;
using static DrakiaXYZ.VersionChecker.VersionChecker;

/* Author: jbs4bmx
 * 
 * Credits: - For inspiration and/or code that this is used in this mod.
 *          - SamSwat,CWX,nexus4880, azsteal, Ereshkigal, and possibly some others.
 *          - If I remember more, I will add them once I decide to stop being such a lazy fuck.
 *          
 * Notes:   Hmmm... there doesn't seem to be anything here yet.
 * 
 */

namespace BotControls
{
    [BepInPlugin("com.jbs4bmx.BotControls", "BotControls", "380.0.1")]
    public class bcPlugin : BaseUnityPlugin
    {
        
        public static GameObject Hook;
        public const int TarkovVersion = 29197;
        private const string SectionName = "Bot Controls by jbs4bmx";
        
        internal static ConfigEntry<KeyboardShortcut> TogglePanel;
        public static ConfigEntry<string> RemoveBodies { get; set; }

        private void Awake()
        {
            if (!VersionChecker.CheckEftVersion(Logger, Info, Config))
            {
                throw new Exception("Invalid EFT Version");
            }

            Logger.LogInfo("Loading: Bot Controls v380.0.1");


            Hook = new GameObject("Bot Controls");
            Hook.AddComponent<bcController>();
            DontDestroyOnLoad(Hook);

            RemoveBodies = Config.Bind(
                SectionName,
                "Remove Bodies",
                "Remove all dead bodies from the map",
                new ConfigDescription(
                    "Remove all dead bodies from the map",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        CustomDrawer = RemoveBodiesButtonDrawer,
                        Order = 1,
                    }
                )
            );

            TogglePanel = Config.Bind(
                SectionName,
                "Open Bot Controls",
                new KeyboardShortcut(KeyCode.F9),
                "The keyboard shortcut that opens Bot Controls"
            );
        }

        private void RemoveBodiesButtonDrawer(ConfigEntryBase entry)
        {
            bool button = GUILayout.Button("Remove Bodies", GUILayout.ExpandWidth(true));
            if (button)
            {
                BodyCleanup(true);
            }
        }
        private void BodyCleanup(bool force = false)
        {
            if (!force && OnlyInRaid.Value && !GameHelper.IsInGame())
            {
                return;
            }

            Logger.LogInfo("Executing the RAM cleaner");

            _emptyWorkingSetMethod.Invoke(null, null);
        }
    }
}
