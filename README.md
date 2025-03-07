# SpeedTestConsole
An internet speed test CLI application; made beautiful with [Spectre.Console](https://github.com/spectreconsole/spectre.console). 

<p align="left">
    <a href="https://github.com/FrankRay78/SpeedTestConsole/issues/new?labels=needs%20triage,bug&template=bug-report---.md">Report Bug</a>
    -
    <a href="https://github.com/FrankRay78/SpeedTestConsole/issues/new?labels=needs%20triage,enhancement&template=feature-request---.md">Request Feature</a>
</p>

<br />


## About The Project
A cross-platform command-line application for performing network speed tests, including server discovery, latency measurement, download and upload speed testing.

SpeedTestConsole is not endorsed by or related to [Speedtest by Ookla](https://www.speedtest.net/) in any way, although their servers are used under the hood.

The obligatory screenshot (as of 6 March 2025):

![2025-03-25 SpeedTestConsole](https://github.com/user-attachments/assets/b2df7f36-f620-4683-88ad-81b913b24070)

<br />


## Background
The idea for this project came from my experience as the Spectre.Console CLI sub-system maintainer, whilst never having used the library for my own production quality application. My motivation is to become expert at developing excellent command line applications, following best practices like the [Command Line Interface Guidelines](https://clig.dev/), and then taking the experience back into my maintainer role. 

This is also known as 'dogfooding' in the tech industry ie. using your own product before expecting others to do the same.

<br />


## Getting Started
I use Windows 10, Visual Studio 2022 Community, and Microsoft .Net 8.0 to develop and build the codebase - you will need the same, or similar.

Then clone this repository locally and build.

<br />


## Usage
`SpeedTestConsole --help` will display detailed usage instructions.

```txt
C:\>SpeedTestConsole.exe -h

   _____                            __  ______                 __    ______                                  __
  / ___/    ____   ___   ___   ____/ / /_  __/  ___    _____  / /_  / ____/  ____    ____    _____  ____    / /  ___
  \__ \    / __ \ / _ \ / _ \ / __  /   / /    / _ \  / ___/ / __/ / /      / __ \  / __ \  / ___/ / __ \  / /  / _ \
 ___/ /   / /_/ //  __//  __// /_/ /   / /    /  __/ (__  ) / /_  / /___   / /_/ / / / / / (__  ) / /_/ / / /  /  __/
/____/   / .___/ \___/ \___/ \__,_/   /_/     \___/ /____/  \__/  \____/   \____/ /_/ /_/ /____/  \____/ /_/   \___/
        /_/

DESCRIPTION:
Internet speed tester including server discovery, latency measurement, download and upload speed testing.

USAGE:
    SpeedTestConsole [OPTIONS] [COMMAND]

OPTIONS:
                         DEFAULT
    -h, --help                            Prints help information.
        --no-download                     Do not perform download test.
        --no-upload                       Do not perform upload test.
    -t, --timestamp                       Include a timestamp.
    -u, --unit           BitsPerSecond    The speed unit.
                                             BitsPerSecond, BytesPerSecond
        --verbosity      Normal           The verbosity level.
                                             Minimal, Normal, Debug
                                          Minimal is ideal for batch scripts and redirected output.

COMMANDS:
    servers    Show the nearest speed test servers.
```

<br />


## Roadmap
- [X] Download speed test
- [X] Upload speed test
- [ ] User-configurable switches
   - [X] BitsPerSecond or BytesPerSecond
   - [ ] Fixed speed unit (eg. Mbps, Gbps)
   - [X] Verbosity of output
   - [X] ~~`--plain` switch for minimal output~~ (nb. implemented by verbosity switch)
   - [ ] Maximum speed test (time)
   - [ ] Maximum speed test (size transferred)
- [ ] Periodically repeat tests
- [ ] Run unit tests on PR
- [ ] NuGet package for the core library

See the [open issues](https://github.com/FrankRay78/SpeedTestConsole/issues) for a full list of proposed features (and known issues).

<br />


##  Contributing
> [!IMPORTANT]\
> I'm not currently accepting pull requests for this project. 

You can contribute by [opening a new issue](https://github.com/FrankRay78/SpeedTestConsole/issues/new/choose) or commenting on existing issues, and you are most welcome to fork the repository for your own purposes. 

But please **don't be offended** if I close or delete issues as I see fit.

<br />


## License
Distributed under the MIT license. See `LICENSE` for more information.

<br />


## Contact
Frank Ray - [LinkedIn](https://www.linkedin.com/in/frankray/) - [Better Software UK](https://bettersoftware.uk)

Project Link: [https://github.com/FrankRay78/SpeedTestConsole](https://github.com/FrankRay78/SpeedTestConsole)

<br />


## Acknowledgments
* [Spectre.Console](https://github.com/spectreconsole/spectre.console)
* [SpeedTestSharp](https://github.com/manuelmayer-dev/SpeedTestSharp)
* [Best-README-Template](https://github.com/othneildrew/Best-README-Template)
* [Standard Readme](https://github.com/RichardLitt/standard-readme)
