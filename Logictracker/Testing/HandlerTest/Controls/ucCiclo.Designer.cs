namespace HandlerTest.Controls
{
    partial class ucCiclo
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
            this.btEntregaCancelada = new System.Windows.Forms.Button();
            this.btEntregaCompletada = new System.Windows.Forms.Button();
            this.cbEntregas = new System.Windows.Forms.ListBox();
            this.lblEntregas = new System.Windows.Forms.Label();
            this.chkMostrarEnMapa = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btEntregaCancelada
            // 
            this.btEntregaCancelada.Location = new System.Drawing.Point(396, 90);
            this.btEntregaCancelada.Name = "btEntregaCancelada";
            this.btEntregaCancelada.Size = new System.Drawing.Size(151, 23);
            this.btEntregaCancelada.TabIndex = 11;
            this.btEntregaCancelada.Text = "Cancelado";
            this.btEntregaCancelada.UseVisualStyleBackColor = true;
            this.btEntregaCancelada.Click += new System.EventHandler(this.btEntregaCancelada_Click);
            // 
            // btEntregaCompletada
            // 
            this.btEntregaCompletada.Location = new System.Drawing.Point(396, 36);
            this.btEntregaCompletada.Name = "btEntregaCompletada";
            this.btEntregaCompletada.Size = new System.Drawing.Size(151, 48);
            this.btEntregaCompletada.TabIndex = 12;
            this.btEntregaCompletada.Text = "Completado";
            this.btEntregaCompletada.UseVisualStyleBackColor = true;
            this.btEntregaCompletada.Click += new System.EventHandler(this.btEntregaCompletada_Click);
            // 
            // cbEntregas
            // 
            this.cbEntregas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.cbEntregas.DisplayMember = "Descripcion";
            this.cbEntregas.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbEntregas.FormattingEnabled = true;
            this.cbEntregas.ItemHeight = 20;
            this.cbEntregas.Location = new System.Drawing.Point(34, 36);
            this.cbEntregas.Name = "cbEntregas";
            this.cbEntregas.Size = new System.Drawing.Size(356, 424);
            this.cbEntregas.TabIndex = 10;
            // 
            // lblEntregas
            // 
            this.lblEntregas.AutoSize = true;
            this.lblEntregas.Location = new System.Drawing.Point(31, 20);
            this.lblEntregas.Name = "lblEntregas";
            this.lblEntregas.Size = new System.Drawing.Size(49, 13);
            this.lblEntregas.TabIndex = 9;
            this.lblEntregas.Text = "Entregas";
            // 
            // chkMostrarEnMapa
            // 
            this.chkMostrarEnMapa.AutoSize = true;
            this.chkMostrarEnMapa.Checked = true;
            this.chkMostrarEnMapa.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMostrarEnMapa.Location = new System.Drawing.Point(396, 135);
            this.chkMostrarEnMapa.Name = "chkMostrarEnMapa";
            this.chkMostrarEnMapa.Size = new System.Drawing.Size(105, 17);
            this.chkMostrarEnMapa.TabIndex = 13;
            this.chkMostrarEnMapa.Text = "Mostrar en mapa";
            this.chkMostrarEnMapa.UseVisualStyleBackColor = true;
            this.chkMostrarEnMapa.CheckedChanged += new System.EventHandler(this.chkMostrarEnMapa_CheckedChanged);
            // 
            // ucCiclo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkMostrarEnMapa);
            this.Controls.Add(this.btEntregaCancelada);
            this.Controls.Add(this.btEntregaCompletada);
            this.Controls.Add(this.cbEntregas);
            this.Controls.Add(this.lblEntregas);
            this.Name = "ucCiclo";
            this.Size = new System.Drawing.Size(867, 494);
            this.Load += new System.EventHandler(this.ucCiclo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btEntregaCancelada;
        private System.Windows.Forms.Button btEntregaCompletada;
        private System.Windows.Forms.ListBox cbEntregas;
        private System.Windows.Forms.Label lblEntregas;
        private System.Windows.Forms.CheckBox chkMostrarEnMapa;
    }
}
