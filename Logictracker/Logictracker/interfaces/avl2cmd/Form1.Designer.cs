namespace avl2cmd
{
    partial class Form1
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
            this.bACK = new System.Windows.Forms.Button();
            this.bNACK = new System.Windows.Forms.Button();
            this.recvText = new System.Windows.Forms.TextBox();
            this.camionID = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.latencia = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.estado = new System.Windows.Forms.ComboBox();
            this.bEnvia = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.camionID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.latencia)).BeginInit();
            this.SuspendLayout();
            // 
            // bACK
            // 
            this.bACK.Location = new System.Drawing.Point(12, 223);
            this.bACK.Name = "bACK";
            this.bACK.Size = new System.Drawing.Size(75, 23);
            this.bACK.TabIndex = 9;
            this.bACK.Text = "ACK";
            this.bACK.UseVisualStyleBackColor = true;
            this.bACK.Click += new System.EventHandler(this.bACK_Click);
            // 
            // bNACK
            // 
            this.bNACK.Location = new System.Drawing.Point(93, 223);
            this.bNACK.Name = "bNACK";
            this.bNACK.Size = new System.Drawing.Size(75, 23);
            this.bNACK.TabIndex = 10;
            this.bNACK.Text = "NACK";
            this.bNACK.UseVisualStyleBackColor = true;
            this.bNACK.Click += new System.EventHandler(this.bNACK_Click);
            // 
            // recvText
            // 
            this.recvText.Location = new System.Drawing.Point(12, 90);
            this.recvText.Multiline = true;
            this.recvText.Name = "recvText";
            this.recvText.Size = new System.Drawing.Size(272, 127);
            this.recvText.TabIndex = 8;
            this.recvText.DoubleClick += new System.EventHandler(this.recvText_DoubleClick);
            // 
            // camionID
            // 
            this.camionID.Location = new System.Drawing.Point(144, 11);
            this.camionID.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.camionID.Name = "camionID";
            this.camionID.Size = new System.Drawing.Size(140, 20);
            this.camionID.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "CODIGO CAMION CMD:";
            // 
            // latencia
            // 
            this.latencia.Location = new System.Drawing.Point(144, 64);
            this.latencia.Name = "latencia";
            this.latencia.Size = new System.Drawing.Size(139, 20);
            this.latencia.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "NUEVO ESTADO:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "LATENCIA (minutos):";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // estado
            // 
            this.estado.DisplayMember = "Value";
            this.estado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.estado.FormattingEnabled = true;
            this.estado.Location = new System.Drawing.Point(144, 37);
            this.estado.MaxDropDownItems = 9;
            this.estado.Name = "estado";
            this.estado.Size = new System.Drawing.Size(139, 21);
            this.estado.TabIndex = 5;
            this.estado.ValueMember = "Key";
            this.estado.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // bEnvia
            // 
            this.bEnvia.Location = new System.Drawing.Point(174, 223);
            this.bEnvia.Name = "bEnvia";
            this.bEnvia.Size = new System.Drawing.Size(109, 23);
            this.bEnvia.TabIndex = 11;
            this.bEnvia.Text = "ENVIA";
            this.bEnvia.UseVisualStyleBackColor = true;
            this.bEnvia.Click += new System.EventHandler(this.bEnvia_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 258);
            this.Controls.Add(this.bEnvia);
            this.Controls.Add(this.estado);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.latencia);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.camionID);
            this.Controls.Add(this.recvText);
            this.Controls.Add(this.bNACK);
            this.Controls.Add(this.bACK);
            this.Name = "Form1";
            this.Text = "AVL2CMD - SIMPLE GAME";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.camionID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.latencia)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bACK;
        private System.Windows.Forms.Button bNACK;
        private System.Windows.Forms.TextBox recvText;
        private System.Windows.Forms.NumericUpDown camionID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown latencia;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox estado;
        private System.Windows.Forms.Button bEnvia;
        private System.Windows.Forms.Timer timer1;
    }
}

