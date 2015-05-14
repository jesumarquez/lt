package etao.comm;
import java.util.Map;
import java.util.HashMap;
import java.net.URLEncoder;
import java.io.UnsupportedEncodingException;

public class Device
{
    public String type;        //!< Tipo de dispositivo.
    public Integer id;          //!< ID de dispositivo
    public String IMEI;     //!< IMEI del modulo GSM
    public String password; //!< Contraseña.
    public Map<String, String> UserSettings;

    public Device()
    {
    	UserSettings = new HashMap<String, String>();
    }
}
