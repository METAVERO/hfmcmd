using System;
using System.Collections.Generic;

using log4net;

using Command;
using CommandLine;



namespace HFM
{

    public class LoadExtractOption : ISetting
    {

        protected Type _optionType;
        protected object _option;

        public string Name { get { return (string)GetOptionProperty("Name"); } }
        public Type ParameterType { get { return GetOptionProperty("CurrentValue").GetType(); } }
        public string Description { get { return null; } }
        public bool IsSensitive { get { return false; } }
        public bool HasDefaultValue { get { return true; } }
        public object DefaultValue { get { return GetOptionProperty("DefaultValue"); } }


        internal LoadExtractOption(Type optionType, object option) {
            _optionType = optionType;
        }


        protected object GetOptionProperty(string prop)
        {
            return _optionType.GetProperty(prop).GetValue(_option, null);
        }

    }



    /// <summary>
    /// Base class for all HFM Load/Extract option collections.
    /// These are implementations of the TODO: SettingsCollection class, used to pass
    /// around the myriad options that govern the behaviour of HFM loads and
    /// extracts of Metadata, Security, Rules, Member Lists, Data, and Journals.
    /// </summary>
    /// <remarks>
    /// These are a sort of hybrid struct/collection/enum:
    /// - each collection has a fixed set of members
    /// - members of the collection can be accessed by ordinal or name, and
    ///   the valid ordinals and names can be determined from a corresponding
    ///   enum type.
    /// - members of the collection have a common set of methods and properties,
    ///   which can return information about the valid values/ranges, default
    ///   value etc
    /// Unfortunately, the collections do not share a common base class, and
    /// so to determine the valid members of the collection, get the default
    /// values for an item in the collection, and set a current value in a
    /// collection for all load and extract option sets, we need to make heavy
    /// use of reflection.
    /// </remarks>
    public abstract class LoadExtractOptions : ISettingsCollection
    {
        // Reference to class logger
        protected static readonly ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected Type _optionsType;
        protected Type _optionType;
        protected Type _enumType;
        protected object _options;
        protected Dictionary<string, LoadExtractOption> _settings;


        public object this[string key]
        {
            get {
                return GetOptionProperty(_settings[key], "CurrentValue");
            }
            set {
                _optionType.GetProperty("CurrentValue").SetValue(_settings[key], value, null);
            }
        }


        public LoadExtractOptions(Type optionsType, Type optionType, Type enumType, object options)
        {
            _optionsType = optionsType;
            _optionType = optionType;
            _enumType = enumType;
            _options = options;
            _settings = new Dictionary<string, LoadExtractOption>();

            foreach(var mbr in Enum.GetNames(_enumType)) {
                // Filter out _MIN and _MAX enum members if present
                if(mbr.EndsWith("_MIN") || mbr.EndsWith("_MAX")) {
                    continue;
                }

                // Obtain the option object corresponding to the enum member
                var option = new LoadExtractOption(optionType, optionsType.GetMethod("get_Item")
                        .Invoke(_options, new object[] { Enum.Parse(_enumType, mbr )}));
                _settings[option.Name] = option;
            }
        }


        public IEnumerable<ISetting> Each()
        {
            foreach(var option in _settings.Values) {
                yield return option;
            }
        }


        protected object GetOptionProperty(object option, string prop)
        {
            return _optionType.GetProperty(prop).GetValue(option, null);
        }


        public List<string> ValidKeys()
        {
            return new List<string>(_settings.Keys);
        }

        public object DefaultValue(string key)
        {
            var option = _settings[key];
            return option.GetType().GetProperty("DefaultValue").GetValue(option, new object[] { key });
        }

    }

}
