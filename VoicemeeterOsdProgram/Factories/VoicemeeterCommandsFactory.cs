using VoicemeeterOsdProgram.Core.Types;

namespace VoicemeeterOsdProgram.Factories
{
    public static class VoicemeeterCommandsFactory
    {
        public static string Gain(int i, StripType t) => GetCommandHead(i, t) + "Gain";

        public static string Mute(int i, StripType t) => GetCommandHead(i, t) + "Mute";

        public static string Solo(int i, StripType t) => GetCommandHead(i, t) + "Solo";

        public static string Mono(int i, StripType t) => GetCommandHead(i, t) + "Mono";

        public static string HardBusAssign(int i, int busIndex) => GetCommandHead(i) + $"A{busIndex}";

        public static string VirtBusAssign(int i, int busIndex) => GetCommandHead(i) + $"B{busIndex}";

        public static string Sel(int i) => GetCommandHead(i, StripType.Output) + "Sel";

        public static string EqOn(int i) => GetCommandHead(i, StripType.Output) + "EQ.on";

        public static string Limiter(int i) => GetCommandHead(i, StripType.Input) + "Limit";

        private static string GetCommandHead(int i, StripType type = StripType.Input)
        {
            return type switch
            {
                StripType.Input => $"Strip[{i}].",
                _ => $"Bus[{i}]."
            };

        }

        public static string Label(int i, StripType type = StripType.Input) => GetCommandHead(i, type) + "Label";
    }
}
