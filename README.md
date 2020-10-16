# Gamanet.SimpleDriver
Exit on Escape or Ctrl + C.

Clear line on Backspace.

Enter is ignored.

Expected input is P[Command_char]:[params]:E.

Implemented commands: text - PT:sometext:E, sound - PS:int_frequency,int_duration_ms:E.

"Impossible" packets are ignored by default.

Used:
* System.IO.Pipelines
* Autofac
* XUnit
