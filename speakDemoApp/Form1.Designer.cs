namespace speakDemoApp
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tree_Area = new System.Windows.Forms.TreeView();
            this.btn_RetrieveArea = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_SearchDev = new System.Windows.Forms.Button();
            this.dGrid_devList = new System.Windows.Forms.DataGridView();
            this.AliasName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SN = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IPV4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HardwareVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoftwareVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_LoginPass = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_LoginName = new System.Windows.Forms.TextBox();
            this.cmb_MicDeviceList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rText_RecordList = new System.Windows.Forms.RichTextBox();
            this.chk_IsAutoSend = new System.Windows.Forms.CheckBox();
            this.prog_RecordProg = new System.Windows.Forms.ProgressBar();
            this.btn_SendToDevice = new System.Windows.Forms.Button();
            this.btn_RecordAudio = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.chk_IsMonoWAV = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numb_FrameInterval = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmb_SamplesPerSec = new System.Windows.Forms.ComboBox();
            this.lab_sendCount = new System.Windows.Forms.Label();
            this.cmb_BitsPerSample = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGrid_devList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numb_FrameInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tree_Area);
            this.groupBox1.Controls.Add(this.btn_RetrieveArea);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.btn_SearchDev);
            this.groupBox1.Controls.Add(this.dGrid_devList);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txt_LoginPass);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txt_LoginName);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(654, 283);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "收听设备";
            // 
            // tree_Area
            // 
            this.tree_Area.CheckBoxes = true;
            this.tree_Area.Location = new System.Drawing.Point(442, 38);
            this.tree_Area.Name = "tree_Area";
            this.tree_Area.Size = new System.Drawing.Size(205, 212);
            this.tree_Area.TabIndex = 13;
            this.toolTip1.SetToolTip(this.tree_Area, "选择将要收听录音播报的区域");
            this.tree_Area.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tree_Area_MouseClick);
            // 
            // btn_RetrieveArea
            // 
            this.btn_RetrieveArea.Location = new System.Drawing.Point(570, 11);
            this.btn_RetrieveArea.Name = "btn_RetrieveArea";
            this.btn_RetrieveArea.Size = new System.Drawing.Size(77, 21);
            this.btn_RetrieveArea.TabIndex = 12;
            this.btn_RetrieveArea.Text = "检索区域";
            this.toolTip1.SetToolTip(this.btn_RetrieveArea, "检索所有设备支持的播报区域");
            this.btn_RetrieveArea.UseVisualStyleBackColor = true;
            this.btn_RetrieveArea.Click += new System.EventHandler(this.btn_RetrieveArea_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(440, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(101, 12);
            this.label9.TabIndex = 11;
            this.label9.Text = "选择收听的区域：";
            // 
            // btn_SearchDev
            // 
            this.btn_SearchDev.Location = new System.Drawing.Point(356, 11);
            this.btn_SearchDev.Name = "btn_SearchDev";
            this.btn_SearchDev.Size = new System.Drawing.Size(72, 21);
            this.btn_SearchDev.TabIndex = 9;
            this.btn_SearchDev.Text = "搜索设备";
            this.toolTip1.SetToolTip(this.btn_SearchDev, "搜索广播设备");
            this.btn_SearchDev.UseVisualStyleBackColor = true;
            this.btn_SearchDev.Click += new System.EventHandler(this.btn_SearchDev_Click);
            // 
            // dGrid_devList
            // 
            this.dGrid_devList.AllowUserToAddRows = false;
            this.dGrid_devList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGrid_devList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AliasName,
            this.Type,
            this.SN,
            this.IPV4,
            this.HardwareVersion,
            this.SoftwareVersion});
            this.dGrid_devList.Location = new System.Drawing.Point(6, 38);
            this.dGrid_devList.Name = "dGrid_devList";
            this.dGrid_devList.RowHeadersVisible = false;
            this.dGrid_devList.RowTemplate.Height = 23;
            this.dGrid_devList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dGrid_devList.ShowCellErrors = false;
            this.dGrid_devList.ShowEditingIcon = false;
            this.dGrid_devList.ShowRowErrors = false;
            this.dGrid_devList.Size = new System.Drawing.Size(422, 239);
            this.dGrid_devList.TabIndex = 8;
            // 
            // AliasName
            // 
            this.AliasName.DataPropertyName = "AliasName";
            this.AliasName.HeaderText = "设备别名";
            this.AliasName.Name = "AliasName";
            // 
            // Type
            // 
            this.Type.DataPropertyName = "Type";
            this.Type.HeaderText = "类型";
            this.Type.Name = "Type";
            // 
            // SN
            // 
            this.SN.DataPropertyName = "SN";
            this.SN.HeaderText = "序列号";
            this.SN.Name = "SN";
            // 
            // IPV4
            // 
            this.IPV4.DataPropertyName = "IPV4";
            this.IPV4.HeaderText = "IP地址";
            this.IPV4.Name = "IPV4";
            // 
            // HardwareVersion
            // 
            this.HardwareVersion.DataPropertyName = "HardwareVersion";
            this.HardwareVersion.HeaderText = "硬件版本";
            this.HardwareVersion.Name = "HardwareVersion";
            // 
            // SoftwareVersion
            // 
            this.SoftwareVersion.DataPropertyName = "SoftwareVersion";
            this.SoftwareVersion.HeaderText = "软件版本";
            this.SoftwareVersion.Name = "SoftwareVersion";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(547, 259);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 7;
            this.label8.Text = "密码：";
            // 
            // txt_LoginPass
            // 
            this.txt_LoginPass.Location = new System.Drawing.Point(594, 256);
            this.txt_LoginPass.Name = "txt_LoginPass";
            this.txt_LoginPass.PasswordChar = '*';
            this.txt_LoginPass.Size = new System.Drawing.Size(53, 21);
            this.txt_LoginPass.TabIndex = 6;
            this.txt_LoginPass.Text = "123456";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(434, 259);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 5;
            this.label7.Text = "登录名：";
            // 
            // txt_LoginName
            // 
            this.txt_LoginName.Location = new System.Drawing.Point(493, 256);
            this.txt_LoginName.Name = "txt_LoginName";
            this.txt_LoginName.Size = new System.Drawing.Size(48, 21);
            this.txt_LoginName.TabIndex = 4;
            this.txt_LoginName.Text = "admin";
            // 
            // cmb_MicDeviceList
            // 
            this.cmb_MicDeviceList.FormattingEnabled = true;
            this.cmb_MicDeviceList.Location = new System.Drawing.Point(81, 301);
            this.cmb_MicDeviceList.Name = "cmb_MicDeviceList";
            this.cmb_MicDeviceList.Size = new System.Drawing.Size(270, 20);
            this.cmb_MicDeviceList.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 304);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "录音设备：";
            // 
            // rText_RecordList
            // 
            this.rText_RecordList.Location = new System.Drawing.Point(14, 327);
            this.rText_RecordList.Name = "rText_RecordList";
            this.rText_RecordList.Size = new System.Drawing.Size(652, 129);
            this.rText_RecordList.TabIndex = 3;
            this.rText_RecordList.Text = "";
            // 
            // chk_IsAutoSend
            // 
            this.chk_IsAutoSend.AutoSize = true;
            this.chk_IsAutoSend.Checked = true;
            this.chk_IsAutoSend.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_IsAutoSend.Location = new System.Drawing.Point(467, 499);
            this.chk_IsAutoSend.Name = "chk_IsAutoSend";
            this.chk_IsAutoSend.Size = new System.Drawing.Size(72, 16);
            this.chk_IsAutoSend.TabIndex = 5;
            this.chk_IsAutoSend.Text = "自动发送";
            this.toolTip1.SetToolTip(this.chk_IsAutoSend, "是否录音完毕后自动发送到目标设备");
            this.chk_IsAutoSend.UseVisualStyleBackColor = true;
            // 
            // prog_RecordProg
            // 
            this.prog_RecordProg.Location = new System.Drawing.Point(14, 492);
            this.prog_RecordProg.Maximum = 30;
            this.prog_RecordProg.Name = "prog_RecordProg";
            this.prog_RecordProg.Size = new System.Drawing.Size(257, 23);
            this.prog_RecordProg.TabIndex = 7;
            // 
            // btn_SendToDevice
            // 
            this.btn_SendToDevice.Image = global::speakDemoApp.Properties.Resources.mail_send_72px;
            this.btn_SendToDevice.Location = new System.Drawing.Point(381, 462);
            this.btn_SendToDevice.Name = "btn_SendToDevice";
            this.btn_SendToDevice.Size = new System.Drawing.Size(80, 80);
            this.btn_SendToDevice.TabIndex = 6;
            this.btn_SendToDevice.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_SendToDevice, "把录音发送到目标设备");
            this.btn_SendToDevice.UseVisualStyleBackColor = true;
            this.btn_SendToDevice.Click += new System.EventHandler(this.btn_SendToDevice_Click);
            // 
            // btn_RecordAudio
            // 
            this.btn_RecordAudio.Image = global::speakDemoApp.Properties.Resources.microphone_72px;
            this.btn_RecordAudio.Location = new System.Drawing.Point(286, 462);
            this.btn_RecordAudio.Name = "btn_RecordAudio";
            this.btn_RecordAudio.Size = new System.Drawing.Size(80, 80);
            this.btn_RecordAudio.TabIndex = 4;
            this.btn_RecordAudio.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btn_RecordAudio, "按住录音，录音时长在15秒以内");
            this.btn_RecordAudio.UseVisualStyleBackColor = true;
            this.btn_RecordAudio.Click += new System.EventHandler(this.btn_RecordAudio_Click);
            this.btn_RecordAudio.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btn_RecordAudio_MouseDown);
            this.btn_RecordAudio.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btn_RecordAudio_MouseUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 473);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "录音进度：";
            // 
            // chk_IsMonoWAV
            // 
            this.chk_IsMonoWAV.AutoSize = true;
            this.chk_IsMonoWAV.Checked = true;
            this.chk_IsMonoWAV.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_IsMonoWAV.Location = new System.Drawing.Point(357, 303);
            this.chk_IsMonoWAV.Name = "chk_IsMonoWAV";
            this.chk_IsMonoWAV.Size = new System.Drawing.Size(78, 16);
            this.chk_IsMonoWAV.TabIndex = 9;
            this.chk_IsMonoWAV.Text = "单声道WAV";
            this.chk_IsMonoWAV.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(441, 304);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "采样率:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(543, 500);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "每帧等候(ms):";
            // 
            // numb_FrameInterval
            // 
            this.numb_FrameInterval.Location = new System.Drawing.Point(631, 494);
            this.numb_FrameInterval.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numb_FrameInterval.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numb_FrameInterval.Name = "numb_FrameInterval";
            this.numb_FrameInterval.Size = new System.Drawing.Size(35, 21);
            this.numb_FrameInterval.TabIndex = 13;
            this.toolTip1.SetToolTip(this.numb_FrameInterval, "每帧音频数据发送的等待时间");
            this.numb_FrameInterval.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // cmb_SamplesPerSec
            // 
            this.cmb_SamplesPerSec.FormattingEnabled = true;
            this.cmb_SamplesPerSec.Items.AddRange(new object[] {
            "11025",
            "22050",
            "44100"});
            this.cmb_SamplesPerSec.Location = new System.Drawing.Point(488, 301);
            this.cmb_SamplesPerSec.Name = "cmb_SamplesPerSec";
            this.cmb_SamplesPerSec.Size = new System.Drawing.Size(59, 20);
            this.cmb_SamplesPerSec.TabIndex = 15;
            // 
            // lab_sendCount
            // 
            this.lab_sendCount.AutoSize = true;
            this.lab_sendCount.Location = new System.Drawing.Point(467, 526);
            this.lab_sendCount.Name = "lab_sendCount";
            this.lab_sendCount.Size = new System.Drawing.Size(11, 12);
            this.lab_sendCount.TabIndex = 16;
            this.lab_sendCount.Text = ".";
            // 
            // cmb_BitsPerSample
            // 
            this.cmb_BitsPerSample.FormattingEnabled = true;
            this.cmb_BitsPerSample.Items.AddRange(new object[] {
            "8",
            "16",
            "32"});
            this.cmb_BitsPerSample.Location = new System.Drawing.Point(606, 301);
            this.cmb_BitsPerSample.Name = "cmb_BitsPerSample";
            this.cmb_BitsPerSample.Size = new System.Drawing.Size(53, 20);
            this.cmb_BitsPerSample.TabIndex = 17;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(553, 304);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(47, 12);
            this.label10.TabIndex = 18;
            this.label10.Text = "编码位:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 554);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.cmb_BitsPerSample);
            this.Controls.Add(this.lab_sendCount);
            this.Controls.Add(this.cmb_SamplesPerSec);
            this.Controls.Add(this.numb_FrameInterval);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chk_IsMonoWAV);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.prog_RecordProg);
            this.Controls.Add(this.btn_SendToDevice);
            this.Controls.Add(this.chk_IsAutoSend);
            this.Controls.Add(this.btn_RecordAudio);
            this.Controls.Add(this.rText_RecordList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmb_MicDeviceList);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "语音信息DEMO";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGrid_devList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numb_FrameInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmb_MicDeviceList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rText_RecordList;
        private System.Windows.Forms.Button btn_RecordAudio;
        private System.Windows.Forms.CheckBox chk_IsAutoSend;
        private System.Windows.Forms.Button btn_SendToDevice;
        private System.Windows.Forms.ProgressBar prog_RecordProg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chk_IsMonoWAV;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numb_FrameInterval;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox cmb_SamplesPerSec;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_LoginPass;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_LoginName;
        private System.Windows.Forms.Label lab_sendCount;
        private System.Windows.Forms.Button btn_SearchDev;
        private System.Windows.Forms.DataGridView dGrid_devList;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmb_BitsPerSample;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DataGridViewTextBoxColumn AliasName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn SN;
        private System.Windows.Forms.DataGridViewTextBoxColumn IPV4;
        private System.Windows.Forms.DataGridViewTextBoxColumn HardwareVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoftwareVersion;
        private System.Windows.Forms.Button btn_RetrieveArea;
        private System.Windows.Forms.TreeView tree_Area;
    }
}

