using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Urbetrack.Postal.GPSAPI;

namespace Urbetrack.Postal.Forms
{
    public partial class GPS : Form
    {
        public Urbetrack.Postal.GPSAPI.GPSHandler glbGPS = new GPSHandler(60);

        public GPSPosition lastPosition;
        public bool gotFix;

        public System.DateTime Time;
        private bool odd;
        private int count;

        public void ST(string msg)
        {
            try {
            StatusLabel.Text = msg;
            StatusLabel.Update();
            StatusLabel.Refresh();
                }
            catch
            {
                
            }
        }


        public GPS()
        {
            InitializeComponent();
        }

        private void btCancelar_Click(object sender, EventArgs e)
        {
            gotFix = false;   
            this.Close();

        }

        private void btAceptar_Click(object sender, EventArgs e)
        {
            if (gotFix) this.Close();
        }

        private void ticking() 
        {
            try
            {
                if (!glbGPS.IsOpen())
                {
                    ST("No esta abierto el GPS!!!");
                }

                GPSPosition Position = glbGPS.GetPosition();

                if ((Position != null))
                {
                    //HDOPLabel.Text = Position.flHorizontalDilutionOfPrecision.ToString("0.00");
                    //ErrorLabel.Text = (Position.flHorizontalDilutionOfPrecision * 6) + " m";
                    //SatelitesLabel.Text = Position.dwSatelliteCount.ToString();

                    //if (Position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_LONGITUDE))
                    //{
                    //    LongLabel.Text = Position.dblLongitude.ToString("0.0000000");
                    //}
                    //else
                    //{
                    //    LongLabel.Text = "";
                    //}

                    //if (Position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_LATITUDE))
                    //{
                    //    LatLabel.Text = Position.dblLatitude.ToString("0.0000000");
                    //}
                    //else
                    //{
                    //    LatLabel.Text = "";
                    //}

                    //try
                    //{
                    //    if (Position.isValidField(GPSPosition.GPS_VALID_FIELDS.GPS_VALID_UTC_TIME))
                    //    {
                    //        StampLabel.Text = Position.Time.ToString("yyyy/MM/dd HH:mm");
                    //    }
                    //    else
                    //    {
                    //        StampLabel.Text = "";
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    StampLabel.Text = ex.ToString(); //Position.Time.ToString();
                    //}

                    //switch (Position.fixType)
                    //{
                    //    case GPSFixType.Unknown:
                    //        FixTypeLabel.Text = "";
                    //        break;
                    //    case GPSFixType.XyD:
                    //        FixTypeLabel.Text = "2D";
                    //        break;
                    //    case GPSFixType.XyzD:
                    //        FixTypeLabel.Text = "3D";
                    //        break;
                    //}

                    //switch (Position.fixQuality)
                    //{
                    //    case GPSFixQuality.DGps:
                    //        FixTypeLabel.Text += " DGPS";
                    //        break;
                    //    case GPSFixQuality.Gps:
                    //        FixTypeLabel.Text += " GPS";
                    //        break;
                    //}

lastPosition = Position;

                    if (glbGPS.IsPositionValid(Position))
                    {
                        pnlFixed.BackColor = Color.LimeGreen;
                        StatusLabel.BackColor = Color.LimeGreen;

                        count++;
                        ST(String.Concat("Posicion obtenida ", count.ToString()));

                        if (count==3) {

                            gotFix = true;
                            btAceptar.Enabled = true;
                            Time = DateTime.Now;
                            //cierro porque ya tengo fix
                            timer1.Enabled = false;
                            this.Close();
                        }
                    }
                    else
                    {
                        count = 0;
                        pnlFixed.BackColor = Color.OrangeRed;
                        StatusLabel.BackColor = Color.OrangeRed;

                        if (!odd)
                        {
                            odd = true;
                            ST("Esperando posición");
                        }
                        else
                        {
                            odd = false;
                            ST("Buscando satélites");
                        }
                        gotFix = false;
                        btAceptar.Enabled = false;
                    }
                }
                else
                {
                    //GrabarButton.Enabled = False
                    pnlFixed.BackColor = Color.Red;
                }

            }
            catch (Exception ex)
            {
                ST(ex.Message.ToString());
            }
        }


        private void timer1_Tick_1(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            ticking();
            if (!gotFix) timer1.Enabled = true;
        }

        private void GPS_Load(object sender, EventArgs e)
        {
            count = 0;
            glbGPS.Open();
            timer1.Enabled = true;
        }

        private void pnlFixed_GotFocus(object sender, EventArgs e)
        {

        }       

    }
}