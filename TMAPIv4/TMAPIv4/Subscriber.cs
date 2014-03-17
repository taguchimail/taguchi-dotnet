using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TMAPIv4
{
	/// <summary>
	/// Subscriber records contain the details of an individual recipient of communcations 
	/// (email, SMS, and so forth); typically this includes one or more addresses along 
	/// with supporting information such as first and last names, date of birth, and 
	/// other user-defined data.
	/// </summary>
	public class Subscriber : Record
	{
		/// <summary>
		/// Instantiate an empty Subscriber object.
		/// </summary>
		/// <param name="context">
		/// A <see cref="Context"/> object determining the TM instance and organization to which
		/// the subscriber belongs.
		/// </param>
		public Subscriber(Context context) : base(context)
		{
			this.resourceType = "subscriber";
		}
		
		/// <summary>
		/// ID of the TaguchiMail record.
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
		/// Title (Mr, Mrs etc)
		/// </summary>
		public string Title
		{
			get { return (string)backing["title"]; }
			set { backing["title"] = new JValue(value); }
		}
		
		/// <summary>
		/// First (given) name
		/// </summary>
		public string FirstName
		{
			get { return (string)backing["firstname"]; }
			set { backing["firstname"] = new JValue(value); }
		}
		
		/// <summary>
		/// Last (family) name
		/// </summary>
		public string LastName
		{
			get { return (string)backing["lastname"]; }
			set { backing["lastname"] = new JValue(value); }
		}
		
		/// <summary>
		/// Notifications field, can store arbitrary application data
		/// </summary>
		public string Notifications
		{
			get { return (string)backing["notifications"]; }
			set { backing["notifications"] = new JValue(value); }
		}
		
		/// <summary>
		/// Extra field, can store arbitrary application data
		/// </summary>
		public string Extra
		{
			get { return (string)backing["extra"]; }
			set { backing["extra"] = new JValue(value); }
		}
		
		/// <summary>
		/// Phone number
		/// </summary>
		public string Phone
		{
			get { return (string)backing["phone"]; }
			set { backing["phone"] = new JValue(value); }
		}
		
		/// <summary>
		/// Date of birth
		/// </summary>
		public string Dob //XXX: Should be a date type, format is ISO8601
		{
			get { return (string)backing["dob"]; }
			set { backing["dob"] = new JValue(value); }
		}
		
		/// <summary>
		/// Postal address line 1
		/// </summary>
		public string Address
		{
			get { return (string)backing["address"]; }
			set { backing["address"] = new JValue(value); }
		}
		
		/// <summary>
		/// Postal address line 2
		/// </summary>
		public string Address2
		{
			get { return (string)backing["address2"]; }
			set { backing["address2"] = new JValue(value); }
		}
		
		/// <summary>
		/// Postal address line 3
		/// </summary>
		public string Address3
		{
			get { return (string)backing["address3"]; }
			set { backing["address3"] = new JValue(value); }
		}
		
		/// <summary>
		/// Postal address city, suburb or locality
		/// </summary>
		public string Suburb
		{
			get { return (string)backing["suburb"]; }
			set { backing["suburb"] = new JValue(value); }
		}
		
		/// <summary>
		/// Postal address state or region
		/// </summary>
		public string State
		{
			get { return (string)backing["state"]; }
			set { backing["state"] = new JValue(value); }
		}
		
		/// <summary>
		/// Postal address country
		/// </summary>
		public string Country
		{
			get { return (string)backing["country"]; }
			set { backing["country"] = new JValue(value); }
		}
		
		/// <summary>
		/// Postal code
		/// </summary>
		public string Postcode
		{
			get { return (string)backing["postcode"]; }
			set { backing["postcode"] = new JValue(value); }
		}
		
		/// <summary>
		/// Gender (M/F, male/female)
		/// </summary>
		public string Gender
		{
			get { return (string)backing["gender"]; }
			set { backing["gender"] = new JValue(value); }
		}
		
		/// <summary>
		/// Email address. If external ID is null, this must be unique.
		/// </summary>
		public string Email
		{
			get { return (string)backing["email"]; }
			set { backing["email"] = new JValue(value); }
		}
		
		/// <summary>
		/// Social media influence rating. Ordinal positive integer scale; higher
		/// values mean more public profile data, more status updates, and/or more
		/// friends. Read-only as this is caculated by TaguchiMail's social media
		/// subsystem.
		/// </summary>
		public int SocialRating
		{
			get { return (int)backing["social_rating"]; }
		}
		
		/// <summary>
		/// Social media aggregate profile. JSON data structure similar to the
		/// OpenSocial v1.1 Person schema.
		/// </summary>
		public string SocialProfile
		{
			get { return (string)backing["social_profile"]; }
		}
		
		/// <summary>
		/// Date/time at which this subscriber globally unsubscribed (or null).
		/// </summary>
		public string UnsubscribeDateTime
		{
			get { return (string)backing["unsubscribed"]; }
			set { backing["unsubscribed"] = new JValue(value); }
		}
		
		/// <summary>
		/// Date/time at which this subscriber's email address was marked as invalid (or null).
		/// </summary>
		public string BounceDateTime
		{
			get { return (string)backing["bounced"]; }
			set { backing["bounced"] = new JValue(value); }
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
		/// Retrieve a custom field value by field name.
		/// </summary>
		/// <param name="field">
		/// A <see cref="System.String"/> indicating the custom field to retrieve.
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> containing the custom field value.
		/// </returns>
		public string GetCustomField(string field)
		{
			if (backing["custom_fields"] == null)
			{
				backing["custom_fields"] = new JArray();
			}
			foreach (JObject field_data in backing["custom_fields"])
			{
				if (((string)field_data["field"]).Equals(field))
				{
					return (string)field_data["data"];
				}
			}
			return null;
		}
		
		/// <summary>
		/// Set a custom field value by field.
		/// </summary>
		/// <param name="field">
		/// A <see cref="System.String"/> containing the name of the field to set.
		/// 
		/// If a field with that name is already defined for this subscriber, the new
		/// value will overwrite the old one.
		/// </param>
		/// <param name="data">
		/// A <see cref="System.String"/> containing the field's data. If a field is intended
		/// to store array or other complex data types, this should be JSON-encoded (or 
		/// serialized to XML depending on application preference).
		/// </param>
		public void SetCustomField(string field, string data)
		{
			if (backing["custom_fields"] == null)
			{
				backing["custom_fields"] = new JArray();
			}
			for (int i = 0; i < ((JArray)backing["custom_fields"]).Count; i++)
			{
				if (((string)backing["custom_fields"][i]["field"]).Equals(field))
				{
					backing["custom_fields"][i]["data"] = new JValue(data);
					return;
				}
			}
			
			// field was not found in the array, so add it
			JObject cf = new JObject();
			cf.Add("field", new JValue(field));
			cf.Add("data", new JValue(data));
			((JArray)backing["custom_fields"]).Add(cf);
		}
		
		/// <summary>
		/// Check the subscription status of a specific list.
		/// </summary>
		/// <param name="listId">
		/// A <see cref="System.String"/> containing the list ID to check subscription status for.
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating whether or not the subscriber is subscribed
		/// to the list.
		/// </returns>
		public bool IsSubscribedToList(string listId)
		{
			if (backing["lists"] == null)
			{
				backing["lists"] = new JArray();
			}
			foreach (JObject list in backing["lists"])
			{
				if (list["list_id"].ToString().Equals(listId))
				{
					if ((string)list["unsubscribed"] == null)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			return false;
		}
		
		public bool IsSubscribedToList(SubscriberList list)
		{
			return this.IsSubscribedToList(list.RecordId);
		}
		
		/// <summary>
		/// Retrieve the subscription option (arbitrary application data) for a specific list.
		/// </summary>
		/// <param name="listId">
		/// A <see cref="System.String"/> containing the list ID to retrieve subscription option for.
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> containing the subscription option.
		/// </returns>
		public string GetSubscriptionOption(string listId)
		{
			if (backing["lists"] == null)
			{
				backing["lists"] = new JArray();
			}
			foreach (JObject list in backing["lists"])
			{
				if (list["list_id"].ToString().Equals(listId))
				{
					return (string)list["option"];
				}
			}
			return null;
		}
		
		public string GetSubscriptionOption(SubscriberList list)
		{
			return this.GetSubscriptionOption(list.RecordId);
		}
		
		/// <summary>
		/// Check the unsubscription status of a specific list.
		/// </summary>
		/// <param name="listId">
		/// A <see cref="System.String"/> containing the list ID to check unsubscription status for.
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/> indicating whether or not the subscriber is unsubscribed
		/// to the list.
		/// </returns>
		public bool IsUnsubscribedFromList(string listId)
		{
			if (backing["lists"] == null)
			{
				backing["lists"] = new JArray();
			}
			foreach (JObject list in backing["lists"])
			{
				if (list["list_id"].ToString().Equals(listId))
				{
					if ((string)list["unsubscribed"] == null)
					{
						return false;
					}
					else
					{
						return true;
					}
				}
			}
			return false;
		}
		
		public bool IsUnsubscribedFromList(SubscriberList list)
		{
			return this.IsUnsubscribedFromList(list.RecordId);
		}
		
		/// <summary>
		/// Retrieve all lists to which this record is subscribed.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String[]"/> containing subscribed list IDs.
		/// </returns>
		public string[] GetSubscribedListIds()
		{
			if (backing["lists"] == null)
			{
				backing["lists"] = new JArray();
			}
			List<string> lists = new List<string>();
			foreach (JObject list in backing["lists"])
			{
				if ((string)list["unsubscribed"] == null)
				{
					lists.Add(list["list_id"].ToString());
				}
			}
			return lists.ToArray();
		}
		
		public SubscriberList[] GetSubscribedLists()
		{
			string[] listIds = this.GetSubscribedListIds();
			List<SubscriberList> lists = new List<SubscriberList>();
			foreach (string listId in listIds)
			{
				lists.Add(SubscriberList.Get(this.context, listId, null));
			}
			return lists.ToArray();
		}
		
		/// <summary>
		/// Retrieve all lists from which this record is unsubscribed.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String[]"/> containing unsubscribed list IDs.
		/// </returns>
		public string[] GetUnsubscribedListIds()
		{
			if (backing["lists"] == null)
			{
				backing["lists"] = new JArray();
			}
			List<string> lists = new List<string>();
			foreach (JObject list in backing["lists"])
			{
				if ((string)list["unsubscribed"] != null)
				{
					lists.Add(list["list_id"].ToString());
				}
			}
			return lists.ToArray();
		}
		
		public SubscriberList[] GetUnsubscribedLists()
		{
			string[] listIds = this.GetUnsubscribedListIds();
			List<SubscriberList> lists = new List<SubscriberList>();
			foreach (string listId in listIds)
			{
				lists.Add(SubscriberList.Get(this.context, listId, null));
			}
			return lists.ToArray();
		}
		
		/// <summary>
		/// Add the subscriber to a specific list, resetting the unsubscribe flag if 
		/// previously set.
		/// </summary>
		/// <param name="listId">
		/// A <see cref="System.String"/> containing the list ID which should be added.
		/// </param>
		/// <param name="option">
		/// A <see cref="System.String"/> containing the list subscription option
		/// (arbitarary application data).
		/// </param>
		public void SubscribeToList(string listId, string option)
		{
			if (backing["lists"] == null)
			{
				backing["lists"] = new JArray();
			}
			for (int i = 0; i < ((JArray)backing["lists"]).Count; i++)
			{
				if (backing["lists"][i]["list_id"].ToString().Equals(listId))
				{
					backing["lists"][i]["option"] = new JValue(option);
					backing["lists"][i]["unsubscribed"] = null;
					return;
				}
			}
			
			// list was not found in the array, so add it
			JObject list = new JObject();
			list.Add("list_id", new JValue(System.Convert.ToInt64(listId, 10)));
			list.Add("option", new JValue(option));
			((JArray)backing["lists"]).Add(list);
		}
		
		public void SubscribeToList(SubscriberList list, string option)
		{
			this.SubscribeToList(list.RecordId, option);
		}
		
		/// <summary>
		/// Unsubscribe from a specific list, adding the list if not already subscribed.
		/// </summary>
		/// <param name="listId">
		/// A <see cref="System.String"/> containing the list ID from which the record
		/// should be unsubscribed.
		/// </param>
		public void UnsubscribeFromList(string listId)
		{
			if (backing["lists"] == null)
			{
				backing["lists"] = new JArray();
			}
			for (int i = 0; i < ((JArray)backing["lists"]).Count; i++)
			{
				if (backing["lists"][i]["list_id"].ToString().Equals(listId) && 
				    		(string)backing["lists"][i]["unsubscribed"] == null)
				{
					backing["lists"][i]["unsubscribed"] = new JValue(true);
					return;
				}
			}
			
			// list was not found in the array, so add it in the unsubscribed state
			JObject list = new JObject();
			list.Add("list_id", new JValue(System.Convert.ToInt64(listId, 10)));
			list.Add("unsubscribed", new JValue(true));
			((JArray)backing["lists"]).Add(list);
		}
		
		public void UnsubscribeFromList(SubscriberList list)
		{
			this.UnsubscribeFromList(list.RecordId);
		}
		
		/// <summary>
		/// Create this record in the TaguchiMail database if it doesn't already exist
		/// (based on a search for records with the same ExternalRef or Email in that order).
		/// 
		/// If it does, simply update what's already in the database. Fields not written
		/// to the backing store (via property update) will not be overwritten in the database.
		/// </summary>
		public void CreateOrUpdate()
		{
			JArray data = new JArray();
			data.Add(this.backing);
			
			JArray results = JArray.Parse(context.MakeRequest(this.resourceType, "CREATEORUPDATE", null, data.ToString(), null, null));
			this.backing = (JObject)results[0];
		}
		
		/// <summary>
		/// Retrieve a single Subscriber based on its TaguchiMail identifier.
		/// </summary>
		/// <param name="context">
		/// A <see cref="Context"/> object determining the TM instance and organization to query.
		/// </param>
		/// <param name="recordId">
		/// A <see cref="System.String"/> containing the subscriber's unique TaguchiMail identifier.
		/// </param>
		/// <returns>
		/// The <see cref="Subscriber"/> with that ID.
		/// </returns>
		public static Subscriber Get(Context context, string recordId, Dictionary<string, string> parameters)
		{
			JArray results = JArray.Parse(context.MakeRequest("subscriber", "GET", recordId, null, parameters, null));
			Subscriber rec = new Subscriber(context);
			rec.backing = (JObject)results[0];
			return rec;
		}
		
		/// <summary>
		/// Retrieve a list of Subscribers based on a query.
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
		/// A <see cref="Subscriber[]"/> matching the query.
		/// </returns>
		public static Subscriber[] Find(Context context, string sort, string order, int offset, int limit, string[] query)
		{
			Dictionary<string,string> parameters = new Dictionary<string, string>();
			parameters["sort"] = sort;
			parameters["order"] = order;
			parameters["offset"] = offset.ToString();
			parameters["limit"] = limit.ToString();
			
			JArray results = JArray.Parse(context.MakeRequest("subscriber", "GET", null, null, parameters, query));
			List<Subscriber> recordSet = new List<Subscriber>();
			foreach (var result in results)
			{
				Subscriber rec = new Subscriber(context);
				rec.backing = (JObject)result;
				recordSet.Add(rec);
			}
			return recordSet.ToArray();
		}
	}
}
