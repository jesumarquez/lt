namespace HandlerTest.Controls
{
    partial class ucMensajeria
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
            this.btExceso = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtExcesoPermitida = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btCancelarExceso = new System.Windows.Forms.Button();
            this.btTerminarExceso = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbMensajes = new System.Windows.Forms.ComboBox();
            this.btEnviarMensaje = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtExcesoPermitida)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btExceso
            // 
            this.btExceso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btExceso.Location = new System.Drawing.Point(279, 11);
            this.btExceso.Name = "btExceso";
            this.btExceso.Size = new System.Drawing.Size(75, 23);
            this.btExceso.TabIndex = 0;
            this.btExceso.Text = "Iniciar";
            this.btExceso.UseVisualStyleBackColor = true;
            this.btExceso.Click += new System.EventHandler(this.btExceso_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtExcesoPermitida);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btCancelarExceso);
            this.groupBox1.Controls.Add(this.btTerminarExceso);
            this.groupBox1.Controls.Add(this.btExceso);
            this.groupBox1.Location = new System.Drawing.Point(13, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(522, 39);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label2.Location = new System.Drawing.Point(167, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Permitida";
            // 
            // txtExcesoPermitida
            // 
            this.txtExcesoPermitida.Location = new System.Drawing.Point(223, 13);
            this.txtExcesoPermitida.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.txtExcesoPermitida.Name = "txtExcesoPermitida";
            this.txtExcesoPermitida.Size = new System.Drawing.Size(50, 20);
            this.txtExcesoPermitida.TabIndex = 1;
            this.txtExcesoPermitida.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Exceso de Velocidad";
            // 
            // btCancelarExceso
            // 
            this.btCancelarExceso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancelarExceso.Enabled = false;
            this.btCancelarExceso.Location = new System.Drawing.Point(441, 10);
            this.btCancelarExceso.Name = "btCancelarExceso";
            this.btCancelarExceso.Size = new System.Drawing.Size(75, 23);
            this.btCancelarExceso.TabIndex = 0;
            this.btCancelarExceso.Text = "Cancelar";
            this.btCancelarExceso.UseVisualStyleBackColor = true;
            this.btCancelarExceso.Click += new System.EventHandler(this.btCancelarExceso_Click);
            // 
            // btTerminarExceso
            // 
            this.btTerminarExceso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btTerminarExceso.Enabled = false;
            this.btTerminarExceso.Location = new System.Drawing.Point(360, 11);
            this.btTerminarExceso.Name = "btTerminarExceso";
            this.btTerminarExceso.Size = new System.Drawing.Size(75, 23);
            this.btTerminarExceso.TabIndex = 0;
            this.btTerminarExceso.Text = "Terminar";
            this.btTerminarExceso.UseVisualStyleBackColor = true;
            this.btTerminarExceso.Click += new System.EventHandler(this.btTerminarExceso_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btEnviarMensaje);
            this.groupBox2.Controls.Add(this.cbMensajes);
            this.groupBox2.Location = new System.Drawing.Point(13, 52);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(522, 39);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // cbMensajes
            // 
            this.cbMensajes.DisplayMember = "Descripcion";
            this.cbMensajes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMensajes.FormattingEnabled = true;
            this.cbMensajes.Location = new System.Drawing.Point(9, 12);
            this.cbMensajes.Name = "cbMensajes";
            this.cbMensajes.Size = new System.Drawing.Size(345, 21);
            this.cbMensajes.TabIndex = 0;
            // 
            // btEnviarMensaje
            // 
            this.btEnviarMensaje.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btEnviarMensaje.Location = new System.Drawing.Point(441, 12);
            this.btEnviarMensaje.Name = "btEnviarMensaje";
            this.btEnviarMensaje.Size = new System.Drawing.Size(75, 23);
            this.btEnviarMensaje.TabIndex = 1;
            this.btEnviarMensaje.Text = "Enviar";
            this.btEnviarMensaje.UseVisualStyleBackColor = true;
            this.btEnviarMensaje.Click += new System.EventHandler(this.btEnviarMensaje_Click);
            // 
            // ucMensajeria
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ucMensajeria";
            this.Size = new System.Drawing.Size(856, 516);
            this.Load += new System.EventHandler(this.ucMensajeria_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtExcesoPermitida)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btExceso;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btCancelarExceso;
        private System.Windows.Forms.Button btTerminarExceso;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown txtExcesoPermitida;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btEnviarMensaje;
        private System.Windows.Forms.ComboBox cbMensajes;
    }
}
