# HFMCmd

HFMCmd is a command-line tool for performing or automating operations in Oracle
Hyperion Financial Management (HFM). It provides a command-line interface for
performing a wide range of repetitive and/or bulk operations on an HFM
application in a simple manner, and facilitates lights out automation of HFM.

HFMCmd uses the published HFM API to perform operations against an HFM
application, but it provides more than just a thin wrapper over the HFM API:
- Where an API function might operate on a single item (e.g. the
  Consolidate API function takes a single Scenario / Year / Entity combination),
  the corresponding command in HFMCmd will typically accept a collection of members
  for each dimension, and automatically manage the looping over each combination.
- Most commands with a large number of parameters provide sensible defaults for
  many parameters. You can consult the online help to determine which parameters
  are required, and which have default values.
- Long-running operations provide progress bars that provide feedback as the
  operation completes.
- Substitution variables can be defined (or environment variables can be used),
  and can then be used virtually anywhere in a command.

In this manner, bulk operations can be performed quickly and easily on large
numbers of scenarios, entities, etc.

## * Important Note Regarding HFM 11.1.2.4 and later *

Oracle recently released version 11.1.2.4 of HFM, which has been completely rearchitected to use the Java technology stack. While this makes strategic sense for Oracle and HFM, it comes at a heavy cost - the new version of HFM has a totally different API, which is not backwards compatible.

This means that HFMCmd cannot be updated to work with the new version of HFM.

Instead, a complete rewrite in Java would be necessary to provide the same functionality for all versions of HFM from 11.1.2.4 and going forward. HFMCmd will continue to work with versions of HFM up to 11.1.2.3, __but will not ever support HFM version 11.1.2.4 (or later)__.


## Available Commands

The available commands are continuously being added to, but already there are
commands available for performing most operations associated with:
* Metadata management (e.g. loading / extracting metadata, rules, etc)
* Calculation, consolidation, translation, etc
* Data loads and extracts
* Process management (starting, promoting, rejecting, submitting, approving etc)
* Document management (e.g. loading / extracting web forms, reports, etc)

To see a complete list of available commands, run:

    HFMCmd Help Commands

To get detailed help for a specific command, use:

    HFMCmd Help <CommandName>


## Downloading Pre-Built Binaries

HFMCmd currently builds with .NET 3.5 and later. However, the builds only
support early binding, which means the order of methods and the argument signature
of methods in the DLLs of your HFM installation *must* match the order of methods
and argument signature of the DLLs that HFMCmd was built against.

With most software, this wouldn't be a problem; new functionality would always be
added via new methods added at the end of the COM virtual dispatch table, which would
mean that software compiled against older versions would continue to work with newer
releases. However, the HFM API is very poor at maintaining backwards compatibility;
with every new release, and even some patch updates, the public API methods end up in
new locations in the virtual dispatch table, and occasioally, the signature of existing
methods is changed.

As a result, you should choose the correct HFMCmd pre-built binary for the version
of HFM that you are running. The correct version will be the HFMCmd built for the version
and patch level you are running; or, if no build is available for your specific release,
the nearest build below your version of HFM. For example, if you are running HFM version
11.1.2.2.500, you should select the build for 11.1.2.2.300. If you attempt to use HFMCmd
against a different version of HFM, you will likely get a version compatibility error.

Pre-built binaries are available at https://github.com/agardiner/hfmcmd/releases.


## Building HFMCmd

As an alternative to downloading pre-built binaries, any machine on which .NET
3.5 or later is installed ought to be able to build HFMCmd from source. This is
as simple as downloading the source code from Github, unzipping it, and running
the appropriate `build_for_<HFM version>.bat` file.


## Installation

HFMCmd is built as a single standalone .EXE file; no installation is required,
simply unzip it to a directory somewhere, and it is ready to run.

### Pre-Requisites

HFMCmd does require an existing installation of the .NET framework, as already
described. It also requires at least a client installation of HFM on the machine
where it is to run.


## Running HFMCmd

HFMCmd supports the following modes of operation:

1.  A single command can be entered on the command-line, along with
    all of the options it requires as keyword arguments, e.g.

    ```
      HFMCmd.exe Consolidate UserName:admin Password:password ClusterName:PROD
          AppName:IFRS Scenario:Actual Year:2008 Periods:May-July Entity:GROUP
    ```

2.  Commands can be read from a command-file specified on the command-line, e.g.

    ```
      HFMCmd.exe consolidate.hfm
    ```

    Contents of consolidate.hfm:

    ```
      SetLogonInfo:
        UserName: admin
        Password: password

      OpenApplication:
        ClusterName: PROD
        AppName: IFRS

      Consolidate:
        Scenario: Actual
        Year: 2008
        Periods: May-July
        Entity:
          - GROUP
          - {LE064.[Base]}
    ```

    Alternatively runtime variables can be passed in command line for specified
    settings.

    ```
    HFMCmd.exe consolidate.hfm Year:2012
    ```

    Contents of consolidate.hfm:

    ```
      SetLogonInfo:
        UserName: admin
        Password: password

      OpenApplication:
        ClusterName: PROD
        AppName: IFRS

      Consolidate:
        Scenario: Actual
        Year: %Year%
        Periods: May-July
        Entity:
          - GROUP
          - {LE064.[Base]}
    ```

    In the above example `%Year%` would be replaced with `2012`.


3. An interactive shell can be started, where commands can be entered at an hfm>
   prompt. To enter the shell, start HFMCmd with no arguments, i.e.

    ```
    HFMCmd
    ```

## Documentation

HFMCmd provides extensive online help capabilities, via the Help command.
* Basic instructions on running HFMCmd can be obtained by running:
    ```
    HFMCmd Help
    ```
* Help on the available commands can be obtained by running:
    ```
    HFMCmd Help Commands
    ```
* Help on a specific command can be obtained by running:
    ```
    HFMCmd Help <Command>
    ```

Additionally, there is a Wiki containing further documentation. It can be found
at [HFMCmd Wiki](https://github.com/agardiner/hfmcmd/wiki). This is in the early
stages of development, but will be added to over time.


## Supported Versions of HFM

HFMCmd has been designed to work with multiple different versions of HFM, and
should work with versions 9.3 through to 11.1.2.3, including support for the new
extended dimensionality introduced in 11.1.2.2.


## Licence

HFMCmd is open source software, licenced under a BSD-style licence. That means
it is free to use or modify by anyone for any purpose - but no responsibility is
taken for any bugs, issues or losses that may arise in the use of the software.
See the LICENCE file for more details.


## Issues / Bugs / Feature Requests

If you do identify an issue, bug, or missing feature, you can log an Issue at
https://github.com/agardiner/hfmcmd/issues. There is no guarantee your issue
will be addressed, but you have a better chance if you log an issue and provide
clear and concise details.

