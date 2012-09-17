using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using log4net;


namespace Command
{

    /// <summary>
    /// Provides a registry of discovered Commands and Factories, as well as
    /// methods for discovering them.
    /// </summary>
    public class Registry
    {
        // Reference to class logger
        protected static readonly ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Dictionary of command instances keyed by command name
        private IDictionary<string, Command> _commands;
        // Dictionary of types to factory constructors/methods/properties
        private IDictionary<Type, Factory> _factories;
        // Dictionary of types to alternate factories
        protected List<Factory> _alternates;


        /// Return the Command object corresponding to the requested name.
        public Command this[string cmdName] { get { return _commands[cmdName]; } }


        /// Constructor
        public Registry()
        {
            _commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);
            _factories = new Dictionary<Type, Factory>();
            _alternates = new List<Factory>();
        }


        /// <summary>
        /// Registers commands (i.e. methods tagged with the Command attribute)
        /// in the current assembly.
        /// </summary>
        public void RegisterNamespace(string ns)
        {
            RegisterNamespace(Assembly.GetExecutingAssembly(), ns);
        }


        /// <summary>
        /// Registers commands from the supplied assembly. Commands methods must
        /// be tagged with the attribute Command to be locatable.
        /// </summary>
        public void RegisterNamespace(Assembly asm, string ns)
        {
            _log.DebugFormat("Searching for commands under namespace '{0}'...", ns);
            foreach(var t in asm.GetExportedTypes()) {
                if(t.Namespace == ns && t.IsClass) {
                    RegisterClass(t);
                }
            }
        }


        /// <summary>
        /// Registers commands and factories from the supplied class.
        /// Commands must be tagged with the attribute Command to be locatable.
        /// </summary>
        public void RegisterClass(Type t)
        {
            Factory factory;
            Command cmd;
            string desc;

            if(t.IsClass) {
                foreach(var mi in t.GetMembers(BindingFlags.Public|BindingFlags.Instance)) {
                    cmd = null;
                    factory = null;
                    desc = null;
                    foreach(var attr in mi.GetCustomAttributes(false)) {
                        if(attr is DescriptionAttribute) {
                            desc = (attr as DescriptionAttribute).Description;
                        }
                        if(attr is CommandAttribute) {
                            cmd = new Command(t, mi as MethodInfo);
                            Add(cmd);
                        }
                        if(attr is FactoryAttribute) {
                            factory = new Factory(mi);
                            Add(factory);
                        }
                        // Add support for alternate factories
                        if(attr is AlternateFactoryAttribute) {
                            factory = new Factory(mi);
                            Add(factory, true);
                        }
                    }
                    if(cmd != null) {
                        cmd.Description = desc;
                        if(factory != null) {
                            cmd._factory = factory;
                            factory._command = cmd;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Registers the specified Command instance.
        /// </summary>
        public void Add(Command cmd)
        {
            _commands.Add(cmd.Name, cmd);
        }



        /// <summary>
        /// Registers the specified Factory instance.
        /// </summary>
        public void Add(Factory factory)
        {
            Add(factory, false);
        }


        /// <summary>
        /// Registers the specified Factory instance as an alternate mechanism
        /// for obtaining objects of the Factory return type. An alternate means
        /// will be tried when no other non-alternate path to create an object
        /// can be found from the current context state.
        /// </summary>
        public void Add(Factory factory, bool isAlternate)
        {
            if(isAlternate) {
                _alternates.Add(factory);
            }
            else {
                _factories.Add(factory.ReturnType, factory);
            }
        }


        /// <summary>
        /// Checks to see if a command with the given name is available.
        /// </summary>
        public bool Contains(string cmdName)
        {
            return _commands.ContainsKey(cmdName);
        }


        /// <summary>
        /// Checks to see if a Factory for the specified type is available.
        /// </summary>
        public bool Contains(Type type)
        {
            return _factories.ContainsKey(type);
        }


        /// <summary>
        /// Returns the Factory instance for objects of the specified Type.
        /// </summary>
        public Factory GetFactory(Type type)
        {
            return _factories[type];
        }


        /// <summary>
        /// Returns an IEnumerable of the alternate Factory objects registered
        /// for the specified type.
        /// </summary>
        public IEnumerable<Factory> GetAlternates(Type type)
        {
            return _alternates.Where(f => f.ReturnType == type);
        }
    }



}
