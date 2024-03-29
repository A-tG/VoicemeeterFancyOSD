﻿using System;
using VoicemeeterOsdProgram.Core.Types;
using System.Windows;
using VoicemeeterOsdProgram.Types;
using Octokit;

namespace VoicemeeterOsdProgram.UiControls.OSD.Strip;

partial class ButtonContainer : IOsdChildElement
{
    public IOsdRootElement OsdParent { get; set; }

    public Func<bool> IsAlwaysVisible { get; set; } = () => false;

    public Func<bool> IsNeverShow { get; set; } = () => false;

    private VoicemeeterNumParam m_vmParam;

    public VoicemeeterNumParam VmParameter
    {
        get => m_vmParam;
        set
        {
            if (value is null)
            {
                if (m_vmParam is not null)
                {
                    m_vmParam.ReadValueChanged -= OnVmValueChanged;
                    m_vmParam.ValueRead -= OnVmValueRead;
                    Btn.Click -= OnBtnClick;
                    m_vmParam = null;
                }
                return;
            }

            m_vmParam = value;
            Btn.State = (uint)m_vmParam.Value;
            m_vmParam.ReadValueChanged += OnVmValueChanged;
            m_vmParam.ValueRead += OnVmValueRead;
            Btn.Click += OnBtnClick;
        }
    }

    private void OnVmValueRead(object sender, ValOldNew<float> e) => Btn.State = (uint)e.newVal;

    private void OnVmValueChanged(object sender, ValOldNew<float> e)
    {
        bool hasOsdParent = OsdParent is not null;
        if (hasOsdParent)
        {
            OsdParent.HasChangesFlag = true;
        }

        if (!IsNeverShow())
        {
            Highlight();
            Visibility = Visibility.Visible;
            if (hasOsdParent)
            {
                OsdParent.HasAnyChildVisibleFlag = true;
            }
        }
    }

    private void OnBtnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not OutlineTglBtn btn) return;

        m_vmParam.Write(btn.State);
    }
}
