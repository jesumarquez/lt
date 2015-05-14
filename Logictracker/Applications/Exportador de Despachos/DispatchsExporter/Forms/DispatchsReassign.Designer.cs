
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Vehiculos;

namespace DispatchsExporter.Forms
{
    partial class DispatchsReassign
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

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
            this.ddlCentroCarga = new System.Windows.Forms.ComboBox();
            this.lineaBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lblCentro = new System.Windows.Forms.Label();
            this.grdDespachos = new System.Windows.Forms.DataGridView();
            this.gridDespachoBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.gridDespachoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnExportar = new System.Windows.Forms.Button();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.reassignamentGrouping = new System.Windows.Forms.GroupBox();
            this.lbVehicles = new System.Windows.Forms.ListBox();
            this.cocheBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnCancelarVehiculo = new System.Windows.Forms.Button();
            this.btnAceptarVehiculo = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.lblError = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.gridDespachoBindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.iconDataGridViewImageColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.fechaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.volumenDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.internoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patenteDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operadorDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriCentroDeCostos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.codigoCentroDeCostosDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mobileIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.lineaBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdDespachos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDespachoBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDespachoBindingSource)).BeginInit();
            this.reassignamentGrouping.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cocheBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDespachoBindingSource2)).BeginInit();
            this.SuspendLayout();
            // 
            // ddlCentroCarga
            // 
            this.ddlCentroCarga.AllowDrop = true;
            this.ddlCentroCarga.DataSource = this.lineaBindingSource;
            this.ddlCentroCarga.DisplayMember = "Descripcion";
            this.ddlCentroCarga.FormattingEnabled = true;
            this.ddlCentroCarga.Location = new System.Drawing.Point(157, 13);
            this.ddlCentroCarga.Name = "ddlCentroCarga";
            this.ddlCentroCarga.Size = new System.Drawing.Size(191, 21);
            this.ddlCentroCarga.TabIndex = 0;
            this.ddlCentroCarga.ValueMember = "Id";
            this.ddlCentroCarga.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // lineaBindingSource
            // 
            this.lineaBindingSource.DataSource = typeof(Linea);
            // 
            // lblCentro
            // 
            this.lblCentro.AutoSize = true;
            this.lblCentro.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCentro.Location = new System.Drawing.Point(36, 16);
            this.lblCentro.Name = "lblCentro";
            this.lblCentro.Size = new System.Drawing.Size(111, 15);
            this.lblCentro.TabIndex = 1;
            this.lblCentro.Text = "Centro de Carga";
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
            this.iconDataGridViewImageColumn,
            this.fechaDataGridViewTextBoxColumn,
            this.volumenDataGridViewTextBoxColumn,
            this.internoDataGridViewTextBoxColumn,
            this.patenteDataGridViewTextBoxColumn,
            this.operadorDataGridViewTextBoxColumn,
            this.DescriCentroDeCostos,
            this.codigoCentroDeCostosDataGridViewTextBoxColumn,
            this.mobileIDDataGridViewTextBoxColumn,
            this.iDDataGridViewTextBoxColumn});
            this.grdDespachos.Cursor = System.Windows.Forms.Cursors.Default;
            this.grdDespachos.DataSource = this.gridDespachoBindingSource1;
            this.grdDespachos.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grdDespachos.Location = new System.Drawing.Point(12, 48);
            this.grdDespachos.MultiSelect = false;
            this.grdDespachos.Name = "grdDespachos";
            this.grdDespachos.ReadOnly = true;
            this.grdDespachos.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.grdDespachos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdDespachos.Size = new System.Drawing.Size(829, 368);
            this.grdDespachos.TabIndex = 2;
            this.grdDespachos.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdDespachos_CellContentClick);
            // 
            // gridDespachoBindingSource1
            // 
            this.gridDespachoBindingSource1.DataSource = typeof(DispatchsExporter.Types.ReportObjects.GridDespacho);
            // 
            // gridDespachoBindingSource
            // 
            this.gridDespachoBindingSource.DataSource = typeof(DispatchsExporter.Types.ReportObjects.GridDespacho);
            // 
            // btnExportar
            // 
            this.btnExportar.Location = new System.Drawing.Point(16, 422);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(149, 34);
            this.btnExportar.TabIndex = 3;
            this.btnExportar.Text = "Confirmar";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.HeaderText = "";
            this.dataGridViewImageColumn1.Image = global::DispatchsExporter.Properties.Resources.moncal;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.ReadOnly = true;
            this.dataGridViewImageColumn1.Width = 25;
            // 
            // reassignamentGrouping
            // 
            this.reassignamentGrouping.Controls.Add(this.lbVehicles);
            this.reassignamentGrouping.Controls.Add(this.btnCancelarVehiculo);
            this.reassignamentGrouping.Controls.Add(this.btnAceptarVehiculo);
            this.reassignamentGrouping.Location = new System.Drawing.Point(847, 61);
            this.reassignamentGrouping.Name = "reassignamentGrouping";
            this.reassignamentGrouping.Size = new System.Drawing.Size(226, 355);
            this.reassignamentGrouping.TabIndex = 4;
            this.reassignamentGrouping.TabStop = false;
            this.reassignamentGrouping.Text = "Reasignar  Despacho";
            this.reassignamentGrouping.Visible = false;
            // 
            // lbVehicles
            // 
            this.lbVehicles.DataSource = this.cocheBindingSource;
            this.lbVehicles.DisplayMember = "Interno";
            this.lbVehicles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbVehicles.FormattingEnabled = true;
            this.lbVehicles.ItemHeight = 15;
            this.lbVehicles.Location = new System.Drawing.Point(8, 19);
            this.lbVehicles.Name = "lbVehicles";
            this.lbVehicles.Size = new System.Drawing.Size(215, 304);
            this.lbVehicles.Sorted = true;
            this.lbVehicles.TabIndex = 2;
            this.lbVehicles.ValueMember = "Id";
            // 
            // cocheBindingSource
            // 
            this.cocheBindingSource.DataSource = typeof(Coche);
            // 
            // btnCancelarVehiculo
            // 
            this.btnCancelarVehiculo.Location = new System.Drawing.Point(122, 328);
            this.btnCancelarVehiculo.Name = "btnCancelarVehiculo";
            this.btnCancelarVehiculo.Size = new System.Drawing.Size(102, 23);
            this.btnCancelarVehiculo.TabIndex = 1;
            this.btnCancelarVehiculo.Text = "Cancelar";
            this.btnCancelarVehiculo.UseVisualStyleBackColor = true;
            this.btnCancelarVehiculo.Click += new System.EventHandler(this.btnCancelarVehiculo_Click);
            // 
            // btnAceptarVehiculo
            // 
            this.btnAceptarVehiculo.Location = new System.Drawing.Point(8, 329);
            this.btnAceptarVehiculo.Name = "btnAceptarVehiculo";
            this.btnAceptarVehiculo.Size = new System.Drawing.Size(98, 23);
            this.btnAceptarVehiculo.TabIndex = 0;
            this.btnAceptarVehiculo.Text = "Asignar";
            this.btnAceptarVehiculo.UseVisualStyleBackColor = true;
            this.btnAceptarVehiculo.Click += new System.EventHandler(this.btnAceptarVehiculo_Click);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(913, 422);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(149, 34);
            this.btnCerrar.TabIndex = 5;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(171, 422);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 6;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DispatchsExporter.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(911, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(162, 59);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(174, 443);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(733, 12);
            this.progressBar.TabIndex = 8;
            this.progressBar.Visible = false;
            // 
            // gridDespachoBindingSource2
            // 
            this.gridDespachoBindingSource2.DataSource = typeof(DispatchsExporter.Types.ReportObjects.GridDespacho);
            // 
            // iconDataGridViewImageColumn
            // 
            this.iconDataGridViewImageColumn.DataPropertyName = "Icon";
            this.iconDataGridViewImageColumn.HeaderText = "Icon";
            this.iconDataGridViewImageColumn.Name = "iconDataGridViewImageColumn";
            this.iconDataGridViewImageColumn.ReadOnly = true;
            this.iconDataGridViewImageColumn.Width = 40;
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
            this.internoDataGridViewTextBoxColumn.Width = 120;
            // 
            // patenteDataGridViewTextBoxColumn
            // 
            this.patenteDataGridViewTextBoxColumn.DataPropertyName = "Patente";
            this.patenteDataGridViewTextBoxColumn.HeaderText = "Patente";
            this.patenteDataGridViewTextBoxColumn.Name = "patenteDataGridViewTextBoxColumn";
            this.patenteDataGridViewTextBoxColumn.ReadOnly = true;
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
            this.DescriCentroDeCostos.Width = 125;
            // 
            // codigoCentroDeCostosDataGridViewTextBoxColumn
            // 
            this.codigoCentroDeCostosDataGridViewTextBoxColumn.DataPropertyName = "CodigoCentroDeCostos";
            this.codigoCentroDeCostosDataGridViewTextBoxColumn.HeaderText = "CodigoCentroDeCostos";
            this.codigoCentroDeCostosDataGridViewTextBoxColumn.Name = "codigoCentroDeCostosDataGridViewTextBoxColumn";
            this.codigoCentroDeCostosDataGridViewTextBoxColumn.ReadOnly = true;
            this.codigoCentroDeCostosDataGridViewTextBoxColumn.Visible = false;
            this.codigoCentroDeCostosDataGridViewTextBoxColumn.Width = 90;
            // 
            // mobileIDDataGridViewTextBoxColumn
            // 
            this.mobileIDDataGridViewTextBoxColumn.DataPropertyName = "MobileID";
            this.mobileIDDataGridViewTextBoxColumn.HeaderText = "MobileID";
            this.mobileIDDataGridViewTextBoxColumn.Name = "mobileIDDataGridViewTextBoxColumn";
            this.mobileIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.mobileIDDataGridViewTextBoxColumn.Visible = false;
            // 
            // iDDataGridViewTextBoxColumn
            // 
            this.iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            this.iDDataGridViewTextBoxColumn.HeaderText = "ID";
            this.iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            this.iDDataGridViewTextBoxColumn.ReadOnly = true;
            this.iDDataGridViewTextBoxColumn.Visible = false;
            // 
            // DispatchsReassign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1076, 468);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.reassignamentGrouping);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.grdDespachos);
            this.Controls.Add(this.lblCentro);
            this.Controls.Add(this.ddlCentroCarga);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "DispatchsReassign";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reasignación de Despachos";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DispatchsView_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.lineaBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdDespachos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDespachoBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDespachoBindingSource)).EndInit();
            this.reassignamentGrouping.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cocheBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDespachoBindingSource2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ddlCentroCarga;
        private System.Windows.Forms.Label lblCentro;
        private System.Windows.Forms.DataGridView grdDespachos;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.GroupBox reassignamentGrouping;
        private System.Windows.Forms.ListBox lbVehicles;
        private System.Windows.Forms.Button btnCancelarVehiculo;
        private System.Windows.Forms.Button btnAceptarVehiculo;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.BindingSource lineaBindingSource;
        private System.Windows.Forms.BindingSource cocheBindingSource;
        private System.Windows.Forms.BindingSource gridDespachoBindingSource;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.BindingSource gridDespachoBindingSource1;
        private System.Windows.Forms.BindingSource gridDespachoBindingSource2;
        private System.Windows.Forms.DataGridViewImageColumn iconDataGridViewImageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fechaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn volumenDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn internoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn patenteDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn operadorDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescriCentroDeCostos;
        private System.Windows.Forms.DataGridViewTextBoxColumn codigoCentroDeCostosDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mobileIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
    }
}