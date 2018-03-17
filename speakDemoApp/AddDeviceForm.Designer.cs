namespace speakDemoApp
{
    partial class AddDeviceForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.txt_DeviceIPV4 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_AddDevice = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "设备IP地址：";
            // 
            // txt_DeviceIPV4
            // 
            this.txt_DeviceIPV4.Location = new System.Drawing.Point(107, 34);
            this.txt_DeviceIPV4.Name = "txt_DeviceIPV4";
            this.txt_DeviceIPV4.Size = new System.Drawing.Size(165, 21);
            this.txt_DeviceIPV4.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "mesg";
            // 
            // btn_AddDevice
            // 
            this.btn_AddDevice.Location = new System.Drawing.Point(197, 79);
            this.btn_AddDevice.Name = "btn_AddDevice";
            this.btn_AddDevice.Size = new System.Drawing.Size(75, 23);
            this.btn_AddDevice.TabIndex = 5;
            this.btn_AddDevice.Text = "确定";
            this.btn_AddDevice.UseVisualStyleBackColor = true;
            this.btn_AddDevice.Click += new System.EventHandler(this.btn_AddDevice_Click);
            // 
            // AddDeviceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 119);
            this.Controls.Add(this.btn_AddDevice);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txt_DeviceIPV4);
            this.Controls.Add(this.label1);
            this.Name = "AddDeviceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加设备";
            this.Load += new System.EventHandler(this.AddDeviceForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_DeviceIPV4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_AddDevice;
    }
}