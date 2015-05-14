/**
 * @(#)Position.java
 *
 *
 * @author 
 * @version 1.00 2008/11/20
 */

package etao.comm;

public class Position
{

	public int id;             //!< ID de dispositivo 
	public String timestamp;   //!< Hora UTC de la toma
	public double lat;         //!< Latitud
	public double lon;         //!< Longitud
	public double vel;         //!< Velocidad
	public int rum;            //!< Rumbo de Deriva 
	
	public Position(){}
	
}
