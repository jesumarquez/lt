package etao.comm;

public class KeepAlive {
	public int id;						//!< Id del dispositivo
	public boolean GPSFixed;			//!< Indica si el dispositivo tiene fix gps.
	public boolean MainBattery;			//!< Indica si el dispositivo funciona con bateria principal o bateria interna.
	public boolean AudioOk;				//!< Indica si el circuito de audio funciona bien.
	public boolean PanicState;			//!< Indica si se ha presionado el boton de panico.
	public String PanicDateTime;		//!< Fecha/Hora en que se presion el boton de panico.
}
