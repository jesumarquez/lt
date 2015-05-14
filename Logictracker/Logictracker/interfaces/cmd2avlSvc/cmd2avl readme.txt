

INSTRUCCIONES DE CONFIGURACION DE CMD2AVL:


Primero se debe instalar en la maquina donde se ejecuta la boca de betonmatic el software adjunto
(CMD2AVL Installer.msi), y se lo instala preferiblemente con los parametros que por defecto 
esten configurados en la interface de instalacion.

Luego, para que la interface CMD2AVL pueda importar los ticketes a la base de datos del AVL, se deberan
configurar algunos parametros en la aplicacion necesarios para que el servicio sepa el origen y
destino de los datos a procesar.

Se debera editar el archivo de configuracion del servicio, que de haberse instalado con los parametros
por defecto sera: "C:\Program Files\Mapas y Sistemas\CMD2AVL Installer\cmd2avlSvc.exe.config"

Este archivo es un XML, y tiene 5 parametros que deberan configurarse para que las cosas funciones.

1- "connstr": Se trata de la cadena de conexion a la base de datos.

	Ejemplo:
		connstr = DATA SOURCE=200.5.235.220; INITIAL CATALOG=pruebainterfaces; PERSIST SECURITY INFO=true; USER ID=sa; PASSWORD=123456;
		
2- "hostid": Nombre de referencia de la maquina donde se instalo la aplicacion, es utilizado para 
             identificar el origen de los datos en la base, en caso de ser necesario.
		
	Ejemplo:
		hostid = Sola01
		
3- "log_file": A medida que se procesan los ticketes, se generara un log con la informacion de resultados
               del procesamiento, este parametro debe completarse con el nombre del archivo de dicho
               log. Se debe tener en cuenta que el directorio asignado debera tener permisos de FullControl 
               para el usuario NetworkSerivce. (Preferiblemente al usuario "Todos").

	Ejemplo:
		log_file = \\LOGSERVER\cmdavl\sola_cmdavl.log

4- "watch_path": Directorio donde la aplicacion debera esperar el archivo de ticketes.

	Ejemplo:
		watch_path = C:\
		
4- "watch_filter": Nombre del archivo de Ticketes.

	Ejemplo:
		watch_filter = Sola01Tickets.txt
		
Una vez configurados estos parametros, se debera ir a la administracion de servicios del sistema
e iniciar el servicio llamado: "CMD2AVL".





