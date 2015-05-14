using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LinqToExcel;
using Tarjetas;
using Tarjetas.Properties;

namespace TarjetasSAI
{
    public partial class Importador : Form
    {
        private bool hasFile;
        private bool hasFolder;
        private DataAccess data;

        public Importador()
        {
            InitializeComponent();
        }

        private void btCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lblFileName_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            lblFileName.Text = openFileDialog1.FileName;
            hasFile = true;
        }

        private void lblFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;
            lblFolder.Text = folderBrowserDialog1.SelectedPath;
            hasFolder = true;
        }

        private void btImportar_Click(object sender, EventArgs e)
        {
            lblResultado.Items.Clear();
            if (!hasFile || !hasFolder)
            {
                ShowResult("Seleccione el archivo de datos y la carpeta de imagenes");
                return;
            }

            var fileName = lblFileName.Text;
            var folder = lblFolder.Text;
            if (!folder.EndsWith("\\")) folder += "\\";

            try
            {
                var file = new ExcelQueryFactory(fileName);
                var datos = from c in file.Worksheet<empleadoImportador>("Empleados")
                           select c;

                var img = 0;
                var ok = 0;
                var mal = 0;
                foreach (var employee in datos)
                {
                    try
                    {
                        var emp = CreateEmployee(employee, folder);
                        ShowResult("OK LEGAJO " + employee.Legajo + (emp.foto == null ? "(SIN FOTO)" : "") + (emp.upcode == null ? " (SIN UPCODE)" : ""));
                        ok++;
                        if (emp.foto != null) img++;
                        data.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        ShowResult("ERROR EN LEGAJO " + employee.Legajo + ": " + ex.Message);
                        mal++;
                    }
                }
                
                ShowResult("");
                ShowResult("Empleados importados: " + ok + " (" + img + " con foto)");
                ShowResult("Empleados no importados: " + mal);
            }
            catch (Exception exc)
            {
                ShowResult(exc.Message);
            }
        }

        private empleado CreateEmployee(empleadoImportador empleado, string folder)
        {
            var img = new byte[0];
            var pictureFile = folder + empleado.Legajo + ".jpg";
            if (File.Exists(pictureFile)) img = GetPicture(pictureFile);//SquarePicture(File.ReadAllBytes(pictureFile));
            else pictureFile = null;

            var empresa = cbEmpresa.SelectedItem as empresa;
            var emp = data.GetEmpleadoByLegajo(empresa.id, empleado.Legajo);

            bool create = (emp == null);
            if (create) emp = new empleado { legajo = empleado.Legajo };

            emp.empresa = empresa.id;
            emp.apellido = empleado.Apellido;
            emp.nombre = empleado.Nombre;
            emp.documento = chkFormatDoc.Checked
                                ? FormatDocumento(empleado.Documento)
                                : empleado.Documento;
            emp.puesto = empleado.Puesto;
            emp.foto = pictureFile != null ? new Binary(img) : null;
            emp.editado = DateTime.Now;

            if (create)
            {
                var upcode = data.GetNextUpcode();

                if (upcode != null)
                {
                    emp.upcode = upcode.code;
                    emp.code = upcode.image;

                    upcode.used = true;
                }
                else
                {
                    ShowResult("No hay upcodes disponibles");
                }
            }

            if (create) data.AddEmpleado(emp);

            return emp;
        }
        public void ShowResult(string msg)
        {
            lblResultado.Items.Add(msg);
            lblResultado.SelectedIndex = lblResultado.Items.Count - 1;
        }
        private string FormatDocumento(string doc)
        {
            var res = "";
            var chars = 0;
            for(int i = doc.Length - 1; i >= 0; i--)
            {
                if (!char.IsNumber(doc[i])) return doc;
                if (chars == 3)
                {
                    res = "." + res;
                    chars = 0;
                }
                res = doc[i] + res;
                chars++;
            }
            return res;
        }

        private byte[] GetPicture(string pictureFile)
        {
            Bitmap bmp;
            using (Image im = Image.FromFile(pictureFile))
            {
                int max = Math.Max(im.Width, im.Height);
                int x = (im.Width == max) ? 0 : (max - im.Width)/2;
                int y = (im.Height == max) ? 0 : (max - im.Height)/2;

                bmp = new Bitmap(max, max);
                bmp.SetResolution(im.HorizontalResolution, im.VerticalResolution);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.White);
                g.DrawImage(im, x, y);
            }
            bmp.Save(pictureFile, ImageFormat.Jpeg);
            bmp.Dispose();
            
            /*using (Image im = Image.FromFile(pictureFile))
            {
                if (im.Width != im.Height)
                {
                    var max = Math.Max(im.Width, im.Height);
                    var im2 = im.Clone();
                    var gr = Graphics.FromImage(im);
                    
                    using ()
                    {
                        var g = Graphics.FromImage(im2);
                        g.Clear(Color.White);
                        g.DrawImage(im, (im2.Width - im.Width)/2, (im2.Height - im.Height)/2);
                        g.Save();
                        im2.Save(pictureFile, ImageFormat.Jpeg);
                    }
                }
            }*/
            GC.Collect();
            return File.ReadAllBytes(pictureFile);
            
        }

        private void btTemplate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            var filename = saveFileDialog1.FileName;

            using (Stream output = File.OpenWrite(filename))
            {
                output.Write(Resources.Empleados, 0, Resources.Empleados.Length);
            }
        }

        private void Importador_Load(object sender, EventArgs e)
        {
            if(data == null) data = new DataAccess();
            LoadEmpresas();
        }
        private void LoadEmpresas()
        {
            cbEmpresa.DataSource = data.GetAllEmpresas().ToList();
        }
    }
}
