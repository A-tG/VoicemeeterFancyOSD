using AtgDev.Voicemeeter.Types;

<<<<<<< Updated upstream
namespace VoicemeeterOsdProgram.Core.Types
=======
namespace VoicemeeterOsdProgram.Core.Types;

public readonly struct VoicemeeterProperties
>>>>>>> Stashed changes
{
    public struct VoicemeeterProperties
    {
        public readonly int hardInputs;
        public readonly int virtInputs;
        public readonly int hardOutputs;
        public readonly int virtOutputs;
        public readonly VoicemeeterType type;

        public VoicemeeterProperties(VoicemeeterType type)
        {
            this.type = type;
            switch (type)
            {
                case VoicemeeterType.Standard:
                    hardInputs = 2;
                    virtInputs = 1;
                    hardOutputs = 1;
                    virtOutputs = 1;
                    break;
                case VoicemeeterType.Banana:
                    hardInputs = 3;
                    virtInputs = 2;
                    hardOutputs = 3;
                    virtOutputs = 2;
                    break;
                case VoicemeeterType.Potato:
                case VoicemeeterType.Potato64:
                    hardInputs = 5;
                    virtInputs = 3;
                    hardOutputs = 5;
                    virtOutputs = 3;
                    break;
                default:
                    hardInputs = 0;
                    virtInputs = 0;
                    hardOutputs = 0;
                    virtOutputs = 0;
                    break;
            }
        }
    }
}
