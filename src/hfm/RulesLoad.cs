using System;
using System.IO;

using log4net;
using HSVSESSIONLib;
using HSVRULESLOADACVLib;
using HFMCONSTANTSLib;

using Command;
using HFMCmd;


namespace HFM
{

    public class RulesLoad
    {

        /// <summary>
        /// Defines the possible rule file formats.
        /// </summary>
        public enum ERulesFormat
        {
            Native = 0,
            CalcManager = 1
        }


        // Reference to class logger
        protected static readonly ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Reference to HFM HsvRulesLoadACV object
        internal readonly HsvRulesLoadACV HsvRulesLoad;


        [Factory]
        public RulesLoad(Session session)
        {
            HsvRulesLoad = new HsvRulesLoadACV();
            HsvRulesLoad.SetSession(session.HsvSession, (int)tagHFM_LANGUAGES.HFM_LANGUAGE_INSTALLED);
        }


        [Command("Loads an HFM application's calculation rules from a native rule or Calculation Manager XML file")]
        public void LoadRules(
                [Parameter("Path to the source rules file")]
                string rulesFile,
                [Parameter("Path to the load log file; if not specified, defaults to same path " +
                             "and name as the source rules file.",
                 DefaultValue = null)]
                string logFile,
                [Parameter("Scan rules file for syntax errors, rather than loading it",
                 DefaultValue = false)]
                bool scanOnly,
                [Parameter("Check integrity of intercompany transactions following rules load",
                 DefaultValue = false)]
                bool checkIntegrity)
        {
            bool errors = false, warnings = false, info = false;

            if(logFile == null || logFile == "") {
                logFile = Path.ChangeExtension(rulesFile, ".log");
            }

            // Ensure rules file exists and logFile is writeable
            Utilities.FileExists(rulesFile);
            Utilities.FileWriteable(logFile);

            HFM.Try("Loading rules",
                    () => HsvRulesLoad.LoadCalcRules2(rulesFile, logFile, scanOnly, checkIntegrity,
                                                      out errors, out warnings, out info));
            if(errors) {
                _log.Error("Rules load resulted in errors; check log file for details");
                // TODO:  Should we show the warnings here?
            }
            if(warnings) {
                _log.Warn("Rules load resulted in warnings; check log file for details");
                // TODO:  Should we show the warnings here?
            }
        }


        [Command("Extracts an HFM application's rules to a native ASCII or XML file")]
        public void ExtractRules(
                [Parameter("Path to the generated rules extract file")]
                string rulesFile,
                [Parameter("Path to the extract log file; if not specified, defaults to same path " +
                           "and name as extract file.", DefaultValue = null)]
                string logFile,
                [Parameter("Format in which to extract rules", DefaultValue = ERulesFormat.Native)]
                ERulesFormat rulesFormat)
        {
            if(logFile == null || logFile == "") {
                logFile = Path.ChangeExtension(rulesFile, ".log");
            }
            // TODO: Display options etc
            _log.FineFormat("    Rules file: {0}", rulesFile);
            _log.FineFormat("    Log file:     {0}", logFile);

            // Ensure rulesFile and logFile are writeable locations
            Utilities.FileWriteable(rulesFile);
            Utilities.FileWriteable(logFile);

            HFM.Try("Extracting rules",
                    () => HsvRulesLoad.ExtractCalcRulesEx(rulesFile, logFile, (int)rulesFormat));
        }

    }

}