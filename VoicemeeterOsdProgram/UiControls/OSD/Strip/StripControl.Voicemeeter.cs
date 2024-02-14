using VoicemeeterOsdProgram.Core.Types;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip;

partial class StripControl
{
    public string defaultLabel = string.Empty;

    private VoicemeeterStrParam m_vmParam;

    public VoicemeeterStrParam VmParameter
    {
        get => m_vmParam;
        set
        {
            if (value is null)
            {
                if (m_vmParam is not null)
                {
                    m_vmParam.ValueRead -= OnVmValueRead;
                    m_vmParam = null;
                }
                return;
            }

            m_vmParam = value;
            m_vmParam.ValueRead += OnVmValueRead;
            m_vmParam.ReadNotifyChanges();
        }
    }

    private void OnVmValueRead(object sender, ValOldNew<string> e)
    {
        string name = e.newVal;
        string newName = string.IsNullOrEmpty(name) ? defaultLabel : name;
        if (StripLabel.Text == newName) return;

        StripLabel.Text = newName;
    }
}
