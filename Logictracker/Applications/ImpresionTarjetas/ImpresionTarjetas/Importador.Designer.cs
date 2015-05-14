namespace TarjetasSAI
{
    partial class Importador
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblFolder = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btCancelar = new System.Windows.Forms.Button();
            this.btImportar = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.lblResultado = new System.Windows.Forms.ListBox();
            this.chkFormatDoc = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btTemplate = new System.Windows.Forms.LinkLabel();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.cbEmpresa = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Archivo Excel|*.xls|Todos los Archivos|*.*";
            this.openFileDialog1.Title = "Abrir Archivo de Datos";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Archivo de Datos";
            // 
            // lblFileName
            // 
            this.lblFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFileName.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblFileName.Location = new System.Drawing.Point(12, 71);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(644, 23);
            this.lblFileName.TabIndex = 1;
            this.lblFileName.Text = "Ninguno";
            this.toolTip1.SetToolTip(this.lblFileName, "El archivo a importar debe ser un archivo formato Excel 97\r\ncon una hoja llamada " +
                    "Empleados con las siguientes columnas:\r\n - Legajo\r\n - Apellido\r\n - Nombre\r\n - Do" +
                    "cumento\r\n - Puesto\r\n");
            this.lblFileName.Click += new System.EventHandler(this.lblFileName_Click);
            // 
            // lblFolder
            // 
            this.lblFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFolder.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblFolder.Location = new System.Drawing.Point(12, 125);
            this.lblFolder.Name = "lblFolder";
            this.lblFolder.Size = new System.Drawing.Size(644, 23);
            this.lblFolder.TabIndex = 3;
            this.lblFolder.Text = "Ninguna";
            this.toolTip1.SetToolTip(this.lblFolder, "La carpeta de imagenes debe contener las fotos \r\nde los empleados en formato .jpg" +
                    ". \r\nEl nombre de cada archivo debe ser el legajo \r\ndel empleado el que pertenece" +
                    ".\r\n");
            this.lblFolder.Click += new System.EventHandler(this.lblFolder_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Carpeta de Imagenes";
            // 
            // btCancelar
            // 
            this.btCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancelar.Location = new System.Drawing.Point(581, 196);
            this.btCancelar.Name = "btCancelar";
            this.btCancelar.Size = new System.Drawing.Size(75, 23);
            this.btCancelar.TabIndex = 4;
            this.btCancelar.Text = "Cerrar";
            this.btCancelar.UseVisualStyleBackColor = true;
            this.btCancelar.Click += new System.EventHandler(this.btCancelar_Click);
            // 
            // btImportar
            // 
            this.btImportar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btImportar.Location = new System.Drawing.Point(499, 196);
            this.btImportar.Name = "btImportar";
            this.btImportar.Size = new System.Drawing.Size(75, 23);
            this.btImportar.TabIndex = 4;
            this.btImportar.Text = "Importar";
            this.btImportar.UseVisualStyleBackColor = true;
            this.btImportar.Click += new System.EventHandler(this.btImportar_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 227);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Resultado";
            // 
            // lblResultado
            // 
            this.lblResultado.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResultado.FormattingEnabled = true;
            this.lblResultado.Location = new System.Drawing.Point(15, 247);
            this.lblResultado.Name = "lblResultado";
            this.lblResultado.Size = new System.Drawing.Size(641, 147);
            this.lblResultado.TabIndex = 6;
            // 
            // chkFormatDoc
            // 
            this.chkFormatDoc.AutoSize = true;
            this.chkFormatDoc.Checked = true;
            this.chkFormatDoc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFormatDoc.Location = new System.Drawing.Point(15, 167);
            this.chkFormatDoc.Name = "chkFormatDoc";
            this.chkFormatDoc.Size = new System.Drawing.Size(194, 17);
            this.chkFormatDoc.TabIndex = 7;
            this.chkFormatDoc.Text = "Formatear Documento (99.999.999)";
            this.toolTip1.SetToolTip(this.chkFormatDoc, "Marcar esta casilla para normalizar los numeros de documento \r\nagregando los punt" +
                    "os separadores de miles.");
            this.chkFormatDoc.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Ayuda";
            // 
            // btTemplate
            // 
            this.btTemplate.AutoSize = true;
            this.btTemplate.Location = new System.Drawing.Point(12, 196);
            this.btTemplate.Name = "btTemplate";
            this.btTemplate.Size = new System.Drawing.Size(103, 13);
            this.btTemplate.TabIndex = 8;
            this.btTemplate.TabStop = true;
            this.btTemplate.Text = "Descargar Template";
            this.btTemplate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btTemplate_LinkClicked);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "Empleados.xls";
            this.saveFileDialog1.Filter = "Excel 97-2003 (*.xls)|*.xls";
            this.saveFileDialog1.Title = "Guardar Excel Template";
            // 
            // cbEmpresa
            // 
            this.cbEmpresa.DisplayMember = "nombre";
            this.cbEmpresa.FormattingEnabled = true;
            this.cbEmpresa.Location = new System.Drawing.Point(390, 31);
            this.cbEmpresa.Name = "cbEmpresa";
            this.cbEmpresa.Size = new System.Drawing.Size(266, 21);
            this.cbEmpresa.TabIndex = 18;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(387, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Empresa";
            // 
            // Importador
            // 
            this.AcceptButton = this.btImportar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancelar;
            this.ClientSize = new System.Drawing.Size(669, 411);
            this.Controls.Add(this.cbEmpresa);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btTemplate);
            this.Controls.Add(this.chkFormatDoc);
            this.Controls.Add(this.lblResultado);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btImportar);
            this.Controls.Add(this.btCancelar);
            this.Controls.Add(this.lblFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.label1);
            this.Name = "Importador";
            this.Text = "Importador";
            this.Load += new System.EventHandler(this.Importador_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Label lblFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btCancelar;
        private System.Windows.Forms.Button btImportar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ListBox lblResultado;
        private System.Windows.Forms.CheckBox chkFormatDoc;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.LinkLabel btTemplate;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ComboBox cbEmpresa;
        private System.Windows.Forms.Label label7;
    }
}