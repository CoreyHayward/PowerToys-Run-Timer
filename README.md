<div align="center">

# PowerToys Run: Timers
  
[![GitHub release](https://img.shields.io/github/v/release/CoreyHayward/PowerToys-Run-Timer?style=flat-square)](https://github.com/CoreyHayward/PowerToys-Run-Timer/releases/latest)
[![GitHub all releases](https://img.shields.io/github/downloads/CoreyHayward/PowerToys-Run-Timer/total?style=flat-square)](https://github.com/CoreyHayward/PowerToys-Run-Timer/releases/)
[![GitHub release (latest by date)](https://img.shields.io/github/downloads/CoreyHayward/PowerToys-Run-Timer/latest/total?style=flat-square)](https://github.com/CoreyHayward/PowerToys-Run-Timer/releases/latest)
[![Mentioned in Awesome PowerToys Run Plugins](https://awesome.re/mentioned-badge-flat.svg)](https://github.com/hlaueriksson/awesome-powertoys-run-plugins)

</div>

Simple [PowerToys Run](https://learn.microsoft.com/windows/powertoys/run) plugin for quickly setting timers.

![Timer Demonstration](/images/Timer.gif)

## Requirements

- PowerToys minimum version 0.76.0

## Installation

- Download the [latest release](https://github.com/CoreyHayward/PowerToys-Run-Timer/releases/) by selecting the architecture that matches your machine: `x64` (more common) or `ARM64`
- Close PowerToys
- Extract the archive to `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`
- Open PowerToys

## Usage
### Setting Timers
- Open PowerToys Run
- Input: "timer {time} {description (optional)}"
- Select the result (ENTER)
- Timer is set and notification is raised when timer is complete

{time} can be specified in a range of formats thanks to [TimeSpanParser](https://github.com/pengowray/TimeSpanParser) some examples include:
- Seconds: 15s | 15 sec(s) | 15 seconds
- Minutes: 1m | 1 min | 1 minute
- Hours: 1h | 1 hr | 1 hour

### Viewing Active Timers
![See active timers](/images/ActiveTimers.png)
- Open PowerToys Run
- Input: "timer"

Timers can also be stopped by clicking the delete button or using the CTRL+ENTER shortcut on a running timer
