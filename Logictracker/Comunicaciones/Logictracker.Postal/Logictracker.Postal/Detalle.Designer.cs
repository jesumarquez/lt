namespace Urbetrack.Postal
{
    partial class Detalle
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.btAceptar = new System.Windows.Forms.MenuItem();
            this.btCancelar = new System.Windows.Forms.MenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbDireccion = new System.Windows.Forms.TextBox();
            this.tbPieza = new System.Windows.Forms.TextBox();
            this.tbCliente = new System.Windows.Forms.TextBox();
            this.tbDestinatario = new System.Windows.Forms.TextBox();
            this.tbTipoS = new System.Windows.Forms.TextBox();
            this.cbAcciones = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.btAceptar);
            this.mainMenu1.MenuItems.Add(this.btCancelar);
            // 
            // btAceptar
            // 
            this.btAceptar.Text = "Aceptar";
            this.btAceptar.Click += new System.EventHandler(this.btAceptar_Click);
            // 
            // btCancelar
            // 
            this.btCancelar.Text = "Cancelar";
            this.btCancelar.Click += new System.EventHandler(this.btCancelar_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 30);
            this.label1.Text = "Dirección:";
            this.label1.ParentChanged += new System.EventHandler(this.label1_ParentChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(0, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 30);
            this.label2.Text = "Pieza:";
            this.label2.ParentChanged += new System.EventHandler(this.label2_ParentChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(0, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 30);
            this.label3.Text = "Cliente:";
            this.label3.ParentChanged += new System.EventHandler(this.label3_ParentChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(0, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 30);
            this.label4.Text = "Destinatario:";
            this.label4.ParentChanged += new System.EventHandler(this.label4_ParentChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(0, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 30);
            this.label5.Text = "Tipo Servicio:";
            this.label5.ParentChanged += new System.EventHandler(this.label5_ParentChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(0, 188);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 30);
            this.label6.Text = "Accion:";
            // 
            // tbDireccion
            // 
            this.tbDireccion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.tbDireccion.Location = new System.Drawing.Point(96, 8);
            this.tbDireccion.Name = "tbDireccion";
            this.tbDireccion.ReadOnly = true;
            this.tbDireccion.Size = new System.Drawing.Size(211, 30);
            this.tbDireccion.TabIndex = 12;
            // 
            // tbPieza
            // 
            this.tbPieza.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.tbPieza.Location = new System.Drawing.Point(96, 44);
            this.tbPieza.Name = "tbPieza";
            this.tbPieza.ReadOnly = true;
            this.tbPieza.Size = new System.Drawing.Size(211, 30);
            this.tbPieza.TabIndex = 25;
            // 
            // tbCliente
            // 
            this.tbCliente.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.tbCliente.Location = new System.Drawing.Point(96, 80);
            this.tbCliente.Name = "tbCliente";
            this.tbCliente.ReadOnly = true;
            this.tbCliente.Size = new System.Drawing.Size(211, 30);
            this.tbCliente.TabIndex = 26;
            // 
            // tbDestinatario
            // 
            this.tbDestinatario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.tbDestinatario.Location = new System.Drawing.Point(96, 116);
            this.tbDestinatario.Name = "tbDestinatario";
            this.tbDestinatario.ReadOnly = true;
            this.tbDestinatario.Size = new System.Drawing.Size(211, 30);
            this.tbDestinatario.TabIndex = 27;
            // 
            // tbTipoS
            // 
            this.tbTipoS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.tbTipoS.Location = new System.Drawing.Point(96, 152);
            this.tbTipoS.Name = "tbTipoS";
            this.tbTipoS.ReadOnly = true;
            this.tbTipoS.Size = new System.Drawing.Size(211, 30);
            this.tbTipoS.TabIndex = 28;
            // 
            // cbAcciones
            // 
            this.cbAcciones.Location = new System.Drawing.Point(96, 188);
            this.cbAcciones.Name = "cbAcciones";
            this.cbAcciones.Size = new System.Drawing.Size(207, 30);
            this.cbAcciones.TabIndex = 36;
            // 
            // Detalle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(131F, 131F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(170)))), ((int)(((byte)(0)))));
            this.ClientSize = new System.Drawing.Size(310, 186);
            this.Controls.Add(this.cbAcciones);
            this.Controls.Add(this.tbTipoS);
            this.Controls.Add(this.tbDestinatario);
            this.Controls.Add(this.tbCliente);
            this.Controls.Add(this.tbPieza);
            this.Controls.Add(this.tbDireccion);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.Name = "Detalle";
            this.Text = "Detalle";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Detalle_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.MenuItem btAceptar;
        private System.Windows.Forms.MenuItem btCancelar;
        private System.Windows.Forms.TextBox tbDireccion;
        private System.Windows.Forms.TextBox tbPieza;
        private System.Windows.Forms.TextBox tbCliente;
        private System.Windows.Forms.TextBox tbDestinatario;
        private System.Windows.Forms.TextBox tbTipoS;
        private System.Windows.Forms.ComboBox cbAcciones;
    }
}