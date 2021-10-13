namespace VoicemeeterOsdProgram.Core.Types
{
    public struct ValOldNew<T>
    {
        public T oldVal;
        public T newVal;

        public ValOldNew(T oldVal, T newVal)
        {
            this.oldVal = oldVal;
            this.newVal = newVal;
        }
    }
}
