using System;
using System.Collections.Generic;
using System.Linq;

using LabPresence.Managers;

namespace LabPresence
{
    public static class Gamemodes
    {
        private static readonly Dictionary<string, Gamemode> _Gamemodes = [];

        public static void RegisterGamemode(this Gamemode gamemode)
        {
            ArgumentNullException.ThrowIfNull(gamemode);

            if (_Gamemodes[gamemode.Barcode] != null)
                throw new ArgumentException("Gamemode is already registered!");

            if (string.IsNullOrWhiteSpace(gamemode.Barcode))
                throw new ArgumentNullException(nameof(gamemode), "The barcode cannot be empty or null!");

            if (gamemode.CustomToolTip == null && gamemode.OverrideTime == null)
                throw new ArgumentException("The gamemode needs to have a custom tooltip and/or override time");

            _Gamemodes.Add(gamemode.Barcode, gamemode);
        }
        public static void RegisterGamemode(GamemodeParams args)
          => RegisterGamemode(new Gamemode(args));

        public static bool UnregisterGamemode(string barcode) => _Gamemodes.Remove(barcode);

        public static bool UnregisterGamemode(this Gamemode gamemode)
            => UnregisterGamemode(gamemode?.Barcode);

        public static bool IsGamemodeRegistered(string barcode)
            => _Gamemodes[barcode] != null;

        public static bool IsGamemodeRegistered(this Gamemode gamemode)
            => IsGamemodeRegistered(gamemode?.Barcode);

        public static Gamemode GetGamemode(string barcode)
            => _Gamemodes[barcode];

        public static int GetGamemodeCount()
            => _Gamemodes.Count;

        public static string[] GetGamemodeBarcodes() => _Gamemodes.Keys.ToArray();

        public static string GetToolTipValue(string barcode)
        {
            var registered = _Gamemodes[barcode];
            if (registered == null || registered.CustomToolTip == null)
                return string.Empty;

            string ret;

            try
            {
                ret = registered.CustomToolTip?.Invoke();
            }
            catch (Exception ex)
            {
                Core.Logger.Error($"An unexpected error has occurred while trying to get value of tooltip of the gamemode with barcode '{barcode}', exception:\n{ex}");
                ret = string.Empty;
            }

            return ret;
        }

        public static string GetToolTipValue(this Gamemode gamemode)
            => GetToolTipValue(gamemode?.Barcode);

        public static Timestamp GetOverrideTime(string barcode)
        {
            var registered = _Gamemodes[barcode];
            if (registered == null || registered.OverrideTime == null)
                return null;

            Timestamp ret;

            try
            {
                ret = registered.OverrideTime?.Invoke();
            }
            catch (Exception ex)
            {
                Core.Logger.Error($"An unexpected error has occurred while trying to get value of tooltip of the gamemode with barcode '{barcode}', exception:\n{ex}");
                ret = null;
            }

            return ret;
        }

        public static Timestamp GetOverrideTime(this Gamemode gamemode)
            => GetOverrideTime(gamemode?.Barcode);
    }

    public struct GamemodeParams {
        public string barcode;
        public float minimumDelay;
        public Func<string> customToolTip;
        public Func<Timestamp> overrideTime;
    }
    public class Gamemode(GamemodeParams gamemodeParams)
    {
        public string Barcode { get; } = gamemodeParams.barcode;

        public float MinimumDelay { get; } = gamemodeParams.minimumDelay;

        public Func<string> CustomToolTip { get; } = gamemodeParams.customToolTip;

        public Func<Timestamp> OverrideTime { get; } = gamemodeParams.overrideTime;
    }
}