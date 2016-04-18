# Arduino-Compatible PC Activity Monitor

This is a fork of LorenzoRogai's Arduino PC Monitor. The software runs as a desktop app and requires CoreTemp and GPU-Z to be installed,
as their libraries are required to fetch real-time CPU and GPU data. 

### Differences from original source

The original PC Monitor displays graphs that are generated from data pulled from CoreTemp and GPU-Z. The purpose of this fork is to
simplify the application for the sole purpose of sending serial data to an Arduino-compatible device (or any other device with a serial 
interface) and rely on microcontroller code and project hardware to display output to the user.

Currently, the application displays a GUI with graphs for real-time data updated every second. Because of its reliance on CoreTemp and GPU-Z,
it's not always necessary to display this graphical data, and sometimes it may be redundant. The simplification of code also is meant to improve compatibility issues some users may have when 
running the original code, such as removing the need for PerformanceCounter which has issues with porting to the Windows OS across multiple languages.
