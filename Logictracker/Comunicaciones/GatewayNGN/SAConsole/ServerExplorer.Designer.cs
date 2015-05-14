namespace SAConsole
{
    partial class ServerExplorer
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerExplorer));
            this.mainTab = new System.Windows.Forms.TabControl();
            this.devicesTab = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.devicesTree = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lblFirmware = new System.Windows.Forms.Label();
            this.btnUpFirm = new System.Windows.Forms.Button();
            this.lblNetwork = new System.Windows.Forms.Label();
            this.lblIMEI = new System.Windows.Forms.Label();
            this.gbContadores = new System.Windows.Forms.GroupBox();
            this.lblMODEM_Resets = new System.Windows.Forms.Label();
            this.lblSystemResets = new System.Windows.Forms.Label();
            this.lblWatchDogResets = new System.Windows.Forms.Label();
            this.lblNETWORK_UDP_ReceivedBytes = new System.Windows.Forms.Label();
            this.lblNETWORK_UDP_SentBytes = new System.Windows.Forms.Label();
            this.lblNETWORK_UDP_ReceivedDgrams = new System.Windows.Forms.Label();
            this.lblNETWORK_UDP_SentDgrams = new System.Windows.Forms.Label();
            this.lblNETWORK_Resets = new System.Windows.Forms.Label();
            this.lblModelo = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.firmware = new System.Windows.Forms.ListBox();
            this.firmFile = new System.Windows.Forms.TextBox();
            this.selectFirm = new System.Windows.Forms.Button();
            this.add = new System.Windows.Forms.Button();
            this.firmName = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.mainTab.SuspendLayout();
            this.devicesTab.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbContadores.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTab
            // 
            this.mainTab.Controls.Add(this.devicesTab);
            this.mainTab.Controls.Add(this.tabPage1);
            this.mainTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTab.Location = new System.Drawing.Point(0, 0);
            this.mainTab.Name = "mainTab";
            this.mainTab.SelectedIndex = 0;
            this.mainTab.Size = new System.Drawing.Size(711, 385);
            this.mainTab.TabIndex = 0;
            // 
            // devicesTab
            // 
            this.devicesTab.Controls.Add(this.splitContainer1);
            this.devicesTab.Location = new System.Drawing.Point(4, 22);
            this.devicesTab.Name = "devicesTab";
            this.devicesTab.Padding = new System.Windows.Forms.Padding(3);
            this.devicesTab.Size = new System.Drawing.Size(703, 359);
            this.devicesTab.TabIndex = 0;
            this.devicesTab.Text = "Dispositivos";
            this.devicesTab.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.devicesTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lblFirmware);
            this.splitContainer1.Panel2.Controls.Add(this.btnUpFirm);
            this.splitContainer1.Panel2.Controls.Add(this.lblNetwork);
            this.splitContainer1.Panel2.Controls.Add(this.lblIMEI);
            this.splitContainer1.Panel2.Controls.Add(this.gbContadores);
            this.splitContainer1.Panel2.Controls.Add(this.lblModelo);
            this.splitContainer1.Size = new System.Drawing.Size(697, 353);
            this.splitContainer1.SplitterDistance = 150;
            this.splitContainer1.TabIndex = 0;
            // 
            // devicesTree
            // 
            this.devicesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.devicesTree.ImageIndex = 0;
            this.devicesTree.ImageList = this.imageList1;
            this.devicesTree.Location = new System.Drawing.Point(0, 0);
            this.devicesTree.Name = "devicesTree";
            this.devicesTree.SelectedImageIndex = 0;
            this.devicesTree.ShowRootLines = false;
            this.devicesTree.Size = new System.Drawing.Size(150, 353);
            this.devicesTree.TabIndex = 0;
            this.devicesTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.devicesTree_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "offline.bmp");
            this.imageList1.Images.SetKeyName(1, "online.bmp");
            // 
            // lblFirmware
            // 
            this.lblFirmware.AutoSize = true;
            this.lblFirmware.Location = new System.Drawing.Point(19, 88);
            this.lblFirmware.Name = "lblFirmware";
            this.lblFirmware.Size = new System.Drawing.Size(52, 13);
            this.lblFirmware.TabIndex = 5;
            this.lblFirmware.Tag = "Firmware: {0}";
            this.lblFirmware.Text = "Firmware:";
            // 
            // btnUpFirm
            // 
            this.btnUpFirm.Location = new System.Drawing.Point(248, 83);
            this.btnUpFirm.Name = "btnUpFirm";
            this.btnUpFirm.Size = new System.Drawing.Size(75, 23);
            this.btnUpFirm.TabIndex = 4;
            this.btnUpFirm.Text = "Flashear";
            this.btnUpFirm.UseVisualStyleBackColor = true;
            this.btnUpFirm.Click += new System.EventHandler(this.btnUpFirm_Click);
            // 
            // lblNetwork
            // 
            this.lblNetwork.AutoSize = true;
            this.lblNetwork.Location = new System.Drawing.Point(19, 62);
            this.lblNetwork.Name = "lblNetwork";
            this.lblNetwork.Size = new System.Drawing.Size(55, 13);
            this.lblNetwork.TabIndex = 3;
            this.lblNetwork.Tag = "Direccion: {0}";
            this.lblNetwork.Text = "Direccion:";
            // 
            // lblIMEI
            // 
            this.lblIMEI.AutoSize = true;
            this.lblIMEI.Location = new System.Drawing.Point(19, 39);
            this.lblIMEI.Name = "lblIMEI";
            this.lblIMEI.Size = new System.Drawing.Size(35, 13);
            this.lblIMEI.TabIndex = 2;
            this.lblIMEI.Tag = "IMEI: {0}";
            this.lblIMEI.Text = "IMEI: ";
            // 
            // gbContadores
            // 
            this.gbContadores.Controls.Add(this.lblMODEM_Resets);
            this.gbContadores.Controls.Add(this.lblSystemResets);
            this.gbContadores.Controls.Add(this.lblWatchDogResets);
            this.gbContadores.Controls.Add(this.lblNETWORK_UDP_ReceivedBytes);
            this.gbContadores.Controls.Add(this.lblNETWORK_UDP_SentBytes);
            this.gbContadores.Controls.Add(this.lblNETWORK_UDP_ReceivedDgrams);
            this.gbContadores.Controls.Add(this.lblNETWORK_UDP_SentDgrams);
            this.gbContadores.Controls.Add(this.lblNETWORK_Resets);
            this.gbContadores.Location = new System.Drawing.Point(22, 140);
            this.gbContadores.Name = "gbContadores";
            this.gbContadores.Size = new System.Drawing.Size(301, 199);
            this.gbContadores.TabIndex = 1;
            this.gbContadores.TabStop = false;
            this.gbContadores.Text = "Contadores";
            // 
            // lblMODEM_Resets
            // 
            this.lblMODEM_Resets.AutoSize = true;
            this.lblMODEM_Resets.Location = new System.Drawing.Point(16, 172);
            this.lblMODEM_Resets.Name = "lblMODEM_Resets";
            this.lblMODEM_Resets.Size = new System.Drawing.Size(87, 13);
            this.lblMODEM_Resets.TabIndex = 5;
            this.lblMODEM_Resets.Tag = "MODEM Resets: {0}";
            this.lblMODEM_Resets.Text = "MODEM Resets:";
            // 
            // lblSystemResets
            // 
            this.lblSystemResets.AutoSize = true;
            this.lblSystemResets.Location = new System.Drawing.Point(16, 25);
            this.lblSystemResets.Name = "lblSystemResets";
            this.lblSystemResets.Size = new System.Drawing.Size(80, 13);
            this.lblSystemResets.TabIndex = 0;
            this.lblSystemResets.Tag = "System Resets: {0}";
            this.lblSystemResets.Text = "System Resets:";
            // 
            // lblWatchDogResets
            // 
            this.lblWatchDogResets.AutoSize = true;
            this.lblWatchDogResets.Location = new System.Drawing.Point(16, 46);
            this.lblWatchDogResets.Name = "lblWatchDogResets";
            this.lblWatchDogResets.Size = new System.Drawing.Size(98, 13);
            this.lblWatchDogResets.TabIndex = 0;
            this.lblWatchDogResets.Tag = "WatchDog Resets: {0}";
            this.lblWatchDogResets.Text = "WatchDog Resets:";
            // 
            // lblNETWORK_UDP_ReceivedBytes
            // 
            this.lblNETWORK_UDP_ReceivedBytes.AutoSize = true;
            this.lblNETWORK_UDP_ReceivedBytes.Location = new System.Drawing.Point(16, 67);
            this.lblNETWORK_UDP_ReceivedBytes.Name = "lblNETWORK_UDP_ReceivedBytes";
            this.lblNETWORK_UDP_ReceivedBytes.Size = new System.Drawing.Size(141, 13);
            this.lblNETWORK_UDP_ReceivedBytes.TabIndex = 0;
            this.lblNETWORK_UDP_ReceivedBytes.Tag = "NETWORK ReceivedBytes: {0}";
            this.lblNETWORK_UDP_ReceivedBytes.Text = "NETWORK ReceivedBytes:";
            // 
            // lblNETWORK_UDP_SentBytes
            // 
            this.lblNETWORK_UDP_SentBytes.AutoSize = true;
            this.lblNETWORK_UDP_SentBytes.Location = new System.Drawing.Point(16, 88);
            this.lblNETWORK_UDP_SentBytes.Name = "lblNETWORK_UDP_SentBytes";
            this.lblNETWORK_UDP_SentBytes.Size = new System.Drawing.Size(117, 13);
            this.lblNETWORK_UDP_SentBytes.TabIndex = 1;
            this.lblNETWORK_UDP_SentBytes.Tag = "NETWORK SentBytes: {0}";
            this.lblNETWORK_UDP_SentBytes.Text = "NETWORK SentBytes:";
            // 
            // lblNETWORK_UDP_ReceivedDgrams
            // 
            this.lblNETWORK_UDP_ReceivedDgrams.AutoSize = true;
            this.lblNETWORK_UDP_ReceivedDgrams.Location = new System.Drawing.Point(16, 109);
            this.lblNETWORK_UDP_ReceivedDgrams.Name = "lblNETWORK_UDP_ReceivedDgrams";
            this.lblNETWORK_UDP_ReceivedDgrams.Size = new System.Drawing.Size(151, 13);
            this.lblNETWORK_UDP_ReceivedDgrams.TabIndex = 2;
            this.lblNETWORK_UDP_ReceivedDgrams.Tag = "NETWORK ReceivedDgrams: {0}";
            this.lblNETWORK_UDP_ReceivedDgrams.Text = "NETWORK ReceivedDgrams:";
            // 
            // lblNETWORK_UDP_SentDgrams
            // 
            this.lblNETWORK_UDP_SentDgrams.AutoSize = true;
            this.lblNETWORK_UDP_SentDgrams.Location = new System.Drawing.Point(16, 130);
            this.lblNETWORK_UDP_SentDgrams.Name = "lblNETWORK_UDP_SentDgrams";
            this.lblNETWORK_UDP_SentDgrams.Size = new System.Drawing.Size(127, 13);
            this.lblNETWORK_UDP_SentDgrams.TabIndex = 3;
            this.lblNETWORK_UDP_SentDgrams.Tag = "NETWORK SentDgrams: {0}";
            this.lblNETWORK_UDP_SentDgrams.Text = "NETWORK SentDgrams:";
            // 
            // lblNETWORK_Resets
            // 
            this.lblNETWORK_Resets.AutoSize = true;
            this.lblNETWORK_Resets.Location = new System.Drawing.Point(16, 151);
            this.lblNETWORK_Resets.Name = "lblNETWORK_Resets";
            this.lblNETWORK_Resets.Size = new System.Drawing.Size(102, 13);
            this.lblNETWORK_Resets.TabIndex = 4;
            this.lblNETWORK_Resets.Tag = "NETWORK Resets: {0}";
            this.lblNETWORK_Resets.Text = "NETWORK Resets:";
            // 
            // lblModelo
            // 
            this.lblModelo.AutoSize = true;
            this.lblModelo.Location = new System.Drawing.Point(19, 15);
            this.lblModelo.Name = "lblModelo";
            this.lblModelo.Size = new System.Drawing.Size(181, 13);
            this.lblModelo.TabIndex = 0;
            this.lblModelo.Tag = "Modelo de Dispositivo: {0}";
            this.lblModelo.Text = "Modelo de Dispositivo: desconocido.";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(703, 359);
            this.tabPage1.TabIndex = 1;
            this.tabPage1.Text = "Firmware";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.firmware);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textBox1);
            this.splitContainer2.Panel2.Controls.Add(this.firmName);
            this.splitContainer2.Panel2.Controls.Add(this.add);
            this.splitContainer2.Panel2.Controls.Add(this.selectFirm);
            this.splitContainer2.Panel2.Controls.Add(this.firmFile);
            this.splitContainer2.Size = new System.Drawing.Size(697, 353);
            this.splitContainer2.SplitterDistance = 232;
            this.splitContainer2.TabIndex = 0;
            // 
            // firmware
            // 
            this.firmware.Dock = System.Windows.Forms.DockStyle.Fill;
            this.firmware.FormattingEnabled = true;
            this.firmware.Location = new System.Drawing.Point(0, 0);
            this.firmware.Name = "firmware";
            this.firmware.Size = new System.Drawing.Size(232, 342);
            this.firmware.TabIndex = 0;
            // 
            // firmFile
            // 
            this.firmFile.Location = new System.Drawing.Point(20, 79);
            this.firmFile.Name = "firmFile";
            this.firmFile.Size = new System.Drawing.Size(258, 20);
            this.firmFile.TabIndex = 0;
            // 
            // selectFirm
            // 
            this.selectFirm.Location = new System.Drawing.Point(284, 76);
            this.selectFirm.Name = "selectFirm";
            this.selectFirm.Size = new System.Drawing.Size(33, 23);
            this.selectFirm.TabIndex = 1;
            this.selectFirm.Text = "...";
            this.selectFirm.UseVisualStyleBackColor = true;
            this.selectFirm.Click += new System.EventHandler(this.selectFirm_Click);
            // 
            // add
            // 
            this.add.Location = new System.Drawing.Point(20, 106);
            this.add.Name = "add";
            this.add.Size = new System.Drawing.Size(75, 23);
            this.add.TabIndex = 2;
            this.add.Text = "Agregar";
            this.add.UseVisualStyleBackColor = true;
            this.add.Click += new System.EventHandler(this.add_Click);
            // 
            // firmName
            // 
            this.firmName.Location = new System.Drawing.Point(20, 20);
            this.firmName.Name = "firmName";
            this.firmName.Size = new System.Drawing.Size(258, 20);
            this.firmName.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(20, 46);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(258, 20);
            this.textBox1.TabIndex = 4;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // ServerExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 385);
            this.Controls.Add(this.mainTab);
            this.Name = "ServerExplorer";
            this.Text = "Explorador de Servidores";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ServerExplorer_Load);
            this.mainTab.ResumeLayout(false);
            this.devicesTab.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.gbContadores.ResumeLayout(false);
            this.gbContadores.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl mainTab;
        private System.Windows.Forms.TabPage devicesTab;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView devicesTree;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label lblModelo;
        private System.Windows.Forms.GroupBox gbContadores;
        private System.Windows.Forms.Label lblIMEI;
        private System.Windows.Forms.Label lblNetwork;
        private System.Windows.Forms.Label lblSystemResets;
        private System.Windows.Forms.Label lblWatchDogResets;
        private System.Windows.Forms.Label lblNETWORK_UDP_ReceivedBytes;
        private System.Windows.Forms.Label lblNETWORK_UDP_SentBytes;
        private System.Windows.Forms.Label lblNETWORK_UDP_ReceivedDgrams;
        private System.Windows.Forms.Label lblNETWORK_UDP_SentDgrams;
        private System.Windows.Forms.Label lblNETWORK_Resets;
        private System.Windows.Forms.Label lblMODEM_Resets;
        private System.Windows.Forms.Button btnUpFirm;
        private System.Windows.Forms.Label lblFirmware;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox firmware;
        private System.Windows.Forms.Button selectFirm;
        private System.Windows.Forms.TextBox firmFile;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox firmName;
        private System.Windows.Forms.Button add;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;


    }
}

