# WAD2PK3
A program to convert Doom WAD files to the [PK3](https://wiki.srb2.org/wiki/PK3) format, written in C# and .NET.

# Command-line usage
Passing parameters `-input` and `-output` will open a command-line window instead of the GUI interface.
Avaliable arguments:

| Argument | Description |
| -------- | ----------- |
| -noextensions | Don't write extensions to filenames. |
| -extlowercase | Don't write uppercase extensions to filenames. |
| -keepprefixes | Don't strip `LUA_` or `SOC_` prefixes. |
| -nomusfolder | Don't make a folder for music files. |
| -nosfxfolder | Don't make a folder for sound effects. |
| -nosprfolder | Don't make subfolders for sprite names. |
| -fastcompression | Use the fastest compression level. |
| -nocompression | Don't use compression. |

# License
All code is GPL3 licensed.

All graphics are from [Sonic Robo Blast 2](https://srb2.org) by Sonic Team Junior.

Sonic Team Junior is in no way affiliated with SEGA or Sonic Team. Sonic Team Junior does not claim ownership of any of SEGA's intellectual property used in SRB2.
