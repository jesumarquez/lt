

using Logictracker.Culture;

namespace HandlerTest
{
    partial class MainFomr
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbVehiculo = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbEmpresa = new System.Windows.Forms.ComboBox();
            this.cbLinea = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPosicion = new System.Windows.Forms.TabPage();
            this.tabMensajeria = new System.Windows.Forms.TabPage();
            this.tabCicloLogistico = new System.Windows.Forms.TabPage();
            this.tabGenerador = new System.Windows.Forms.TabPage();
            this.tabConfig = new System.Windows.Forms.TabPage();
            this.tabScheduler = new System.Windows.Forms.TabPage();
            this.ucPositions1 = new HandlerTest.Controls.ucPositions();
            this.ucMensajeria1 = new HandlerTest.Controls.ucMensajeria();
            this.ucCiclo1 = new HandlerTest.Controls.ucCiclo();
            this.ucGenerador1 = new HandlerTest.Controls.ucGenerador();
            this.ucConfig1 = new HandlerTest.Controls.ucConfig();
            this.ucScheduler1 = new HandlerTest.Controls.ucScheduler();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPosicion.SuspendLayout();
            this.tabMensajeria.SuspendLayout();
            this.tabCicloLogistico.SuspendLayout();
            this.tabGenerador.SuspendLayout();
            this.tabConfig.SuspendLayout();
            this.tabScheduler.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(863, 466);
            this.splitContainer1.SplitterDistance = 70;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbVehiculo);
            this.groupBox2.Location = new System.Drawing.Point(251, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(246, 68);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            // 
            // cbVehiculo
            // 
            this.cbVehiculo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbVehiculo.DisplayMember = "Interno";
            this.cbVehiculo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVehiculo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbVehiculo.FormattingEnabled = true;
            this.cbVehiculo.Location = new System.Drawing.Point(6, 25);
            this.cbVehiculo.Name = "cbVehiculo";
            this.cbVehiculo.Size = new System.Drawing.Size(234, 28);
            this.cbVehiculo.TabIndex = 10;
            this.cbVehiculo.SelectedIndexChanged += new System.EventHandler(this.cbVehiculo_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbEmpresa);
            this.groupBox1.Controls.Add(this.cbLinea);
            this.groupBox1.Location = new System.Drawing.Point(4, -1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(239, 69);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            // 
            // cbEmpresa
            // 
            this.cbEmpresa.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbEmpresa.DisplayMember = "RazonSocial";
            this.cbEmpresa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEmpresa.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbEmpresa.FormattingEnabled = true;
            this.cbEmpresa.Location = new System.Drawing.Point(6, 10);
            this.cbEmpresa.Name = "cbEmpresa";
            this.cbEmpresa.Size = new System.Drawing.Size(227, 24);
            this.cbEmpresa.TabIndex = 8;
            this.cbEmpresa.SelectedIndexChanged += new System.EventHandler(this.cbEmpresa_SelectedIndexChanged);
            // 
            // cbLinea
            // 
            this.cbLinea.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLinea.DisplayMember = "Descripcion";
            this.cbLinea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLinea.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbLinea.FormattingEnabled = true;
            this.cbLinea.Location = new System.Drawing.Point(6, 39);
            this.cbLinea.Name = "cbLinea";
            this.cbLinea.Size = new System.Drawing.Size(227, 24);
            this.cbLinea.TabIndex = 9;
            this.cbLinea.SelectedIndexChanged += new System.EventHandler(this.cbLinea_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPosicion);
            this.tabControl1.Controls.Add(this.tabMensajeria);
            this.tabControl1.Controls.Add(this.tabCicloLogistico);
            this.tabControl1.Controls.Add(this.tabScheduler);
            this.tabControl1.Controls.Add(this.tabGenerador);
            this.tabControl1.Controls.Add(this.tabConfig);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(863, 392);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPosicion
            // 
            this.tabPosicion.Controls.Add(this.ucPositions1);
            this.tabPosicion.Location = new System.Drawing.Point(4, 22);
            this.tabPosicion.Name = "tabPosicion";
            this.tabPosicion.Padding = new System.Windows.Forms.Padding(3);
            this.tabPosicion.Size = new System.Drawing.Size(855, 366);
            this.tabPosicion.TabIndex = 0;
            this.tabPosicion.Text = "Posicion";
            this.tabPosicion.UseVisualStyleBackColor = true;
            // 
            // tabMensajeria
            // 
            this.tabMensajeria.Controls.Add(this.ucMensajeria1);
            this.tabMensajeria.Location = new System.Drawing.Point(4, 22);
            this.tabMensajeria.Name = "tabMensajeria";
            this.tabMensajeria.Padding = new System.Windows.Forms.Padding(3);
            this.tabMensajeria.Size = new System.Drawing.Size(855, 366);
            this.tabMensajeria.TabIndex = 3;
            this.tabMensajeria.Text = "Mensajería";
            this.tabMensajeria.UseVisualStyleBackColor = true;
            // 
            // tabCicloLogistico
            // 
            this.tabCicloLogistico.Controls.Add(this.ucCiclo1);
            this.tabCicloLogistico.Location = new System.Drawing.Point(4, 22);
            this.tabCicloLogistico.Name = "tabCicloLogistico";
            this.tabCicloLogistico.Padding = new System.Windows.Forms.Padding(3);
            this.tabCicloLogistico.Size = new System.Drawing.Size(855, 366);
            this.tabCicloLogistico.TabIndex = 2;
            this.tabCicloLogistico.Text = CultureManager.GetMenu("SYS_CICLO_LOGISTICO");
            this.tabCicloLogistico.UseVisualStyleBackColor = true;
            // 
            // tabGenerador
            // 
            this.tabGenerador.Controls.Add(this.ucGenerador1);
            this.tabGenerador.Location = new System.Drawing.Point(4, 22);
            this.tabGenerador.Name = "tabGenerador";
            this.tabGenerador.Padding = new System.Windows.Forms.Padding(3);
            this.tabGenerador.Size = new System.Drawing.Size(855, 366);
            this.tabGenerador.TabIndex = 4;
            this.tabGenerador.Text = "Generador";
            this.tabGenerador.UseVisualStyleBackColor = true;
            // 
            // tabConfig
            // 
            this.tabConfig.Controls.Add(this.ucConfig1);
            this.tabConfig.Location = new System.Drawing.Point(4, 22);
            this.tabConfig.Name = "tabConfig";
            this.tabConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfig.Size = new System.Drawing.Size(855, 366);
            this.tabConfig.TabIndex = 1;
            this.tabConfig.Text = "Configuración";
            this.tabConfig.UseVisualStyleBackColor = true;
            // 
            // tabScheduler
            // 
            this.tabScheduler.Controls.Add(this.ucScheduler1);
            this.tabScheduler.Location = new System.Drawing.Point(4, 22);
            this.tabScheduler.Name = "tabScheduler";
            this.tabScheduler.Padding = new System.Windows.Forms.Padding(3);
            this.tabScheduler.Size = new System.Drawing.Size(855, 366);
            this.tabScheduler.TabIndex = 5;
            this.tabScheduler.Text = "Scheduler";
            this.tabScheduler.UseVisualStyleBackColor = true;
            // 
            // ucPositions1
            // 
            this.ucPositions1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucPositions1.Location = new System.Drawing.Point(3, 3);
            this.ucPositions1.Name = "ucPositions1";
            this.ucPositions1.Size = new System.Drawing.Size(849, 360);
            this.ucPositions1.TabIndex = 0;
            // 
            // ucMensajeria1
            // 
            this.ucMensajeria1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucMensajeria1.Location = new System.Drawing.Point(3, 3);
            this.ucMensajeria1.Name = "ucMensajeria1";
            this.ucMensajeria1.Size = new System.Drawing.Size(849, 360);
            this.ucMensajeria1.TabIndex = 0;
            // 
            // ucCiclo1
            // 
            this.ucCiclo1.Distribucion = null;
            this.ucCiclo1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucCiclo1.Location = new System.Drawing.Point(3, 3);
            this.ucCiclo1.Name = "ucCiclo1";
            this.ucCiclo1.Size = new System.Drawing.Size(849, 360);
            this.ucCiclo1.TabIndex = 0;
            // 
            // ucGenerador1
            // 
            this.ucGenerador1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucGenerador1.Location = new System.Drawing.Point(3, 3);
            this.ucGenerador1.Name = "ucGenerador1";
            this.ucGenerador1.Size = new System.Drawing.Size(849, 360);
            this.ucGenerador1.TabIndex = 0;
            // 
            // ucConfig1
            // 
            this.ucConfig1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucConfig1.Location = new System.Drawing.Point(3, 3);
            this.ucConfig1.Name = "ucConfig1";
            this.ucConfig1.Queue = ".\\private$\\eventos_trax";
            this.ucConfig1.Size = new System.Drawing.Size(849, 360);
            this.ucConfig1.TabIndex = 0;
            // 
            // ucScheduler1
            // 
            this.ucScheduler1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucScheduler1.Location = new System.Drawing.Point(3, 3);
            this.ucScheduler1.Name = "ucScheduler1";
            this.ucScheduler1.Size = new System.Drawing.Size(849, 360);
            this.ucScheduler1.TabIndex = 0;
            // 
            // MainFomr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(863, 466);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainFomr";
            this.Text = "LogicTester";
            this.Load += new System.EventHandler(this.MainFomr_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFomr_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPosicion.ResumeLayout(false);
            this.tabMensajeria.ResumeLayout(false);
            this.tabCicloLogistico.ResumeLayout(false);
            this.tabGenerador.ResumeLayout(false);
            this.tabConfig.ResumeLayout(false);
            this.tabScheduler.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox cbVehiculo;
        private System.Windows.Forms.ComboBox cbEmpresa;
        private System.Windows.Forms.ComboBox cbLinea;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPosicion;
        private System.Windows.Forms.TabPage tabConfig;
        private System.Windows.Forms.TabPage tabCicloLogistico;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private HandlerTest.Controls.ucPositions ucPositions1;
        private HandlerTest.Controls.ucCiclo ucCiclo1;
        private HandlerTest.Controls.ucConfig ucConfig1;
        private System.Windows.Forms.TabPage tabMensajeria;
        private HandlerTest.Controls.ucMensajeria ucMensajeria1;
        private System.Windows.Forms.TabPage tabGenerador;
        private HandlerTest.Controls.ucGenerador ucGenerador1;
        private System.Windows.Forms.TabPage tabScheduler;
        private HandlerTest.Controls.ucScheduler ucScheduler1;
    }
}