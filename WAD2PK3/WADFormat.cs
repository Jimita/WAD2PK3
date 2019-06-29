/*  
 *  WADFormat.cs
 *  CCopyright (C) 2019 Jaime "Lactozilla" Passos

 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.

 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.

 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

// The WAD file reading and saving code was written from scratch by me (Lactozilla).
// The only reference used was the Doom Wiki's article on the WAD format (https://doomwiki.org/wiki/WAD).

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;

namespace WADFormat
{
    class WADReader
    {
        private bool terminal_output;
        public WADReader(bool terminal_output)
        {
            this.terminal_output = terminal_output;
        }

        public string[] maplumpordering = { "MAP01", // dummy
            "THINGS",
            "LINEDEFS", "SIDEDEFS", "VERTEXES",
            "SEGS", "SSECTORS", "NODES",
            "SECTORS",
            // optional
            "REJECT", "BLOCKMAP"
        };

        public void FatalWADReadError(string error)
        {
            if (!terminal_output) MessageBox.Show(error, "FATAL EXCEPTION", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                Console.Write("\nFATAL EXCEPTION! ");
                Console.WriteLine(error);
                Console.WriteLine("Press any key to quit.");
                Console.ReadKey(true);
            }
            Process.GetCurrentProcess().Kill();
        }

        public bool IsLumpGraphic(byte[] lump_data, uint size)
        {
            int header = 6;                  // size of patch header
            if (size > header && size >= 13) // size of a 1x1 patch (13 bytes)
            {
                int read;
                int lump_read_bytes_as_uint16()
                {
                    if (read + 1 > size) return 0;
                    return BitConverter.ToUInt16(new byte[2] { lump_data[read], lump_data[read + 1] }, 0);
                }
                uint lump_read_bytes_as_uint32()
                {
                    if (read + 3 > size) return 0;
                    return BitConverter.ToUInt32(new byte[4] { lump_data[read], lump_data[read + 1], lump_data[read + 2], lump_data[read + 3] }, 0);
                }
                uint width = 0, height = 0;
                uint[] column_offsets = new uint[0];
                for (read = 0; read < size;)
                {
                    width = (uint)lump_read_bytes_as_uint16();
                    read += 2;      // short
                    height = (uint)lump_read_bytes_as_uint16();
                    if (width >= 32768 || (int)width < 1 || height >= 32768 || (int)height < 1)
                        return false;
                    Array.Resize(ref column_offsets, (int)width);
                    read += 6;      // short * 3
                    for (int col = 0; col < width - 1; col++)
                    {
                        column_offsets[col] = lump_read_bytes_as_uint32();
                        read += 4;      // long
                    }
                    break;
                }
                if (size < header + (width * 2))
                    return false;
                for (int col = 0; col < width - 1; col++)
                {
                    if (column_offsets[col] > size || column_offsets[col] < header)
                        return false;
                }
                return true;
            }
            return false;
        }

        public WADFile ReadFile(string wadfilename, object sender)
        {
            // create WADFile
            WADFile wad = new WADFile();
            wad.filename = wadfilename;

            // read wad from file stream
            using (FileStream fileStream = new FileStream(wadfilename, FileMode.Open))
            {
                byte[] wad_data = new byte[fileStream.Length];
                float read_percentage = 0;
                int total_read_bytes = 0;
                int bytes_to_read = (int)fileStream.Length;
                int i = 0;

                while (bytes_to_read > 0)
                {
                    int n = fileStream.Read(wad_data, i, bytes_to_read);
                    if (n == 0) break;
                    bytes_to_read -= n;
                }
                bytes_to_read = wad_data.Length;

                // byte to type
                uint read_bytes_as_uint32()
                {
                    return BitConverter.ToUInt32(new byte[4] { wad_data[i], wad_data[i + 1], wad_data[i + 2], wad_data[i + 3] }, 0);
                }
                string read_bytes_as_string(int length)
                {
                    int real_length = 0;
                    byte[] stringdata = new byte[length];
                    for (int j = 0; j < length; j++)
                    {
                        byte nchar = wad_data[i + j];
                        if (nchar == 0) break;
                        if (nchar == 47) nchar = 43; // convert / to +
                        stringdata[j] = nchar;
                        real_length++;
                    }
                    return Encoding.ASCII.GetString(stringdata, 0, real_length);
                }

                // begin read from wad_data array
                uint num_lumps = 0;
                uint directory_offset = 0;
                uint current_lump = 0;

                // read header
                for (i = 0; i < bytes_to_read;)
                {
                    string indentifier = read_bytes_as_string(4);
                    i += 4;

                    Console.WriteLine("Reading WAD header...");

                    // detect unsupported WAD types
                    if (indentifier == "ZWAD")
                        FatalWADReadError("Unsupported compressed WAD indentifier!");
                    if (!(indentifier == "PWAD" || indentifier == "IWAD" || indentifier == "SDLL"))
                        FatalWADReadError("Unknown WAD indentifier!");

                    // read number of lumps
                    num_lumps = read_bytes_as_uint32();
                    i += 4;
                    // read directory offset
                    directory_offset = read_bytes_as_uint32();
                    i += 4;

                    Console.WriteLine("Type {0}, {1} lumps", indentifier, num_lumps);

                    break;
                }

                total_read_bytes = bytes_to_read - (int)directory_offset;

                // read from the created directory
                wad.lumps = new WADLump[num_lumps];
                wad.numlumps = 0;

                LumpType current_lump_type = LumpType.Generic;
                bool singletype = true;

                int curmaplump = 0;
                int lastmaplumpmarker = 0;
                bool identifymaplumps = false;

                // lump write loop
                for (i = (int)directory_offset; i < bytes_to_read;)
                {
                    string name;
                    uint offset = 0;
                    uint size = 0;

                    if (current_lump >= num_lumps)
                    {
                        if (terminal_output)
                            Console.WriteLine("\nNOTICE: The input file most likely has a corrupted directory.\n");
                        break;
                    }

                    // read lump offset
                    offset = read_bytes_as_uint32();
                    i += 4;
                    // read lump size
                    size = read_bytes_as_uint32();
                    i += 4;
                    // read lump name
                    name = read_bytes_as_string(8);
                    i += 8;

                    Console.WriteLine(string.Format("Reading lump {0} ({1} bytes, offset {2})", name, size.ToString(), offset.ToString()));

                    read_percentage = current_lump;
                    read_percentage /= num_lumps;
                    read_percentage *= 100;
                    if (sender != null) (sender as BackgroundWorker).ReportProgress(Math.Min((int)read_percentage, 100));

                    // identify lump type
                    bool is_marker = false;
                    if (current_lump_type == LumpType.Generic || current_lump_type == LumpType.Skin)
                    {
                        if (current_lump_type != LumpType.Skin)
                            singletype = true;
                        if (name.Length >= 4 && name.Substring(0, 4) == "LUA_") current_lump_type = LumpType.Lua;
                        else if (name.Length >= 4 && name.Substring(0, 4) == "SOC_") current_lump_type = LumpType.SOC;
                        else if (name == "MAINCFG") current_lump_type = LumpType.SOC; /* SOC */
                        else if (name == "OBJCTCFG") current_lump_type = LumpType.SOC; /* SOC */
                        else if (name.Substring(0, 2) == "O_") current_lump_type = LumpType.Music;
                        else if (name.Substring(0, 2) == "D_") current_lump_type = LumpType.Music; /* MIDI */
                        else if (name.Substring(0, 2) == "DS") current_lump_type = LumpType.Sound; /* Sound (maybe) */
                        else if (name == "PLAYPAL" || (name.Length == 7 && name.Substring(0, 3) == "PAL")) current_lump_type = LumpType.Palette;
                        else if (name == "COLORMAP" || (name.Length == 7 && name.Substring(0, 3) == "CLM")) current_lump_type = LumpType.Colormap;
                        else if (name.Length == 7 && name.Substring(0, 5) == "TRANS") current_lump_type = LumpType.Transmap;
                        else if (name.Length == 8 && name.Substring(0, 4) == "FADE") current_lump_type = LumpType.Fademask;
                        else if (name.Length == 8 && name.Substring(0, 5) == "DEMO_") current_lump_type = LumpType.Demo;
                        else if (name == "CREDITS" || name == "README" || name == "ANIMDEFS" || name == "TEXTURES") current_lump_type = LumpType.Text; /* Text */
                        // marker start
                        else if (name == "DT_START") { current_lump_type = LumpType.Generic; is_marker = true; singletype = false; }
                        else if (name == "LV_START") { current_lump_type = LumpType.Generic; is_marker = true; singletype = false; }
                        else if (name == "FA_START") { current_lump_type = LumpType.Fademask; is_marker = true; singletype = false; }
                        else if (name == "GX_START") { current_lump_type = LumpType.Graphic; is_marker = true; singletype = false; }
                        else if (name == "TX_START") { current_lump_type = LumpType.Texture; is_marker = true; singletype = false; }
                        else if (name == "P_START" || name == "PP_START") { current_lump_type = LumpType.Patch; is_marker = true; singletype = false; }
                        else if (name == "F_START" || name == "FF_START") { current_lump_type = LumpType.Flat; is_marker = true; singletype = false; }
                        else if (name == "S_START" || name == "SS_START") { current_lump_type = LumpType.Sprite; is_marker = true; singletype = false; }
                        // skins
                        else if (name.Length >= 6 && name.Substring(0, 6) == "S_SKIN") { current_lump_type = LumpType.Skin; singletype = false; }
                    }
                    else
                    {
                        // marker end
                        if (name == "DT_END" || name == "LV_END"
                            || name == "TX_END" || name == "GX_END"
                            || name == "P_END" || name == "PP_END"
                            || name == "F_END" || name == "FF_END"
                            || name == "S_END" || name == "SS_END"
                            || name == "FA_END"
                            )
                        {
                            current_lump_type = LumpType.Generic;
                            is_marker = true;
                            singletype = false;
                        }
                    }

                    // ========= MAP SAVING =========
                    bool wrotemapmarker = false;
                    if (identifymaplumps)
                    {
                        if (curmaplump > 0 && (curmaplump > 10 || name != maplumpordering[curmaplump]))
                        {
                            wad.lumps[lastmaplumpmarker].maplumpinfo.end_lump = wad.numlumps - 1;
                            curmaplump = 0;
                            identifymaplumps = false;
                        }
                        else
                        {
                            current_lump_type = LumpType.Map;
                            curmaplump++;
                        }
                    }
                    if (name.Substring(0, 3) == "MAP" && name.Length == 5)
                    {
                        identifymaplumps = true;
                        lastmaplumpmarker = wad.numlumps;
                        wrotemapmarker = true;
                        is_marker = true;
                        curmaplump++;

                        wad.lumps[lastmaplumpmarker] = new WADLump();
                        wad.lumps[lastmaplumpmarker].length = 0;
                        wad.lumps[lastmaplumpmarker].lumpname = name;
                        wad.lumps[lastmaplumpmarker].type = LumpType.Map;
                        wad.lumps[lastmaplumpmarker].maplumpinfo.start_lump = lastmaplumpmarker;
                    }
                    if (wrotemapmarker)
                        goto skiplumpwrite;
                    // ========= MAP SAVING =========

                    // write lump header
                    wad.lumps[wad.numlumps] = new WADLump();
                    wad.lumps[wad.numlumps].length = 0;
                    wad.lumps[wad.numlumps].lumpname = name;
                    if (is_marker)
                        wad.lumps[wad.numlumps].type = LumpType.Marker;
                    if (!identifymaplumps)
                    {
                        wad.lumps[wad.numlumps].maplumpinfo.start_lump = -1;
                        wad.lumps[wad.numlumps].maplumpinfo.end_lump = -1;
                    }

                    // read lump data
                    byte[] lump_data = new byte[size];
                    if (!is_marker)
                    {
                        if (size > 0)
                        {
                            int k = 0;
                            for (int j = (int)offset; j < offset + size; j++)
                            {
                                lump_data[k] = wad_data[j];
                                k++; total_read_bytes++;
                            }
                            wad.lumps[wad.numlumps].data = lump_data;
                            wad.lumps[wad.numlumps].length = (int)size;
                        }

                        // Detect if the lump is a graphic.
                        if (current_lump_type == LumpType.Generic)
                        {
                            if (IsLumpGraphic(lump_data, size))
                                current_lump_type = LumpType.Graphic;
                        }
                        wad.lumps[wad.numlumps].type = current_lump_type;
                    }
                    // The ordering is actually written correctly without the folders,
                    // until another editor is involved
                    if (current_lump_type == LumpType.Skin && name.Substring(0, 6) != "S_SKIN")
                    {
                        if (!IsLumpGraphic(lump_data, size))
                            current_lump_type = LumpType.Generic;
                    }
                    skiplumpwrite: wad.numlumps++; current_lump++;
                    if (singletype) current_lump_type = LumpType.Generic;
                }

                // close file stream
                fileStream.Close();
                // done!
                return wad;
            }
        }
    }
    // the wad itself
    class WADFile
    {
        // data
        public string filename;
        public int numlumps;
        public WADLump[] lumps;

        // (General) identify music format
        public string IdentifyMusicLumpExtension(WADLump lumpy, string _default)
        {
            if (lumpy.data == null) return _default;
            char[] data = Encoding.ASCII.GetString(lumpy.data).ToCharArray();
            string readstringloop(int length, int i = 0)
            {
                string str = ""; length += i;
                if (i >= data.Length) return str;
                for (; i < length; i++)
                    str += data[i];
                return str;
            }

            // MIDI
            if (readstringloop(4) == "MThd") return ".mid";                     // Standard MIDI file

            // Tracker formats
            if (readstringloop(4, 1080) == "M.K.") return ".mod";               // Protracker MK module
            if (readstringloop(4, 1080) == "M!K!") return ".mod";               // Generic MK
            if (readstringloop(4, 1080) == "4CHN") return ".mod";               // Generic Amiga module
            if (readstringloop(4, 1080) == "6CHN") return ".mod";               // ?
            if (readstringloop(4, 1080) == "8CHN") return ".mod";               // Octalyser module
            if (readstringloop(4, 1080) == "FLT4") return ".mod";               // ?
            if (readstringloop(4, 1080) == "FLT8") return ".mod";               // ?

            if (readstringloop(4, 44) == "SCRM") return ".s3m";                 // ScreamTracker module
            if (readstringloop(17) == "Extended module: ") return ".xm";        // XM module
            if (readstringloop(4) == "IMPM") return ".it";                      // Impulse Tracker module

            // Digital music formats
            if (readstringloop(12) == "RIFFWAVE") return ".wav";                // RIFF WAV
            if (readstringloop(4) == "OggS"                                     // (Ogg container header)
             && readstringloop(6, 29) == "vorbis") return ".ogg";               // Vorbis format
            if (readstringloop(4) == "flaC") return ".flac";                    // FLAC

            // libgme formats
            if (readstringloop(4) == "Vgm ") return ".vgm";                           // VGM
            if (readstringloop(27) == "SNES - SPC700 Sound File Data") return ".spc"; // SNES SPC-700
            // (no vgz)

            return _default;
        }

        // (PK3) identify full path for a lump
        public string IdentifyFullPath(WADLump lumpy, bool UseExtensions, bool UppercaseExtensions, bool StripPrefixes, bool MusicFolder, bool SFXFolder, bool SpriteSubfolders)
        {
            string f = lumpy.lumpname;
            // detect extensions
            string ext = "";
            // lump names or lump types that won't write extensions
            if (lumpy.type == LumpType.Marker || lumpy.type == LumpType.Fademask
                || f == "ANIMDEFS" || f == "TEXTURES")
                UseExtensions = false;
            // use extensions otherwise if requested to
            if (UseExtensions)
            {
                switch (lumpy.type)
                {
                    case LumpType.Lua: { ext = ".lua"; break; }
                    case LumpType.SOC: { ext = ".soc"; break; }
                    // === FALLBACKS ===
                    case LumpType.Texture:
                    case LumpType.Patch:
                    case LumpType.Sprite:
                    case LumpType.Graphic: { ext = ".gfx"; break; }
                    // === END FALLBACKS ===
                    case LumpType.Flat: { ext = ".raw"; break; }
                    case LumpType.Music: { ext = IdentifyMusicLumpExtension(lumpy, ".mus"); break; }
                    case LumpType.Sound: { ext = IdentifyMusicLumpExtension(lumpy, ".sfx"); break; }
                    case LumpType.Map: { ext = ".wad"; break; }
                    case LumpType.Text: { ext = ".txt"; break; }
                    case LumpType.Palette: { ext = ".pal"; break; }
                    case LumpType.Colormap: { ext = ".clm"; break; }
                    default: { ext = ".lmp"; break; }
                }
            }
            if (UppercaseExtensions) ext = ext.ToUpper();

            if (lumpy.maplumpinfo.start_lump > 0)
                return "Maps/" + f + ext;

            switch (lumpy.type)
            {
                case LumpType.Lua:
                {
                    if (!StripPrefixes) return "Lua/" + f + ext;
                    else return ("Lua/" + f.Substring(4) + ext);
                }
                case LumpType.SOC:
                {
                    string defaultreturn = ("SOC/" + f + ext);
                    if (!StripPrefixes) return defaultreturn;
                    else
                    {
                        if (f.Substring(0,4) == "SOC_") return ("SOC/" + f.Substring(4) + ext);
                        else return defaultreturn;
                    }
                }
                case LumpType.Skin: return "Skins/" + f;
                case LumpType.Graphic: return "Graphics/" + f + ext;
                case LumpType.Patch: return "Patches/" + f + ext;
                case LumpType.Texture: return "Textures/" + f + ext;
                case LumpType.Flat: return "Flats/" + f + ext;
                case LumpType.Sprite:
                {
                    if (!SpriteSubfolders) return ("Sprites/" + f + ext);
                    else return ("Sprites/" + f.Substring(0, 4) + "/" + f + ext);
                }
                case LumpType.Music:
                {
                    if (MusicFolder) return "Music/" + f + ext;
                    else return f + ext;
                }
                case LumpType.Sound:
                {
                    if (SFXFolder) return "Sounds/" + f + ext;
                    else return f + ext;
                }
                default: return f + ext;
            }
        }
        // (PK3) identify directory for a lump
        public string IdentifyDirectory(WADLump lumpy, bool MusicFolder, bool SFXFolder)
        {
            switch (lumpy.type)
            {
                case LumpType.Lua: return "Lua/";
                case LumpType.SOC: return "SOC/";
                case LumpType.Graphic: return "Graphics/";
                case LumpType.Patch: return "Patches/";
                case LumpType.Texture: return "Textures/";
                case LumpType.Flat: return "Flats/";
                case LumpType.Sprite: return "Sprites/";
                case LumpType.Music: return (MusicFolder ? "Music/" : null);
                case LumpType.Sound: return (SFXFolder ? "Music/" : null);
                default: return null;
            }
        }
        // (PK3) Create WAD as a byte array
        public byte[] CreateWAD()
        {
            byte[] reswad = new byte[0];
            int wpos = 0;
            int wadsize = 0;

            int directoryoffset_head;
            int directoryoffset_pointer;

            int[] lump_pointers = new int[numlumps];
            int[] lump_sizes = new int[numlumps];
            string[] lump_names = new string[numlumps];

            void seek(int length)
            {
                wadsize += length;
                wpos += length;
            }
            void write(byte data, int length)
            {
                if (wpos + length > wadsize)
                {
                    wadsize += length;
                    Array.Resize(ref reswad, wadsize);
                }
                reswad[wpos] = data;
                wpos += length;
            }
            void writestringloop(string str, bool null_terminated)
            {
                char[] chars = str.ToCharArray();
                for (int i = 0; i < str.Length; i++) write((byte)chars[i], 1);
                if (null_terminated) write(0, 1);
            }
            void writestringloop_len(string str, int length)
            {
                char[] chars = str.ToCharArray();
                for (int i = 0; i < length; i++)
                {
                    if (i < str.Length) write((byte)chars[i], 1);
                    else write(0, 1);
                }
            }
            void writebyteloop(byte[] bytes)
            {
                wadsize += bytes.Length;
                Array.Resize(ref reswad, wadsize);
                for (int i = 0; i < bytes.Length; i++)
                {
                    reswad[wpos] = bytes[i];
                    wpos++;
                }
            }

            // write header
            writestringloop("PWAD", false);
            writebyteloop(BitConverter.GetBytes((uint)numlumps));
            // skip directory pointer
            directoryoffset_head = wpos;
            seek(4);
            // write lump data
            for (int curlump = 0; curlump < numlumps; curlump++)
            {
                lump_pointers[curlump] = wpos;
                lump_sizes[curlump] = lumps[curlump].length;
                lump_names[curlump] = lumps[curlump].lumpname;
                if (lumps[curlump].length > 0) writebyteloop(lumps[curlump].data);
            }
            // write the lump directory
            directoryoffset_pointer = wpos;
            for (int curlump = 0; curlump < numlumps; curlump++)
            {
                writebyteloop(BitConverter.GetBytes((uint)lump_pointers[curlump]));
                writebyteloop(BitConverter.GetBytes((uint)lump_sizes[curlump]));
                writestringloop_len(lump_names[curlump],8);
            }
            // write pointer to lump directory
            wpos = directoryoffset_head;
            writebyteloop(BitConverter.GetBytes((uint)directoryoffset_pointer));
            // done!
            return reswad;
        }
        // (PK3) Save WAD to PK3
        public void WritePK3(
            string output_filename,
            CompressionLevel compression_level,
            bool WriteExtensions, bool UseUppercaseExtensions, bool StripPrefixes, bool WriteMusicFolder, bool WriteSFXFolder, bool WriteSpriteSubfolders,
            object sender, bool terminal_output
        ) {
            using (FileStream zip_stream = new FileStream(output_filename, FileMode.CreateNew))
            {
                using (var compressStream = new MemoryStream())
                {
                    using (ZipArchive archive = new ZipArchive(zip_stream, ZipArchiveMode.Create, true))
                    {
                        string[] created_folders = new string[numlumps];
                        for (int i = 0; i < numlumps;)
                        {
                            WADLump lump = lumps[i];
                            // ========= MAP SAVING =========
                            if (lump.maplumpinfo.start_lump >= 0)
                            {
                                ZipArchiveEntry entry = archive.CreateEntry(IdentifyFullPath(lump, true, UseUppercaseExtensions, StripPrefixes, false, false, false), compression_level);
                                i = lump.maplumpinfo.end_lump;
                                using (StreamWriter writer = new StreamWriter(entry.Open()))
                                {
                                    int lumpcount = (i - lump.maplumpinfo.start_lump) + 1;
                                    if (lumpcount < 0)
                                    {
                                        lumpcount = numlumps - lump.maplumpinfo.start_lump;     // WAD end reached
                                        i = -1;
                                    }
                                    Console.WriteLine("Creating map WAD for map " + lump.lumpname);
                                    WADFile mapwad = new WADFile();
                                    mapwad.lumps = new WADLump[lumpcount];
                                    mapwad.numlumps = lumpcount;
                                    for (int j = 0; j < lumpcount; j++)
                                    {
                                        WADLump curmaplump = lumps[lump.maplumpinfo.start_lump + j];
                                        mapwad.lumps[j] = new WADLump();
                                        mapwad.lumps[j].lumpname = (j == 0 ? lump.lumpname : new WADReader(terminal_output).maplumpordering[j]);
                                        mapwad.lumps[j].data = curmaplump.data;
                                        mapwad.lumps[j].length = curmaplump.length;
                                    }
                                    Console.WriteLine("Writing map WAD for map " + lump.lumpname);
                                    byte[] realmapwad = mapwad.CreateWAD();
                                    writer.BaseStream.Write(realmapwad, 0, realmapwad.Length);
                                }
                                if (i == -1) break;
                            }
                            // ========= MAP SAVING =========
                            else
                            {
                                if (lump.type != LumpType.Marker || (lump.lumpname == "FA_START" || lump.lumpname == "FA_END"))
                                {
                                    // Write the directory before writing the file.
                                    // This is required for SRB2's PK3 implementation.
                                    string directory = IdentifyDirectory(lump, WriteMusicFolder, WriteSFXFolder);
                                    if (directory != null)
                                    {
                                        foreach (string thisdir in created_folders)
                                            if (thisdir == directory) goto nofolder;
                                        Console.WriteLine(string.Format("Creating folder {0}", directory));
                                        created_folders[i] = directory;
                                        archive.CreateEntry(directory);
                                    }
                                    nofolder:
                                    ZipArchiveEntry entry = archive.CreateEntry(IdentifyFullPath(lump, WriteExtensions, UseUppercaseExtensions, StripPrefixes, WriteMusicFolder, WriteSFXFolder, WriteSpriteSubfolders));
                                    Console.WriteLine(string.Format("Writing lump {0} ({1} bytes, type {2})", lump.lumpname, lump.length.ToString(), lump.type.ToString()));
                                    using (StreamWriter writer = new StreamWriter(entry.Open()))
                                    {
                                        if (lump.data != null)
                                            writer.BaseStream.Write(lump.data, 0, lump.length);
                                    }
                                }
                            }
                            i++;
                            if (sender != null) (sender as BackgroundWorker).ReportProgress(i);
                        }
                    }
                }
            }
        }
    }

    class WADLump
    {
        public string lumpname;
        public int length;
        public byte[] data;
        public LumpType type;
        public MapLumpInfo maplumpinfo;
    }

    enum LumpType
    {
        Generic,
        Lua, SOC, Skin,
        Graphic, Patch, Texture, Flat, Sprite,
        Music, Sound,
        Text, Marker, Demo,
        Palette, Colormap, Transmap, Fademask,
        Map
    };

    struct MapLumpInfo { public int start_lump, end_lump; }
}
