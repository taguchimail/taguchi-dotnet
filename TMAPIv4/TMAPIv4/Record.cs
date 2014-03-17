
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TMAPIv4
{
	/// <summary>
	/// Base class for TM record types.
	/// </summary>
	public class Record
	{
		protected string resourceType; // No late static binding...
		protected Context context;
		protected JObject backing;
		
		/// <summary>
		/// Instantiate an empty Record object.
		/// </summary>
		/// <param name="context">
		/// A <see cref="Context"/> object determining the TM instance and organization to which
		/// the record belongs.
		/// </param>
		public Record(Context context)
		{
			this.context = context;
			this.backing = new JObject();
		}
		
		/// <summary>
		/// Retrieve a single Record based on its TaguchiMail identifier.
		/// </summary>
		/// <param name="resourceType">
		/// A <see cref="System.String"/> containing the resource type (passed in by subclass methods).
		/// </param>
		/// <param name="context">
		/// A <see cref="Context"/> object determining the TM instance and organization to query.
		/// </param>
		/// <param name="recordId">
		/// A <see cref="System.String"/> containing the record's unique TaguchiMail identifier.
		/// </param>
		/// <returns>
		/// The <see cref="Record"/> with that ID.
		/// </returns>
		public static Record Get(string resourceType, Context context, string recordId, Dictionary<string, string> parameters)
		{
			JArray results = JArray.Parse(context.MakeRequest(resourceType, "GET", recordId, null, parameters, null));
			Record rec = new Record(context);
			rec.backing = (JObject)results[0];
			return rec;
		}
		
		/// <summary>
		/// Retrieve a list of Records based on a query.
		/// </summary>
		/// <param name="resourceType">
		/// A <see cref="System.String"/> containing the resource type (passed in by subclass methods).
		/// </param>
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
		/// A <see cref="Record[]"/> matching the query.
		/// </returns>
		public static Record[] Find(string resourceType, Context context, string sort, string order, int offset, int limit, string[] query)
		{
			Dictionary<string,string> parameters = new Dictionary<string, string>();
			parameters["sort"] = sort;
			parameters["order"] = order;
			parameters["offset"] = offset.ToString();
			parameters["limit"] = limit.ToString();
			
			JArray results = JArray.Parse(context.MakeRequest(resourceType, "GET", null, null, parameters, query));
			List<Record> recordSet = new List<Record>();
			foreach (var result in results)
			{
				Record rec = new Record(context);
				rec.backing = (JObject)result;
				recordSet.Add(rec);
			}
			return recordSet.ToArray();
		}
		
		/// <summary>
		/// Save this record to the TaguchiMail database.
		/// </summary>
		public virtual void Update()
		{
			JArray data = new JArray();
			data.Add(this.backing);
			
			JArray results = JArray.Parse(context.MakeRequest(this.resourceType, "PUT", this.backing["id"].ToString(), data.ToString(), null, null));
			this.backing = (JObject)results[0];
		}
		
		/// <summary>
		/// Create this record in the TaguchiMail database.
		/// </summary>
		public virtual void Create()
		{
			JArray data = new JArray();
			data.Add(this.backing);
			
			JArray results = JArray.Parse(context.MakeRequest(this.resourceType, "POST", null, data.ToString(), null, null));
			this.backing = (JObject)results[0];
		}
	}
}
