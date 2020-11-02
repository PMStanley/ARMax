# About
This project is a C#/.net Wrapper program/example for ARMaxDLL.dll

Use the program or code included as you want. 
Same license as ARMaxDLL.dll.

The included 'ARMaxDLL.dll' file was downloaded from https://www.ps2savetools.com/

## Project setup 
Using ARMaxDLL.dll from C#/.NET will require you to target x86, not 'AnyCPU'

[Right click on Project] -> Properties -> Build -> Platform target -> x86


## File positions 
Internal file '0' does not exist, when using functions that take file index, valid values are 1 +.
Not all functions exposed by ARMaxDLL.dll are wrapped by the native methods class.

For functions that do not return info about file position or size:

	 return value 0 ==> good; return value other == error 
	 
	 Consult ARMAx wiki for error codes
