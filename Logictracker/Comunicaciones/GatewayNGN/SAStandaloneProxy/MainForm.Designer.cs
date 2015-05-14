namespace Urbetrack.GatewayMovil
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDSR = new System.Windows.Forms.Label();
            this.lblCD = new System.Windows.Forms.Label();
            this.lblCTS = new System.Windows.Forms.Label();
            this.lblXbeeStat = new System.Windows.Forms.Label();
            this.updater = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.coreState = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblStPosi = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblStPump = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.lblInterQ = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblSpineState = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.svcInterQ = new System.ServiceProcess.ServiceController();
            this.svcOVPN = new System.ServiceProcess.ServiceController();
            this.descarga = new System.Windows.Forms.Button();
            this.Grid = new SourceGrid.Grid();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(8, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Estado XBEE:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // lblDSR
            // 
            this.lblDSR.AutoSize = true;
            this.lblDSR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.lblDSR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDSR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDSR.ForeColor = System.Drawing.Color.Yellow;
            this.lblDSR.Location = new System.Drawing.Point(130, 60);
            this.lblDSR.Name = "lblDSR";
            this.lblDSR.Size = new System.Drawing.Size(35, 15);
            this.lblDSR.TabIndex = 19;
            this.lblDSR.Text = "DSR";
            // 
            // lblCD
            // 
            this.lblCD.AutoSize = true;
            this.lblCD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.lblCD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCD.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCD.ForeColor = System.Drawing.Color.Yellow;
            this.lblCD.Location = new System.Drawing.Point(171, 60);
            this.lblCD.Name = "lblCD";
            this.lblCD.Size = new System.Drawing.Size(26, 15);
            this.lblCD.TabIndex = 18;
            this.lblCD.Text = "CD";
            // 
            // lblCTS
            // 
            this.lblCTS.AutoSize = true;
            this.lblCTS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(188)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            this.lblCTS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCTS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCTS.ForeColor = System.Drawing.Color.Yellow;
            this.lblCTS.Location = new System.Drawing.Point(91, 60);
            this.lblCTS.Name = "lblCTS";
            this.lblCTS.Size = new System.Drawing.Size(33, 15);
            this.lblCTS.TabIndex = 17;
            this.lblCTS.Text = "CTS";
            // 
            // lblXbeeStat
            // 
            this.lblXbeeStat.AutoSize = true;
            this.lblXbeeStat.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblXbeeStat.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblXbeeStat.Location = new System.Drawing.Point(88, 43);
            this.lblXbeeStat.Name = "lblXbeeStat";
            this.lblXbeeStat.Size = new System.Drawing.Size(116, 17);
            this.lblXbeeStat.TabIndex = 16;
            this.lblXbeeStat.Text = "ESPERA DATOS";
            // 
            // updater
            // 
            this.updater.Enabled = true;
            this.updater.Interval = 250;
            this.updater.Tick += new System.EventHandler(this.updater_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.Controls.Add(this.coreState);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.lblCTS);
            this.panel1.Controls.Add(this.lblStPosi);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.lblStPump);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.pictureBox3);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblDSR);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.lblCD);
            this.panel1.Controls.Add(this.lblXbeeStat);
            this.panel1.Location = new System.Drawing.Point(7, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(818, 88);
            this.panel1.TabIndex = 1;
            // 
            // coreState
            // 
            this.coreState.AutoSize = true;
            this.coreState.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.coreState.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.coreState.Location = new System.Drawing.Point(299, 44);
            this.coreState.Name = "coreState";
            this.coreState.Size = new System.Drawing.Size(26, 17);
            this.coreState.TabIndex = 30;
            this.coreState.Text = "??";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(210, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 30;
            this.label5.Text = "Sistema Central:";
            this.label5.DoubleClick += new System.EventHandler(this.label5_DoubleClick);
            // 
            // lblStPosi
            // 
            this.lblStPosi.AutoSize = true;
            this.lblStPosi.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStPosi.ForeColor = System.Drawing.Color.DarkRed;
            this.lblStPosi.Location = new System.Drawing.Point(559, 61);
            this.lblStPosi.Name = "lblStPosi";
            this.lblStPosi.Size = new System.Drawing.Size(26, 17);
            this.lblStPosi.TabIndex = 33;
            this.lblStPosi.Text = "??";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(436, 62);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(105, 13);
            this.label8.TabIndex = 32;
            this.label8.Text = "Cola - POSICIONES:";
            // 
            // lblStPump
            // 
            this.lblStPump.AutoSize = true;
            this.lblStPump.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStPump.ForeColor = System.Drawing.Color.DarkRed;
            this.lblStPump.Location = new System.Drawing.Point(559, 43);
            this.lblStPump.Name = "lblStPump";
            this.lblStPump.Size = new System.Drawing.Size(26, 17);
            this.lblStPump.TabIndex = 31;
            this.lblStPump.Text = "??";
            this.lblStPump.Click += new System.EventHandler(this.lblStPump_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(436, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "Cola - COMBUSTIBLE:";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::Urbetrack.GatewayMovil.Recursos.gateway_mobile;
            this.pictureBox3.Location = new System.Drawing.Point(113, 0);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(54, 31);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox3.TabIndex = 25;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Urbetrack.GatewayMovil.Recursos.logo_urbetrack;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(114, 31);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 24;
            this.pictureBox2.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Puerto COM:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Image = global::Urbetrack.GatewayMovil.Recursos.barra;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(818, 31);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 22;
            this.pictureBox1.TabStop = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(107, 392);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(72, 13);
            this.linkLabel1.TabIndex = 7;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Nomeclaturas";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // lblInterQ
            // 
            this.lblInterQ.AutoSize = true;
            this.lblInterQ.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInterQ.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblInterQ.Location = new System.Drawing.Point(611, 391);
            this.lblInterQ.Name = "lblInterQ";
            this.lblInterQ.Size = new System.Drawing.Size(26, 17);
            this.lblInterQ.TabIndex = 29;
            this.lblInterQ.Text = "??";
            this.lblInterQ.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(518, 392);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Estado INTERQ:";
            this.label4.Visible = false;
            // 
            // lblSpineState
            // 
            this.lblSpineState.AutoSize = true;
            this.lblSpineState.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSpineState.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.lblSpineState.Location = new System.Drawing.Point(395, 391);
            this.lblSpineState.Name = "lblSpineState";
            this.lblSpineState.Size = new System.Drawing.Size(26, 17);
            this.lblSpineState.TabIndex = 27;
            this.lblSpineState.Text = "??";
            this.lblSpineState.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(311, 392);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Estado SPINE:";
            this.label3.Visible = false;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Gainsboro;
            this.button1.Location = new System.Drawing.Point(742, 387);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Terminar";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // svcInterQ
            // 
            this.svcInterQ.ServiceName = "Urbetrack.InterQ";
            // 
            // svcOVPN
            // 
            this.svcOVPN.ServiceName = "OpenVPNService";
            // 
            // descarga
            // 
            this.descarga.BackColor = System.Drawing.Color.Gainsboro;
            this.descarga.Location = new System.Drawing.Point(10, 387);
            this.descarga.Name = "descarga";
            this.descarga.Size = new System.Drawing.Size(91, 23);
            this.descarga.TabIndex = 6;
            this.descarga.Text = "Actualizar Base";
            this.descarga.UseVisualStyleBackColor = false;
            this.descarga.Click += new System.EventHandler(this.button2_Click);
            // 
            // Grid
            // 
            this.Grid.BackColor = System.Drawing.Color.White;
            this.Grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Grid.ColumnsCount = 5;
            this.Grid.EnableSort = true;
            this.Grid.FixedRows = 1;
            this.Grid.Location = new System.Drawing.Point(9, 116);
            this.Grid.Name = "Grid";
            this.Grid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.Grid.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.Grid.Size = new System.Drawing.Size(816, 257);
            this.Grid.TabIndex = 5;
            this.Grid.TabStop = true;
            this.Grid.ToolTipText = "";
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.Gray;
            this.pictureBox4.Location = new System.Drawing.Point(10, 120);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(819, 258);
            this.pictureBox4.TabIndex = 3;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.Gray;
            this.pictureBox5.Location = new System.Drawing.Point(10, 19);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(819, 80);
            this.pictureBox5.TabIndex = 4;
            this.pictureBox5.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(108)))), ((int)(((byte)(3)))));
            this.ClientSize = new System.Drawing.Size(832, 418);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.descarga);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Grid);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblInterQ);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.lblSpineState);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = " Urbetrack - Gateway Movil";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.MainForm_HelpButtonClicked);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDSR;
        private System.Windows.Forms.Label lblCD;
        private System.Windows.Forms.Label lblCTS;
        private System.Windows.Forms.Label lblXbeeStat;
        private System.Windows.Forms.Timer updater;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Label lblSpineState;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblInterQ;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblStPosi;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblStPump;
        private System.ServiceProcess.ServiceController svcInterQ;
        internal SourceGrid.Grid Grid;
        private System.ServiceProcess.ServiceController svcOVPN;
        private System.Windows.Forms.Button descarga;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label coreState;
    }
}