ARMax
=========

ARMax is a Delphi unit for working with Action Replay Max (AR Max) save files for the Playstation 2.
It is the core code used in ARMaxDLL available at [PS2 Save Tools].




Usage
--------------

Simply add the unit to your project as you would any other Delphi unit.

See Wiki for API: https://github.com/PMStanley/ARMax/wiki 



Additional information
-----------

*ARMax.pas* uses 2 additional non-standard Delphi units which are included for completenes but are not part of the package or licence.

* *crc32.pas* - Standard CRC32 implementation by NAG Software Solutions (no longer around AFAIK)
* *myLZAri.pas* - An implementation of LZAri modified by [gothi] to use TMemorySteam instead of a local file.  Original author unknown.

The *roundUp* function was provided by [Ross Ridge]

TODO
----
Remove unnecessary references and debug code utilising dialogs and memos.


License
----

* ARMax.pas - MIT
* CRC32.pas - unknown at this time
* myLZAri.pas - unknown at this time


[gothi]:http://gothi.co.uk
[PS2 Save Tools]:http://www.ps2savetools.com
[Ross Ridge]:http://www.csclub.uwaterloo.ca:11068/mymc/
