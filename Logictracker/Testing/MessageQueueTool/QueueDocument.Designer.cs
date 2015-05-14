namespace MessageQueueTool
{
    partial class QueueDocument
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
            this.pageAsyncProc = new System.Windows.Forms.TabControl();
            this.pageGeneral = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.nudInject = new System.Windows.Forms.NumericUpDown();
            this.btnPurgeQueue = new System.Windows.Forms.Button();
            this.btnDeleteQueue = new System.Windows.Forms.Button();
            this.btnCreateQueue = new System.Windows.Forms.Button();
            this.lblQueueState = new System.Windows.Forms.Label();
            this.txtQueueName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pageSyncProc = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.uiqp1Port = new System.Windows.Forms.TextBox();
            this.uiqp1Start = new System.Windows.Forms.Button();
            this.uiqp1DstQueue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.uiqp1Address = new System.Windows.Forms.TextBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.uiqp2Port = new System.Windows.Forms.TextBox();
            this.uiqp2Start = new System.Windows.Forms.Button();
            this.uiqp2DstQueue = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.uiqp2Address = new System.Windows.Forms.TextBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.trans = new System.Windows.Forms.CheckBox();
            this.pageAsyncProc.SuspendLayout();
            this.pageGeneral.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInject)).BeginInit();
            this.pageSyncProc.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageAsyncProc
            // 
            this.pageAsyncProc.Controls.Add(this.pageGeneral);
            this.pageAsyncProc.Controls.Add(this.pageSyncProc);
            this.pageAsyncProc.Controls.Add(this.tabPage1);
            this.pageAsyncProc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageAsyncProc.Location = new System.Drawing.Point(0, 0);
            this.pageAsyncProc.Name = "pageAsyncProc";
            this.pageAsyncProc.SelectedIndex = 0;
            this.pageAsyncProc.Size = new System.Drawing.Size(608, 273);
            this.pageAsyncProc.TabIndex = 0;
            // 
            // pageGeneral
            // 
            this.pageGeneral.Controls.Add(this.trans);
            this.pageGeneral.Controls.Add(this.groupBox2);
            this.pageGeneral.Controls.Add(this.btnPurgeQueue);
            this.pageGeneral.Controls.Add(this.btnDeleteQueue);
            this.pageGeneral.Controls.Add(this.btnCreateQueue);
            this.pageGeneral.Controls.Add(this.lblQueueState);
            this.pageGeneral.Controls.Add(this.txtQueueName);
            this.pageGeneral.Controls.Add(this.label1);
            this.pageGeneral.Location = new System.Drawing.Point(4, 22);
            this.pageGeneral.Name = "pageGeneral";
            this.pageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.pageGeneral.Size = new System.Drawing.Size(600, 247);
            this.pageGeneral.TabIndex = 0;
            this.pageGeneral.Text = "Colas";
            this.pageGeneral.UseVisualStyleBackColor = true;
            this.pageGeneral.Click += new System.EventHandler(this.pageGeneral_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.nudInject);
            this.groupBox2.Location = new System.Drawing.Point(22, 84);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(238, 76);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Injeccion de datos";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Mensajes";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(157, 47);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Injectar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // nudInject
            // 
            this.nudInject.Increment = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudInject.Location = new System.Drawing.Point(81, 26);
            this.nudInject.Maximum = new decimal(new int[] {
            8192,
            0,
            0,
            0});
            this.nudInject.Name = "nudInject";
            this.nudInject.Size = new System.Drawing.Size(63, 20);
            this.nudInject.TabIndex = 1;
            this.nudInject.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // btnPurgeQueue
            // 
            this.btnPurgeQueue.Location = new System.Drawing.Point(209, 45);
            this.btnPurgeQueue.Name = "btnPurgeQueue";
            this.btnPurgeQueue.Size = new System.Drawing.Size(75, 23);
            this.btnPurgeQueue.TabIndex = 5;
            this.btnPurgeQueue.Text = "Purgar";
            this.btnPurgeQueue.UseVisualStyleBackColor = true;
            this.btnPurgeQueue.Click += new System.EventHandler(this.btnPurgeQueue_Click);
            // 
            // btnDeleteQueue
            // 
            this.btnDeleteQueue.Location = new System.Drawing.Point(115, 45);
            this.btnDeleteQueue.Name = "btnDeleteQueue";
            this.btnDeleteQueue.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteQueue.TabIndex = 4;
            this.btnDeleteQueue.Text = "Eliminar";
            this.btnDeleteQueue.UseVisualStyleBackColor = true;
            this.btnDeleteQueue.Click += new System.EventHandler(this.btnDeleteQueue_Click);
            // 
            // btnCreateQueue
            // 
            this.btnCreateQueue.Location = new System.Drawing.Point(22, 45);
            this.btnCreateQueue.Name = "btnCreateQueue";
            this.btnCreateQueue.Size = new System.Drawing.Size(75, 23);
            this.btnCreateQueue.TabIndex = 3;
            this.btnCreateQueue.Text = "Crear";
            this.btnCreateQueue.UseVisualStyleBackColor = true;
            this.btnCreateQueue.Click += new System.EventHandler(this.btnCreateQueue_Click);
            // 
            // lblQueueState
            // 
            this.lblQueueState.Font = new System.Drawing.Font("Courier New", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.lblQueueState.Location = new System.Drawing.Point(258, 12);
            this.lblQueueState.Name = "lblQueueState";
            this.lblQueueState.Size = new System.Drawing.Size(325, 17);
            this.lblQueueState.TabIndex = 2;
            this.lblQueueState.Text = "ESTADO: DESCONOCIDO";
            // 
            // txtQueueName
            // 
            this.txtQueueName.Location = new System.Drawing.Point(72, 10);
            this.txtQueueName.Name = "txtQueueName";
            this.txtQueueName.Size = new System.Drawing.Size(163, 20);
            this.txtQueueName.TabIndex = 1;
            this.txtQueueName.Tag = ".\\private$\\posiciones";
            this.txtQueueName.TextChanged += new System.EventHandler(this.txtQueueName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nombre:";
            // 
            // pageSyncProc
            // 
            this.pageSyncProc.Controls.Add(this.groupBox3);
            this.pageSyncProc.Controls.Add(this.radioButton2);
            this.pageSyncProc.Controls.Add(this.groupBox1);
            this.pageSyncProc.Controls.Add(this.radioButton1);
            this.pageSyncProc.Location = new System.Drawing.Point(4, 22);
            this.pageSyncProc.Name = "pageSyncProc";
            this.pageSyncProc.Padding = new System.Windows.Forms.Padding(3);
            this.pageSyncProc.Size = new System.Drawing.Size(600, 247);
            this.pageSyncProc.TabIndex = 1;
            this.pageSyncProc.Text = "Proceso Secuencial";
            this.pageSyncProc.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(124, 114);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(468, 48);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Clonacion Local de Cola";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(387, 17);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Iniciar";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(79, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(178, 20);
            this.textBox1.TabIndex = 7;
            this.textBox1.Text = "destino";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Cola Destino";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(20, 114);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(72, 17);
            this.radioButton2.TabIndex = 2;
            this.radioButton2.Text = "Clonacion";
            this.radioButton2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.uiqp1Port);
            this.groupBox1.Controls.Add(this.uiqp1Start);
            this.groupBox1.Controls.Add(this.uiqp1DstQueue);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.uiqp1Address);
            this.groupBox1.Location = new System.Drawing.Point(124, 18);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(468, 75);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Despacho TCP/IP (UIQP/1.1)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(229, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(10, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = ":";
            // 
            // uiqp1Port
            // 
            this.uiqp1Port.Location = new System.Drawing.Point(242, 20);
            this.uiqp1Port.Name = "uiqp1Port";
            this.uiqp1Port.Size = new System.Drawing.Size(49, 20);
            this.uiqp1Port.TabIndex = 3;
            this.uiqp1Port.Text = "7543";
            // 
            // uiqp1Start
            // 
            this.uiqp1Start.Location = new System.Drawing.Point(387, 44);
            this.uiqp1Start.Name = "uiqp1Start";
            this.uiqp1Start.Size = new System.Drawing.Size(75, 23);
            this.uiqp1Start.TabIndex = 6;
            this.uiqp1Start.Text = "Iniciar";
            this.uiqp1Start.UseVisualStyleBackColor = true;
            this.uiqp1Start.Click += new System.EventHandler(this.uiqp1Start_Click);
            // 
            // uiqp1DstQueue
            // 
            this.uiqp1DstQueue.Location = new System.Drawing.Point(79, 46);
            this.uiqp1DstQueue.Name = "uiqp1DstQueue";
            this.uiqp1DstQueue.Size = new System.Drawing.Size(178, 20);
            this.uiqp1DstQueue.TabIndex = 5;
            this.uiqp1DstQueue.Text = "destino";
            this.uiqp1DstQueue.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Cola Destino";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Destino";
            // 
            // uiqp1Address
            // 
            this.uiqp1Address.Location = new System.Drawing.Point(79, 20);
            this.uiqp1Address.Name = "uiqp1Address";
            this.uiqp1Address.Size = new System.Drawing.Size(144, 20);
            this.uiqp1Address.TabIndex = 1;
            this.uiqp1Address.Text = "localhost";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(20, 18);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(98, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Despacho TCP";
            this.radioButton1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.radioButton3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(600, 247);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Proceso Asincronico";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.uiqp2Port);
            this.groupBox4.Controls.Add(this.uiqp2Start);
            this.groupBox4.Controls.Add(this.uiqp2DstQueue);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.uiqp2Address);
            this.groupBox4.Location = new System.Drawing.Point(124, 18);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(468, 75);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Despacho TCP/IP (UIQP/1.2)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(229, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(10, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = ":";
            // 
            // uiqp2Port
            // 
            this.uiqp2Port.Location = new System.Drawing.Point(242, 20);
            this.uiqp2Port.Name = "uiqp2Port";
            this.uiqp2Port.Size = new System.Drawing.Size(49, 20);
            this.uiqp2Port.TabIndex = 3;
            this.uiqp2Port.Text = "7543";
            // 
            // uiqp2Start
            // 
            this.uiqp2Start.Location = new System.Drawing.Point(387, 44);
            this.uiqp2Start.Name = "uiqp2Start";
            this.uiqp2Start.Size = new System.Drawing.Size(75, 23);
            this.uiqp2Start.TabIndex = 6;
            this.uiqp2Start.Text = "Iniciar";
            this.uiqp2Start.UseVisualStyleBackColor = true;
            this.uiqp2Start.Click += new System.EventHandler(this.uiqp2Start_Click);
            // 
            // uiqp2DstQueue
            // 
            this.uiqp2DstQueue.Location = new System.Drawing.Point(79, 46);
            this.uiqp2DstQueue.Name = "uiqp2DstQueue";
            this.uiqp2DstQueue.Size = new System.Drawing.Size(178, 20);
            this.uiqp2DstQueue.TabIndex = 5;
            this.uiqp2DstQueue.Text = "destino";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 49);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Cola Destino";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Destino";
            // 
            // uiqp2Address
            // 
            this.uiqp2Address.Location = new System.Drawing.Point(79, 20);
            this.uiqp2Address.Name = "uiqp2Address";
            this.uiqp2Address.Size = new System.Drawing.Size(144, 20);
            this.uiqp2Address.TabIndex = 1;
            this.uiqp2Address.Text = "localhost";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Checked = true;
            this.radioButton3.Location = new System.Drawing.Point(20, 18);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(98, 17);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Despacho TCP";
            this.radioButton3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 333;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 2000;
            this.timer2.Tag = "W";
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // trans
            // 
            this.trans.AutoSize = true;
            this.trans.Location = new System.Drawing.Point(302, 49);
            this.trans.Name = "trans";
            this.trans.Size = new System.Drawing.Size(93, 17);
            this.trans.TabIndex = 7;
            this.trans.Text = "Transaccional";
            this.trans.UseVisualStyleBackColor = true;
            // 
            // QueueDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 273);
            this.Controls.Add(this.pageAsyncProc);
            this.Name = "QueueDocument";
            this.Text = "QueueDocument";
            this.Load += new System.EventHandler(this.QueueDocument_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.QueueDocument_FormClosing);
            this.pageAsyncProc.ResumeLayout(false);
            this.pageGeneral.ResumeLayout(false);
            this.pageGeneral.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudInject)).EndInit();
            this.pageSyncProc.ResumeLayout(false);
            this.pageSyncProc.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl pageAsyncProc;
        private System.Windows.Forms.TabPage pageGeneral;
        private System.Windows.Forms.TabPage pageSyncProc;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtQueueName;
        private System.Windows.Forms.Label lblQueueState;
        private System.Windows.Forms.Button btnCreateQueue;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnPurgeQueue;
        private System.Windows.Forms.Button btnDeleteQueue;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button uiqp1Start;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox uiqp1Port;
        private System.Windows.Forms.TextBox uiqp1DstQueue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox uiqp1Address;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown nudInject;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox uiqp2Port;
        private System.Windows.Forms.Button uiqp2Start;
        private System.Windows.Forms.TextBox uiqp2DstQueue;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox uiqp2Address;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.CheckBox trans;
    }
}