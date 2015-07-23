#region Usings

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LinqToExcel;
using Logictracker.DatabaseTracer.Core;
using Logictracker.Types.BusinessObjects.Components;
using Logictracker.Types.BusinessObjects.Dispositivos;
using Logictracker.Types.BusinessObjects.Documentos;
using Logictracker.Types.BusinessObjects.Vehiculos;
using Logictracker.Types.ImportadorObjects;
using Logictracker.Web.BaseClasses.BasePages;
using Logictracker.Culture;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.ReferenciasGeograficas;
using Logictracker.Web.CustomWebControls.Labels;
using System.Collections.Generic;
using Logictracker.DAL.Factories;

#endregion

namespace Logictracker.Parametrizacion
{   
    public partial class ParametrizacionImportData : ApplicationSecuredPage
    {
        #region Private Const Properties

        /// <summary>
        /// File expected message variable name.
        /// </summary>
        private const string FileExpected = "FILE_EXPECTED";

        /// <summary>
        /// File format message variable name.
        /// </summary>
        private const string FileFormat = "FILE_FORMAT";

        /// <summary>
        /// Incopatible file extension message variable name.
        /// </summary>
        private const string IncompatibleExtension = "INCOMPATIBLE_EXTENSION";

        /// <summary>
        /// KML files mime T.
        /// </summary>
        private const string KmlMimeType = "application/vnd.google-earth.kml+xml";

        /// <summary>
        /// Pois import ok message variable name.
        /// </summary>
        private const string PoiImportOk = "POI_IMPORT_OK";

        /// <summary>
        /// Excel Type
        /// </summary>
        private const string XlsType = "application/vnd.ms-excel";

        #endregion

        #region Protected Properties

        /// <summary>
        /// Get label error message.
        /// </summary>
        protected override InfoLabel LblInfo { get { return infoLabel1; } }

        #endregion

        #region Geocercas Import

        private bool CheckXlsFile()
        {
            if (!fuImportData.HasFile)
            {
                LblInfo.Text = CultureManager.GetError(FileExpected);
                return false;
            }
            if (fuImportData.PostedFile.ContentType == XlsType || 
                String.Compare(Path.GetExtension(fuImportData.PostedFile.FileName), ".XLS", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }
            LblInfo.Text = string.Format(CultureManager.GetError(IncompatibleExtension), "XLS");
            return false;
        }

        private bool CheckKmlFile()
        {
            if (!fuImportData.HasFile)
            {
                LblInfo.Text = CultureManager.GetError(FileExpected);
                return false;
            }
            if (fuImportData.PostedFile.ContentType == KmlMimeType ||
                String.Compare(Path.GetExtension(fuImportData.PostedFile.FileName), ".KML", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }
            LblInfo.Text = string.Format(CultureManager.GetError(IncompatibleExtension), "KML");
            return false;
        }
        /// <summary>
        /// Imports the specified geocercas using the selected filter values.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImportarGeocercas_Click(object sender, EventArgs e)
        {
            if (CheckKmlFile())
                ImportGeocercas();
                
        }

        /// <summary>
        /// Saves as geocercas all items read from the specified file.
        /// </summary>
        private void ImportGeocercas()
        {
            var fileName = string.Concat(Server.MapPath(TmpDir), DateTime.Now.ToString("yyyyMMdd-HHmmss"), ".xml");

            try
            {
                fuImportData.SaveAs(fileName);

                var tipoGeocerca = DAOFactory.TipoReferenciaGeograficaDAO.FindById(ddlTipoGeocerca.Selected);
                var linea = DAOFactory.LineaDAO.FindById(ddlBaseGeocerca.Selected);
                var kml = XDocument.Load(fileName);
                
                /* Debo obtener el namspace */


                XNamespace ns = "http://earth.google.com/kml/2.2";
                
                var features = from feature in kml.Descendants(ns+"Placemark") select feature;

                foreach (var feature in features)
                {

                    var descriptionNode = feature.Descendants(ns + "description").FirstOrDefault();
                    if (descriptionNode == null)
                        descriptionNode = feature.Descendants(ns + "SimpleData").FirstOrDefault(x => x.Attribute("name") != null &&  x.Attribute("name").Value == "description");
                    var descripcion = descriptionNode == null? "N/D" : descriptionNode.Value;

                    var poligono = feature.Descendants(ns+"Polygon").First();

                    var area = ((XElement)poligono.Descendants(ns + "LinearRing").First().FirstNode).Value.Replace("\r", "").Replace("\n", "").Replace("\t", "");

                    var puntos = (from punto in area.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                  select new PointF
                                             {
                                                 X = (float)Convert.ToDouble(punto.Split(',')[0], CultureInfo.InvariantCulture),
                                                 Y = (float)Convert.ToDouble(punto.Split(',')[1], CultureInfo.InvariantCulture)
                                             }).ToList();
                
                    var geocerca = new ReferenciaGeografica
                                       {
                                           Codigo = Guid.NewGuid().ToString("N"),
                                           Descripcion = descripcion,
                                           Color = tipoGeocerca.Color,
                                           EsFin = tipoGeocerca.EsFin,
                                           EsInicio = tipoGeocerca.EsInicio,
                                           EsIntermedio = tipoGeocerca.EsIntermedio,
                                           InhibeAlarma = tipoGeocerca.InhibeAlarma,
                                           Empresa = linea.Empresa, 
                                           Linea = linea,
                                           TipoReferenciaGeografica = tipoGeocerca,
                                           Vigencia = new Vigencia {Inicio = DateTime.UtcNow }
                                       };

                    var points = new Poligono();

                    points.AddPoints(puntos.Count > 100 ? Poligono.Reduce(puntos) : puntos);
                
                    geocerca.Historia.Add(
                        new HistoriaGeoRef
                            {
                                ReferenciaGeografica = geocerca,
                                Poligono = points,
                                Vigencia = new Vigencia {Inicio = DateTime.UtcNow }
                            }
                        );

                    DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(geocerca);
                    STrace.Trace("QtreeReset", "ImportData 1");
                }

                infoLabel1.Mode = InfoLabelMode.INFO;
                infoLabel1.Text = CultureManager.GetSystemMessage(PoiImportOk);
            }
            catch (Exception e) { ShowError(e); }
            finally { File.Delete(fileName); }
        }

        #endregion

        #region Points Import

        /// <summary>
        /// Interest points data import.
        /// </summary>
        private void ImportPuntosDeInteres()
        {
            var fileName = string.Concat(Server.MapPath(TmpDir), DateTime.Now.ToString("yyyyMMdd-HHmmss"), ".xml");

            try
            {
                fuImportData.SaveAs(fileName);

                var tipoPunto = DAOFactory.TipoReferenciaGeograficaDAO.FindById(ddlTipoDomicilio.Selected);
                var linea = DAOFactory.LineaDAO.FindById(ddlBase.Selected);
                var kml = XDocument.Load(fileName);

                //Reads interest points data from givven kml file.
                var features = from feature in kml.Descendants("Placemark")
                               select new
                                          {
                                              Description = ((XElement) feature.FirstNode).Value,
                                              Longitude = Convert.ToDouble(((XElement) feature.LastNode).Value.Split(',')[0], CultureInfo.InvariantCulture),
                                              Latitude = Convert.ToDouble(((XElement) feature.LastNode).Value.Split(',')[1], CultureInfo.InvariantCulture)
                                          };

                //Generate interest points based on the information readed from the giveen file.
                foreach (var feature in features)
                {
                    var posicion = new Direccion
                                       {
                                           Altura = -1,
                                           IdMapa = -1,
                                           Provincia = string.Empty,
                                           IdCalle = -1,
                                           IdEsquina = -1,
                                           IdEntrecalle = -1,
                                           Latitud = feature.Latitude,
                                           Longitud = feature.Longitude,
                                           Partido = string.Empty,
                                           Pais = string.Empty,
                                           Calle = string.Empty,
                                           Descripcion = feature.Description,
                                           Vigencia = new Vigencia { Inicio = DateTime.UtcNow }
                                       };

                    //Constructs the point.
                    var puntoDeInteres = new ReferenciaGeografica
                                             {
                                                 Descripcion = feature.Description,
                                                 Empresa = linea.Empresa, 
                                                 Linea = linea,
                                                 EsFin = tipoPunto.EsFin,
                                                 EsInicio = tipoPunto.EsInicio,
                                                 EsIntermedio = tipoPunto.EsIntermedio,
                                                 InhibeAlarma = tipoPunto.InhibeAlarma,
                                                 TipoReferenciaGeografica = tipoPunto,
                                                 Vigencia = new Vigencia {Inicio = DateTime.UtcNow },
                                                 Icono = tipoPunto.Icono
                                             };

                    puntoDeInteres.Historia.Add(new HistoriaGeoRef
                                                    {
                                                        ReferenciaGeografica = puntoDeInteres,
                                                        Direccion = posicion,
                                                        Vigencia = new Vigencia { Inicio = DateTime.UtcNow },
                                                    }
                        );


                    DAOFactory.ReferenciaGeograficaDAO.SingleSaveOrUpdate(puntoDeInteres);
                    STrace.Trace("QtreeReset", "ImportData 2");
                }

                infoLabel1.Mode = InfoLabelMode.INFO;
                infoLabel1.Text = CultureManager.GetSystemMessage(PoiImportOk);
            }
            catch(Exception e) { ShowError(e); }
            finally { File.Delete(fileName); }
        }

        /// <summary>
        /// Interest points import button click. Checks wither if there is un uploaded file and if
        /// its T is the expected one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImportarPuntosDeInteres_Click(object sender, EventArgs e)
        {
            if (fuImportData.HasFile)
                if (fuImportData.PostedFile.ContentType == KmlMimeType) ImportPuntosDeInteres();
                else LblInfo.Text = string.Format(CultureManager.GetError(IncompatibleExtension), "KML");
            else LblInfo.Text = CultureManager.GetError(FileExpected);
        }

        /// <summary>
        /// Hides or show diameter controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cbGeocercas_CheckedChanged(object sender, EventArgs e) { npRadio.Enabled = cbGeocercas.Checked; }

        #endregion

        #region Employees Import

        /// <summary>
        /// Downloads the FormularioEmpleados.xls Template.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnTemplateEmpleado_Click(object sender, EventArgs e)
        {
            //Set the appropriate ContentType.
            Response.ContentType = "application/vnd.ms-excel";
        
            Response.AppendHeader("content-disposition", "attachment; filename=FormularioEmpleados.xls");

            //Write the file directly to the HTTP content output stream.
            Response.WriteFile(MapPath("ImportadorMasivo/Templates/FormularioEmpleados.xls"));

            Response.End();
        }

        /// <summary>
        /// Opens the Employees Help Window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnHelpEmpleado_Click(object sender, EventArgs e)
        {
            OpenWin("ImportadorMasivo/Help/HelpEmpleado.aspx", "Help Empleado", 250, 450);
        }

        /// <summary>
        /// Click import Employees button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImportarChoferes_Click(object sender, EventArgs e)
        {
            if (CheckXlsFile())
                ImportEmployees();
            }

        /// <summary>
        /// Employees massive import procedure using LinqToExcel
        /// </summary>
        private void ImportEmployees()
        {
            var fileName = string.Concat(Server.MapPath(TmpDir), DateTime.Now.ToString("yyyyMMdd-HHmmss"), ".xls");

            try
            {
                fuImportData.SaveAs(fileName);

                var empresa = DAOFactory.EmpresaDAO.FindById(ddlLocation.Selected);
                var linea = ddlPlanta.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlPlanta.Selected) : null;
                var tipo = ddlTipoEmpleado.Selected > 0 ? DAOFactory.TipoEmpleadoDAO.FindById(ddlTipoEmpleado.Selected) : null;
                var file = new ExcelQueryFactory(fileName);
                var data = from c in file.Worksheet<EmpleadoImportador>("Importar")
                           select c;
                
                int contadorImportados = 0;
                int contadorRepetidos = 0;

                List<string> listPatentesInexistentes = new List<string>();


                foreach (var empleadoImportador in data)
                {
                    var empleado = CreateEmployee(empleadoImportador, empresa, linea, tipo);
                    if (empleado.Legajo != null)
                    {
                        var empleadoDuplicado = DAOFactory.EmpleadoDAO.FindByLegajo(empresa.Id, linea != null ? linea.Id : -1, empleado.Legajo);

                        if (empleadoDuplicado == null)
                        {
                            if ( empleadoImportador.Transportista != null)
                            {
                                var transportista = DAOFactory.TransportistaDAO.FindByCodigo(empresa.Id, linea!=null? linea.Id:-1, empleadoImportador.Transportista);
                                
                                
                                empleado.Transportista = transportista;

                            }

                            if (empleadoImportador.Categoria != null)
                            {
                                var categoria = DAOFactory.CategoriaAccesoDAO.FindByName(new []{ empresa.Id }, new [] {linea != null ? linea.Id : -1}, empleadoImportador.Categoria);
                                empleado.Categoria = categoria;

                            }
                         
                            DAOFactory.EmpleadoDAO.SaveOrUpdate(empleado); //So id doesn't insert empty rows

                            if (empleadoImportador.Patente != string.Empty)
                            {
                                var vehiculo = DAOFactory.CocheDAO.FindByPatente(empresa.Id, empleadoImportador.Patente);
                                if (vehiculo != null)
                                {
                                    vehiculo.Chofer = empleado;
                                    DAOFactory.CocheDAO.SaveOrUpdate(vehiculo);
                                }
                                else
                                {
                                    listPatentesInexistentes.Add(empleadoImportador.Patente);
                                }

                            }
                                
                            contadorImportados++;

                           
                        }
                        
                        if (empleadoDuplicado != null)
                        {
                            if (empleadoImportador.Patente != string.Empty)
                            {
                                var vehiculo = DAOFactory.CocheDAO.FindByPatente(empresa.Id, empleadoImportador.Patente);
                                if (vehiculo != null)
                                {
                                    vehiculo.Chofer = empleadoDuplicado;
                                    DAOFactory.CocheDAO.SaveOrUpdate(vehiculo);
                                }
                                else
                                {
                                    listPatentesInexistentes.Add(empleadoImportador.Patente);
                                }

                            }
                            if (empleadoImportador.Transportista != null)
                            {
                                var transportista = DAOFactory.TransportistaDAO.FindByCodigo(empresa.Id, linea != null ? linea.Id : -1, empleadoImportador.Transportista);


                                empleado.Transportista = transportista;

                            }

                            contadorRepetidos++;

                        }
                        
                    }

                    var exportarTxt = chkExportarTxt.Checked;

                    if (exportarTxt)
                    {
                        if (listPatentesInexistentes.Count > 0)
                        {
                            ExportarInexistentes(listPatentesInexistentes);
                        }
                    }
 
               } 

                infoLabel1.Mode = InfoLabelMode.INFO;
                infoLabel1.Text = @"Se ha finalizado con exito la importacion de Empleados. " + " (Importados:" + contadorImportados.ToString() + " Duplicados:" + contadorRepetidos.ToString()+ ")";
            }
            catch (Exception e) { ShowError(new Exception(CultureManager.GetError(FileFormat), e)); }
            finally { File.Delete(fileName); }
        }

        private void ExportarInexistentes(List<string> listaRepetidos)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Users\\Matias\\Desktop\\noEncontrados.txt",false);

            foreach (string aux in listaRepetidos)
            {
                
                file.WriteLine(aux);

            }

            file.Close();
        
        }

        /// <summary>
        /// CreateGte a new Empleado Object with the employee values
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="tipo"></param>
        private static Empleado CreateEmployee(EmpleadoImportador employee, Empresa empresa, Linea linea, TipoEmpleado tipo)
        {
            var e = new Empleado();

            var entidad = new Entidad 
                              {
                                  Apellido = employee.NombreApellido,
                                  NroDocumento = employee.NroDoc
                              };

            e.Empresa = empresa;
            e.Linea = linea;
            e.TipoEmpleado = tipo;
            e.Mail = employee.Mail ?? String.Empty;
            e.Legajo = employee.Legajo;
            e.Entidad = entidad;
         
            return e;
        }

        #endregion

        #region Mobiles Import

        protected void btnTemplateVehiculo_Click(object sender, EventArgs e)
        {
            //Set the appropriate ContentType.
            Response.ContentType = "application/vnd.ms-excel";

            Response.AppendHeader("content-disposition", "attachment; filename=FormularioVehiculos.xls");

            //Write the file directly to the HTTP content output stream.
            Response.WriteFile(MapPath("ImportadorMasivo/Templates/FormularioVehiculos.xls"));

            Response.End();
        }

        protected void btnHelpVehiculo_Click(object sender, EventArgs e) { OpenWin("ImportadorMasivo/Help/HelpVehiculo.aspx", "Help Vehiculo", 250, 450); }

        /// <summary>
        /// Mobile import button click. Checks wither if there is un uploaded file and if
        /// its T is the expected one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImportarVehiculos_Click(object sender, EventArgs e)
        {
            if (CheckXlsFile())
                ImportMobiles();
        }

        /// <summary>
        /// Mobile massive import procedure using LinqToExcel
        /// </summary>
            private void ImportMobiles()
        {
            string fileName = null;
            int contador = 0;
            try
            {
                if (ddlPlantaVehiculos.Selected <= 0) ThrowMustEnter("PARENTI02");

                if(ddlTipoVehiculo.Selected <= 0) ThrowMustEnter("PARENTI17");


                fileName = string.Concat(Server.MapPath(TmpDir), DateTime.Now.ToString("yyyyMMdd-HHmmss"), ".xls");

            
                fuImportData.SaveAs(fileName);

                var linea = DAOFactory.LineaDAO.FindById(ddlPlantaVehiculos.Selected);
                var tipo = DAOFactory.TipoCocheDAO.FindById(ddlTipoVehiculo.Selected);
                var distrito = DAOFactory.EmpresaDAO.FindById(ddlLocacionVehiculos.Selected);

                var file = new ExcelQueryFactory(fileName);
                var mobilesList = file.Worksheet("Importar").ToList();
                

                for (int i = 0; i < mobilesList.Count; i++)
                {
                    var aMobile = new Coche();

                    aMobile.Empresa = distrito;
                    aMobile.Linea = linea;
                    aMobile.TipoCoche = tipo;
                    aMobile.Interno = mobilesList[i][0].ToString();
                    aMobile.Patente = mobilesList[i][1].ToString();
                    aMobile.Marca = DAOFactory.MarcaDAO.GetByDescripcion(ddlDistrito.Selected, ddlBase.Selected,mobilesList[i][2].ToString());
                    //aMobile.Marca = DAOFactory.MarcaDAO.FindById(int.Parse(mobilesList[i][2].ToString()));
                    aMobile.Modelo = DAOFactory.ModeloDAO.FindByDescripcion(ddlDistrito.Selected,-1,mobilesList[i][3].ToString());
                    //aMobile.Modelo = DAOFactory.ModeloDAO.FindByCodigo(ddlDistrito.Selected,ddlBase.Selected,mobilesList[i][3].ToString());                   
                    short anio;
                    aMobile.AnioPatente = short.TryParse(mobilesList[i][4],out anio) ? anio : (short)0;
                    aMobile.ModeloDescripcion=aMobile.Modelo!=null?aMobile.Modelo.Descripcion:string.Empty;
                    aMobile.Poliza = aMobile.Poliza ?? string.Empty;
                    

                    var byInterno = DAOFactory.CocheDAO.FindByInterno(ddlLocacionVehiculos.SelectedValues, ddlPlantaVehiculos.SelectedValues, aMobile.Interno);
                    var duplicado = byInterno != null && byInterno.Id != aMobile.Id;
                    if (duplicado) continue;

                    if (aMobile.Interno != null && aMobile.Patente != null) DAOFactory.CocheDAO.SaveOrUpdate(aMobile); //So it doesn't insert empty rows

                    contador++;
                }
    

                //var data = from c in file.Worksheet<Coche>("Importar")select c;
            
                //foreach (var mobile in data)
                //{
                //    SetMobileDefaultValues(mobile, linea, tipo);

                //    var byInterno = DAOFactory.CocheDAO.FindByInterno(new[] {-1}, ddlPlantaVehiculos.SelectedValues, mobile.Interno);
                //    var duplicado = byInterno != null && byInterno.Id != mobile.Id;
                //    if (duplicado) continue;

                //    if (mobile.Interno != null && mobile.Patente != null) DAOFactory.CocheDAO.SaveOrUpdate(mobile); //So it doesn't insert empty rows
                //}

                infoLabel1.Mode = InfoLabelMode.INFO;
                infoLabel1.Text = @"Se ha finalizado con exito la importacion de:"+contador +" Vehiculos con internos no repetidos.";
            }
            catch (Exception e) {ShowError(new Exception(CultureManager.GetError(FileFormat), e)); }
            finally { if(!string.IsNullOrEmpty(fileName)) File.Delete(fileName); }
        }

         private static void SetMobileDefaultValues(Coche mobile, Linea linea, TipoCoche tipo)
        {
            mobile.Empresa = linea.Empresa;
            mobile.Linea = linea;
            mobile.TipoCoche = tipo;
            mobile.Marca.Descripcion = mobile.Marca != null ? mobile.Marca.Descripcion : String.Empty;
            mobile.ModeloDescripcion = mobile.Modelo != null ? mobile.Modelo.Descripcion : String.Empty;
            mobile.NroChasis = mobile.NroChasis ?? String.Empty;
            mobile.NroMotor = mobile.NroMotor ?? String.Empty;
            mobile.Poliza = mobile.Poliza ?? String.Empty;
        }

        #endregion

        #region Cards Import

        protected void btnTemplateTarjeta_Click(object sender, EventArgs e)
        {
            //Set the appropriate ContentType.
            Response.ContentType = "application/vnd.ms-excel";

            Response.AppendHeader("content-disposition", "attachment; filename=FormularioTarjetas.xls");

            //Write the file directly to the HTTP content output stream.
            Response.WriteFile(MapPath("ImportadorMasivo/Templates/FormularioTarjetas.xls"));

            Response.End();
        }

        protected void btnHelpTarjeta_Click(object sender, EventArgs e) { OpenWin("ImportadorMasivo/Help/HelpTarjeta.aspx", "Help Tarjeta", 250, 450); }

        /// <summary>
        /// Card import button click. Checks wither if there is un uploaded file and if
        /// its T is the expected one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImportarTarjetas_Click(object sender, EventArgs e)
        {
            if (CheckXlsFile())
                ImportCards();
        }

        /// <summary>
        /// Card massive import procedure using LinqToExcel
        /// </summary>
        private void ImportCards()
        {
            var fileName = string.Concat(Server.MapPath(TmpDir), DateTime.Now.ToString("yyyyMMdd-HHmmss"), ".xls");

            try
            {
                fuImportData.SaveAs(fileName);

                var linea = ddlBaseTarjeta.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBaseTarjeta.Selected) : null;
                var empresa = ddlDistritoTarjeta.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlDistritoTarjeta.Selected) : linea != null ? linea.Empresa : null;
            
                var file = new ExcelQueryFactory(fileName);
                var data = from c in file.Worksheet<TarjetaImportador>("Importar") select c;
            
                foreach (var cardImp in data)
                {
                    var card = SetCardDefaultValues(cardImp, empresa, linea);

                    if (string.IsNullOrEmpty(card.Numero) || string.IsNullOrEmpty(card.Pin)) continue;

                    var chofer = !string.IsNullOrEmpty(cardImp.Legajo) ? DAOFactory.EmpleadoDAO.GetByLegajo(ddlDistritoTarjeta.Selected, ddlBaseTarjeta.Selected, cardImp.Legajo.Trim()) : null;

                    if (chofer != null)
                    {
                        chofer.Tarjeta = card;
                        DAOFactory.EmpleadoDAO.SaveOrUpdate(chofer);
                    }

                    else DAOFactory.TarjetaDAO.SaveOrUpdate(card);
                }

                infoLabel1.Mode = InfoLabelMode.INFO;
                infoLabel1.Text = @"Se ha finalizado con exito la importacion de Tarjetas.";
            }
            catch (Exception e) { ShowError(new Exception(CultureManager.GetError(FileFormat), e)); }
            finally { File.Delete(fileName); }
        }

        /// <summary>
        /// Sets the Card default values
        /// </summary>
        /// <param name="card"></param>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        private static Tarjeta SetCardDefaultValues(TarjetaImportador card, Empresa empresa, Linea linea)
        {
            return new Tarjeta
                       {
                           Numero = card.Numero.Trim() ?? string.Empty,
                           Pin = card.Pin.Trim() ?? string.Empty,
                           Linea = linea,
                           Empresa = empresa
                       };
        }

        #endregion

        #region Device Import

        protected void btnTemplateDispositivo_Click(object sender, EventArgs e)
        {
            //Set the appropriate ContentType.
            Response.ContentType = "application/vnd.ms-excel";

            Response.AppendHeader("content-disposition", "attachment; filename=FormularioDispositivos.xls");

            //Write the file directly to the HTTP content output stream.
            Response.WriteFile(MapPath("ImportadorMasivo/Templates/FormularioDispositivos.xls"));

            Response.End();
        }
        protected void btnTemplateDocumento_Click(object sender, EventArgs e)
        {
            //Set the appropriate ContentType.
            Response.ContentType = "application/vnd.ms-excel";

            Response.AppendHeader("content-disposition", "attachment; filename=FormularioDocumentos.xls");

            //Write the file directly to the HTTP content output stream.
            Response.WriteFile(MapPath("ImportadorMasivo/Templates/FormularioDocumentos.xls"));

            Response.End();
        }

        protected void btnHelpDispositivo_Click(object sender, EventArgs e) { OpenWin("ImportadorMasivo/Help/HelpDispositivo.aspx", "Help Dispositivo", 250, 450); }
        
        protected void btnHelpDocumento_Click(object sender, EventArgs e) { OpenWin("ImportadorMasivo/Help/HelpDocumento.aspx", "Help Documento", 250, 450); }

        /// <summary>
        /// Card import button click. Checks wither if there is un uploaded file and if
        /// its T is the expected one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImportarDispositivo_Click(object sender, EventArgs e)
        {
            if (CheckXlsFile())
                ImportDevices();
        }

        protected void btnImportarDocumento_Click(object sender, EventArgs e)
        {
            if (CheckXlsFile())
                ImportDocuments();
        }


        /// <summary>
        /// Card massive import procedure using LinqToExcel
        /// </summary>
        private void ImportDevices()
        {
            var fileName = string.Concat(Server.MapPath(TmpDir), DateTime.Now.ToString("yyyyMMdd-HHmmss"), ".xls");

            try
            {
                fuImportData.SaveAs(fileName);

                var linea = ddlBaseDispositivo.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBaseDispositivo.Selected) : null;
                var empresa = ddlDistritoDispositivo.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlDistritoDispositivo.Selected) : linea != null ? linea.Empresa : null;
                var tipo = DAOFactory.TipoDispositivoDAO.FindById(ddlTipoDispositivo.Selected);
            
                var file = new ExcelQueryFactory(fileName);
                var data = from c in file.Worksheet<DispositivoImportador>("Importar") select c;

                foreach (var dispImp in data)
                {
                    if (!TieneDatos(dispImp)) continue;

                    var device = SetDeviceDefaultValues(dispImp, empresa, linea, tipo);
                    var coche = !string.IsNullOrEmpty(dispImp.Interno) && linea != null
                                    ? DAOFactory.CocheDAO.GetByInterno(new[] {ddlDistritoDispositivo.Selected},
                                                                       new[] {ddlBaseDispositivo.Selected},
                                                                       dispImp.Interno)
                                    : null;
                    
                    if (coche != null)
                    {
                        coche.Dispositivo = device;

                        DAOFactory.CocheDAO.SaveOrUpdate(coche);
                    }
                    else DAOFactory.DispositivoDAO.SaveOrUpdate(device);

                    if (device.IdNum == 0) device.IdNum = device.Id;

                    DAOFactory.DispositivoDAO.SaveOrUpdate(device);
                }

                infoLabel1.Mode = InfoLabelMode.INFO;
                infoLabel1.Text = @"Se ha finalizado con exito la importacion de Dispositivos.";
            }
            catch (Exception e) { ShowError(new Exception(CultureManager.GetError(FileFormat), e)); }
            finally { File.Delete(fileName); }
        }

        //IMPORTADOR DE DOCUMENTOS
        
        private void ImportDocuments()
        {
            var fileName = string.Concat(Server.MapPath(TmpDir), DateTime.Now.ToString("yyyyMMdd-HHmmss"), ".xls");
            var contadorDocumentos = 0;

            try
            {
                fuImportData.SaveAs(fileName);

                var linea = ddlBaseDocumento.Selected > 0 ? DAOFactory.LineaDAO.FindById(ddlBaseDocumento.Selected) : null;
                var empresa = ddlDistritoDocumento.Selected > 0 ? DAOFactory.EmpresaDAO.FindById(ddlDistritoDocumento.Selected) : linea != null ? linea.Empresa : null;
                var tipo = DAOFactory.TipoDocumentoDAO.FindById(ddlTipoDocumento.Selected);

                var file = new ExcelQueryFactory(fileName);
                var data = from c in file.Worksheet<DocumentoImportador>("Importar") select c;

                string listVehiculosInexistentes = string.Empty;

                foreach (var docImp in data)
                {
                    if (!TieneDatos(docImp)) continue;

                    var document = SetDocumentDefaultValues(docImp, empresa, linea, tipo);

                    var vehiculo = DAOFactory.CocheDAO.FindByPatente(empresa.Id, docImp.PatenteVehiculo);

                    if (vehiculo != null)
                    {
                        document.Vehiculo = vehiculo;
                    }
                    else
                    {
                        var nuevaPatente = docImp.PatenteVehiculo.Replace(" ",string.Empty);
                        var vehiculo2 = DAOFactory.CocheDAO.FindByPatente(empresa.Id, nuevaPatente);
                        
                        if(vehiculo2 != null)
                        {
                            document.Vehiculo = vehiculo2;
                        }
                        else
                        {
                            if (listVehiculosInexistentes != String.Empty)
                            {
                                listVehiculosInexistentes = listVehiculosInexistentes + ", ";
                            }

                            listVehiculosInexistentes  = listVehiculosInexistentes + docImp.PatenteVehiculo;
                        }

                    }

                   // DAOFactory.DocumentoDAO.SaveOrUpdate(document);

                    contadorDocumentos++;
                }

                infoLabel1.Mode = InfoLabelMode.INFO;

                if (listVehiculosInexistentes != string.Empty)
                {
                    infoLabel1.Text = @"- Se ha finalizado con éxito la importación de " + contadorDocumentos.ToString() +
                                      " Documentos. <br> <br> - Vehículos inexistentes en la plataforma: " +
                                      listVehiculosInexistentes;
                }
                else
                {
                    infoLabel1.Text = @"- Se ha finalizado con éxito la importación de " + contadorDocumentos.ToString() +
                                      " Documentos.";
                }


            }
            catch (Exception e) { ShowError(new Exception(CultureManager.GetError(FileFormat), e)); }
            finally { File.Delete(fileName); }
        }


        private static bool TieneDatos(DispositivoImportador disp)
        {
            return (!string.IsNullOrEmpty(disp.Codigo) && !string.IsNullOrEmpty(disp.Clave) && disp.PollInterval > 0 && !string.IsNullOrEmpty(disp.Imei)
                    && !string.IsNullOrEmpty(disp.IpAdress) && disp.Port > 0 && !string.IsNullOrEmpty(disp.Telefono));
        }

        private static bool TieneDatos(DocumentoImportador doc)
        {
            return (!string.IsNullOrEmpty(doc.CodigoDocumento) && !string.IsNullOrEmpty(doc.DescripcionDocumento) && (doc.VencimientoDocumento.HasValue) && !string.IsNullOrEmpty(doc.PatenteVehiculo));

        }
    
        /// <summary>
        /// Sets the Card default values
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="empresa"></param>
        /// <param name="linea"></param>
        /// <param name="tipo"></param>
        private static Dispositivo SetDeviceDefaultValues(DispositivoImportador dev, Empresa empresa, Linea linea, TipoDispositivo tipo)
        {
            return new Dispositivo
                       {
                           Codigo = dev.Codigo.Trim(),
                           TipoDispositivo = tipo,
                           PollInterval = dev.PollInterval,
                           Port = dev.Port,
                           Estado = 0,
                           Imei = dev.Imei.Trim(),
                           Tablas = "0",
                           Clave = dev.Clave.Trim(),
                           Empresa = empresa,
                           Linea = linea,
                           Telefono = dev.Telefono.Trim()
                       };
        }

        private static Documento SetDocumentDefaultValues(DocumentoImportador doc, Empresa empresa, Linea linea, TipoDocumento tipo)
        {
            return new Documento
            {
                TipoDocumento = tipo,
                Empresa = empresa,
                Linea = linea,
                Codigo = doc.CodigoDocumento,
                Descripcion = doc.DescripcionDocumento,
                Vencimiento = doc.VencimientoDocumento.Value, 
                Presentacion = doc.PresentacionDocumento,
                FechaAlta = DateTime.UtcNow
                
            };
        }

        #endregion

        #region Security

        /// <summary>
        /// Get security refference.
        /// </summary>
        /// <returns></returns>
        protected override string GetRefference() { return "IMPORTDATA"; }

        #endregion

        #region Temp Dir Generation

        /// <summary>
        /// Checks that the temporary directory exists.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack) return;

            var dir = Server.MapPath(TmpDir);

            if (Directory.Exists(dir)) return;

            Directory.CreateDirectory(dir);
        }

        #endregion
    }

}