#region Usings

using System;
using System.IO;
using System.Text;
using System.Xml;
using Logictracker.DAL.DAO.BaseClasses;
using Logictracker.Types.BusinessObjects;
using Logictracker.Types.BusinessObjects.Auditoria;
using Logictracker.Types.InterfacesAndBaseClasses;
using NHibernate;

#endregion

namespace Logictracker.DAL.DAO.BusinessObjects.Auditoria
{
    public class EntityAuditDAO : GenericDAO<EntityAudit>
    {
        #region Constructor

        /// <summary>
        /// Instanciates a new data access class using the provided nhibernate sessions.
        /// </summary>
        /// <param name="session"></param>
//        public EntityAuditDAO(ISession session) : base(session) { }

        #endregion

        public void AuditSave(object newObj, object oldObj, Usuario user)
        {

            var auditObj = GenerateSaveAuditObject(newObj, oldObj, user);

            var o = newObj as IAuditable;

            if (auditObj == null || o == null) return;

            auditObj.AuditedId = o.Id;

            SaveOrUpdate(auditObj);
        }

        public void AuditDelete(object obj, Usuario user)
        {
            var auditObj = GenerateDeleteAuditObject(obj, user);

            SaveOrUpdate(auditObj);
        }

        #region Private Methods

        private static EntityAudit GenerateDeleteAuditObject(object obj, Usuario user)
        {
            var o = obj as IAuditable;

            if (o == null) return null;

            var entityHasChanged = false;

            return new EntityAudit
                {
                    Fecha = DateTime.UtcNow,
                    Usuario = user,
                    AuditedId = o.Id,
                    Tabla = o.TypeOf().ToString(),
                    Data = GenerateXMLAuditDocument(obj,null,ref entityHasChanged,true)
                };

        }

        private static EntityAudit GenerateSaveAuditObject(object newObj, object oldObj, Usuario user)
        {
            var newObject = newObj as IAuditable;

            if (newObject == null) return null;

            var entityHasChanged = false;

            var auditObj = new EntityAudit
            {
                Fecha = DateTime.UtcNow,
                Usuario = user,
                AuditedId = newObject.Id,
                Tabla = newObject.TypeOf().ToString(),
                Data = GenerateXMLAuditDocument(newObj, oldObj, ref entityHasChanged, false)
            };

            return entityHasChanged ? auditObj : null;
        }

        private static XmlDocument GenerateXMLAuditDocument(object newObj, object oldObj, ref bool hasChanged, bool isDelete)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var xmlData = new XmlTextWriter(sw);
            xmlData.WriteStartDocument();
            xmlData.WriteStartElement("audit");

            if(!isDelete) xmlData = GenerateXMLSaveContent(oldObj, newObj, ref hasChanged, xmlData);

            xmlData.WriteEndElement();
            xmlData.WriteEndDocument();
            xmlData.Flush();
            xmlData.Close();

            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(sb.ToString());
            return xmldoc;
        }

        private static XmlTextWriter GenerateXMLSaveContent(object oldObj, object newObj, ref bool hasChanged, XmlTextWriter xmlWriter)
        {
            hasChanged = false;

            if (oldObj == null)
            {
                hasChanged = true;
                foreach (var prop in newObj.GetType().GetProperties())
                    GeneratePropertyXML(xmlWriter, prop.ToString(), String.Empty, prop.GetValue(newObj, null) ?? String.Empty);
            }
            else
            {
                foreach (var prop in newObj.GetType().GetProperties())
                {
                    var oldValue = prop.GetValue(oldObj, null);
                    var newValue = prop.GetValue(newObj, null);

                    if ((oldValue == null && newValue == null) || (oldValue != null && oldValue.Equals(newValue))) continue;

                    hasChanged = true;

                    var oldVal = oldValue as IAuditable;
                    var newVal = newValue as IAuditable;

                    GeneratePropertyXML(xmlWriter, prop.ToString(),
                            oldVal != null ? oldVal.Id : oldValue ?? String.Empty,
                            newVal != null ? newVal.Id : newValue ?? String.Empty);
                }
            }
            return xmlWriter;
        }

        private static void GeneratePropertyXML(XmlTextWriter xmlWriter, string propertyName, object oldValue, object newValue)
        {
            xmlWriter.WriteStartElement("property");

            xmlWriter.WriteAttributeString("Name", propertyName);
            xmlWriter.WriteStartElement("oldValue");
            xmlWriter.WriteAttributeString("value", oldValue.ToString());

            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("newValue");
            xmlWriter.WriteAttributeString("value", newValue.ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        #endregion
    }
}
