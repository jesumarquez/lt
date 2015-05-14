namespace TarjetasSAI
{
    partial class Back
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
            System.Windows.Forms.Button btGuardar;
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkLegajo = new System.Windows.Forms.CheckBox();
            this.chkDocumento = new System.Windows.Forms.CheckBox();
            this.chkNombre = new System.Windows.Forms.CheckBox();
            this.chkApellido = new System.Windows.Forms.CheckBox();
            this.chkFoto = new System.Windows.Forms.CheckBox();
            this.chkUpcode = new System.Windows.Forms.CheckBox();
            this.panelImagen = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblPlaceLegajo = new System.Windows.Forms.Label();
            this.lblPlaceDocumento = new System.Windows.Forms.Label();
            this.lblPlaceNombre = new System.Windows.Forms.Label();
            this.lblPlaceApellido = new System.Windows.Forms.Label();
            this.lblPlaceUpcode = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.lblPlaceFoto = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numLeft = new System.Windows.Forms.NumericUpDown();
            this.numTop = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.lblUbicarItem = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            btGuardar = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.panelImagen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.lblPlaceUpcode.SuspendLayout();
            this.lblPlaceFoto.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTop)).BeginInit();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.DisplayMember = "ShowName";
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(1, 1);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(268, 394);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(275, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 42);
            this.button1.TabIndex = 1;
            this.button1.Text = "Activar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Archivos .JPG|*.jpg|Archivos .PNG|*.png|Archivos .GIF|*.gif|Todos los Archivos|*." +
                "*";
            this.openFileDialog1.Title = "Foto del Empleado";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(275, 58);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 41);
            this.button3.TabIndex = 5;
            this.button3.Text = "Nuevo";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.panelImagen);
            this.groupBox1.Controls.Add(this.chkUpcode);
            this.groupBox1.Controls.Add(this.chkFoto);
            this.groupBox1.Controls.Add(this.chkApellido);
            this.groupBox1.Controls.Add(this.chkNombre);
            this.groupBox1.Controls.Add(this.chkDocumento);
            this.groupBox1.Controls.Add(this.chkLegajo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(btGuardar);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(388, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(331, 392);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 34);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(190, 20);
            this.textBox1.TabIndex = 3;
            // 
            // btGuardar
            // 
            btGuardar.Location = new System.Drawing.Point(232, 321);
            btGuardar.Name = "btGuardar";
            btGuardar.Size = new System.Drawing.Size(75, 23);
            btGuardar.TabIndex = 4;
            btGuardar.Text = "Guardar";
            btGuardar.UseVisualStyleBackColor = true;
            btGuardar.Click += new System.EventHandler(this.btGuardar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Nombre";
            // 
            // chkLegajo
            // 
            this.chkLegajo.AutoSize = true;
            this.chkLegajo.Location = new System.Drawing.Point(226, 34);
            this.chkLegajo.Name = "chkLegajo";
            this.chkLegajo.Size = new System.Drawing.Size(58, 17);
            this.chkLegajo.TabIndex = 5;
            this.chkLegajo.Text = "Legajo";
            this.chkLegajo.UseVisualStyleBackColor = true;
            this.chkLegajo.CheckedChanged += new System.EventHandler(this.chkPlace_CheckedChanged);
            // 
            // chkDocumento
            // 
            this.chkDocumento.AutoSize = true;
            this.chkDocumento.Location = new System.Drawing.Point(226, 57);
            this.chkDocumento.Name = "chkDocumento";
            this.chkDocumento.Size = new System.Drawing.Size(81, 17);
            this.chkDocumento.TabIndex = 5;
            this.chkDocumento.Text = "Documento";
            this.chkDocumento.UseVisualStyleBackColor = true;
            this.chkDocumento.CheckedChanged += new System.EventHandler(this.chkPlace_CheckedChanged);
            // 
            // chkNombre
            // 
            this.chkNombre.AutoSize = true;
            this.chkNombre.Location = new System.Drawing.Point(226, 80);
            this.chkNombre.Name = "chkNombre";
            this.chkNombre.Size = new System.Drawing.Size(63, 17);
            this.chkNombre.TabIndex = 5;
            this.chkNombre.Text = "Nombre";
            this.chkNombre.UseVisualStyleBackColor = true;
            this.chkNombre.CheckedChanged += new System.EventHandler(this.chkPlace_CheckedChanged);
            // 
            // chkApellido
            // 
            this.chkApellido.AutoSize = true;
            this.chkApellido.Location = new System.Drawing.Point(226, 103);
            this.chkApellido.Name = "chkApellido";
            this.chkApellido.Size = new System.Drawing.Size(63, 17);
            this.chkApellido.TabIndex = 5;
            this.chkApellido.Text = "Apellido";
            this.chkApellido.UseVisualStyleBackColor = true;
            this.chkApellido.CheckedChanged += new System.EventHandler(this.chkPlace_CheckedChanged);
            // 
            // chkFoto
            // 
            this.chkFoto.AutoSize = true;
            this.chkFoto.Location = new System.Drawing.Point(226, 126);
            this.chkFoto.Name = "chkFoto";
            this.chkFoto.Size = new System.Drawing.Size(47, 17);
            this.chkFoto.TabIndex = 5;
            this.chkFoto.Text = "Foto";
            this.chkFoto.UseVisualStyleBackColor = true;
            this.chkFoto.CheckedChanged += new System.EventHandler(this.chkPlace_CheckedChanged);
            // 
            // chkUpcode
            // 
            this.chkUpcode.AutoSize = true;
            this.chkUpcode.Location = new System.Drawing.Point(226, 149);
            this.chkUpcode.Name = "chkUpcode";
            this.chkUpcode.Size = new System.Drawing.Size(64, 17);
            this.chkUpcode.TabIndex = 5;
            this.chkUpcode.Text = "Upcode";
            this.chkUpcode.UseVisualStyleBackColor = true;
            this.chkUpcode.CheckedChanged += new System.EventHandler(this.chkPlace_CheckedChanged);
            // 
            // panelImagen
            // 
            this.panelImagen.Controls.Add(this.lblPlaceFoto);
            this.panelImagen.Controls.Add(this.lblPlaceUpcode);
            this.panelImagen.Controls.Add(this.lblPlaceApellido);
            this.panelImagen.Controls.Add(this.lblPlaceNombre);
            this.panelImagen.Controls.Add(this.lblPlaceDocumento);
            this.panelImagen.Controls.Add(this.lblPlaceLegajo);
            this.panelImagen.Controls.Add(this.pictureBox1);
            this.panelImagen.Location = new System.Drawing.Point(6, 60);
            this.panelImagen.Name = "panelImagen";
            this.panelImagen.Size = new System.Drawing.Size(190, 284);
            this.panelImagen.TabIndex = 6;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(190, 284);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // lblPlaceLegajo
            // 
            this.lblPlaceLegajo.AutoSize = true;
            this.lblPlaceLegajo.BackColor = System.Drawing.Color.Transparent;
            this.lblPlaceLegajo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblPlaceLegajo.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlaceLegajo.Location = new System.Drawing.Point(73, 15);
            this.lblPlaceLegajo.Name = "lblPlaceLegajo";
            this.lblPlaceLegajo.Size = new System.Drawing.Size(38, 12);
            this.lblPlaceLegajo.TabIndex = 4;
            this.lblPlaceLegajo.Text = "[Legajo]";
            this.lblPlaceLegajo.Visible = false;
            this.lblPlaceLegajo.Click += new System.EventHandler(this.lblPlaceLegajo_Click);
            // 
            // lblPlaceDocumento
            // 
            this.lblPlaceDocumento.AutoSize = true;
            this.lblPlaceDocumento.BackColor = System.Drawing.Color.Transparent;
            this.lblPlaceDocumento.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblPlaceDocumento.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlaceDocumento.Location = new System.Drawing.Point(73, 48);
            this.lblPlaceDocumento.Name = "lblPlaceDocumento";
            this.lblPlaceDocumento.Size = new System.Drawing.Size(59, 12);
            this.lblPlaceDocumento.TabIndex = 5;
            this.lblPlaceDocumento.Text = "[Documento]";
            this.lblPlaceDocumento.Visible = false;
            this.lblPlaceDocumento.Click += new System.EventHandler(this.lblPlaceDocumento_Click);
            // 
            // lblPlaceNombre
            // 
            this.lblPlaceNombre.AutoSize = true;
            this.lblPlaceNombre.BackColor = System.Drawing.Color.Transparent;
            this.lblPlaceNombre.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblPlaceNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlaceNombre.Location = new System.Drawing.Point(73, 71);
            this.lblPlaceNombre.Name = "lblPlaceNombre";
            this.lblPlaceNombre.Size = new System.Drawing.Size(44, 12);
            this.lblPlaceNombre.TabIndex = 6;
            this.lblPlaceNombre.Text = "[Nombre]";
            this.lblPlaceNombre.Visible = false;
            this.lblPlaceNombre.Click += new System.EventHandler(this.lblPlaceNombre_Click);
            // 
            // lblPlaceApellido
            // 
            this.lblPlaceApellido.AutoSize = true;
            this.lblPlaceApellido.BackColor = System.Drawing.Color.Transparent;
            this.lblPlaceApellido.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblPlaceApellido.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlaceApellido.Location = new System.Drawing.Point(73, 94);
            this.lblPlaceApellido.Name = "lblPlaceApellido";
            this.lblPlaceApellido.Size = new System.Drawing.Size(44, 12);
            this.lblPlaceApellido.TabIndex = 7;
            this.lblPlaceApellido.Text = "[Apellido]";
            this.lblPlaceApellido.Visible = false;
            this.lblPlaceApellido.Click += new System.EventHandler(this.lblPlaceApellido_Click);
            // 
            // lblPlaceUpcode
            // 
            this.lblPlaceUpcode.BackColor = System.Drawing.Color.White;
            this.lblPlaceUpcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPlaceUpcode.Controls.Add(this.label4);
            this.lblPlaceUpcode.Location = new System.Drawing.Point(12, 172);
            this.lblPlaceUpcode.Name = "lblPlaceUpcode";
            this.lblPlaceUpcode.Size = new System.Drawing.Size(64, 69);
            this.lblPlaceUpcode.TabIndex = 8;
            this.lblPlaceUpcode.Visible = false;
            this.lblPlaceUpcode.Click += new System.EventHandler(this.lblPlaceUpcode_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label4.Location = new System.Drawing.Point(10, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "upcode";
            this.label4.Click += new System.EventHandler(this.lblPlaceUpcode_Click);
            // 
            // lblPlaceFoto
            // 
            this.lblPlaceFoto.BackColor = System.Drawing.Color.White;
            this.lblPlaceFoto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPlaceFoto.Controls.Add(this.label5);
            this.lblPlaceFoto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblPlaceFoto.Location = new System.Drawing.Point(82, 152);
            this.lblPlaceFoto.Name = "lblPlaceFoto";
            this.lblPlaceFoto.Size = new System.Drawing.Size(93, 89);
            this.lblPlaceFoto.TabIndex = 9;
            this.lblPlaceFoto.Visible = false;
            this.lblPlaceFoto.Click += new System.EventHandler(this.lblPlaceFoto_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "foto";
            this.label5.Click += new System.EventHandler(this.lblPlaceFoto_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.lblUbicarItem);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.numTop);
            this.groupBox2.Controls.Add(this.numLeft);
            this.groupBox2.Location = new System.Drawing.Point(215, 190);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(92, 111);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ubicar Item";
            // 
            // numLeft
            // 
            this.numLeft.Location = new System.Drawing.Point(30, 58);
            this.numLeft.Maximum = new decimal(new int[] {
            3075,
            0,
            0,
            0});
            this.numLeft.Name = "numLeft";
            this.numLeft.Size = new System.Drawing.Size(56, 20);
            this.numLeft.TabIndex = 0;
            this.numLeft.ValueChanged += new System.EventHandler(this.numPlace_ValueChanged);
            // 
            // numTop
            // 
            this.numTop.Location = new System.Drawing.Point(30, 84);
            this.numTop.Maximum = new decimal(new int[] {
            4897,
            0,
            0,
            0});
            this.numTop.Name = "numTop";
            this.numTop.Size = new System.Drawing.Size(56, 20);
            this.numTop.TabIndex = 0;
            this.numTop.ValueChanged += new System.EventHandler(this.numPlace_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "X";
            // 
            // lblUbicarItem
            // 
            this.lblUbicarItem.AutoSize = true;
            this.lblUbicarItem.Location = new System.Drawing.Point(10, 29);
            this.lblUbicarItem.Name = "lblUbicarItem";
            this.lblUbicarItem.Size = new System.Drawing.Size(51, 13);
            this.lblUbicarItem.TabIndex = 1;
            this.lblUbicarItem.Text = "(ninguno)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Y";
            // 
            // Back
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.Name = "Back";
            this.Size = new System.Drawing.Size(731, 409);
            this.Load += new System.EventHandler(this.Back_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Back_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelImagen.ResumeLayout(false);
            this.panelImagen.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.lblPlaceUpcode.ResumeLayout(false);
            this.lblPlaceUpcode.PerformLayout();
            this.lblPlaceFoto.ResumeLayout(false);
            this.lblPlaceFoto.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTop)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblUbicarItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numTop;
        private System.Windows.Forms.NumericUpDown numLeft;
        private System.Windows.Forms.Panel panelImagen;
        private System.Windows.Forms.Panel lblPlaceFoto;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel lblPlaceUpcode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblPlaceApellido;
        private System.Windows.Forms.Label lblPlaceNombre;
        private System.Windows.Forms.Label lblPlaceDocumento;
        private System.Windows.Forms.Label lblPlaceLegajo;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox chkUpcode;
        private System.Windows.Forms.CheckBox chkFoto;
        private System.Windows.Forms.CheckBox chkApellido;
        private System.Windows.Forms.CheckBox chkNombre;
        private System.Windows.Forms.CheckBox chkDocumento;
        private System.Windows.Forms.CheckBox chkLegajo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
    }
}