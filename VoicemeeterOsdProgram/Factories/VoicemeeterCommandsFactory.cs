namespace VoicemeeterOsdProgram.Factories
{
    public static class VoicemeeterCommandsFactory
    {
        public static string GetInputGain(int i) => $"{GetStrip(i)}Gain";
        public static string GetInputMono(int i) => $"{GetStrip(i)}Mono";
        public static string GetInputSolo(int i) => $"{GetStrip(i)}Solo";
        public static string GetInputMute(int i) => $"{GetStrip(i)}Mute";
        public static string GetInputLabel(int i) => $"{GetStrip(i)}Label";

        public static string GetBusGain(int i) => $"{GetBus(i)}Gain";
        public static string GetBusMono(int i) => $"{GetBus(i)}Mono";
        public static string GetBusMute(int i) => $"{GetBus(i)}Mute";

        private static string GetStrip(int i) => $"Strip[{i}].";
        private static string GetBus(int i) => $"Bus[{i}].";
    }
}
