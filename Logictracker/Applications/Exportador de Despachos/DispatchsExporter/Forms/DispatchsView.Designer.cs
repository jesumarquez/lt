namespace DispatchsExporter.Forms
{
    partial class DispatchsView
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
            this.lblCentro = new System.Windows.Forms.Label();
            this.ddlCentroCarga = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ddlCentroDeCarga = new System.Windows.Forms.ComboBox();
            this.grdDespachos = new System.Windows.Forms.DataGridView();
            this.dpDesde = new System.Windows.Forms.DateTimePicker();
            this.dpHasta = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.gridDespachoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.fechaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.volumenDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.internoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patenteDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operadorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriCentroDeCostos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.codigoCentroDeCostosDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grdDespachos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDespachoBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCentro
            // 
            this.lblCentro.AutoSize = true;
            this.lblCentro.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCentro.Location = new System.Drawing.Point(-149, -34);
            this.lblCentro.Name = "lblCentro";
            this.lblCentro.Size = new System.Drawing.Size(111, 15);
            this.lblCentro.TabIndex = 4;
            this.lblCentro.Text = "Centro de Carga";
            // 
            // ddlCentroCarga
            // 
            this.ddlCentroCarga.AllowDrop = true;
            this.ddlCentroCarga.DisplayMember = "Descripcion";
            this.ddlCentroCarga.FormattingEnabled = true;
            this.ddlCentroCarga.Location = new System.Drawing.Point(-28, -37);
            this.ddlCentroCarga.Name = "ddlCentroCarga";
            this.ddlCentroCarga.Size = new System.Drawing.Size(191, 21);
            this.ddlCentroCarga.TabIndex = 3;
            this.ddlCentroCarga.ValueMember = "Id";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "Centro de Carga";
            // 
            // ddlCentroDeCarga
            // 
            this.ddlCentroDeCarga.AllowDrop = true;
            this.ddlCentroDeCarga.DisplayMember = "Descripcion";
            this.ddlCentroDeCarga.FormattingEnabled = true;
            this.ddlCentroDeCarga.Location = new System.Drawing.Point(135, 20);
            this.ddlCentroDeCarga.Name = "ddlCentroDeCarga";
            this.ddlCentroDeCarga.Size = new System.Drawing.Size(169, 21);
            this.ddlCentroDeCarga.TabIndex = 5;
            this.ddlCentroDeCarga.ValueMember = "Id";
            // 
            // grdDespachos
            // 
            this.grdDespachos.AllowUserToAddRows = false;
            this.grdDespachos.AllowUserToDeleteRows = false;
            this.grdDespachos.AllowUserToResizeColumns = false;
            this.grdDespachos.AllowUserToResizeRows = false;
            this.grdDespachos.AutoGenerateColumns = false;
            this.grdDespachos.BackgroundColor = System.Drawing.SystemColors.ActiveBorder;
            this.grdDespachos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grdDespachos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdDespachos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fechaDataGridViewTextBoxColumn,
            this.volumenDataGridViewTextBoxColumn,
            this.internoDataGridViewTextBoxColumn,
            this.patenteDataGridViewTextBoxColumn,
            this.operadorDataGridViewTextBoxColumn,
            this.DescriCentroDeCostos,
            this.codigoCentroDeCostosDataGridViewTextBoxColumn});
            this.grdDespachos.Cursor = System.Windows.Forms.Cursors.Default;
            this.grdDespachos.DataSource = this.gridDespachoBindingSource;
            this.grdDespachos.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grdDespachos.Location = new System.Drawing.Point(12, 54);
            this.grdDespachos.MultiSelect = false;
            this.grdDespachos.Name = "grdDespachos";
            this.grdDespachos.ReadOnly = true;
            this.grdDespachos.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.grdDespachos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdDespachos.Size = new System.Drawing.Size(919, 358);
            this.grdDespachos.TabIndex = 7;
            this.grdDespachos.Visible = false;
            // 
            // dpDesde
            // 
            this.dpDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dpDesde.Location = new System.Drawing.Point(373, 21);
            this.dpDesde.Name = "dpDesde";
            this.dpDesde.Size = new System.Drawing.Size(95, 20);
            this.dpDesde.TabIndex = 8;
            // 
            // dpHasta
            // 
            this.dpHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dpHasta.Location = new System.Drawing.Point(539, 21);
            this.dpHasta.Name = "dpHasta";
            this.dpHasta.Size = new System.Drawing.Size(99, 20);
            this.dpHasta.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(315, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "Desde:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(482, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Hasta:";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(652, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(90, 21);
            this.btnSearch.TabIndex = 12;
            this.btnSearch.Text = "Buscar";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DispatchsExporter.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(769, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(162, 54);
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // gridDespachoBindingSource
            // 
            this.gridDespachoBindingSource.DataSource = typeof(DispatchsExporter.Types.ReportObjects.GridDespacho);
            // 
            // fechaDataGridViewTextBoxColumn
            // 
            this.fechaDataGridViewTextBoxColumn.DataPropertyName = "Fecha";
            this.fechaDataGridViewTextBoxColumn.HeaderText = "Fecha";
            this.fechaDataGridViewTextBoxColumn.Name = "fechaDataGridViewTextBoxColumn";
            this.fechaDataGridViewTextBoxColumn.ReadOnly = true;
            this.fechaDataGridViewTextBoxColumn.Width = 120;
            // 
            // volumenDataGridViewTextBoxColumn
            // 
            this.volumenDataGridViewTextBoxColumn.DataPropertyName = "Volumen";
            this.volumenDataGridViewTextBoxColumn.HeaderText = "Volumen";
            this.volumenDataGridViewTextBoxColumn.Name = "volumenDataGridViewTextBoxColumn";
            this.volumenDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // internoDataGridViewTextBoxColumn
            // 
            this.internoDataGridViewTextBoxColumn.DataPropertyName = "Interno";
            this.internoDataGridViewTextBoxColumn.HeaderText = "Interno";
            this.internoDataGridViewTextBoxColumn.Name = "internoDataGridViewTextBoxColumn";
            this.internoDataGridViewTextBoxColumn.ReadOnly = true;
            this.internoDataGridViewTextBoxColumn.Width = 150;
            // 
            // patenteDataGridViewTextBoxColumn
            // 
            this.patenteDataGridViewTextBoxColumn.DataPropertyName = "Patente";
            this.patenteDataGridViewTextBoxColumn.HeaderText = "Patente";
            this.patenteDataGridViewTextBoxColumn.Name = "patenteDataGridViewTextBoxColumn";
            this.patenteDataGridViewTextBoxColumn.ReadOnly = true;
            this.patenteDataGridViewTextBoxColumn.Width = 150;
            // 
            // operadorDataGridViewTextBoxColumn
            // 
            this.operadorDataGridViewTextBoxColumn.DataPropertyName = "Operador";
            this.operadorDataGridViewTextBoxColumn.HeaderText = "Operador";
            this.operadorDataGridViewTextBoxColumn.Name = "operadorDataGridViewTextBoxColumn";
            this.operadorDataGridViewTextBoxColumn.ReadOnly = true;
            this.operadorDataGridViewTextBoxColumn.Width = 180;
            // 
            // DescriCentroDeCostos
            // 
            this.DescriCentroDeCostos.DataPropertyName = "DescriCentroDeCostos";
            this.DescriCentroDeCostos.HeaderText = "Centro De Costos";
            this.DescriCentroDeCostos.Name = "DescriCentroDeCostos";
            this.DescriCentroDeCostos.ReadOnly = true;
            this.DescriCentroDeCostos.Width = 160;
            // 
            // codigoCentroDeCostosDataGridViewTextBoxColumn
            // 
            this.codigoCentroDeCostosDataGridViewTextBoxColumn.DataPropertyName = "CodigoCentroDeCostos";
            this.codigoCentroDeCostosDataGridViewTextBoxColumn.HeaderText = "CodigoCentroDeCostos";
            this.codigoCentroDeCostosDataGridViewTextBoxColumn.Name = "codigoCentroDeCostosDataGridViewTextBoxColumn";
            this.codigoCentroDeCostosDataGridViewTextBoxColumn.ReadOnly = true;
            this.codigoCentroDeCostosDataGridViewTextBoxColumn.Visible = false;
            this.codigoCentroDeCostosDataGridViewTextBoxColumn.Width = 80;
            // 
            // DispatchsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 426);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dpHasta);
            this.Controls.Add(this.dpDesde);
            this.Controls.Add(this.grdDespachos);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ddlCentroDeCarga);
            this.Controls.Add(this.lblCentro);
            this.Controls.Add(this.ddlCentroCarga);
            this.MaximizeBox = false;
            this.Name = "DispatchsView";
            this.Text = "Reporte de Despachos";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DispatchsView_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.grdDespachos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDespachoBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCentro;
        private System.Windows.Forms.ComboBox ddlCentroCarga;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ddlCentroDeCarga;
        private System.Windows.Forms.DataGridView grdDespachos;
        private System.Windows.Forms.DateTimePicker dpDesde;
        private System.Windows.Forms.DateTimePicker dpHasta;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.BindingSource gridDespachoBindingSource;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn fechaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn volumenDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn internoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn patenteDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn operadorDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescriCentroDeCostos;
        private System.Windows.Forms.DataGridViewTextBoxColumn codigoCentroDeCostosDataGridViewTextBoxColumn;
    }
}