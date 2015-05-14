CREATE UNIQUE NONCLUSTERED INDEX [IX_PARENTI03] ON [ope.ope_even_11_last_vehicle_event] (RELA_PARENTI03, OPEEVEN11_tipo_EVENTO) INCLUDE (RELA_OPEEVEN01)

CREATE INDEX [IX_PARENTI03_INICIO] ON [ope.ope_posi_07_cab_datamart] (RELA_PARENTI03, OPEPOSI07_INICIO, OPEPOSI07_FIN) INCLUDE (ID_OPEPOSI07) WITH (SORT_IN_TEMPDB = ON, ONLINE = ON)
CREATE INDEX [IX_PARENTI09_INICIO] ON [ope.ope_posi_07_cab_datamart] (RELA_PARENTI09, OPEPOSI07_INICIO, OPEPOSI07_FIN) INCLUDE (ID_OPEPOSI07) WITH (SORT_IN_TEMPDB = ON, ONLINE = ON)

DROP INDEX [IX_parenti03_inicio_includes_3] ON [ope.ope_posi_07_cab_datamart]
DROP INDEX [IX_parenti03_inicio_includes_6] ON [ope.ope_posi_07_cab_datamart]
DROP INDEX [IX_Empleado_Fecha] ON [ope.ope_posi_07_cab_datamart]
DROP INDEX [_dta_index_ope.ope_posi_07_cab_datamart_5_1758629308__K3_K11_K12_K14_K1_4_5_6_7_8_9_10_13_15_16_17_18_19012345] ON [ope.ope_posi_07_cab_datamart]
DROP INDEX [_dta_index_ope.ope_posi_07_cab_datamart_5_1758629308__K5_K11_K1_3_4_6_7_8_9_10_12_13_14_15_16_17_18_19012345] ON [ope.ope_posi_07_cab_datamart]
DROP INDEX [_dta_index_ope.ope_posi_07_cab_datamart_5_1758629308__K9_K11_9987] ON [ope.ope_posi_07_cab_datamart]