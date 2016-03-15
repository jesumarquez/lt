Feature: Ordenes

Scenario:REQ 10 – Filtro de Ordenes por criterios
	Los criterios a utilizar para filtrar las órdenes a visualizar son:
	Distrito (único), Base (único) ,Transportista (múltiple) , Entrega (único) , Fecha Desde (inclusivo) , Fecha Hasta (inclusivo).
	Todos los valores de los filtros deben tener en cuenta el usuario de la plataforma

	Given Estoy logeado 
	When  Voy a la pagina ordenes
	Then veo los filtros
	| filtro        |
	| Distrito      |
	| Base          |
	| Transportista |
	| Entrega       |
	| Fecha Desde   |
	| Fecha Hasta   |
	# TODO: test valores unicos para usuario

Scenario: REQ 20 – Visualización de la Grilla
	En la grilla se visualizarán los siguientes campos que conforman una orden:
	Empresa, Transportista, Código Entrega, Razón Social, Código Pedido, Fecha y Hora del pedido (Formato DD/MM HH:MM)
	Cada orden además mostrará una grilla hija con los siguientes campos:
		Producto, Litros,  ajuste realizado 
	El ajuste por omisión es 0.

	Given Estoy logeado
	When  Voy a la pagina ordenes
	#And filtro 'distrito' con valor 'Petrobras'
	And filtro 'fecha desde' con valor '7/3/2016' 
	And filtro 'fecha hasta' con valor '8/3/2016'
	And presiono buscar
	Then Veo una grilla con las columnas:
	 | Columna        |
	 | Empresa        |
	 | Transportista  |
	 | Código Entrega |
	 | Razón Social   |
	 | Código Pedido  |
	 | Pedido         |
	#And Veo una grilla con las columnas:
	# | Columna  |
	# | Producto |
	# | Litros   |
	
# TODO: Validar valores de la grilla hija

