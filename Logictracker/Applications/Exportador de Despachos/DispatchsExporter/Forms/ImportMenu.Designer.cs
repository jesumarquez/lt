namespace DispatchsExporter.Forms
{
    partial class ImportMenu
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
            this.lblMessage = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnSalir = new System.Windows.Forms.Button();
            this.btnVerExportados = new System.Windows.Forms.Button();
            this.btnExportarDespachos = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(55, 228);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(242, 13);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Por favor, aguarde mientras se importan los datos.";
            this.lblMessage.Visible = false;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(39, 259);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(391, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 2;
            this.progressBar.Visible = false;
            // 
            // btnSalir
            // 
            this.btnSalir.Enabled = false;
            this.btnSalir.Location = new System.Drawing.Point(268, 296);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(162, 23);
            this.btnSalir.TabIndex = 3;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Visible = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnVerExportados
            // 
            this.btnVerExportados.Location = new System.Drawing.Point(41, 110);
            this.btnVerExportados.Name = "btnVerExportados";
            this.btnVerExportados.Size = new System.Drawing.Size(391, 42);
            this.btnVerExportados.TabIndex = 4;
            this.btnVerExportados.Text = "Ver Despachos Exportados";
            this.btnVerExportados.UseVisualStyleBackColor = true;
            this.btnVerExportados.Click += new System.EventHandler(this.btnExportarVehiculos_Click);
            // 
            // btnExportarDespachos
            // 
            this.btnExportarDespachos.Location = new System.Drawing.Point(41, 62);
            this.btnExportarDespachos.Name = "btnExportarDespachos";
            this.btnExportarDespachos.Size = new System.Drawing.Size(391, 42);
            this.btnExportarDespachos.TabIndex = 5;
            this.btnExportarDespachos.Text = "Iniciar Cierre Diario";
            this.btnExportarDespachos.UseVisualStyleBackColor = true;
            this.btnExportarDespachos.Click += new System.EventHandler(this.btnExportar_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DispatchsExporter.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(317, -1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(162, 57);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // ImportMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 324);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnExportarDespachos);
            this.Controls.Add(this.btnVerExportados);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "ImportMenu";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Importador de Datos";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Import_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Button btnVerExportados;
        private System.Windows.Forms.Button btnExportarDespachos;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}