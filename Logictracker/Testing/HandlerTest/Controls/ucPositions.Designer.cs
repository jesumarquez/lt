namespace HandlerTest.Controls
{
    partial class ucPositions
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lnkUltima = new System.Windows.Forms.LinkLabel();
            this.chkRandomSpeed = new System.Windows.Forms.CheckBox();
            this.txtVelocidad = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtLatitud = new System.Windows.Forms.TextBox();
            this.txtLongitud = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.mapControl1 = new Compumap.Controls.Map.MapControl();
            this.chkMostrarBase = new System.Windows.Forms.CheckBox();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtVelocidad)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.chkMostrarBase);
            this.splitContainer2.Panel1.Controls.Add(this.lnkUltima);
            this.splitContainer2.Panel1.Controls.Add(this.chkRandomSpeed);
            this.splitContainer2.Panel1.Controls.Add(this.txtVelocidad);
            this.splitContainer2.Panel1.Controls.Add(this.button1);
            this.splitContainer2.Panel1.Controls.Add(this.label6);
            this.splitContainer2.Panel1.Controls.Add(this.txtLatitud);
            this.splitContainer2.Panel1.Controls.Add(this.txtLongitud);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.label4);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.mapControl1);
            this.splitContainer2.Size = new System.Drawing.Size(903, 504);
            this.splitContainer2.SplitterDistance = 269;
            this.splitContainer2.TabIndex = 1;
            // 
            // lnkUltima
            // 
            this.lnkUltima.AutoSize = true;
            this.lnkUltima.Location = new System.Drawing.Point(10, 132);
            this.lnkUltima.Name = "lnkUltima";
            this.lnkUltima.Size = new System.Drawing.Size(126, 13);
            this.lnkUltima.TabIndex = 12;
            this.lnkUltima.TabStop = true;
            this.lnkUltima.Text = "Ultima posición reportada";
            this.lnkUltima.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkUltima_LinkClicked);
            // 
            // chkRandomSpeed
            // 
            this.chkRandomSpeed.AutoSize = true;
            this.chkRandomSpeed.Location = new System.Drawing.Point(162, 68);
            this.chkRandomSpeed.Name = "chkRandomSpeed";
            this.chkRandomSpeed.Size = new System.Drawing.Size(66, 17);
            this.chkRandomSpeed.TabIndex = 11;
            this.chkRandomSpeed.Text = "Random";
            this.chkRandomSpeed.UseVisualStyleBackColor = true;
            this.chkRandomSpeed.CheckedChanged += new System.EventHandler(this.chkRandomSpeed_CheckedChanged);
            // 
            // txtVelocidad
            // 
            this.txtVelocidad.Location = new System.Drawing.Point(76, 67);
            this.txtVelocidad.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.txtVelocidad.Name = "txtVelocidad";
            this.txtVelocidad.Size = new System.Drawing.Size(58, 20);
            this.txtVelocidad.TabIndex = 10;
            this.txtVelocidad.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button1.Location = new System.Drawing.Point(72, 416);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(127, 49);
            this.button1.TabIndex = 9;
            this.button1.Text = "Enviar Posicion";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Velocidad";
            // 
            // txtLatitud
            // 
            this.txtLatitud.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLatitud.Location = new System.Drawing.Point(76, 15);
            this.txtLatitud.Name = "txtLatitud";
            this.txtLatitud.Size = new System.Drawing.Size(168, 20);
            this.txtLatitud.TabIndex = 4;
            this.txtLatitud.TextChanged += new System.EventHandler(this.txtLatitud_TextChanged);
            // 
            // txtLongitud
            // 
            this.txtLongitud.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLongitud.Location = new System.Drawing.Point(76, 41);
            this.txtLongitud.Name = "txtLongitud";
            this.txtLongitud.Size = new System.Drawing.Size(168, 20);
            this.txtLongitud.TabIndex = 3;
            this.txtLongitud.TextChanged += new System.EventHandler(this.txtLatitud_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Latitud";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Longitud";
            // 
            // mapControl1
            // 
            this.mapControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapControl1.Location = new System.Drawing.Point(0, 0);
            this.mapControl1.Name = "mapControl1";
            this.mapControl1.Size = new System.Drawing.Size(630, 504);
            this.mapControl1.TabIndex = 1;
            // 
            // chkMostrarBase
            // 
            this.chkMostrarBase.AutoSize = true;
            this.chkMostrarBase.Checked = true;
            this.chkMostrarBase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMostrarBase.Location = new System.Drawing.Point(13, 177);
            this.chkMostrarBase.Name = "chkMostrarBase";
            this.chkMostrarBase.Size = new System.Drawing.Size(88, 17);
            this.chkMostrarBase.TabIndex = 13;
            this.chkMostrarBase.Text = "Mostrar Base";
            this.chkMostrarBase.UseVisualStyleBackColor = true;
            this.chkMostrarBase.CheckedChanged += new System.EventHandler(this.chkMostrarBase_CheckedChanged);
            // 
            // ucPositions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer2);
            this.Name = "ucPositions";
            this.Size = new System.Drawing.Size(903, 504);
            this.Load += new System.EventHandler(this.ucPositions_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtVelocidad)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.LinkLabel lnkUltima;
        private System.Windows.Forms.CheckBox chkRandomSpeed;
        private System.Windows.Forms.NumericUpDown txtVelocidad;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtLatitud;
        private System.Windows.Forms.TextBox txtLongitud;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private Compumap.Controls.Map.MapControl mapControl1;
        private System.Windows.Forms.CheckBox chkMostrarBase;
    }
}
