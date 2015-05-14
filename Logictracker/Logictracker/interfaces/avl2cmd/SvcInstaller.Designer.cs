namespace avl2cmd
{
    partial class SvcInstaller
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
            this.AVL2CMDProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.AVL2CMDServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // AVL2CMDServiceInstaller
            // 
            this.AVL2CMDServiceInstaller.Description = "Interface de estados de camiones para Command Data";
            this.AVL2CMDServiceInstaller.DisplayName = "AVL a Command Data";
            this.AVL2CMDServiceInstaller.ServiceName = "ServiceAVL2CMD";
            this.AVL2CMDServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // AVL2CMDProcessInstaller
            // 
            this.AVL2CMDProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.AVL2CMDProcessInstaller.Password = null;
            this.AVL2CMDProcessInstaller.Username = null;
            // 
            // SvcInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.AVL2CMDProcessInstaller,
            this.AVL2CMDServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller AVL2CMDProcessInstaller;
        private System.ServiceProcess.ServiceInstaller AVL2CMDServiceInstaller;
    }
}