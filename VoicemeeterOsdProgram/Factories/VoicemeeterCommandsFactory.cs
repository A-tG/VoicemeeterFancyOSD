namespace VoicemeeterOsdProgram.Factories
{
    public static class VoicemeeterCommandsFactory
    {
        public static string GetStripGain(int i) => $"{GetStrip(i)}Gain";
        public static string GetStripMono(int i) => $"{GetStrip(i)}Mono";
        public static string GetStripSolo(int i) => $"{GetStrip(i)}Solo";
        public static string GetStripMute(int i) => $"{GetStrip(i)}Mute";
        public static string GetStripLabel(int i) => $"{GetStrip(i)}Label";

        public static string GetBusGain(int i) => $"{GetBus(i)}Gain";
        public static string GetBusMono(int i) => $"{GetBus(i)}Mono";
        public static string GetBusSolo(int i) => $"{GetBus(i)}Solo";
        public static string GetBusMute(int i) => $"{GetBus(i)}Mute";

        private static string GetStrip(int i) => $"Strip[{i}].";
        private static string GetBus(int i) => $"Bus[{i}].";
    }
}
