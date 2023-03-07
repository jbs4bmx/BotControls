using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

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
    [BepInPlugin("com.jbs4bmx.BotControls", "BotControls", "350.0.1")]
    public class bcPlugin : BaseUnityPlugin
    {
        public static GameObject Hook;

        private const string SectionName = "Main";
        internal static ConfigEntry<KeyboardShortcut> TogglePanel;

        private void Awake()
        {
            Logger.LogInfo("Loading: Bot Controls v350.0.1");
            Hook = new GameObject("Bot Controls");
            Hook.AddComponent<bcController>();
            DontDestroyOnLoad(Hook);

            TogglePanel = Config.Bind(
                SectionName,
                "Bot Controls Control Panel Toggle Key",
                new KeyboardShortcut(KeyCode.F9),
                "The keyboard shortcut that toggles bot control panel");
        }
    }
}
