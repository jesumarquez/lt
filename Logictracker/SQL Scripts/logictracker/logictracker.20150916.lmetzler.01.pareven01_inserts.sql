USE [logictracker]
GO

insert into pareven01
select	NULL as 'rela_parenti02', 0 as 'pareven01_destino', 
		'1000' as 'pareven01_codigo', 
		'Estado Logistico Cumplido Manual' as 'pareven01_descrip', 
		'Estado Logistico Manual' as 'pareven01_mensaje',
		0 as 'pareven01_origen', 8 as 'pareven01_ttl', 0 as 'pareven01_baja', 0 as 'pareven01_acceso', 
		2 as 'rela_parenti16', 0 as 'pareven01_esalarma', 86 as 'rela_parenti11', 8458 as 'pareven01_revision',
		NULL as 'rela_parenti01', 0 as 'pareven01_es_solo_rta'

insert into pareven01
select	NULL as 'rela_parenti02', 0 as 'pareven01_destino', 
		'1009' as 'pareven01_codigo', 
		'Estado Logistico Cumplido Entrada' as 'pareven01_descrip', 
		'Estado Logistico Entrada' as 'pareven01_mensaje',
		0 as 'pareven01_origen', 8 as 'pareven01_ttl', 0 as 'pareven01_baja', 0 as 'pareven01_acceso', 
		2 as 'rela_parenti16', 0 as 'pareven01_esalarma', 86 as 'rela_parenti11', 8458 as 'pareven01_revision',
		NULL as 'rela_parenti01', 0 as 'pareven01_es_solo_rta'

insert into pareven01
select	NULL as 'rela_parenti02', 0 as 'pareven01_destino', 
		'1010' as 'pareven01_codigo', 
		'Estado Logistico Cumplido Salida' as 'pareven01_descrip', 
		'Estado Logistico Salida' as 'pareven01_mensaje',
		0 as 'pareven01_origen', 8 as 'pareven01_ttl', 0 as 'pareven01_baja', 0 as 'pareven01_acceso', 
		2 as 'rela_parenti16', 0 as 'pareven01_esalarma', 86 as 'rela_parenti11', 8458 as 'pareven01_revision',
		NULL as 'rela_parenti01', 0 as 'pareven01_es_solo_rta'

insert into pareven01
select	NULL as 'rela_parenti02', 0 as 'pareven01_destino', 
		'1013' as 'pareven01_codigo', 
		'Estado Logistico Cumplido Manual Realizado' as 'pareven01_descrip', 
		'Estado Logistico Realizado' as 'pareven01_mensaje',
		0 as 'pareven01_origen', 8 as 'pareven01_ttl', 0 as 'pareven01_baja', 0 as 'pareven01_acceso', 
		2 as 'rela_parenti16', 0 as 'pareven01_esalarma', 86 as 'rela_parenti11', 8458 as 'pareven01_revision',
		NULL as 'rela_parenti01', 0 as 'pareven01_es_solo_rta'

insert into pareven01
select	NULL as 'rela_parenti02', 0 as 'pareven01_destino', 
		'1014' as 'pareven01_codigo', 
		'Estado Logistico Cumplido Manual No Realizado' as 'pareven01_descrip', 
		'Estado Logistico No Realizado' as 'pareven01_mensaje',
		0 as 'pareven01_origen', 8 as 'pareven01_ttl', 0 as 'pareven01_baja', 0 as 'pareven01_acceso', 
		2 as 'rela_parenti16', 0 as 'pareven01_esalarma', 86 as 'rela_parenti11', 8458 as 'pareven01_revision',
		NULL as 'rela_parenti01', 0 as 'pareven01_es_solo_rta'