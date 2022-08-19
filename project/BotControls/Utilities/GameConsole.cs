using Aki.Reflection.Utils;
using EFT.UI;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BotControls.Utilities
{
    static class GameConsole
    {
        private static readonly Type consoleCommandType;
        private static readonly ConstructorInfo consoleCommandConstructor;
        private static readonly MethodInfo consoleCommandsAddMethod;
        private static readonly FieldInfo commandsField;

        static GameConsole()
        {
            consoleCommandType = PatchConstants.EftTypes.Single(x => x.GetProperty("Regex") != null && x.GetMethod("TryExecute") != null);
            consoleCommandConstructor = consoleCommandType.GetConstructor(new[] { typeof(string), typeof(Action<Match>) });
            consoleCommandsAddMethod = AccessTools.Field(typeof(ConsoleScreen), nameof(ConsoleScreen.Commands)).FieldType.GetMethod("Add", new[] { consoleCommandType });
            commandsField = AccessTools.Field(typeof(ConsoleScreen), nameof(ConsoleScreen.Commands));
        }

        public static void AddCommand(string regular, Action<Match> onExecute)
        {
            var commands = commandsField.GetValue(null);
            var command = consoleCommandConstructor.Invoke(new object[] { regular, onExecute });
            consoleCommandsAddMethod.Invoke(commands, new[] { command });
        }
    }
}
