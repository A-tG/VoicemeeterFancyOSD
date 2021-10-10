namespace VoicemeeterOsdProgram.Core.Types
{
    public interface IVmParamReadable
    {
        public void Read()
        {
            ReadIsIgnoreEvent(false);
        }

        public void ReadNoEvent()
        {
            ReadIsIgnoreEvent(true);
        }

        public void ReadIsIgnoreEvent(bool isIgnore);
    }
}
