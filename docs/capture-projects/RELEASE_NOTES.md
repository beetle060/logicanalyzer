# LogicAnalyzer 6.0 · Capture Projects v1

This fork release, dated 2026-11-22, adds reusable capture setups and a connector-oriented view to the LogicAnalyzer 6.0 desktop application.

OpenAI Codex implemented the Capture Projects changes from feature requests and feedback by beetle060, who also tested them with their logic analyzers.

- **Capture projects.** Save channel names, enabled channels, colors, and capture and trigger settings in a `.laproj` file. Load and save projects from the **File** menu or **Capture settings**. You can select a project before opening the analyzer and review it before starting a capture.
- **Channel-list import.** Load a version 2 Falcon-format JSON channel list. The importer assigns names by channel number so repeated names can be used on different channels.
- **Physical channel layout.** On 24-channel analyzers, show connectors as two rows per device with the lower row reversed. The wider, resizable Capture dialog can scroll horizontally on narrower displays. The app remembers the layout preference.
- **Last-used settings.** Capture settings accepted in the dialog are written to the application settings directory so they can be restored the next time the dialog opens.

## Build and use

The desktop build was verified for Linux x86-64 with .NET 8. Project import and save/load were checked with a simulated 48-channel setup, and the physical layout was checked with 24, 48, and 72 simulated channels. This release covers the desktop application. See the [Linux build and usage guide](https://github.com/beetle060/logicanalyzer/tree/capture-projects/docs/capture-projects) and the [upstream documentation](https://github.com/gusmanb/logicanalyzer/wiki).
