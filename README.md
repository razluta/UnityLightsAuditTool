# Unity Lights Audit Tool [![License](https://img.shields.io/badge/License-MIT-lightgrey.svg?style=flat)](http://mit-license.org)

This repository is a Unity Editor tool for auditing all the lights in your Unity scene(s).

![](/Screenshots/UnityLightsAuditTool_screenshot01.png)

The tool has been verified on the following versions of Unity:
- 2021.3

*  *  *  *  *

The tool is a utility for Unity game developers that helps analyze, sort and display data about all the lights in the scene(s).

*  *  *  *  *

## Features

* Collect and display all lights in a scene
* Sort (toggle ascending/descending by clicking on the collumn title) lights based on different properties (type, range, intensity, etc.)
* Easily identify lights causing issues or impacting performance (due to size, shadow quality, etc)

*  *  *  *  *

## Installation

To use the Unity Lights Auditor Tool in your Unity project, simply:

### Option A) Clone or download the repository and drop it in your Unity project.
1. Clone or download the repository
2. Import the `UnityLightsAuditorTool` folder into your Unity project's `Assets` folder

### Option B) Option B) Add the repository to the package manifest (go in YourProject/Packages/ and open the "manifest.json" file and add "com..." line in the depenencies section). If you don't have Git installed, Unity will require you to install it.
```
{
  "dependencies": {
      ...
      "com.razluta.unitylightsaudittool": "https://github.com/razluta/UnityLightsAuditTool.git"
      ...
  }
}
```
### Option C) Add the repository to the Unity Package Manager using the Package Manager dropdown.
The repository is at: https://github.com/razluta/UnityLightsAuditTool.git

*  *  *  *  *

## Using the tool

To start using the tool, go to: (Menu bar) Raz's Tools > Lights Audit Tool.
The functionality is as follows:
* the "Reset parameters" button will revert the tool to only filter for Realtime lights
* the "Audit scene" button will search the scene for lights and by default arrange them in descending order of range magnitude (bigger lights at the top)
* the "Clear audit results" will scrap the current search and clear the UI
* each of the column headers (Name, Type, etc) will toggle ascending/descending sorting of the lights based on that parameter (by name from A-Z or flipped, by Intensity from 0 going up to the highest intensity or backwards).
* clicking on any light in the list will ping and select the light in the scene

*  *  *  *  *

## Contributing

If you would like to contribute to the Unity Lights Auditor Tool, please:

1. Fork the repository
2. Create a new branch
3. Make your changes
4. Submit a pull request

*  *  *  *  *

## License

The Unity Lights Auditor Tool is licensed under the MIT License. See the `LICENSE` file for details.
