using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TMAPIv4
{
	public class Campaign : Record
	{
		public Campaign (Context context) : base(context)
		{
			this.resourceType = "campaign";
		}
		
		/// <summary>
		/// ID of the TaguchiMail campaign record.
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
		/// Campaign name
		/// </summary>
		public string Name
		{
			get { return (string)backing["name"]; }
			set { backing["name"] = new JValue(value); }
		}
		
		/// <summary>
		/// Date/time at which this campaign started (or is scheduled to start)
		/// </summary>
		public string StartDateTime
		{
			get { return (string)backing["date"]; }
			set { backing["date"] = new JValue(value); }
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
		/// Campaign status; leave null if not used
		/// </summary>
		public string Status
		{
			get { return (string)backing["status"]; }
		}
		
		/// <summary>
		/// Retrieve a single Campaign based on its TaguchiMail identifier.
		/// </summary>
		/// <param name="context">
		/// A <see cref="Context"/> object determining the TM instance and organization to query.
		/// </param>
		/// <param name="recordId">
		/// A <see cref="System.String"/> containing the list's unique TaguchiMail identifier.
		/// </param>
		/// <returns>
		/// The <see cref="Campaign"/> with that ID.
		/// </returns>
		public static Campaign Get(Context context, string recordId, Dictionary<string, string> parameters)
		{
			JArray results = JArray.Parse(context.MakeRequest("campaign", "GET", recordId, null, parameters, null));
			Campaign rec = new Campaign(context);
			rec.backing = (JObject)results[0];
			return rec;
		}
		
		/// <summary>
		/// Retrieve a list of Campaigns based on a query.
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
		/// A <see cref="Campaign[]"/> matching the query.
		/// </returns>
		public static Campaign[] Find(Context context, string sort, string order, int offset, int limit, string[] query)
		{
			Dictionary<string,string> parameters = new Dictionary<string, string>();
			parameters["sort"] = sort;
			parameters["order"] = order;
			parameters["offset"] = offset.ToString();
			parameters["limit"] = limit.ToString();
			
			JArray results = JArray.Parse(context.MakeRequest("campaign", "GET", null, null, parameters, query));
			List<Campaign> recordSet = new List<Campaign>();
			foreach (var result in results)
			{
				Campaign rec = new Campaign(context);
				rec.backing = (JObject)result;
				recordSet.Add(rec);
			}
			return recordSet.ToArray();
		}
	}
}

