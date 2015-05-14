/**
 * @(#)MessageIn.java
 *
 *
 * @author ev
 * @version 1.00 2008/11/20
 */
package etao.comm;

public class MessageIn
{
	public int id;          //!< ID de dispositivo 
	public Position pos;    //!< Posicion del Mensaje
	public int code;        //!< Codigo.
	public String message;  //!< Texto del mensaje.

	public MessageIn(){}
}
