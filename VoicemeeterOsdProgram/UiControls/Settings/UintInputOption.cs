namespace VoicemeeterOsdProgram.UiControls.Settings
{
    public class UintInputOption : InputOption
    {
        public UintInputOption() : base()
        {
            filterTextFunc = IsOnlyDigits;
        }

        private bool IsOnlyDigits(string text)
        {
            foreach (var ch in text)
            {
                if (!char.IsDigit(ch)) return false;
            }
            return true;
        }
    }
}
