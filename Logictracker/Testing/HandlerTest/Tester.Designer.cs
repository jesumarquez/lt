namespace HandlerTest
{
    partial class Tester
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
            this.timerGen = new System.Windows.Forms.Timer(this.components);
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.radTDGlobalSat = new System.Windows.Forms.RadioButton();
            this.radTDEnfora = new System.Windows.Forms.RadioButton();
            this.radTDFulmar = new System.Windows.Forms.RadioButton();
            this.radTDUnetelv2 = new System.Windows.Forms.RadioButton();
            this.radTDGte = new System.Windows.Forms.RadioButton();
            this.radTDUnetelv1 = new System.Windows.Forms.RadioButton();
            this.radDisp = new System.Windows.Forms.RadioButton();
            this.chkVeloAutoGen = new System.Windows.Forms.CheckBox();
            this.panSendGen = new System.Windows.Forms.Panel();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.btGenerador = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.numVeloGen = new System.Windows.Forms.NumericUpDown();
            this.numFrecuenciaGen = new System.Windows.Forms.NumericUpDown();
            this.cbVehiculosGen = new System.Windows.Forms.CheckedListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.btEntregaCancelada = new System.Windows.Forms.Button();
            this.btEntregaCompletada = new System.Windows.Forms.Button();
            this.cbEntregas = new System.Windows.Forms.ListBox();
            this.lblEntregas = new System.Windows.Forms.Label();
            this.btCargarDistribucion = new System.Windows.Forms.Button();
            this.btSendMessage = new System.Windows.Forms.Button();
            this.txtTextoMensaje = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cbMensaje = new System.Windows.Forms.ComboBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtQueueName = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.cbFuera = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbDentro = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtVelocidad = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.lblEnviado = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbVehiculo = new System.Windows.Forms.ComboBox();
            this.cbEmpresa = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbLinea = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtLatitud = new System.Windows.Forms.TextBox();
            this.txtLongitud = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.mapControl1 = new Compumap.Controls.Map.MapControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.chkDatamartRegenera = new System.Windows.Forms.CheckBox();
            this.btDatamart = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.dthastaDatamart = new System.Windows.Forms.DateTimePicker();
            this.dtDesdeDatamart = new System.Windows.Forms.DateTimePicker();
            this.cbVehiculosDatamart = new System.Windows.Forms.ListBox();
            this.timerSent = new System.Windows.Forms.Timer(this.components);
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVeloGen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFrecuenciaGen)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtVelocidad)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerGen
            // 
            this.timerGen.Tick += new System.EventHandler(this.TimerGenTick);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.radTDGlobalSat);
            this.tabPage4.Controls.Add(this.radTDEnfora);
            this.tabPage4.Controls.Add(this.radTDFulmar);
            this.tabPage4.Controls.Add(this.radTDUnetelv2);
            this.tabPage4.Controls.Add(this.radTDGte);
            this.tabPage4.Controls.Add(this.radTDUnetelv1);
            this.tabPage4.Controls.Add(this.radDisp);
            this.tabPage4.Controls.Add(this.chkVeloAutoGen);
            this.tabPage4.Controls.Add(this.panSendGen);
            this.tabPage4.Controls.Add(this.button5);
            this.tabPage4.Controls.Add(this.button6);
            this.tabPage4.Controls.Add(this.btGenerador);
            this.tabPage4.Controls.Add(this.label12);
            this.tabPage4.Controls.Add(this.label14);
            this.tabPage4.Controls.Add(this.label11);
            this.tabPage4.Controls.Add(this.numVeloGen);
            this.tabPage4.Controls.Add(this.numFrecuenciaGen);
            this.tabPage4.Controls.Add(this.cbVehiculosGen);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(897, 475);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Generador";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // radTDGlobalSat
            // 
            this.radTDGlobalSat.AutoSize = true;
            this.radTDGlobalSat.Location = new System.Drawing.Point(732, 393);
            this.radTDGlobalSat.Name = "radTDGlobalSat";
            this.radTDGlobalSat.Size = new System.Drawing.Size(69, 17);
            this.radTDGlobalSat.TabIndex = 8;
            this.radTDGlobalSat.Text = "GobalSat";
            this.radTDGlobalSat.UseVisualStyleBackColor = true;
            // 
            // radTDEnfora
            // 
            this.radTDEnfora.AutoSize = true;
            this.radTDEnfora.Location = new System.Drawing.Point(732, 370);
            this.radTDEnfora.Name = "radTDEnfora";
            this.radTDEnfora.Size = new System.Drawing.Size(56, 17);
            this.radTDEnfora.TabIndex = 8;
            this.radTDEnfora.Text = "Enfora";
            this.radTDEnfora.UseVisualStyleBackColor = true;
            // 
            // radTDFulmar
            // 
            this.radTDFulmar.AutoSize = true;
            this.radTDFulmar.Location = new System.Drawing.Point(732, 347);
            this.radTDFulmar.Name = "radTDFulmar";
            this.radTDFulmar.Size = new System.Drawing.Size(56, 17);
            this.radTDFulmar.TabIndex = 8;
            this.radTDFulmar.Text = "Fulmar";
            this.radTDFulmar.UseVisualStyleBackColor = true;
            // 
            // radTDUnetelv2
            // 
            this.radTDUnetelv2.AutoSize = true;
            this.radTDUnetelv2.Location = new System.Drawing.Point(732, 324);
            this.radTDUnetelv2.Name = "radTDUnetelv2";
            this.radTDUnetelv2.Size = new System.Drawing.Size(72, 17);
            this.radTDUnetelv2.TabIndex = 8;
            this.radTDUnetelv2.Text = "Unetel V2";
            this.radTDUnetelv2.UseVisualStyleBackColor = true;
            // 
            // radTDGte
            // 
            this.radTDGte.AutoSize = true;
            this.radTDGte.Checked = true;
            this.radTDGte.Location = new System.Drawing.Point(732, 278);
            this.radTDGte.Name = "radTDGte";
            this.radTDGte.Size = new System.Drawing.Size(47, 17);
            this.radTDGte.TabIndex = 8;
            this.radTDGte.TabStop = true;
            this.radTDGte.Text = "GTE";
            this.radTDGte.UseVisualStyleBackColor = true;
            // 
            // radTDUnetelv1
            // 
            this.radTDUnetelv1.AutoSize = true;
            this.radTDUnetelv1.Location = new System.Drawing.Point(732, 301);
            this.radTDUnetelv1.Name = "radTDUnetelv1";
            this.radTDUnetelv1.Size = new System.Drawing.Size(72, 17);
            this.radTDUnetelv1.TabIndex = 8;
            this.radTDUnetelv1.Text = "Unetel V1";
            this.radTDUnetelv1.UseVisualStyleBackColor = true;
            // 
            // radDisp
            // 
            this.radDisp.AutoSize = true;
            this.radDisp.Location = new System.Drawing.Point(732, 255);
            this.radDisp.Name = "radDisp";
            this.radDisp.Size = new System.Drawing.Size(88, 17);
            this.radDisp.TabIndex = 7;
            this.radDisp.Text = "Al Dispatcher";
            this.radDisp.UseVisualStyleBackColor = true;
            // 
            // chkVeloAutoGen
            // 
            this.chkVeloAutoGen.AutoSize = true;
            this.chkVeloAutoGen.Location = new System.Drawing.Point(808, 137);
            this.chkVeloAutoGen.Name = "chkVeloAutoGen";
            this.chkVeloAutoGen.Size = new System.Drawing.Size(48, 17);
            this.chkVeloAutoGen.TabIndex = 5;
            this.chkVeloAutoGen.Text = "Auto";
            this.chkVeloAutoGen.UseVisualStyleBackColor = true;
            this.chkVeloAutoGen.CheckedChanged += new System.EventHandler(this.ChkVeloAutoGenCheckedChanged);
            // 
            // panSendGen
            // 
            this.panSendGen.BackColor = System.Drawing.Color.DarkGreen;
            this.panSendGen.Location = new System.Drawing.Point(872, 455);
            this.panSendGen.Name = "panSendGen";
            this.panSendGen.Size = new System.Drawing.Size(29, 24);
            this.panSendGen.TabIndex = 6;
            this.panSendGen.Visible = false;
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.Location = new System.Drawing.Point(652, 438);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 3;
            this.button5.Text = "Todo";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.BtSelectGenClick);
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.Location = new System.Drawing.Point(673, 177);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 3;
            this.button6.Text = "Enviar Una";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.Button6Click);
            // 
            // btGenerador
            // 
            this.btGenerador.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btGenerador.Location = new System.Drawing.Point(808, 177);
            this.btGenerador.Name = "btGenerador";
            this.btGenerador.Size = new System.Drawing.Size(75, 23);
            this.btGenerador.TabIndex = 3;
            this.btGenerador.Text = "Iniciar";
            this.btGenerador.UseVisualStyleBackColor = true;
            this.btGenerador.Click += new System.EventHandler(this.BtGeneradorClick);
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(827, 11);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "segundos.";
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(670, 138);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 13);
            this.label14.TabIndex = 2;
            this.label14.Text = "Velocidad";
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(670, 11);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(75, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Reportar cada";
            // 
            // numVeloGen
            // 
            this.numVeloGen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numVeloGen.Location = new System.Drawing.Point(732, 134);
            this.numVeloGen.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numVeloGen.Name = "numVeloGen";
            this.numVeloGen.Size = new System.Drawing.Size(70, 20);
            this.numVeloGen.TabIndex = 1;
            // 
            // numFrecuenciaGen
            // 
            this.numFrecuenciaGen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numFrecuenciaGen.Location = new System.Drawing.Point(751, 6);
            this.numFrecuenciaGen.Name = "numFrecuenciaGen";
            this.numFrecuenciaGen.Size = new System.Drawing.Size(70, 20);
            this.numFrecuenciaGen.TabIndex = 1;
            this.numFrecuenciaGen.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // cbVehiculosGen
            // 
            this.cbVehiculosGen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbVehiculosGen.FormattingEnabled = true;
            this.cbVehiculosGen.Location = new System.Drawing.Point(9, 7);
            this.cbVehiculosGen.Name = "cbVehiculosGen";
            this.cbVehiculosGen.Size = new System.Drawing.Size(637, 454);
            this.cbVehiculosGen.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox7);
            this.tabPage3.Controls.Add(this.btSendMessage);
            this.tabPage3.Controls.Add(this.txtTextoMensaje);
            this.tabPage3.Controls.Add(this.label10);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.cbMensaje);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(897, 475);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Messages";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.btEntregaCancelada);
            this.groupBox7.Controls.Add(this.btEntregaCompletada);
            this.groupBox7.Controls.Add(this.cbEntregas);
            this.groupBox7.Controls.Add(this.lblEntregas);
            this.groupBox7.Controls.Add(this.btCargarDistribucion);
            this.groupBox7.Location = new System.Drawing.Point(417, 36);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(431, 401);
            this.groupBox7.TabIndex = 4;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Distribucion";
            // 
            // btEntregaCancelada
            // 
            this.btEntregaCancelada.Location = new System.Drawing.Point(259, 372);
            this.btEntregaCancelada.Name = "btEntregaCancelada";
            this.btEntregaCancelada.Size = new System.Drawing.Size(151, 23);
            this.btEntregaCancelada.TabIndex = 8;
            this.btEntregaCancelada.Text = "Cancelado";
            this.btEntregaCancelada.UseVisualStyleBackColor = true;
            this.btEntregaCancelada.Click += new System.EventHandler(this.btEntregaCancelada_Click);
            // 
            // btEntregaCompletada
            // 
            this.btEntregaCompletada.Location = new System.Drawing.Point(259, 318);
            this.btEntregaCompletada.Name = "btEntregaCompletada";
            this.btEntregaCompletada.Size = new System.Drawing.Size(151, 48);
            this.btEntregaCompletada.TabIndex = 8;
            this.btEntregaCompletada.Text = "Completado";
            this.btEntregaCompletada.UseVisualStyleBackColor = true;
            this.btEntregaCompletada.Click += new System.EventHandler(this.btEntregaCompletada_Click);
            // 
            // cbEntregas
            // 
            this.cbEntregas.DisplayMember = "Descripcion";
            this.cbEntregas.FormattingEnabled = true;
            this.cbEntregas.Location = new System.Drawing.Point(9, 94);
            this.cbEntregas.Name = "cbEntregas";
            this.cbEntregas.Size = new System.Drawing.Size(226, 303);
            this.cbEntregas.TabIndex = 7;
            // 
            // lblEntregas
            // 
            this.lblEntregas.AutoSize = true;
            this.lblEntregas.Location = new System.Drawing.Point(6, 78);
            this.lblEntregas.Name = "lblEntregas";
            this.lblEntregas.Size = new System.Drawing.Size(49, 13);
            this.lblEntregas.TabIndex = 6;
            this.lblEntregas.Text = "Entregas";
            // 
            // btCargarDistribucion
            // 
            this.btCargarDistribucion.Location = new System.Drawing.Point(6, 19);
            this.btCargarDistribucion.Name = "btCargarDistribucion";
            this.btCargarDistribucion.Size = new System.Drawing.Size(151, 23);
            this.btCargarDistribucion.TabIndex = 4;
            this.btCargarDistribucion.Text = "Cargar";
            this.btCargarDistribucion.UseVisualStyleBackColor = true;
            this.btCargarDistribucion.Click += new System.EventHandler(this.btCargarDistribucion_Click);
            // 
            // btSendMessage
            // 
            this.btSendMessage.Location = new System.Drawing.Point(16, 109);
            this.btSendMessage.Name = "btSendMessage";
            this.btSendMessage.Size = new System.Drawing.Size(151, 23);
            this.btSendMessage.TabIndex = 3;
            this.btSendMessage.Text = "Save";
            this.btSendMessage.UseVisualStyleBackColor = true;
            this.btSendMessage.Click += new System.EventHandler(this.btSendMessage_Click);
            // 
            // txtTextoMensaje
            // 
            this.txtTextoMensaje.Location = new System.Drawing.Point(81, 71);
            this.txtTextoMensaje.Name = "txtTextoMensaje";
            this.txtTextoMensaje.Size = new System.Drawing.Size(249, 20);
            this.txtTextoMensaje.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 74);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(61, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Texto Extra";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Mensaje";
            // 
            // cbMensaje
            // 
            this.cbMensaje.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMensaje.FormattingEnabled = true;
            this.cbMensaje.Location = new System.Drawing.Point(81, 36);
            this.cbMensaje.Name = "cbMensaje";
            this.cbMensaje.Size = new System.Drawing.Size(249, 21);
            this.cbMensaje.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(897, 475);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Map";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox5);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox4);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.mapControl1);
            this.splitContainer1.Size = new System.Drawing.Size(891, 469);
            this.splitContainer1.SplitterDistance = 483;
            this.splitContainer1.TabIndex = 1;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtQueueName);
            this.groupBox5.Location = new System.Drawing.Point(5, 250);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(257, 53);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Message Queue";
            // 
            // txtQueueName
            // 
            this.txtQueueName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQueueName.Location = new System.Drawing.Point(21, 19);
            this.txtQueueName.Name = "txtQueueName";
            this.txtQueueName.Size = new System.Drawing.Size(215, 20);
            this.txtQueueName.TabIndex = 6;
            this.txtQueueName.Text = ".\\private$\\eventos_trax";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.button3);
            this.groupBox4.Controls.Add(this.button4);
            this.groupBox4.Controls.Add(this.cbFuera);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.cbDentro);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Location = new System.Drawing.Point(269, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(211, 462);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Geocercas";
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(67, 220);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(56, 23);
            this.button3.TabIndex = 11;
            this.button3.Text = "Refresh";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(129, 220);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(76, 23);
            this.button4.TabIndex = 10;
            this.button4.Text = "Recalcular";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // cbFuera
            // 
            this.cbFuera.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFuera.FormattingEnabled = true;
            this.cbFuera.Location = new System.Drawing.Point(6, 246);
            this.cbFuera.Name = "cbFuera";
            this.cbFuera.Size = new System.Drawing.Size(199, 212);
            this.cbFuera.TabIndex = 9;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 230);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Fuera";
            // 
            // cbDentro
            // 
            this.cbDentro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDentro.FormattingEnabled = true;
            this.cbDentro.Location = new System.Drawing.Point(6, 29);
            this.cbDentro.Name = "cbDentro";
            this.cbDentro.Size = new System.Drawing.Size(199, 186);
            this.cbDentro.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Dentro";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtVelocidad);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.lblEnviado);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(6, 309);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(257, 77);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Envio";
            // 
            // txtVelocidad
            // 
            this.txtVelocidad.Location = new System.Drawing.Point(72, 34);
            this.txtVelocidad.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.txtVelocidad.Name = "txtVelocidad";
            this.txtVelocidad.Size = new System.Drawing.Size(58, 20);
            this.txtVelocidad.TabIndex = 6;
            this.txtVelocidad.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(136, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Enviar Posicion";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblEnviado
            // 
            this.lblEnviado.AutoSize = true;
            this.lblEnviado.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnviado.ForeColor = System.Drawing.Color.SeaGreen;
            this.lblEnviado.Location = new System.Drawing.Point(156, 61);
            this.lblEnviado.Name = "lblEnviado";
            this.lblEnviado.Size = new System.Drawing.Size(53, 13);
            this.lblEnviado.TabIndex = 4;
            this.lblEnviado.Text = "Enviado";
            this.lblEnviado.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Velocidad";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbVehiculo);
            this.groupBox2.Controls.Add(this.cbEmpresa);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.cbLinea);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(5, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(257, 119);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Vehiculo";
            // 
            // cbVehiculo
            // 
            this.cbVehiculo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbVehiculo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVehiculo.FormattingEnabled = true;
            this.cbVehiculo.Location = new System.Drawing.Point(72, 81);
            this.cbVehiculo.Name = "cbVehiculo";
            this.cbVehiculo.Size = new System.Drawing.Size(164, 21);
            this.cbVehiculo.TabIndex = 5;
            this.cbVehiculo.SelectedIndexChanged += new System.EventHandler(this.cbVehiculo_SelectedIndexChanged);
            // 
            // cbEmpresa
            // 
            this.cbEmpresa.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbEmpresa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEmpresa.FormattingEnabled = true;
            this.cbEmpresa.Location = new System.Drawing.Point(72, 27);
            this.cbEmpresa.Name = "cbEmpresa";
            this.cbEmpresa.Size = new System.Drawing.Size(164, 21);
            this.cbEmpresa.TabIndex = 3;
            this.cbEmpresa.SelectedIndexChanged += new System.EventHandler(this.cbEmpresa_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 84);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Vehiculo";
            // 
            // cbLinea
            // 
            this.cbLinea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLinea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLinea.FormattingEnabled = true;
            this.cbLinea.Location = new System.Drawing.Point(72, 54);
            this.cbLinea.Name = "cbLinea";
            this.cbLinea.Size = new System.Drawing.Size(164, 21);
            this.cbLinea.TabIndex = 4;
            this.cbLinea.SelectedIndexChanged += new System.EventHandler(this.cbLinea_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Linea";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Empresa";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtLatitud);
            this.groupBox1.Controls.Add(this.txtLongitud);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Location = new System.Drawing.Point(5, 128);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 116);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Posicion";
            // 
            // txtLatitud
            // 
            this.txtLatitud.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLatitud.Location = new System.Drawing.Point(72, 19);
            this.txtLatitud.Name = "txtLatitud";
            this.txtLatitud.Size = new System.Drawing.Size(164, 20);
            this.txtLatitud.TabIndex = 0;
            this.txtLatitud.TextChanged += new System.EventHandler(this.txtLatLon_TextChanged);
            // 
            // txtLongitud
            // 
            this.txtLongitud.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLongitud.Location = new System.Drawing.Point(72, 45);
            this.txtLongitud.Name = "txtLongitud";
            this.txtLongitud.Size = new System.Drawing.Size(164, 20);
            this.txtLongitud.TabIndex = 0;
            this.txtLongitud.TextChanged += new System.EventHandler(this.txtLatLon_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Latitud";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Longitud";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(32, 77);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(204, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Goto Current";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // mapControl1
            // 
            this.mapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapControl1.Location = new System.Drawing.Point(0, 0);
            this.mapControl1.Name = "mapControl1";
            this.mapControl1.Size = new System.Drawing.Size(404, 469);
            this.mapControl1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(905, 501);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(897, 475);
            this.tabPage2.TabIndex = 4;
            this.tabPage2.Text = "Scheduler";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.chkDatamartRegenera);
            this.groupBox6.Controls.Add(this.btDatamart);
            this.groupBox6.Controls.Add(this.label15);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Controls.Add(this.dthastaDatamart);
            this.groupBox6.Controls.Add(this.dtDesdeDatamart);
            this.groupBox6.Controls.Add(this.cbVehiculosDatamart);
            this.groupBox6.Location = new System.Drawing.Point(8, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(302, 341);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Datamart";
            // 
            // chkDatamartRegenera
            // 
            this.chkDatamartRegenera.AutoSize = true;
            this.chkDatamartRegenera.Location = new System.Drawing.Point(7, 287);
            this.chkDatamartRegenera.Name = "chkDatamartRegenera";
            this.chkDatamartRegenera.Size = new System.Drawing.Size(73, 17);
            this.chkDatamartRegenera.TabIndex = 6;
            this.chkDatamartRegenera.Text = "Regenera";
            this.chkDatamartRegenera.UseVisualStyleBackColor = true;
            // 
            // btDatamart
            // 
            this.btDatamart.Location = new System.Drawing.Point(221, 312);
            this.btDatamart.Name = "btDatamart";
            this.btDatamart.Size = new System.Drawing.Size(75, 23);
            this.btDatamart.TabIndex = 5;
            this.btDatamart.Text = "Ejecutar";
            this.btDatamart.UseVisualStyleBackColor = true;
            this.btDatamart.Click += new System.EventHandler(this.btDatamart_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(7, 244);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(33, 13);
            this.label15.TabIndex = 4;
            this.label15.Text = "hasta";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 197);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(36, 13);
            this.label13.TabIndex = 3;
            this.label13.Text = "desde";
            // 
            // dthastaDatamart
            // 
            this.dthastaDatamart.Location = new System.Drawing.Point(6, 260);
            this.dthastaDatamart.Name = "dthastaDatamart";
            this.dthastaDatamart.Size = new System.Drawing.Size(200, 20);
            this.dthastaDatamart.TabIndex = 2;
            // 
            // dtDesdeDatamart
            // 
            this.dtDesdeDatamart.Location = new System.Drawing.Point(6, 213);
            this.dtDesdeDatamart.Name = "dtDesdeDatamart";
            this.dtDesdeDatamart.Size = new System.Drawing.Size(200, 20);
            this.dtDesdeDatamart.TabIndex = 1;
            // 
            // cbVehiculosDatamart
            // 
            this.cbVehiculosDatamart.FormattingEnabled = true;
            this.cbVehiculosDatamart.Location = new System.Drawing.Point(6, 19);
            this.cbVehiculosDatamart.Name = "cbVehiculosDatamart";
            this.cbVehiculosDatamart.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.cbVehiculosDatamart.Size = new System.Drawing.Size(290, 160);
            this.cbVehiculosDatamart.TabIndex = 0;
            // 
            // timerSent
            // 
            this.timerSent.Interval = 5000;
            this.timerSent.Tick += new System.EventHandler(this.timerSent_Tick);
            // 
            // Tester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 501);
            this.Controls.Add(this.tabControl1);
            this.Name = "Tester";
            this.Text = "Tester";
            this.Load += new System.EventHandler(this.Tester_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Tester_FormClosing);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVeloGen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFrecuenciaGen)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtVelocidad)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerGen;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.RadioButton radTDGlobalSat;
        private System.Windows.Forms.RadioButton radTDEnfora;
        private System.Windows.Forms.RadioButton radTDFulmar;
        private System.Windows.Forms.RadioButton radTDUnetelv2;
        private System.Windows.Forms.RadioButton radTDGte;
        private System.Windows.Forms.RadioButton radTDUnetelv1;
        private System.Windows.Forms.RadioButton radDisp;
        private System.Windows.Forms.CheckBox chkVeloAutoGen;
        private System.Windows.Forms.Panel panSendGen;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button btGenerador;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown numVeloGen;
        private System.Windows.Forms.NumericUpDown numFrecuenciaGen;
        private System.Windows.Forms.CheckedListBox cbVehiculosGen;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btSendMessage;
        private System.Windows.Forms.TextBox txtTextoMensaje;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbMensaje;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ListBox cbFuera;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListBox cbDentro;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown txtVelocidad;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbVehiculo;
        private System.Windows.Forms.ComboBox cbEmpresa;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbLinea;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtLatitud;
        private System.Windows.Forms.TextBox txtLongitud;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private Compumap.Controls.Map.MapControl mapControl1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label lblEnviado;
        private System.Windows.Forms.Timer timerSent;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtQueueName;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btDatamart;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DateTimePicker dthastaDatamart;
        private System.Windows.Forms.DateTimePicker dtDesdeDatamart;
        private System.Windows.Forms.ListBox cbVehiculosDatamart;
        private System.Windows.Forms.CheckBox chkDatamartRegenera;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button btEntregaCancelada;
        private System.Windows.Forms.Button btEntregaCompletada;
        private System.Windows.Forms.ListBox cbEntregas;
        private System.Windows.Forms.Label lblEntregas;
        private System.Windows.Forms.Button btCargarDistribucion;
    }
}