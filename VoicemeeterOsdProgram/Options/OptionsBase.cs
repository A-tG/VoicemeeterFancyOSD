using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace VoicemeeterOsdProgram.Options
{
    public abstract class OptionsBase
    {
        public virtual IEnumerable<KeyValuePair<string, string>> ToDict() => new Dictionary<string, string>();

        public virtual void FromDict(Dictionary<string, string> list) { }

        public List<string> GetOptionDescription(string memberName)
        {
            List<string> comments = new();
            var m = GetType().GetProperty(memberName);
            if (m is null) return comments;

            var type = m.PropertyType;
            bool isEnumerable = type.IsGenericType && (m.GetValue(this) is IEnumerable);

            if (m.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute att)
            {
                comments.Add(' ' + att.Description);
            }
            if (type.IsEnum)
            {
                comments.Add(GetEnumValuesDescription(type));
            } else if (isEnumerable)
            {
                var tParams = type.GetGenericArguments();
                if (tParams.Length == 1)
                {
                    var desc = GetEnumValuesDescription(tParams[0]);
                    if (!string.IsNullOrEmpty(desc))
                    {
                        comments.Add(desc);
                    }
                }
            }
            return comments;
        }

        protected string GetEnumValuesDescription(Type t)
        {
            return t.IsEnum ? " Possible values: " + string.Join(", ", t.GetEnumNames()) : string.Empty;
        }

        protected void HandlePropertyChange<T>(ref T oldVal, ref T newVal, EventHandler<T> eventIfNotEqual)
        {
            if (oldVal.Equals(newVal)) return;

            oldVal = newVal;
            eventIfNotEqual?.Invoke(this, newVal);
        }

        protected void HandlePropertyChange<T>(ref IEnumerable<T> oldVal, ref IEnumerable<T> newVal, EventHandler<IEnumerable<T>> eventIfNotEqual)
        {
            if (oldVal.SequenceEqual<T>(newVal)) return;

            oldVal = newVal;
            eventIfNotEqual?.Invoke(this, newVal);
        }

        protected bool TryParseFrom(string toMemberName, string value)
        {
            try
            {
                ParseFrom(toMemberName, value);
                return true;
            }
            catch { }
            return false;
        }

        protected void ParseFrom(string toMemberName, string value)
        {
            var prop = GetType().GetProperty(toMemberName);

            var type = prop.PropertyType;
            object convRes = ParseFrom(type, value);

            if (convRes is not null)
            {
                prop.SetValue(this, convRes);
            }
        }

        protected object ParseFrom(Type toType, string value)
        {
            object res;
            if (toType.IsEnum)
            {
                res = Enum.Parse(toType, value);
            }
            else
            {
                res = Convert.ChangeType(value, toType);
            }
            return res;
        }

        protected IEnumerable<T> ParseEnumerableFrom<T>(string val)
        {
            List<T> resList = new();
            var values = val.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (string v in values)
            {
                try
                {
                    resList.Add((T)ParseFrom(typeof(T), v));
                }
                catch { }
            }
            return resList;
        }
    }
}
