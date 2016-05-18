# Arduino-Compatible PC Activity Monitor

This is a fork of LorenzoRogai's Arduino PC Monitor. The software runs as a desktop app and requires CoreTemp and GPU-Z to be installed,
as their libraries are required to fetch real-time CPU and GPU data.

This project would be split into two parts: One is the desktop C# program used to obtain the data and transmit it via serial interface. The other is the program that will run on an AVR microcontroller, using object oriented C++ libraries for enabling serial communication and I2C/two-wire interface with a OLED display.

### Differences from original source

The original PC Monitor displays graphs that are generated from data pulled from CoreTemp and GPU-Z. The purpose of this fork is to
simplify the application for the sole purpose of sending serial data to an Arduino-compatible device (or any other device with a serial 
interface) and rely on microcontroller code and project hardware to display output to the user. The simplification of code also is meant to improve compatibility issues some users may have when running the original code, such as removing the need for PerformanceCounter which has issues with porting to the Windows OS across multiple languages.

### AVR compatibility

The current target MCU is on a DigiSpark or compatible device that uses a ATtiny*x*5 microcontroller, but ATtiny85 is recommended for its memory capacity. The USI UART library is used to provide TWI(I2C) communication [see doc here](http://www.atmel.com/Images/doc4300.pdf) and a modified version of the [SEEED OLED library](https://github.com/Seeed-Studio/OLED_Display_128X64) for controlling a display with a SSD130x driver (driver IC for this project may change in the future).
