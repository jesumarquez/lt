using System;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Windows.Forms;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;

namespace Tarjetas
{
    public partial class PrintView : Form
    {
        public TarjetaUnica oRpt = null;
        private DataAccess data = null;

        public PrintView(DataAccess data)
        {
            InitializeComponent();
            this.data = data;
            oRpt = new TarjetaUnica();
        }

        public void LoadData()
        {
            LoadBackground();

            SelectPrinter();

            var dataSet = new Imprimir();

            const string sql = "SELECT legajo, apellido, imprimir.nombre, documento, upcode, foto, code, [image] FROM imprimir, back where active = 1";

            var dataContext = data.DataContext;
            var dataAdapter = new SqlCeDataAdapter(sql, dataContext.Connection as SqlCeConnection);

            /* Llenas el Dataset*/
            dataAdapter.Fill(dataSet, "imprimir");

            if(dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            {
                MessageBox.Show("No hay tarjetas seleccionadas para imprimir");
                btCerrar_Click(null, null);
            }

            /* Llenas el Reporte con llo que tiene el DataSet */
            oRpt.SetDataSource(dataSet);

            /* Luego viualizas tu reporte en control CrystalReportViewer */
            crystalReportViewer1.ReportSource = oRpt;

            crystalReportViewer1.RefreshReport();

            HideTheTabControl();

            RefreshPageButtons();
        }

        private void LoadBackground()
        {
            var back = data.GetActiveBack();

            // Legajo
            if (back != null && !back.print_legajo)
            {
                HideReportObject("legajo1");
            }
            else
            {
                var left = back != null ? back.legajo_left : back.Legajo.Left;
                var top = back != null ? back.legajo_top : back.Legajo.Top;
                PositionReportObject("legajo1", left, top);
            }

            // Documento
            if (back != null && !back.print_documento) { HideReportObject("documento1"); }
            else
            {
                var left = back != null ? back.documento_left : back.Documento.Left;
                var top = back != null ? back.documento_top : back.Documento.Top;
                PositionReportObject("documento1", left, top);
            }

            // Upcode
            if (back != null && !back.print_upcode)
            {
                HideReportObject("code2");
                HideReportObject("upcode1");
            }
            else
            {
                var left = back != null ? back.upcode_left : back.Upcode.Left;
                var top = back != null ? back.upcode_top : back.Upcode.Top;
                PositionReportObject("code2", left, top);
                PositionReportObject("upcode1", left + back.Upcode.OffsetLabelLeft, top + back.Upcode.OffsetLabelTop);
            }

            // Foto
            if (back != null && !back.print_foto) { HideReportObject("foto1"); }
            else
            {
                var left = back != null ? back.foto_left : back.Foto.Left;
                var top = back != null ? back.foto_top : back.Foto.Top;
                PositionReportObject("foto1", left, top);
            }

            // Nombre
            if (back != null && !back.print_nombre) { HideReportObject("nombre1"); }
            else
            {
                var left = back != null ? back.nombre_left : back.Nombre.Left;
                var top = back != null ? back.nombre_top : back.Nombre.Top;
                PositionReportObject("nombre1", left, top);
            }

            // Apellido
            if (back != null && !back.print_apellido) { HideReportObject("apellido1"); }
            else
            {
                var left = back != null ? back.apellido_left : back.Apellido.Left;
                var top = back != null ? back.apellido_top : back.Apellido.Top;
                PositionReportObject("apellido1", left, top);
            }
        }

        private void SelectPrinter()
        {
            // 86mmx54mm Card
            try
            {
                if (oRpt.PrintOptions.PageContentHeight != 4915 || oRpt.PrintOptions.PageContentWidth != 3072)
                {
                    MessageBox.Show("El tamaño del papel no es correcto.", "Advertencia",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch(Exception ex)
            {
                
            }
        }
        public void HideTheTabControl()
        {
            foreach (Control c1 in crystalReportViewer1.Controls)
            {
                if (c1 is PageView)
                {
                    var pv = (PageView)c1;
                    foreach (Control c2 in pv.Controls)
                    {
                        var tc = c2 as TabControl;
                        if (tc != null)
                        {
                            tc.ItemSize = new Size(0, 1);
                            tc.SizeMode = TabSizeMode.Fixed;
                        }
                    }
                }
            }
        }
        private void PositionReportObject(string objectName, int left, int top)
        {
            oRpt.ReportDefinition.ReportObjects[objectName].Left = left;
            oRpt.ReportDefinition.ReportObjects[objectName].Top = top;
            oRpt.ReportDefinition.ReportObjects[objectName].ObjectFormat.EnableSuppress = false;
        }

        private void HideReportObject(string objectName)
        {
            oRpt.ReportDefinition.ReportObjects[objectName].ObjectFormat.EnableSuppress = true;
        }

        private void btCerrar_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btExportar_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() != DialogResult.OK) return;
            oRpt.ExportToDisk(ExportFormatType.PortableDocFormat, saveFileDialog1.FileName);
        }

        private void btStart_Click(object sender, EventArgs e)
        {
            crystalReportViewer1.ShowFirstPage();
            RefreshPageButtons();
        }

        private void btPrev_Click(object sender, EventArgs e)
        {
            crystalReportViewer1.ShowPreviousPage();
            RefreshPageButtons();
        }

        private void btNext_Click(object sender, EventArgs e)
        {
            crystalReportViewer1.ShowNextPage();
            RefreshPageButtons();
        }

        private void btEnd_Click(object sender, EventArgs e)
        {
            crystalReportViewer1.ShowLastPage();
            RefreshPageButtons();
        }
        private void RefreshPageButtons()
        {
            var page = crystalReportViewer1.GetCurrentPageNumber();                      
            var pagecount = ((PageView)crystalReportViewer1.Controls[0]).GetLastPageNumber();
            btStart.Enabled = btPrev.Enabled = page > 1;
            btEnd.Enabled = btNext.Enabled = page < pagecount;
        }
    }
}
