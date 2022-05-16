using AtgDev.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace VoicemeeterOsdProgram.Options
{
    public abstract class OptionsBase
    {
        private Dictionary<string, PropertyInfo> m_Properties;

        public Logger logger;

        private Dictionary<string, PropertyInfo> Properties
        {
            get
            {
                if (m_Properties is null)
                {
                    m_Properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(p => p.Name);
                }
                return m_Properties;
            }
        }

        // It's easier to parse simple types automatically,
        // then more complex types like Array, List can be parsed manually in overriden method 
        public virtual IEnumerable<KeyValuePair<string, string>> ToDict() => ToDictSimpleTypesAuto();

        public virtual void FromDict(Dictionary<string, string> dict) => FromDictSimpleTypesAuto(dict);

        public virtual bool TryParseFrom(string toPropertyName, string fromVal)
        {
            try
            {
                ParseFrom(toPropertyName, fromVal);
                return true;
            }
            catch (Exception e)
            {
                logger?.LogError($"Parsing to option \"{toPropertyName}\" from value \"{fromVal}\": {e.GetType} {e.Message}");
            }
            return false;
        }

        public virtual bool TryParseTo(string fromPropertyName, out string toVal)
        {
            try
            {
                toVal = Parse(fromPropertyName);
                return true;
            }
            catch (Exception e)
            {
                logger?.LogError($"Parsing from option \"{fromPropertyName}\": {e.GetType} {e.Message}");
            }
            toVal = "";
            return false;
        }

        public IEnumerable<KeyValuePair<string, string>> ToDictSimpleTypesAuto() => ToDictSkipComplexTypesAuto(true);

        public void FromDictSimpleTypesAuto(Dictionary<string, string> dict) => FromDictSkipComplexTypesAuto(dict, true);

        public List<string> GetOptionDescription(string memberName) => GetOptionDescription(GetType().GetProperty(memberName));

        protected IEnumerable<KeyValuePair<string, string>> ToDictAllTypes() => ToDictSkipComplexTypesAuto(false);

        protected void FromDictAllTypes(Dictionary<string, string> dict) => FromDictSkipComplexTypesAuto(dict, false);

        protected bool IsSimpleType(Type t) => t.IsPrimitive || t.IsEnum || 
            (t == typeof(string)) || (t == typeof(decimal));

        protected void FromDictSkipComplexTypesAuto(Dictionary<string, string> dict, bool isSkip)
        {
            foreach (var item in Properties)
            {
                var p = item.Value;
                if (isSkip && !IsSimpleType(p.PropertyType)) continue;

                string name = p.Name;
                if (dict.ContainsKey(name))
                {
                    TryParseFrom(name, dict[name]);
                }
            }
        }

        public IEnumerable<KeyValuePair<string, string>> ToDictSkipComplexTypesAuto(bool isSkip)
        {
            Dictionary<string, string> dict = new();
            foreach (var item in Properties)
            {
                var p = item.Value;
                if (isSkip && !IsSimpleType(p.PropertyType)) continue;

                if (TryParseTo(p.Name, out string val))
                {
                    dict.Add(p.Name, val);
                }
            }
            return dict;
        }

        protected string GetEnumerableDescription(Type t)
        {
            string desc = "";
            var tParams = t.GetGenericArguments();
            if (tParams.Length == 1)
            {
                desc = GetEnumValuesDescription(tParams[0]);
            }
            return desc;
        }

        protected string GetEnumValuesDescription(Type t)
        {
            return t.IsEnum ? "Possible values: " + string.Join(", ", t.GetEnumNames()) : string.Empty;
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

        protected void ParseFrom(string toPropertyName, string fromVal)
        {
            var prop = GetType().GetProperty(toPropertyName, BindingFlags.Public | BindingFlags.Instance);
            ParseFrom(prop, fromVal);
        }

        protected object ParseFrom(Type toType, string fromVal)
        {
            object res;
            if (toType.IsEnum)
            {
                res = Enum.Parse(toType, fromVal, true);
            }
            else
            {
                res = Convert.ChangeType(fromVal, toType, CultureInfo.InvariantCulture);
            }
            return res;
        }

        protected string Parse(string fromPropertyName)
        {
            return Convert.ToString(Properties[fromPropertyName].GetValue(this), CultureInfo.InvariantCulture);
        }

        protected IEnumerable<T> ParseEnumerableFrom<T>(string fromVal, string separator)
        {
            List<T> resList = new();
            var values = fromVal.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (string v in values)
            {
                try
                {
                    resList.Add((T)ParseFrom(typeof(T), v));
                }
                catch (Exception e)
                {
                    logger?.LogError($"Parsing value \"{v}\" from enumerable \"{fromVal}\": {e.GetType} {e.Message}");
                }
            }
            return resList;
        }

        private void ParseFrom(PropertyInfo toProp, string fromVal)
        {
            var type = toProp.PropertyType;
            object convRes = ParseFrom(type, fromVal);

            if (convRes is not null)
            {
                toProp.SetValue(this, convRes);
            }
        }

        private List<string> GetOptionDescription(PropertyInfo p)
        {
            List<string> comments = new();
            if (p is null) return comments;

            var type = p.PropertyType;
            bool isEnumerable = type.IsGenericType && (p.GetValue(this) is IEnumerable);

            if (p.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute att)
            {
                comments.Add(att.Description);
            }
            if (type.IsEnum)
            {
                comments.Add(GetEnumValuesDescription(type));
            }
            else if (isEnumerable)
            {
                string desc = GetEnumerableDescription(type);
                if (!string.IsNullOrEmpty(desc))
                {
                    comments.Add(desc);
                }
            }
            return comments;
        }
    }
}
