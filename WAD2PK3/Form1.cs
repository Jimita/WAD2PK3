/*  
 *  WAD2PK3
 *  Copyright (C) 2019 Jaime "Lactozilla" Passos

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

// Did you know that a cat can jump up to six times its length?
// Did you know that cats have 5 toes on its front paws and 4 on each back paw?
// Did you know that each side of a cat's face has about 12 whiskers?
// Did you know that each cat is born with an unique pattern on its nose?
// Did you know that cats can greet between themselves by touching noses?

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WADFormat;

namespace WAD2PK3
{
    public partial class Form1 : Form
    {
        bool WADLoaded = false;
        bool IsKartWAD = false;
        WADFile LoadedWADFile;
        CompressionLevel compression_level = CompressionLevel.Optimal;

        string input_path;
        string output_path;
        bool terminal_output;

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        public Form1(string[] args)
        {
            InitializeComponent();
            dataGridView1_Update();
            fileicon.Image = Properties.Resources.M_FNOPE;

            bool absolute_path(string path)
            {
                return new Regex(@"^[a-zA-Z]:\\$").IsMatch(path.Substring(0,3));
            }

            // detect arguments
            string input = "";
            bool has_input = false;
            bool has_output = false;
            if (args.Length > 0) input = args[0];
            if (input.Length > 0)
            {
                // dropped file
                if (absolute_path(input))
                {
                    input_path = input;
                    open_file(true);
                }
                // command line
                else
                {
                    // Flag that the program was called with command-line arguments.
                    // This will open an new console window.
                    terminal_output = true;
                    AllocConsole();

                    Console.WriteLine("WAD2PK3 by Jaime \"Lactozilla\" Passos");

                    checkBox_extensions.Checked = true;
                    checkBox_uppercase.Checked = true;
                    checkBox_prefixes.Checked = true;
                    checkBox_musicfolders.Checked = true;
                    checkBox_soundfolders.Checked = true;
                    checkBox_spritefolders.Checked = true;

                    string app_path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] == "-input")
                        {
                            if (i + 1 < args.Length)
                            {
                                string filename = args[i + 1];
                                if (filename.Substring(0, 0) == "-")
                                    Error("Unexpected argument in input filename!");
                                if (absolute_path(input)) input_path = filename;
                                else input_path = app_path + "\\" + filename;
                                has_input = true;
                                i++;
                            }
                            else
                                Error("Input file not specified!");
                            if (!File.Exists(input_path))
                                Error("Input file does not exist!");
                        }
                        else if (args[i] == "-output")
                        {
                            if (i + 1 < args.Length)
                            {
                                string filename = args[i + 1];
                                if (filename.Substring(0, 0) == "-")
                                    Error("Unexpected argument in input filename!");
                                if (absolute_path(input)) output_path = filename;
                                else output_path = app_path + "\\" + filename;
                                has_output = true;
                                i++;
                            }
                            else
                                Error("Output file not specified!");
                        }

                        else if (args[i] == "-noextensions") checkBox_extensions.Checked = false;
                        else if (args[i] == "-extlowercase") checkBox_uppercase.Checked = false;
                        else if (args[i] == "-keepprefixes") checkBox_prefixes.Checked = false;
                        else if (args[i] == "-nomusfolder") checkBox_musicfolders.Checked = false;
                        else if (args[i] == "-nosfxfolder") checkBox_soundfolders.Checked = false;
                        else if (args[i] == "-nosprfolder") checkBox_spritefolders.Checked = false;

                        else if (args[i] == "-fastcompression") compression_level = CompressionLevel.Fastest;
                        else if (args[i] == "-nocompression") compression_level = CompressionLevel.NoCompression;
                    }
                    if (has_input)
                    {
                        if (!has_output) Error("Output file not specified!");
                        if (input_path == output_path) Error("Filenames cannot match!");
                        if (File.Exists(output_path))
                        {
                            Console.Write("NOTICE: Output file already exists. Overwrite? (y/n) ");
                            while (true)
                            {
                                char key = char.ToLower(Console.ReadKey(true).KeyChar);
                                if (key == 'y')
                                {
                                    File.Delete(output_path);
                                    break;
                                }
                                else if (key == 'n')
                                {
                                    Console.WriteLine("\nFile overwrite cancelled by user request.");
                                    kms();
                                }
                            }
                        }
                        Console.WriteLine("\n");
                        open_file(false);
                        save_file(false);
                        //Success("Successfully saved " + Path.GetFileName(output_path) + "!");
                        Process.GetCurrentProcess().Kill();
                    }
                    else if (has_output) Error("Input file not specified!");
                }
            }
        }

        private void kms()
        {
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey(true);
            Process.GetCurrentProcess().Kill();
        }

        private void load_file_internal(object sender)
        {
            // create WADReader object
            WADReader Reader = new WADReader(terminal_output);
            // read from file
            WADFile WAD = Reader.ReadFile(input_path, sender);
            LoadedWADFile = WAD;
            WADLoaded = true;
            IsKartWAD = (Path.GetExtension(LoadedWADFile.filename) == ".kart");
        }

        private void save_file_internal(object sender)
        {
            // set compression level
            compression_level = CompressionLevel.Optimal;
            if (radio_fastestcompression.Checked) compression_level = CompressionLevel.Fastest;
            else if (radio_nocompression.Checked) compression_level = CompressionLevel.NoCompression;
            // save
            LoadedWADFile.WritePK3(output_path, compression_level,
                checkBox_extensions.Checked,
                checkBox_uppercase.Checked,
                checkBox_prefixes.Checked,

                checkBox_musicfolders.Checked,
                checkBox_soundfolders.Checked,
                checkBox_spritefolders.Checked,

                sender, terminal_output
            );
        }

        private void open_file(bool async)
        {
            int file_length = (int)(new FileInfo(input_path).Length);
            if (file_length < 4) Error("Loaded WAD file too small!");
            else
            {
                if (async)
                {
                    thread_load.RunWorkerAsync();
                    // update GUI
                    disable_buttons();
                    dataGridView1.Rows.Clear();
                    // reset progress bar
                    progressBar1.Visible = true;
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = 100;
                    progressBar1.Value = 1;
                    progressBar1.Step = 1;
                }
                else
                    load_file_internal(null);
            }
        }

        private void save_file(bool async)
        {
            if (async)
            {
                thread_save.RunWorkerAsync();
                // update GUI
                disable_buttons();
                // reset progress bar
                progressBar1.Visible = true;
                progressBar1.Minimum = 0;
                progressBar1.Maximum = LoadedWADFile.numlumps;
                progressBar1.Value = 1;
                progressBar1.Step = 1;
            }
            else save_file_internal(null);
        }

        // ===============================
        static readonly string[] SizeSuffixes = {" bytes", "kb", "mb", "gb", "tb"};
        static string SizeSuffix(long value, int decimalPlaces = 2)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }

            int i = 0;
            decimal dValue = value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }
            if (i == 0) decimalPlaces = 0;
            return string.Format("{0:n" + decimalPlaces + "}{1}", dValue, SizeSuffixes[i]);
        }

        // strings
        string DialogOpen_Filter = "WAD file (*.wad)|*.wad|Kart asset (*.kart)|*.kart|All files (*.*)|*.*";
        string DialogSave_Filter = "PK3 file (*.pk3)|*.pk3|ZIP file (*.zip)|*.zip|All files (*.*)|*.*";
        string[] Error_Messages =
        {
            "With great C# comes great Exceptions",
            "I'll strap a firework to me tail and fly away",
            "NOOOOOOOooooooooo.......",
            "Failure",
            "He's dead, Jim",
            "Not today",
            "I am absolutely and thoroughly done with you",
            "It can only be attributable to human error",
            "Something happened",
            "You can't do that",
            "This is the wrongest you've ever got it.",
            "That's it I've had it with you that does it I'm done that's the last straw",
            "The operation finished successfully",
            "Jimita the Cat proudly presents:",
        };

        // Error
        void Error(string message)
        {
            if (!terminal_output)
            {
                Random rnd = new Random();
                string title = Error_Messages[rnd.Next(Error_Messages.Length)];
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Console.Write("ERROR! ");
                Console.WriteLine(message);
                kms();
            }
        }

        // Success
        void Success(string message)
        {
            if (!terminal_output) MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else Console.WriteLine(message);
        }

        // ===============================
        private void SetupLumpList()
        {
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Lump Name";
            dataGridView1.Columns[1].Name = "Lump Size";
            dataGridView1.Columns[2].Name = "Lump Type";

            foreach (DataGridViewColumn column in dataGridView1.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView1.Rows.Clear();

            foreach (WADLump lump in LoadedWADFile.lumps)
            {
                string lumptype = lump.type.ToString();
                if (lumptype == "Skin" && !(lump.lumpname.Length >= 6 && lump.lumpname.Substring(0, 6) == "S_SKIN"))
                    lumptype = "Sprite";

                string[] formatted = { " " + lump.lumpname, SizeSuffix(lump.length), lumptype };
                DataGridViewRow theRow = dataGridView1.Rows[dataGridView1.Rows.Add(formatted)];
                if (lumptype == "Graphic" || lumptype == "Sprite" || lumptype == "Patch"
                 || lumptype == "Texture" || lumptype == "Flat")
                    theRow.DefaultCellStyle.BackColor = Color.FromArgb(209, 209, 255);
                else if (lumptype == "Music" || lumptype == "Sound")
                    theRow.DefaultCellStyle.BackColor = Color.FromArgb(255, 209, 209);
                else if (lumptype == "Map")
                    theRow.DefaultCellStyle.BackColor = Color.FromArgb(219, 255, 227);
                else if (lumptype == "Lua" || lumptype == "SOC")
                    theRow.DefaultCellStyle.BackColor = Color.FromArgb(239, 216, 115);
                else if (lumptype == "Palette" || lumptype == "Colormap"
                      || lumptype == "Transmap" || lumptype == "Fademask"
                      || lumptype == "Demo"
                    )
                    theRow.DefaultCellStyle.BackColor = Color.FromArgb(255, 241, 209);
            }
        }

        private void dataGridView1_Update()
        {
            if (WADLoaded) SetupLumpList();
        }

        private void disable_buttons()
        {
            button_open.Enabled = false;
            button_save.Enabled = false;
            checkBox_extensions.Enabled = false;
            checkBox_uppercase.Enabled = false;
            checkBox_prefixes.Enabled = false;
            checkBox_musicfolders.Enabled = false;
            checkBox_soundfolders.Enabled = false;
            checkBox_spritefolders.Enabled = false;
            radio_optimalcompression.Enabled = false;
            radio_fastestcompression.Enabled = false;
            radio_nocompression.Enabled = false;
        }

        private void enable_buttons()
        {
            button_open.Enabled = true;
            button_save.Enabled = true;
            checkBox_extensions.Enabled = true;
            checkBox_uppercase.Enabled = true;
            checkBox_prefixes.Enabled = true;
            checkBox_musicfolders.Enabled = true;
            checkBox_soundfolders.Enabled = true;
            checkBox_spritefolders.Enabled = true;
            radio_optimalcompression.Enabled = true;
            radio_fastestcompression.Enabled = true;
            radio_nocompression.Enabled = true;
        }

        // Open WAD
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dialog_Open = new OpenFileDialog();
            Dialog_Open.Filter = DialogOpen_Filter;
            Dialog_Open.Title = "Select WAD file";
            Dialog_Open.DefaultExt = "wad";
            if (Dialog_Open.ShowDialog() == DialogResult.OK)
            {
                input_path = Dialog_Open.FileName;
                open_file(true);
            }
        }

        // Save PK3
        private void button_save_Click(object sender, EventArgs e)
        {
            if (!WADLoaded) Error("No WAD file loaded for saving!");
            else
            {
                SaveFileDialog Dialog_Save = new SaveFileDialog();
                Dialog_Save.Filter = DialogSave_Filter;
                Dialog_Save.Title = "Select destination PK3 file";
                Dialog_Save.DefaultExt = "pk3";
                Dialog_Save.OverwritePrompt = true;
                Dialog_Save.FileName = Path.GetFileNameWithoutExtension(LoadedWADFile.filename);
                if (Dialog_Save.ShowDialog() == DialogResult.OK)
                {
                    if (!Dialog_Save.FileName.Equals(input_path))
                    {
                        output_path = Dialog_Save.FileName;
                        save_file(true);
                    }
                    else
                        Error("Filenames cannot match!");
                }
            }
        }

        /// background workers
        // save
        private void thread_save_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (File.Exists(output_path)) File.Delete(output_path);
            save_file_internal(sender);
        }

        private void thread_save_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void thread_save_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // uh oh! exception thrown
                Error(e.Error.ToString());
            }
            else
                Success("Successfully saved " + Path.GetFileName(output_path) + "!");
            // update GUI
            enable_buttons();
            progressBar1.Visible = false;
        }

        // load
        private void thread_load_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            load_file_internal(sender);
        }

        private void thread_load_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void thread_load_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // uh oh! exception thrown
                Error(e.Error.ToString());
            }
            else
            {
                // update GUI
                if (!IsKartWAD)
                    fileicon.Image = Properties.Resources.M_FWAD;
                else
                    fileicon.Image = Properties.Resources.M_FKART;
                label2.Text = Path.GetFileName(input_path);

                dataGridView1_Update();
            }
            // update GUI
            enable_buttons();
            progressBar1.Visible = false;
        }
    }
}
