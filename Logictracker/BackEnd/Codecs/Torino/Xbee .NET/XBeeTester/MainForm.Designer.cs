namespace XBeeTester
{
    partial class MainForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblTimeout = new System.Windows.Forms.Label();
            this.cbDTR = new System.Windows.Forms.CheckBox();
            this.cbRTS = new System.Windows.Forms.CheckBox();
            this.lblDSR = new System.Windows.Forms.Label();
            this.lblCD = new System.Windows.Forms.Label();
            this.lblCTS = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lblXbeeStat = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.updater = new System.Windows.Forms.Timer(this.components);
            this.grpSender = new System.Windows.Forms.GroupBox();
            this.lblQueue = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.packetizer = new System.Windows.Forms.Timer(this.components);
            this.btnStart = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabs = new System.Windows.Forms.TabControl();
            this.page_engine = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.page_log = new System.Windows.Forms.TabPage();
            this.log = new System.Windows.Forms.ListBox();
            this.page_info = new System.Windows.Forms.TabPage();
            this.page_loopbtest = new System.Windows.Forms.TabPage();
            this.btnClear = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.stats = new XBeeTester.FixedListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.loopbackInfo = new XBeeTester.FixedListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.tbTimeout = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tbXbeePortRate = new System.Windows.Forms.TextBox();
            this.tbXbeePortCom = new System.Windows.Forms.TextBox();
            this.tbBytesPerPacket = new System.Windows.Forms.TextBox();
            this.tbPacketsPerSecond = new System.Windows.Forms.TextBox();
            this.tbRemoteAddress = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.grpSender.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabs.SuspendLayout();
            this.page_engine.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.page_log.SuspendLayout();
            this.page_info.SuspendLayout();
            this.page_loopbtest.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.tbTimeout);
            this.groupBox1.Controls.Add(this.lblTimeout);
            this.groupBox1.Controls.Add(this.cbDTR);
            this.groupBox1.Controls.Add(this.cbRTS);
            this.groupBox1.Controls.Add(this.lblDSR);
            this.groupBox1.Controls.Add(this.lblCD);
            this.groupBox1.Controls.Add(this.lblCTS);
            this.groupBox1.Controls.Add(this.comboBox3);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.comboBox2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lblXbeeStat);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbXbeePortRate);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbXbeePortCom);
            this.groupBox1.Location = new System.Drawing.Point(7, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(225, 379);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Interface XBee";
            // 
            // lblTimeout
            // 
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Location = new System.Drawing.Point(13, 292);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(48, 13);
            this.lblTimeout.TabIndex = 18;
            this.lblTimeout.Text = "Timeout:";
            // 
            // cbDTR
            // 
            this.cbDTR.AutoSize = true;
            this.cbDTR.Location = new System.Drawing.Point(113, 266);
            this.cbDTR.Name = "cbDTR";
            this.cbDTR.Size = new System.Drawing.Size(49, 17);
            this.cbDTR.TabIndex = 17;
            this.cbDTR.Text = "DTR";
            this.cbDTR.UseVisualStyleBackColor = true;
            this.cbDTR.CheckedChanged += new System.EventHandler(this.cbDTR_CheckedChanged);
            // 
            // cbRTS
            // 
            this.cbRTS.AutoSize = true;
            this.cbRTS.Location = new System.Drawing.Point(59, 266);
            this.cbRTS.Name = "cbRTS";
            this.cbRTS.Size = new System.Drawing.Size(48, 17);
            this.cbRTS.TabIndex = 16;
            this.cbRTS.Text = "RTS";
            this.cbRTS.UseVisualStyleBackColor = true;
            this.cbRTS.CheckedChanged += new System.EventHandler(this.cbRTS_CheckedChanged);
            // 
            // lblDSR
            // 
            this.lblDSR.AutoSize = true;
            this.lblDSR.BackColor = System.Drawing.Color.Red;
            this.lblDSR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDSR.Location = new System.Drawing.Point(97, 248);
            this.lblDSR.Name = "lblDSR";
            this.lblDSR.Size = new System.Drawing.Size(32, 15);
            this.lblDSR.TabIndex = 15;
            this.lblDSR.Text = "DSR";
            // 
            // lblCD
            // 
            this.lblCD.AutoSize = true;
            this.lblCD.BackColor = System.Drawing.Color.Red;
            this.lblCD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCD.Location = new System.Drawing.Point(149, 248);
            this.lblCD.Name = "lblCD";
            this.lblCD.Size = new System.Drawing.Size(24, 15);
            this.lblCD.TabIndex = 14;
            this.lblCD.Text = "CD";
            // 
            // lblCTS
            // 
            this.lblCTS.AutoSize = true;
            this.lblCTS.BackColor = System.Drawing.Color.Red;
            this.lblCTS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCTS.Location = new System.Drawing.Point(48, 248);
            this.lblCTS.Name = "lblCTS";
            this.lblCTS.Size = new System.Drawing.Size(30, 15);
            this.lblCTS.TabIndex = 13;
            this.lblCTS.Text = "CTS";
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "NONE",
            "HARDWARE",
            "XON/XOFF"});
            this.comboBox3.Location = new System.Drawing.Point(92, 172);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(113, 21);
            this.comboBox3.TabIndex = 12;
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 173);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Ctrl. Flujo:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 144);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Stop Bits:";
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "NONE",
            "1",
            "1,5",
            "2"});
            this.comboBox2.Location = new System.Drawing.Point(92, 143);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(113, 21);
            this.comboBox2.TabIndex = 9;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 117);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Paridad:";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "NONE",
            "ODD",
            "EVEN",
            "MARK",
            "SPACE"});
            this.comboBox1.Location = new System.Drawing.Point(92, 114);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(113, 21);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.Text = "0";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 89);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Data Bits:";
            // 
            // lblXbeeStat
            // 
            this.lblXbeeStat.AutoSize = true;
            this.lblXbeeStat.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblXbeeStat.ForeColor = System.Drawing.Color.Red;
            this.lblXbeeStat.Location = new System.Drawing.Point(43, 217);
            this.lblXbeeStat.Name = "lblXbeeStat";
            this.lblXbeeStat.Size = new System.Drawing.Size(131, 22);
            this.lblXbeeStat.TabIndex = 4;
            this.lblXbeeStat.Text = "DESCONOCIDO";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Velocidad:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Puerto COM:";
            // 
            // updater
            // 
            this.updater.Enabled = true;
            this.updater.Interval = 250;
            this.updater.Tick += new System.EventHandler(this.updater_Tick);
            // 
            // grpSender
            // 
            this.grpSender.Controls.Add(this.tbBytesPerPacket);
            this.grpSender.Controls.Add(this.tbPacketsPerSecond);
            this.grpSender.Controls.Add(this.lblQueue);
            this.grpSender.Controls.Add(this.label4);
            this.grpSender.Controls.Add(this.label3);
            this.grpSender.Location = new System.Drawing.Point(242, 5);
            this.grpSender.Name = "grpSender";
            this.grpSender.Size = new System.Drawing.Size(225, 102);
            this.grpSender.TabIndex = 2;
            this.grpSender.TabStop = false;
            this.grpSender.Tag = "Generador de Paquetes {0} {1}";
            this.grpSender.Text = "Generador de Paquetes";
            // 
            // lblQueue
            // 
            this.lblQueue.AutoSize = true;
            this.lblQueue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQueue.ForeColor = System.Drawing.Color.MediumBlue;
            this.lblQueue.Location = new System.Drawing.Point(13, 78);
            this.lblQueue.Name = "lblQueue";
            this.lblQueue.Size = new System.Drawing.Size(79, 13);
            this.lblQueue.TabIndex = 2;
            this.lblQueue.Tag = "Cola Normal|Exceso de Paquetes {0}";
            this.lblQueue.Text = "Cola Normal.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Bytes x paquete:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Paquetes x segundo:";
            // 
            // packetizer
            // 
            this.packetizer.Tick += new System.EventHandler(this.packetizer_Tick);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(289, 420);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(92, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Tag = "Loopback Test|Detener";
            this.btnStart.Text = "Loopback Test";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbRemoteAddress);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(242, 117);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(225, 61);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Loopback Test";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Destino:";
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.page_engine);
            this.tabs.Controls.Add(this.page_log);
            this.tabs.Controls.Add(this.page_info);
            this.tabs.Controls.Add(this.page_loopbtest);
            this.tabs.Location = new System.Drawing.Point(2, 2);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(481, 416);
            this.tabs.TabIndex = 9;
            // 
            // page_engine
            // 
            this.page_engine.Controls.Add(this.groupBox3);
            this.page_engine.Controls.Add(this.groupBox1);
            this.page_engine.Controls.Add(this.grpSender);
            this.page_engine.Controls.Add(this.groupBox2);
            this.page_engine.Location = new System.Drawing.Point(4, 22);
            this.page_engine.Name = "page_engine";
            this.page_engine.Padding = new System.Windows.Forms.Padding(3);
            this.page_engine.Size = new System.Drawing.Size(473, 390);
            this.page_engine.TabIndex = 0;
            this.page_engine.Text = "Setup";
            this.page_engine.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox3);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.textBox2);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Location = new System.Drawing.Point(242, 184);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(225, 200);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Transfer Test";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 33);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Destino:";
            // 
            // page_log
            // 
            this.page_log.Controls.Add(this.log);
            this.page_log.Location = new System.Drawing.Point(4, 22);
            this.page_log.Name = "page_log";
            this.page_log.Padding = new System.Windows.Forms.Padding(3);
            this.page_log.Size = new System.Drawing.Size(473, 390);
            this.page_log.TabIndex = 3;
            this.page_log.Text = "Stack Log";
            this.page_log.UseVisualStyleBackColor = true;
            // 
            // log
            // 
            this.log.FormattingEnabled = true;
            this.log.Location = new System.Drawing.Point(6, 6);
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(460, 381);
            this.log.TabIndex = 14;
            // 
            // page_info
            // 
            this.page_info.Controls.Add(this.stats);
            this.page_info.Location = new System.Drawing.Point(4, 22);
            this.page_info.Name = "page_info";
            this.page_info.Padding = new System.Windows.Forms.Padding(3);
            this.page_info.Size = new System.Drawing.Size(473, 390);
            this.page_info.TabIndex = 1;
            this.page_info.Text = "Stack Info";
            this.page_info.UseVisualStyleBackColor = true;
            // 
            // page_loopbtest
            // 
            this.page_loopbtest.Controls.Add(this.loopbackInfo);
            this.page_loopbtest.Location = new System.Drawing.Point(4, 22);
            this.page_loopbtest.Name = "page_loopbtest";
            this.page_loopbtest.Padding = new System.Windows.Forms.Padding(3);
            this.page_loopbtest.Size = new System.Drawing.Size(473, 390);
            this.page_loopbtest.TabIndex = 2;
            this.page_loopbtest.Text = "Loopback Test Info";
            this.page_loopbtest.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(215, 420);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(68, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Limpiar";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(387, 420);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 23);
            this.button1.TabIndex = 11;
            this.button1.Tag = "Transfer Test|Detener";
            this.button1.Text = "Transfer Test";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(11, 64);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 23);
            this.button2.TabIndex = 4;
            this.button2.Tag = "Loopback Test|Detener";
            this.button2.Text = "Comando AT";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // stats
            // 
            this.stats.AllowColumnReorder = true;
            this.stats.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.stats.FullRowSelect = true;
            this.stats.GridLines = true;
            this.stats.Location = new System.Drawing.Point(6, 6);
            this.stats.Name = "stats";
            this.stats.Size = new System.Drawing.Size(461, 378);
            this.stats.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.stats.TabIndex = 0;
            this.stats.UseCompatibleStateImageBehavior = false;
            this.stats.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Propiedad";
            this.columnHeader1.Width = 168;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Valor";
            // 
            // loopbackInfo
            // 
            this.loopbackInfo.AllowColumnReorder = true;
            this.loopbackInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.loopbackInfo.FullRowSelect = true;
            this.loopbackInfo.GridLines = true;
            this.loopbackInfo.Location = new System.Drawing.Point(6, 6);
            this.loopbackInfo.Name = "loopbackInfo";
            this.loopbackInfo.Size = new System.Drawing.Size(461, 378);
            this.loopbackInfo.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.loopbackInfo.TabIndex = 1;
            this.loopbackInfo.UseCompatibleStateImageBehavior = false;
            this.loopbackInfo.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Propiedad";
            this.columnHeader3.Width = 168;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Valor";
            // 
            // textBox3
            // 
            this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::XBeeTester.Properties.Settings.Default, "DeviceChannel", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox3.Location = new System.Drawing.Point(109, 66);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(96, 20);
            this.textBox3.TabIndex = 5;
            this.textBox3.Text = global::XBeeTester.Properties.Settings.Default.DeviceChannel;
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(60, 30);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(145, 20);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = global::XBeeTester.Properties.Settings.Default.RemoteAddress;
            // 
            // textBox4
            // 
            this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::XBeeTester.Properties.Settings.Default, "XbeeChannel", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox4.Location = new System.Drawing.Point(92, 325);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(113, 20);
            this.textBox4.TabIndex = 20;
            this.textBox4.Text = global::XBeeTester.Properties.Settings.Default.XbeeChannel;
            // 
            // tbTimeout
            // 
            this.tbTimeout.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::XBeeTester.Properties.Settings.Default, "XBeeConnectionTimeout", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbTimeout.Location = new System.Drawing.Point(92, 289);
            this.tbTimeout.Name = "tbTimeout";
            this.tbTimeout.Size = new System.Drawing.Size(113, 20);
            this.tbTimeout.TabIndex = 19;
            this.tbTimeout.Text = global::XBeeTester.Properties.Settings.Default.XBeeConnectionTimeout;
            this.tbTimeout.TextChanged += new System.EventHandler(this.tbTimeout_TextChanged);
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::XBeeTester.Properties.Settings.Default, "XBeePortDataBits", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(92, 86);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(113, 20);
            this.textBox1.TabIndex = 6;
            this.textBox1.Text = global::XBeeTester.Properties.Settings.Default.XBeePortDataBits;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // tbXbeePortRate
            // 
            this.tbXbeePortRate.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::XBeeTester.Properties.Settings.Default, "XBeePortRate", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbXbeePortRate.Location = new System.Drawing.Point(92, 58);
            this.tbXbeePortRate.Name = "tbXbeePortRate";
            this.tbXbeePortRate.Size = new System.Drawing.Size(113, 20);
            this.tbXbeePortRate.TabIndex = 2;
            this.tbXbeePortRate.Text = global::XBeeTester.Properties.Settings.Default.XBeePortRate;
            this.tbXbeePortRate.TextChanged += new System.EventHandler(this.tbXbeePortRate_TextChanged);
            // 
            // tbXbeePortCom
            // 
            this.tbXbeePortCom.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::XBeeTester.Properties.Settings.Default, "XBeePortCOM", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tbXbeePortCom.Location = new System.Drawing.Point(92, 30);
            this.tbXbeePortCom.Name = "tbXbeePortCom";
            this.tbXbeePortCom.Size = new System.Drawing.Size(113, 20);
            this.tbXbeePortCom.TabIndex = 0;
            this.tbXbeePortCom.Text = global::XBeeTester.Properties.Settings.Default.XBeePortCOM;
            this.tbXbeePortCom.TextChanged += new System.EventHandler(this.tbXbeePortCom_TextChanged);
            // 
            // tbBytesPerPacket
            // 
            this.tbBytesPerPacket.Location = new System.Drawing.Point(126, 49);
            this.tbBytesPerPacket.Name = "tbBytesPerPacket";
            this.tbBytesPerPacket.Size = new System.Drawing.Size(79, 20);
            this.tbBytesPerPacket.TabIndex = 4;
            this.tbBytesPerPacket.Text = global::XBeeTester.Properties.Settings.Default.BytesPerPacket;
            this.tbBytesPerPacket.TextChanged += new System.EventHandler(this.tbBytesPerPacket_TextChanged);
            // 
            // tbPacketsPerSecond
            // 
            this.tbPacketsPerSecond.Location = new System.Drawing.Point(126, 24);
            this.tbPacketsPerSecond.Name = "tbPacketsPerSecond";
            this.tbPacketsPerSecond.Size = new System.Drawing.Size(79, 20);
            this.tbPacketsPerSecond.TabIndex = 3;
            this.tbPacketsPerSecond.Text = global::XBeeTester.Properties.Settings.Default.PacketsPerSecond;
            this.tbPacketsPerSecond.TextChanged += new System.EventHandler(this.tbPacketsPerSecond_TextChanged);
            // 
            // tbRemoteAddress
            // 
            this.tbRemoteAddress.Location = new System.Drawing.Point(60, 30);
            this.tbRemoteAddress.Name = "tbRemoteAddress";
            this.tbRemoteAddress.Size = new System.Drawing.Size(145, 20);
            this.tbRemoteAddress.TabIndex = 1;
            this.tbRemoteAddress.Text = global::XBeeTester.Properties.Settings.Default.RemoteAddress;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(12, 322);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(54, 23);
            this.button3.TabIndex = 21;
            this.button3.Tag = "Loopback Test|Detener";
            this.button3.Text = "Canal";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.btnStart);
            this.Name = "MainForm";
            this.Text = " ";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(MainForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpSender.ResumeLayout(false);
            this.grpSender.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.page_engine.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.page_log.ResumeLayout(false);
            this.page_info.ResumeLayout(false);
            this.page_loopbtest.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblXbeeStat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbXbeePortRate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbXbeePortCom;
        private System.Windows.Forms.Timer updater;
        private System.Windows.Forms.GroupBox grpSender;
        private System.Windows.Forms.Timer packetizer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblQueue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbBytesPerPacket;
        private System.Windows.Forms.TextBox tbPacketsPerSecond;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbRemoteAddress;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage page_engine;
        private System.Windows.Forms.TabPage page_info;
        private XBeeTester.FixedListView stats;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TabPage page_loopbtest;
        private FixedListView loopbackInfo;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.TabPage page_log;
        private System.Windows.Forms.ListBox log;
        private System.Windows.Forms.CheckBox cbDTR;
        private System.Windows.Forms.CheckBox cbRTS;
        private System.Windows.Forms.Label lblDSR;
        private System.Windows.Forms.Label lblCD;
        private System.Windows.Forms.Label lblCTS;
        private System.Windows.Forms.TextBox tbTimeout;
        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button button3;
    }
}

