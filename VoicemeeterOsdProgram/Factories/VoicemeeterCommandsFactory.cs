using VoicemeeterOsdProgram.Core.Types;

namespace VoicemeeterOsdProgram.Factories
{
    public static class VoicemeeterCommandsFactory
    {
        public static string Gain(int i, StripType t) => GetCommandHead(i, t) + "Gain";

        public static string Mute(int i, StripType t) => GetCommandHead(i, t) + "Mute";

        public static string Solo(int i, StripType t) => GetCommandHead(i, t) + "Solo";

        public static string Mono(int i, StripType t) => GetCommandHead(i, t) + "Mono";

        private static string GetCommandHead(int i, StripType type = StripType.Input)
        {
            return type switch
            {
                StripType.Input => $"Strip[{i}].",
                _ => $"Bus[{i}]."
            };

        }

        public static string InputLabel(int i) => $"{GetCommandHead(i)}Label";
    }
}
