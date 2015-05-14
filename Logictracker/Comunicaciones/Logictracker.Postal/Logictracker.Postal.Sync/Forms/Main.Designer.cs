namespace Urbetrack.Postal.Sync.Forms
{
    partial class Main
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
            this.gbDevice = new System.Windows.Forms.GroupBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.lblDevice = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gbRoutes = new System.Windows.Forms.GroupBox();
            this.btnAdminRoute = new System.Windows.Forms.Button();
            this.btnSendRoute = new System.Windows.Forms.Button();
            this.lbRoutes = new System.Windows.Forms.ListBox();
            this.btnSync = new System.Windows.Forms.Button();
            this.gbRuta = new System.Windows.Forms.GroupBox();
            this.lblDistributor = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.gbDevice.SuspendLayout();
            this.gbRoutes.SuspendLayout();
            this.gbRuta.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbDevice
            // 
            this.gbDevice.Controls.Add(this.btnDisconnect);
            this.gbDevice.Controls.Add(this.lblDevice);
            this.gbDevice.Controls.Add(this.btnConnect);
            this.gbDevice.Controls.Add(this.label1);
            this.gbDevice.Location = new System.Drawing.Point(13, 12);
            this.gbDevice.Name = "gbDevice";
            this.gbDevice.Size = new System.Drawing.Size(200, 133);
            this.gbDevice.TabIndex = 1;
            this.gbDevice.TabStop = false;
            this.gbDevice.Text = "Dispositivo";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(112, 104);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(82, 23);
            this.btnDisconnect.TabIndex = 6;
            this.btnDisconnect.Text = "Desconectar";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.BtnDisconnectClick);
            // 
            // lblDevice
            // 
            this.lblDevice.AutoSize = true;
            this.lblDevice.Location = new System.Drawing.Point(56, 20);
            this.lblDevice.Name = "lblDevice";
            this.lblDevice.Size = new System.Drawing.Size(0, 13);
            this.lblDevice.TabIndex = 5;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(5, 104);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(82, 23);
            this.btnConnect.TabIndex = 3;
            this.btnConnect.Text = "Conectar";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.BtnConnectClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Equipo:";
            // 
            // gbRoutes
            // 
            this.gbRoutes.Controls.Add(this.btnAdminRoute);
            this.gbRoutes.Controls.Add(this.btnSendRoute);
            this.gbRoutes.Controls.Add(this.lbRoutes);
            this.gbRoutes.Location = new System.Drawing.Point(220, 12);
            this.gbRoutes.Name = "gbRoutes";
            this.gbRoutes.Size = new System.Drawing.Size(200, 272);
            this.gbRoutes.TabIndex = 2;
            this.gbRoutes.TabStop = false;
            this.gbRoutes.Text = "Rutas";
            // 
            // btnAdminRoute
            // 
            this.btnAdminRoute.Location = new System.Drawing.Point(112, 239);
            this.btnAdminRoute.Name = "btnAdminRoute";
            this.btnAdminRoute.Size = new System.Drawing.Size(82, 23);
            this.btnAdminRoute.TabIndex = 2;
            this.btnAdminRoute.Text = "Administrar";
            this.btnAdminRoute.UseVisualStyleBackColor = true;
            this.btnAdminRoute.Click += new System.EventHandler(this.BtnAdminRouteClick);
            // 
            // btnSendRoute
            // 
            this.btnSendRoute.Enabled = false;
            this.btnSendRoute.Location = new System.Drawing.Point(7, 239);
            this.btnSendRoute.Name = "btnSendRoute";
            this.btnSendRoute.Size = new System.Drawing.Size(82, 23);
            this.btnSendRoute.TabIndex = 1;
            this.btnSendRoute.Text = "A PDA";
            this.btnSendRoute.UseVisualStyleBackColor = true;
            this.btnSendRoute.Click += new System.EventHandler(this.BtnSendRouteClick);
            // 
            // lbRoutes
            // 
            this.lbRoutes.DisplayMember = "ShowName";
            this.lbRoutes.Enabled = false;
            this.lbRoutes.FormattingEnabled = true;
            this.lbRoutes.Location = new System.Drawing.Point(7, 20);
            this.lbRoutes.Name = "lbRoutes";
            this.lbRoutes.Size = new System.Drawing.Size(187, 212);
            this.lbRoutes.TabIndex = 0;
            this.lbRoutes.SelectedIndexChanged += new System.EventHandler(this.LbRoutesSelectedIndexChanged);
            // 
            // btnSync
            // 
            this.btnSync.Enabled = false;
            this.btnSync.Location = new System.Drawing.Point(6, 101);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(82, 23);
            this.btnSync.TabIndex = 0;
            this.btnSync.Text = "A PTM";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.BtnSyncClick);
            // 
            // gbRuta
            // 
            this.gbRuta.Controls.Add(this.lblDistributor);
            this.gbRuta.Controls.Add(this.label3);
            this.gbRuta.Controls.Add(this.btnSync);
            this.gbRuta.Location = new System.Drawing.Point(12, 150);
            this.gbRuta.Name = "gbRuta";
            this.gbRuta.Size = new System.Drawing.Size(200, 134);
            this.gbRuta.TabIndex = 3;
            this.gbRuta.TabStop = false;
            this.gbRuta.Text = "Ruta";
            // 
            // lblDistributor
            // 
            this.lblDistributor.AutoSize = true;
            this.lblDistributor.Location = new System.Drawing.Point(76, 16);
            this.lblDistributor.Name = "lblDistributor";
            this.lblDistributor.Size = new System.Drawing.Size(0, 13);
            this.lblDistributor.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Distribuidor:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 300);
            this.Controls.Add(this.gbRuta);
            this.Controls.Add(this.gbRoutes);
            this.Controls.Add(this.gbDevice);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.ShowIcon = false;
            this.Text = "Sincronizador";
            this.gbDevice.ResumeLayout(false);
            this.gbDevice.PerformLayout();
            this.gbRoutes.ResumeLayout(false);
            this.gbRuta.ResumeLayout(false);
            this.gbRuta.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbDevice;
        private System.Windows.Forms.GroupBox gbRoutes;
        private System.Windows.Forms.Button btnSendRoute;
        private System.Windows.Forms.ListBox lbRoutes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Button btnAdminRoute;
        private System.Windows.Forms.GroupBox gbRuta;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblDevice;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblDistributor;


    }
}

