namespace VoicemeeterOsdProgram.Core.Types
{
    public interface IVmParamReadable
    {
        public void ReadNotifyChanges();

        public void Read();

        public void ReadIsNotifyChanges(bool isIgnore);
    }
}
