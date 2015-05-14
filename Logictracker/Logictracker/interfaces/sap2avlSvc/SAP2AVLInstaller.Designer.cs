namespace sap2avlSvc
{
    partial class SAP2AVLInstaller
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
            this.sap2avlProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.sap2avlServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // sap2avlProcessInstaller
            // 
            this.sap2avlProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.sap2avlProcessInstaller.Password = null;
            this.sap2avlProcessInstaller.Username = null;
            // 
            // sap2avlServiceInstaller
            // 
            this.sap2avlServiceInstaller.Description = "Interface de importacion de maestros SAP para loma negra s.a.";
            this.sap2avlServiceInstaller.DisplayName = "Interfaz SAP a AVL (Lomax)";
            this.sap2avlServiceInstaller.ServiceName = "SAP2AVL";
            // 
            // SAP2AVLInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.sap2avlProcessInstaller,
            this.sap2avlServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller sap2avlProcessInstaller;
        private System.ServiceProcess.ServiceInstaller sap2avlServiceInstaller;
    }
}