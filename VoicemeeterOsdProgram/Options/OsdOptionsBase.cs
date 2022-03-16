using System;
using System.Collections.Generic;
using System.ComponentModel;
using VoicemeeterOsdProgram.Types;

namespace VoicemeeterOsdProgram.Options
{
    public class OsdOptionsBase : OptionsBase
    {
        protected uint m_displayIndex;
        protected HorAlignment m_horizontalAlignment = HorAlignment.Right;
        protected VertAlignment m_verticalAlignment = VertAlignment.Top;

        [Description("0 - is a primary display, 1 - is a secondary, etc")]
        public uint DisplayIndex
        {
            get => m_displayIndex;
            set => HandlePropertyChange(ref m_displayIndex, ref value, DisplayIndexChanged);
        }

        public HorAlignment HorizontalAlignment
        {
            get => m_horizontalAlignment;
            set => HandlePropertyChange(ref m_horizontalAlignment, ref value, HorizontalAlignmentChanged);
        }

        public VertAlignment VerticalAlignment
        {
            get => m_verticalAlignment;
            set => HandlePropertyChange(ref m_verticalAlignment, ref value, VerticalAlignmentChanged);
        }

        public override IEnumerable<KeyValuePair<string, string>> ToDict()
        {
            Dictionary<string, string> list = new(base.ToDict());
            list.Add(nameof(DisplayIndex), DisplayIndex.ToString());
            list.Add(nameof(HorizontalAlignment), HorizontalAlignment.ToString());
            list.Add(nameof(VerticalAlignment), VerticalAlignment.ToString());
            return list;
        }

        public override void FromDict(Dictionary<string, string> list)
        {
            List<string> names = new();
            names.Add(nameof(DisplayIndex));
            names.Add(nameof(HorizontalAlignment));
            names.Add(nameof(VerticalAlignment));
            foreach (var n in names)
            {
                if (list.ContainsKey(n))
                {
                    TryParseFrom(n, list[n]);
                }
            }
        }

        public event EventHandler<uint> DisplayIndexChanged;
        public event EventHandler<HorAlignment> HorizontalAlignmentChanged;
        public event EventHandler<VertAlignment> VerticalAlignmentChanged;
    }
}
