using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TMAPIv4
{
	public class TemplateRevision
	{
		protected JObject backing;
		protected Template template;
		
		/// <summary>
		/// Contains the revision's Data Description, which controls the form interface 
		/// created by TaguchiMail to edit activities using this template. This document 
		/// indirectly determines the structure of the source document used by the 
		/// template's stylesheet.
		/// </summary>
		public string Format
		{
			get { return (string)backing["format"]; }
			set { backing["format"] = new JValue(value); }
		}
		
		/// <summary>
		/// Contains the revision XSLT stylesheet. This is normally created within the 
		/// TaguchiMail UI, and is designed to work with the XML documents created by 
		/// the activity edit interface; these are created based on the format field, 
		/// which defines allowable data types and document structure.
		/// </summary>
		public string Content
		{
			get { return (string)backing["content"]; }
			set { backing["format"] = new JValue(value); }
		}
		
		/// <summary>
		/// ID of the template revision record in the database.
		/// </summary>
		public string RecordId
		{
			get { return backing["id"].ToString(); }
		}
		
		/// <summary>
		/// Create a new template revision, given a parent Template, a format document
		/// (aka Data Description), and a content document (XSL stylesheet).
		/// </summary>
		/// <param name="template">
		/// A <see cref="Template"/>
		/// </param>
		/// <param name="format">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="content">
		/// A <see cref="System.String"/>
		/// </param>
		public TemplateRevision (Template template, string format, string content)
		{
			this.backing = new JObject();
			this.template = template;
			this.Format = format;
			this.Content = content;
		}
		
		public TemplateRevision (Template template, JObject revision)
		{
			this.backing = revision;
			this.template = template;
		}
	}
	
	public class Template : Record
	{
		protected JArray existingRevisions;
		
		public Template (Context context) : base(context)
		{
			this.resourceType = "template";
		}
		
		/// <summary>
		/// ID of the TaguchiMail template record.
		/// </summary>
		public string RecordId
		{
			get { return backing["id"].ToString(); }
		}
		
		/// <summary>
		/// External ID/reference, intended to store external application primary keys.
		/// 
		/// If not null, this field must be unique.
		/// </summary>
		public string Ref
		{
			get { return (string)backing["ref"]; }
			set { backing["ref"] = new JValue(value); }
		}
		
		/// <summary>
		/// Template name
		/// </summary>
		public string Name
		{
			get { return (string)backing["name"]; }
			set { backing["name"] = new JValue(value); }
		}
		
		/// <summary>
		/// This is matched up with Activity types to determine what
		/// to show the user in the Suggested Templates/Other Templates sections
		/// </summary>
		public string Type
		{
			get { return (string)backing["type"]; }
			set { backing["type"] = new JValue(value); }
		}
		
		/// <summary>
		/// This can be any application-defined value
		/// </summary>
		public string Subtype
		{
			get { return (string)backing["subtype"]; }
			set { backing["subtype"] = new JValue(value); }
		}
		
		/// <summary>
		/// Arbitrary application XML data store.
		/// </summary>
		public string XmlData
		{
			get { return (string)backing["data"]; }
			set { backing["data"] = new JValue(value); }
		}
		
		/// <summary>
		/// Template status; leave null if not used
		/// </summary>
		public string Status
		{
			get { return (string)backing["status"]; }
		}
		
		/// <summary>
		/// Latest template revision content. If set, a new revision will be created upon template create/update.
		/// </summary>
		public TemplateRevision LatestRevision
		{
			get 
			{
				if (((JArray)backing["revisions"]).Count > 0) 
				{
					return new TemplateRevision(this, (JObject)backing["revisions"][0]);
				}
				else if (existingRevisions.Count > 0)
				{
					return new TemplateRevision(this, (JObject)existingRevisions[0]);
				}
				else
				{
					return null;
				}
			}
			set 
			{
				JObject revision = new JObject();
				revision["content"] = new JValue(value.Content);
				revision["format"] = new JValue(value.Format);
				if (((JArray)backing["revisions"]).Count > 0) 
				{
					backing["revisions"][0] = revision;
				}
				else
				{
					((JArray)backing["revisions"]).Add(revision);
				}
			}
		}
		
		/// <summary>
		/// Save this template to the TaguchiMail database.
		/// </summary>
		public override void Update()
		{
			base.Update();
			// need to move the existing revisions to avoid re-creating the same
			// ones if this object is saved again
			this.existingRevisions = (JArray)this.backing["revisions"];
			this.backing["revisions"] = new JArray();
		}
		
		/// <summary>
		/// Create this template in the TaguchiMail database.
		/// </summary>
		public override void Create()
		{
			base.Create();
			// need to move the existing revisions to avoid re-creating the same
			// ones if this object is saved again
			this.existingRevisions = (JArray)this.backing["revisions"];
			this.backing["revisions"] = new JArray();
		}
		
		/// <summary>
		/// Retrieve a single Template based on its TaguchiMail identifier.
		/// </summary>
		/// <param name="context">
		/// A <see cref="Context"/> object determining the TM instance and organization to query.
		/// </param>
		/// <param name="recordId">
		/// A <see cref="System.String"/> containing the template's unique TaguchiMail identifier.
		/// </param>
		/// <returns>
		/// The <see cref="Template"/> with that ID.
		/// </returns>
		public static Template Get(Context context, string recordId, Dictionary<string, string> parameters)
		{
			JArray results = JArray.Parse(context.MakeRequest("template", "GET", recordId, null, parameters, null));
			Template rec = new Template(context);
			rec.backing = (JObject)results[0];
			rec.existingRevisions = (JArray)rec.backing["revisions"];
			// clear out existing revisions so they're not sent back to the server on update
			rec.backing["revisions"] = new JArray(); 
			return rec;
		}
		
		/// <summary>
		/// Retrieve a single Template based on its TaguchiMail identifier, with its latest revision content.
		/// </summary>
		/// <param name="context">
		/// A <see cref="Context"/> object determining the TM instance and organization to query.
		/// </param>
		/// <param name="recordId">
		/// A <see cref="System.String"/> containing the template's unique TaguchiMail identifier.
		/// </param>
		/// <returns>
		/// The <see cref="Template"/> with that ID.
		/// </returns>
		public static Template GetWithContent(Context context, string recordId, Dictionary<string, string> parameters)
		{
			Dictionary<string, string> newParams = new Dictionary<string, string>(parameters);
			newParams.Add("revisions", "latest");
			return Template.Get(context, recordId, newParams);
		}
		
		/// <summary>
		/// Retrieve a list of Templates based on a query.
		/// </summary>
		/// <param name="context">
		/// A <see cref="Context"/> object determining the TM instance and organization to query.
		/// </param>
		/// <param name="sort">
		/// A <see cref="System.String"/> indicating which of the record's fields should be used to
		/// sort the output.
		/// </param>
		/// <param name="order">
		/// A <see cref="System.String"/> containing either 'asc' or 'desc', indicating whether the
		/// result list should be returned in ascending or descending order. 
		/// </param>
		/// <param name="offset">
		/// A <see cref="System.Int32"/> indicating the index of the first record to be returned in
		/// the list.
		/// </param>
		/// <param name="limit">
		/// A <see cref="System.Int32"/> indicating the maximum number of records to return.
		/// </param>
		/// <param name="query">
		/// An <see cref="System.String[]"/> of query predicates, each of the form:
		/// [field]-[operator]-[value]
		/// where [field] is one of the defined resource fields, [operator] is one of the below-listed comparison operators, 
		/// and [value] is a string value to which the field should be compared.
		/// 
		/// Supported operators:
		/// * eq: mapped to SQL '=', tests for equality between [field] and [value] (case-sensitive for strings);
		/// * neq: mapped to SQL '!=', tests for inequality between [field] and [value] (case-sensitive for strings);
		/// * lt: mapped to SQL '&lt;', tests if [field] is less than [value];
		/// * gt: mapped to SQL '&gt;', tests if [field] is greater than [value];
		/// * lte: mapped to SQL '&lt;=', tests if [field] is less than or equal to [value];
		/// * gte: mapped to SQL '&gt;=', tests if [field] is greater than or equal to [value];
		/// * re: mapped to PostgreSQL '~', interprets [value] as a POSIX regular expression and tests if [field] matches it;
		/// * rei: mapped to PostgreSQL '~*', performs a case-insensitive POSIX regular expression match;
		/// * like: mapped to SQL 'LIKE' (case-sensitive);
		/// * is: mapped to SQL 'IS', should be used to test for NULL values in the database as [field]-eq-null is always false;
		/// * nt: mapped to SQL 'IS NOT', should be used to test for NOT NULL values in the database as [field]-neq-null is always false.
		/// </param>
		/// <returns>
		/// A <see cref="Template[]"/> matching the query.
		/// </returns>
		public static Template[] Find(Context context, string sort, string order, int offset, int limit, string[] query)
		{
			Dictionary<string,string> parameters = new Dictionary<string, string>();
			parameters["sort"] = sort;
			parameters["order"] = order;
			parameters["offset"] = offset.ToString();
			parameters["limit"] = limit.ToString();
			
			JArray results = JArray.Parse(context.MakeRequest("template", "GET", null, null, parameters, query));
			List<Template> recordSet = new List<Template>();
			foreach (var result in results)
			{
				Template rec = new Template(context);
				rec.backing = (JObject)result;
				rec.existingRevisions = (JArray)rec.backing["revisions"];
				// clear out existing revisions so they're not sent back to the server on update
				rec.backing["revisions"] = new JArray(); 
				recordSet.Add(rec);
			}
			return recordSet.ToArray();
		}
	}
}

