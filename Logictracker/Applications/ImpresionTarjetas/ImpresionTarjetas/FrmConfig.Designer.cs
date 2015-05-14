namespace Tarjetas
{
    partial class FrmConfig
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
            this.cbEmpresa = new System.Windows.Forms.ListBox();
            this.btNuevo = new System.Windows.Forms.Button();
            this.btModificar = new System.Windows.Forms.Button();
            this.btEliminar = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabEstilos = new System.Windows.Forms.TabPage();
            this.back1 = new TarjetasSAI.Back();
            this.tabEmpresas = new System.Windows.Forms.TabPage();
            this.tabUpcode = new System.Windows.Forms.TabPage();
            this.btUploadUpcode = new System.Windows.Forms.LinkLabel();
            this.lblCoutUpcode = new System.Windows.Forms.Label();
            this.cbUpcode = new System.Windows.Forms.ListBox();
            this.tabDb = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.btDownloadDb = new System.Windows.Forms.LinkLabel();
            this.saveDb = new System.Windows.Forms.SaveFileDialog();
            this.openFileDb = new System.Windows.Forms.OpenFileDialog();
            this.openFileUpcode = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.tabEstilos.SuspendLayout();
            this.tabEmpresas.SuspendLayout();
            this.tabUpcode.SuspendLayout();
            this.tabDb.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbEmpresa
            // 
            this.cbEmpresa.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbEmpresa.DisplayMember = "nombre";
            this.cbEmpresa.FormattingEnabled = true;
            this.cbEmpresa.Location = new System.Drawing.Point(8, 16);
            this.cbEmpresa.Name = "cbEmpresa";
            this.cbEmpresa.Size = new System.Drawing.Size(548, 407);
            this.cbEmpresa.TabIndex = 0;
            // 
            // btNuevo
            // 
            this.btNuevo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btNuevo.Location = new System.Drawing.Point(562, 16);
            this.btNuevo.Name = "btNuevo";
            this.btNuevo.Size = new System.Drawing.Size(75, 23);
            this.btNuevo.TabIndex = 1;
            this.btNuevo.Text = "Nuevo";
            this.btNuevo.UseVisualStyleBackColor = true;
            this.btNuevo.Click += new System.EventHandler(this.btNuevo_Click);
            // 
            // btModificar
            // 
            this.btModificar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btModificar.Location = new System.Drawing.Point(562, 45);
            this.btModificar.Name = "btModificar";
            this.btModificar.Size = new System.Drawing.Size(75, 23);
            this.btModificar.TabIndex = 1;
            this.btModificar.Text = "Modificar";
            this.btModificar.UseVisualStyleBackColor = true;
            this.btModificar.Click += new System.EventHandler(this.btModificar_Click);
            // 
            // btEliminar
            // 
            this.btEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btEliminar.Location = new System.Drawing.Point(562, 74);
            this.btEliminar.Name = "btEliminar";
            this.btEliminar.Size = new System.Drawing.Size(75, 23);
            this.btEliminar.TabIndex = 1;
            this.btEliminar.Text = "Eliminar";
            this.btEliminar.UseVisualStyleBackColor = true;
            this.btEliminar.Click += new System.EventHandler(this.btEliminar_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabEstilos);
            this.tabControl1.Controls.Add(this.tabEmpresas);
            this.tabControl1.Controls.Add(this.tabUpcode);
            this.tabControl1.Controls.Add(this.tabDb);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(766, 470);
            this.tabControl1.TabIndex = 1;
            // 
            // tabEstilos
            // 
            this.tabEstilos.Controls.Add(this.back1);
            this.tabEstilos.Location = new System.Drawing.Point(4, 22);
            this.tabEstilos.Name = "tabEstilos";
            this.tabEstilos.Size = new System.Drawing.Size(758, 444);
            this.tabEstilos.TabIndex = 3;
            this.tabEstilos.Text = "Estilos";
            this.tabEstilos.UseVisualStyleBackColor = true;
            // 
            // back1
            // 
            this.back1.Data = null;
            this.back1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.back1.Location = new System.Drawing.Point(0, 0);
            this.back1.Name = "back1";
            this.back1.Size = new System.Drawing.Size(758, 444);
            this.back1.TabIndex = 0;
            // 
            // tabEmpresas
            // 
            this.tabEmpresas.Controls.Add(this.btEliminar);
            this.tabEmpresas.Controls.Add(this.cbEmpresa);
            this.tabEmpresas.Controls.Add(this.btModificar);
            this.tabEmpresas.Controls.Add(this.btNuevo);
            this.tabEmpresas.Location = new System.Drawing.Point(4, 22);
            this.tabEmpresas.Name = "tabEmpresas";
            this.tabEmpresas.Padding = new System.Windows.Forms.Padding(3);
            this.tabEmpresas.Size = new System.Drawing.Size(758, 444);
            this.tabEmpresas.TabIndex = 0;
            this.tabEmpresas.Text = "Empresas";
            this.tabEmpresas.UseVisualStyleBackColor = true;
            // 
            // tabUpcode
            // 
            this.tabUpcode.Controls.Add(this.btUploadUpcode);
            this.tabUpcode.Controls.Add(this.lblCoutUpcode);
            this.tabUpcode.Controls.Add(this.cbUpcode);
            this.tabUpcode.Location = new System.Drawing.Point(4, 22);
            this.tabUpcode.Name = "tabUpcode";
            this.tabUpcode.Padding = new System.Windows.Forms.Padding(3);
            this.tabUpcode.Size = new System.Drawing.Size(758, 444);
            this.tabUpcode.TabIndex = 2;
            this.tabUpcode.Text = "Upcode";
            this.tabUpcode.UseVisualStyleBackColor = true;
            // 
            // btUploadUpcode
            // 
            this.btUploadUpcode.AutoSize = true;
            this.btUploadUpcode.Location = new System.Drawing.Point(232, 26);
            this.btUploadUpcode.Name = "btUploadUpcode";
            this.btUploadUpcode.Size = new System.Drawing.Size(82, 13);
            this.btUploadUpcode.TabIndex = 3;
            this.btUploadUpcode.TabStop = true;
            this.btUploadUpcode.Text = "Cargar upcodes";
            this.btUploadUpcode.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btUploadUpcode_LinkClicked);
            // 
            // lblCoutUpcode
            // 
            this.lblCoutUpcode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCoutUpcode.AutoSize = true;
            this.lblCoutUpcode.Location = new System.Drawing.Point(19, 415);
            this.lblCoutUpcode.Name = "lblCoutUpcode";
            this.lblCoutUpcode.Size = new System.Drawing.Size(98, 13);
            this.lblCoutUpcode.TabIndex = 2;
            this.lblCoutUpcode.Text = "Total: 0     Libres: 0";
            // 
            // cbUpcode
            // 
            this.cbUpcode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.cbUpcode.FormattingEnabled = true;
            this.cbUpcode.Location = new System.Drawing.Point(13, 16);
            this.cbUpcode.Name = "cbUpcode";
            this.cbUpcode.Size = new System.Drawing.Size(200, 381);
            this.cbUpcode.TabIndex = 1;
            // 
            // tabDb
            // 
            this.tabDb.Controls.Add(this.label2);
            this.tabDb.Controls.Add(this.label1);
            this.tabDb.Controls.Add(this.linkLabel1);
            this.tabDb.Controls.Add(this.btDownloadDb);
            this.tabDb.Location = new System.Drawing.Point(4, 22);
            this.tabDb.Name = "tabDb";
            this.tabDb.Padding = new System.Windows.Forms.Padding(3);
            this.tabDb.Size = new System.Drawing.Size(758, 444);
            this.tabDb.TabIndex = 1;
            this.tabDb.Text = "Base de Datos";
            this.tabDb.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(331, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Remplaza la base de datos actual por la seleccionada por el usuario.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(351, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hace una copia de la base de datos tal como se encuentra actualmente.";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(22, 88);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(123, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Restaurar base de datos";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // btDownloadDb
            // 
            this.btDownloadDb.AutoSize = true;
            this.btDownloadDb.Location = new System.Drawing.Point(22, 35);
            this.btDownloadDb.Name = "btDownloadDb";
            this.btDownloadDb.Size = new System.Drawing.Size(194, 13);
            this.btDownloadDb.TabIndex = 0;
            this.btDownloadDb.TabStop = true;
            this.btDownloadDb.Text = "Copia de seguridad de la base de datos";
            this.btDownloadDb.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btDownloadDb_LinkClicked);
            // 
            // saveDb
            // 
            this.saveDb.FileName = "tarjetas.sdf";
            this.saveDb.Filter = "Base de datos|*.sdf";
            this.saveDb.Title = "Guardar base de datos";
            // 
            // openFileDb
            // 
            this.openFileDb.FileName = "tarjetas.sdf";
            this.openFileDb.Filter = "Base de datos|*.sdf";
            this.openFileDb.Title = "Abrir base de datos";
            // 
            // openFileUpcode
            // 
            this.openFileUpcode.Filter = "Archivos .PNG|*.png|Todos los Archivos|*.*";
            this.openFileUpcode.Multiselect = true;
            this.openFileUpcode.Title = "Abrir archivos de imagen Upcode";
            // 
            // FrmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 470);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfig";
            this.ShowInTaskbar = false;
            this.Text = "Configuración";
            this.Load += new System.EventHandler(this.FrmConfig_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabEstilos.ResumeLayout(false);
            this.tabEmpresas.ResumeLayout(false);
            this.tabUpcode.ResumeLayout(false);
            this.tabUpcode.PerformLayout();
            this.tabDb.ResumeLayout(false);
            this.tabDb.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btEliminar;
        private System.Windows.Forms.Button btModificar;
        private System.Windows.Forms.Button btNuevo;
        private System.Windows.Forms.ListBox cbEmpresa;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabEmpresas;
        private System.Windows.Forms.TabPage tabDb;
        private System.Windows.Forms.LinkLabel btDownloadDb;
        private System.Windows.Forms.SaveFileDialog saveDb;
        private System.Windows.Forms.OpenFileDialog openFileDb;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabUpcode;
        private System.Windows.Forms.ListBox cbUpcode;
        private System.Windows.Forms.Label lblCoutUpcode;
        private System.Windows.Forms.LinkLabel btUploadUpcode;
        private System.Windows.Forms.OpenFileDialog openFileUpcode;
        private System.Windows.Forms.TabPage tabEstilos;
        private TarjetasSAI.Back back1;
    }
}