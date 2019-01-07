namespace WAD2PK3
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button_open = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox_extensions = new System.Windows.Forms.CheckBox();
            this.checkBox_uppercase = new System.Windows.Forms.CheckBox();
            this.checkBox_prefixes = new System.Windows.Forms.CheckBox();
            this.checkBox_musicfolders = new System.Windows.Forms.CheckBox();
            this.checkBox_soundfolders = new System.Windows.Forms.CheckBox();
            this.checkBox_spritefolders = new System.Windows.Forms.CheckBox();
            this.thread_save = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.thread_load = new System.ComponentModel.BackgroundWorker();
            this.label4 = new System.Windows.Forms.Label();
            this.radio_optimalcompression = new System.Windows.Forms.RadioButton();
            this.radio_fastestcompression = new System.Windows.Forms.RadioButton();
            this.radio_nocompression = new System.Windows.Forms.RadioButton();
            this.fileicon = new System.Windows.Forms.PictureBox();
            this.appicon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileicon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.appicon)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Lucida Console", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(82, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(301, 65);
            this.label1.TabIndex = 1;
            this.label1.Text = "WAD2PK3";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGridView1.ColumnHeadersHeight = 20;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Location = new System.Drawing.Point(389, 12);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.ReadOnly = true;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(321, 323);
            this.dataGridView1.StandardTab = true;
            this.dataGridView1.TabIndex = 2;
            // 
            // button_open
            // 
            this.button_open.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_open.Location = new System.Drawing.Point(16, 90);
            this.button_open.Name = "button_open";
            this.button_open.Size = new System.Drawing.Size(153, 49);
            this.button_open.TabIndex = 3;
            this.button_open.Text = "Open WAD";
            this.button_open.UseVisualStyleBackColor = true;
            this.button_open.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_save
            // 
            this.button_save.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_save.Location = new System.Drawing.Point(16, 258);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(153, 49);
            this.button_save.TabIndex = 4;
            this.button_save.Text = "Save PK3";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 223);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "No file loaded.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(188, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(174, 24);
            this.label3.TabIndex = 7;
            this.label3.Text = "PK3 Export Options";
            // 
            // checkBox_extensions
            // 
            this.checkBox_extensions.AutoSize = true;
            this.checkBox_extensions.Checked = true;
            this.checkBox_extensions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_extensions.Location = new System.Drawing.Point(192, 121);
            this.checkBox_extensions.Name = "checkBox_extensions";
            this.checkBox_extensions.Size = new System.Drawing.Size(120, 17);
            this.checkBox_extensions.TabIndex = 8;
            this.checkBox_extensions.Text = "Write file extensions";
            this.checkBox_extensions.UseVisualStyleBackColor = true;
            // 
            // checkBox_uppercase
            // 
            this.checkBox_uppercase.AutoSize = true;
            this.checkBox_uppercase.Checked = true;
            this.checkBox_uppercase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_uppercase.Location = new System.Drawing.Point(192, 144);
            this.checkBox_uppercase.Name = "checkBox_uppercase";
            this.checkBox_uppercase.Size = new System.Drawing.Size(164, 17);
            this.checkBox_uppercase.TabIndex = 9;
            this.checkBox_uppercase.Text = "Use all-uppercase extensions";
            this.checkBox_uppercase.UseVisualStyleBackColor = true;
            // 
            // checkBox_prefixes
            // 
            this.checkBox_prefixes.AutoSize = true;
            this.checkBox_prefixes.Checked = true;
            this.checkBox_prefixes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_prefixes.Location = new System.Drawing.Point(192, 167);
            this.checkBox_prefixes.Name = "checkBox_prefixes";
            this.checkBox_prefixes.Size = new System.Drawing.Size(153, 17);
            this.checkBox_prefixes.TabIndex = 10;
            this.checkBox_prefixes.Text = "Remove Lua/SOC prefixes";
            this.checkBox_prefixes.UseVisualStyleBackColor = true;
            // 
            // checkBox_musicfolders
            // 
            this.checkBox_musicfolders.AutoSize = true;
            this.checkBox_musicfolders.Checked = true;
            this.checkBox_musicfolders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_musicfolders.Location = new System.Drawing.Point(192, 190);
            this.checkBox_musicfolders.Name = "checkBox_musicfolders";
            this.checkBox_musicfolders.Size = new System.Drawing.Size(116, 17);
            this.checkBox_musicfolders.TabIndex = 11;
            this.checkBox_musicfolders.Text = "Create music folder";
            this.checkBox_musicfolders.UseVisualStyleBackColor = true;
            // 
            // checkBox_soundfolders
            // 
            this.checkBox_soundfolders.AutoSize = true;
            this.checkBox_soundfolders.Checked = true;
            this.checkBox_soundfolders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_soundfolders.Location = new System.Drawing.Point(192, 213);
            this.checkBox_soundfolders.Name = "checkBox_soundfolders";
            this.checkBox_soundfolders.Size = new System.Drawing.Size(118, 17);
            this.checkBox_soundfolders.TabIndex = 12;
            this.checkBox_soundfolders.Text = "Create sound folder";
            this.checkBox_soundfolders.UseVisualStyleBackColor = true;
            // 
            // checkBox_spritefolders
            // 
            this.checkBox_spritefolders.AutoSize = true;
            this.checkBox_spritefolders.Checked = true;
            this.checkBox_spritefolders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_spritefolders.Location = new System.Drawing.Point(192, 236);
            this.checkBox_spritefolders.Name = "checkBox_spritefolders";
            this.checkBox_spritefolders.Size = new System.Drawing.Size(185, 17);
            this.checkBox_spritefolders.TabIndex = 13;
            this.checkBox_spritefolders.Text = "Create subfolders for sprite names";
            this.checkBox_spritefolders.UseVisualStyleBackColor = true;
            // 
            // thread_save
            // 
            this.thread_save.WorkerReportsProgress = true;
            this.thread_save.DoWork += new System.ComponentModel.DoWorkEventHandler(this.thread_save_DoWork);
            this.thread_save.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.thread_save_ProgressChanged);
            this.thread_save.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.thread_save_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(16, 324);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(153, 25);
            this.progressBar1.TabIndex = 14;
            this.progressBar1.Visible = false;
            // 
            // thread_load
            // 
            this.thread_load.WorkerReportsProgress = true;
            this.thread_load.DoWork += new System.ComponentModel.DoWorkEventHandler(this.thread_load_DoWork);
            this.thread_load.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.thread_load_ProgressChanged);
            this.thread_load.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.thread_load_RunWorkerCompleted);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(188, 256);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(192, 24);
            this.label4.TabIndex = 15;
            this.label4.Text = "Compression Options";
            // 
            // radio_optimalcompression
            // 
            this.radio_optimalcompression.AutoSize = true;
            this.radio_optimalcompression.Checked = true;
            this.radio_optimalcompression.Location = new System.Drawing.Point(192, 283);
            this.radio_optimalcompression.Name = "radio_optimalcompression";
            this.radio_optimalcompression.Size = new System.Drawing.Size(60, 17);
            this.radio_optimalcompression.TabIndex = 16;
            this.radio_optimalcompression.TabStop = true;
            this.radio_optimalcompression.Text = "Optimal";
            this.radio_optimalcompression.UseVisualStyleBackColor = true;
            // 
            // radio_fastestcompression
            // 
            this.radio_fastestcompression.AutoSize = true;
            this.radio_fastestcompression.Location = new System.Drawing.Point(192, 306);
            this.radio_fastestcompression.Name = "radio_fastestcompression";
            this.radio_fastestcompression.Size = new System.Drawing.Size(59, 17);
            this.radio_fastestcompression.TabIndex = 17;
            this.radio_fastestcompression.Text = "Fastest";
            this.radio_fastestcompression.UseVisualStyleBackColor = true;
            // 
            // radio_nocompression
            // 
            this.radio_nocompression.AutoSize = true;
            this.radio_nocompression.Location = new System.Drawing.Point(192, 329);
            this.radio_nocompression.Name = "radio_nocompression";
            this.radio_nocompression.Size = new System.Drawing.Size(51, 17);
            this.radio_nocompression.TabIndex = 18;
            this.radio_nocompression.Text = "None";
            this.radio_nocompression.UseVisualStyleBackColor = true;
            // 
            // fileicon
            // 
            this.fileicon.Image = global::WAD2PK3.Properties.Resources.M_FWAD;
            this.fileicon.Location = new System.Drawing.Point(56, 156);
            this.fileicon.Name = "fileicon";
            this.fileicon.Size = new System.Drawing.Size(64, 64);
            this.fileicon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.fileicon.TabIndex = 5;
            this.fileicon.TabStop = false;
            this.fileicon.Tag = "";
            // 
            // appicon
            // 
            this.appicon.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.appicon.Image = global::WAD2PK3.Properties.Resources.icon;
            this.appicon.Location = new System.Drawing.Point(17, 12);
            this.appicon.Name = "appicon";
            this.appicon.Size = new System.Drawing.Size(64, 64);
            this.appicon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.appicon.TabIndex = 0;
            this.appicon.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(724, 361);
            this.Controls.Add(this.radio_nocompression);
            this.Controls.Add(this.radio_fastestcompression);
            this.Controls.Add(this.radio_optimalcompression);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.checkBox_spritefolders);
            this.Controls.Add(this.checkBox_soundfolders);
            this.Controls.Add(this.checkBox_musicfolders);
            this.Controls.Add(this.checkBox_prefixes);
            this.Controls.Add(this.checkBox_uppercase);
            this.Controls.Add(this.checkBox_extensions);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fileicon);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_open);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.appicon);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(740, 400);
            this.MinimumSize = new System.Drawing.Size(740, 400);
            this.Name = "Form1";
            this.Text = "WAD2PK3";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileicon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.appicon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox appicon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button_open;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.PictureBox fileicon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox_extensions;
        private System.Windows.Forms.CheckBox checkBox_uppercase;
        private System.Windows.Forms.CheckBox checkBox_prefixes;
        private System.Windows.Forms.CheckBox checkBox_musicfolders;
        private System.Windows.Forms.CheckBox checkBox_soundfolders;
        private System.Windows.Forms.CheckBox checkBox_spritefolders;
        private System.ComponentModel.BackgroundWorker thread_save;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker thread_load;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radio_optimalcompression;
        private System.Windows.Forms.RadioButton radio_fastestcompression;
        private System.Windows.Forms.RadioButton radio_nocompression;
    }
}

