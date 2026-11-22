# LogicAnalyzer Capture Projects

Capture projects let you reuse a measurement setup. A `.laproj` file stores channel names, enabled channels, colors, sample rate, sample counts, and trigger settings. To open or save recorded samples in a `.lac` file, use **File → Open…** or **File → Save…**.

## Build and run on Ubuntu 24.04

These commands build the desktop application for 64-bit x86 Linux. The .NET 8 SDK includes the runtime used by the resulting build. The Python development package supplies the shared library used by the Sigrok decoders. [Install .NET on Ubuntu](https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu-install#ubuntu-2404)

```bash
sudo apt update
sudo apt install dotnet-sdk-8.0 python3-dev
git clone --branch capture-projects https://github.com/beetle060/logicanalyzer.git
cd logicanalyzer

dotnet publish Software/LogicAnalyzer/LogicAnalyzer/LogicAnalyzer.csproj \
  -c Release -r linux-x64 --self-contained false -o ./bin/linux-x64

./bin/linux-x64/LogicAnalyzer
```

If you already cloned the fork, run `git switch capture-projects` and `git pull --ff-only` in the repository, then run the publish and launch commands. If you downloaded a release source archive, extract it and run those same two commands from the extracted directory.

## Save and load a project

1. Open your analyzer. For a setup with multiple analyzers, select **Multidevice** before clicking **Open device**.
2. Click **Capture** to open **Capture settings**. Configure your channels, sample rate, and trigger.
3. Click **Save project…** to write a `.laproj` file. You can save directly from this dialog before starting a measurement. **File → Save project…** is also available from the main window.
4. To reuse the setup, select **File → Load project…** and choose the file. You can choose it before connecting the analyzer. Open a compatible device and click **Capture** to review the loaded settings. Click **Accept** when you are ready to capture.

**Load project…** in **Capture settings** applies the file directly to the open dialog. Both load commands also accept version 2 JSON channel lists using the `falcon030-logic-analyzer-connections` schema. The import enables the listed channels and assigns names by channel number. Colors and trigger settings come from the current setup. A file needs a device setup with enough channels for its highest channel number.

## Match the physical connectors

Turn on **Settings → Physical channel layout** for a setup with 24 channels per analyzer. Each device appears as two rows. Read both rows from left to right.

| Analyzer | Top row | Bottom row |
| --- | --- | --- |
| 1 | 1 → 12 | 24 → 13 |
| 2 | 25 → 36 | 48 → 37 |
| 3 | 49 → 60 | 72 → 61 |

The dialog opens up to 50% wider in this mode. Drag its edges to resize it, or scroll horizontally when the window is too narrow to show all twelve columns. The app remembers your layout choice across restarts. Projects keep the same channel numbers in both layouts.

## Upstream project and license

This fork builds on Gusmanb’s LogicAnalyzer. For hardware setup, firmware, and the original documentation, see the [upstream project](https://github.com/gusmanb/logicanalyzer) and its [wiki](https://github.com/gusmanb/logicanalyzer/wiki). The repository includes the [GNU GPL version 3 license](../../LICENSE).
