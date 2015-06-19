namespace HandlerTest.Controls
{
    partial class ucConfig
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtQueueName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtQueueType = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Queue";
            // 
            // txtQueueName
            // 
            this.txtQueueName.Location = new System.Drawing.Point(89, 22);
            this.txtQueueName.Name = "txtQueueName";
            this.txtQueueName.Size = new System.Drawing.Size(298, 20);
            this.txtQueueName.TabIndex = 15;
            this.txtQueueName.Text = ".\\private$\\eventos_trax";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Queue Type";
            // 
            // txtQueueType
            // 
            this.txtQueueType.Location = new System.Drawing.Point(89, 48);
            this.txtQueueType.Name = "txtQueueType";
            this.txtQueueType.Size = new System.Drawing.Size(298, 20);
            this.txtQueueType.TabIndex = 17;
            this.txtQueueType.Text = "msmq";
            // 
            // ucConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtQueueType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtQueueName);
            this.Name = "ucConfig";
            this.Size = new System.Drawing.Size(799, 444);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtQueueName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtQueueType;
    }
}
