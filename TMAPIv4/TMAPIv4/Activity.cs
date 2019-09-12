using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TMAPIv4
{
	public class ActivityRevision
	{
		protected JObject backing;
		protected Activity activity;

		/// <summary>
		/// Contains revision XML content. This is typically set up in the TaguchiMail UI,
		/// and normally includes an XML document structure based on RSS. However,
		/// if UI access is not required, this content can have an arbitrary (valid)
		/// structure; the only requirement is that the transform associated with the
		/// template convert this content into a valid intermediate MIME document.
		/// </summary>
		public string Content
		{
			get { return (string)backing["content"]; }
			set { backing["content"] = new JValue(value); }
		}

		/// <summary>
		/// If 'deployed', this revision is publicly available, otherwise only test
		/// events may use this revision.
		/// </summary>
		public string ApprovalStatus
		{
			get { return (string)backing["approval_status"]; }
			set { backing["approval_status"] = new JValue(value); }
		}

		/// <summary>
		/// ID of the activity revision record in the database.
		/// </summary>
		public string RecordId
		{
			get { return backing["id"].ToString(); }
		}

		/// <summary>
		/// Create a new activity revision, given a parent Activity and a content document
		/// (aka Source Document).
		/// </summary>
		/// <param name="activity">
		/// A <see cref="Activity"/>
		/// </param>
		/// <param name="content">
		/// A <see cref="System.String"/>
		/// </param>
		public ActivityRevision (Activity activity, string content)
		{
			this.backing = new JObject();
			this.activity = activity;
			this.Content = content;
		}

		public ActivityRevision (Activity activity, JObject revision)
		{
			this.backing = revision;
			this.activity = activity;
		}
	}

	public class Activity : Record
	{
		protected JArray existingRevisions;

		public Activity (Context context) : base(context)
		{
			this.resourceType = "activity";
		}

		/// <summary>
		/// ID of the TaguchiMail activity record.
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
		/// Activity name
		/// </summary>
		public string Name
		{
			get { return (string)backing["name"]; }
			set { backing["name"] = new JValue(value); }
		}

		/// <summary>
		/// This is matched up with Template types to determine what
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
		/// An expression representing the activity's target subscriber set
		/// </summary>
		public string TargetExpression
		{
			get { return (string)backing["target_expression"]; }
			set { backing["target_expression"] = new JValue(value); }
		}

		/// <summary>
		/// Approval workflow status of the activity; JSON object containing
		/// a list of locked-in depoyment settings used by the queue command.
		/// </summary>
		public string ApprovalStatus
		{
			get { return (string)backing["approval_status"]; }
			set { backing["approval_status"] = new JValue(value); }
		}

		/// <summary>
		/// Date/time at which this activity was deployed (or is scheduled to deploy)
		/// </summary>
		public string DeployDateTime
		{
			get { return (string)backing["date"]; }
			set { backing["date"] = new JValue(value); }
		}

		/// <summary>
		/// The ID of the Template this activity uses
		/// </summary>
		public string TemplateId
		{
			get { return (string)backing["template_id"]; }
			set { backing["template_id"] = new JValue(value); }
		}

		/// <summary>
		/// The ID of the Campaign to which this activity belongs
		/// </summary>
		public string CampaignId
		{
			get { return (string)backing["campaign_id"]; }
			set { backing["campaign_id"] = new JValue(value); }
		}

		/// <summary>
		/// Maximum deployment rate of this message, in messages per minute. If 0,
		/// the activity is suspended. Web pages (and other pull messages) ignore this value.
		/// </summary>
		public int Throttle
		{
			get { return (int)backing["throttle"]; }
			set { backing["throttle"] = new JValue(value); }
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
		/// Activity status; leave null if not used
		/// </summary>
		public string Status
		{
			get { return (string)backing["status"]; }
		}

		/// <summary>
		/// Latest activity revision content. If set, a new revision will be
		/// created upon activity create/update.
		/// </summary>
		public ActivityRevision LatestRevision
		{
			get
			{
				if (((JArray)backing["revisions"]).Count > 0)
				{
					return new ActivityRevision(this, (JObject)backing["revisions"][0]);
				}
				else if (existingRevisions.Count > 0)
				{
					return new ActivityRevision(this, (JObject)existingRevisions[0]);
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
		/// Save this activity to the TaguchiMail database.
		/// </summary>
		public override void Update()
		{
			base.Update();
			// avoid re-saving existing revisions
			this.backing["revisions"] = new JArray();
		}

		/// <summary>
		/// Create this activity in the TaguchiMail database.
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
		/// Sends a proof message for an activity record to the list with the specified ID.
		/// </summary>
		/// <param name="proofListId">
		/// A <see cref="System.String"/> indicating List ID of the proof list to which the messages will be sent.
		/// </param>
		/// <param name="subjectTag">
		/// A <see cref="System.String"/> to display at the start of the subject line.
		/// </param>
		/// <param name="customMessage">
		/// A <see cref="System.String"/> with a custom message which will be included in the proof header.
		/// </param>
		public virtual void Proof(string proofListId, string subjectTag, string customMessage)
		{
			JArray data = new JArray();
			data.Add(new JObject());
			data[0]["id"] = new JValue(this.RecordId);
			data[0]["list_id"] = new JValue(proofListId);
			data[0]["tag"] = new JValue(subjectTag);
			data[0]["message"] = new JValue(customMessage);

			context.MakeRequest(this.resourceType, "PROOF", this.backing["id"].ToString(), data.ToString(), null, null);
		}

		/// <summary>
		/// Sends a proof message for an activity record to a specific list.
		/// </summary>
		/// <param name="proofList">
		/// A <see cref="SubscriberList"/> indicating the list to which the messages will be sent.
		/// </param>
		/// <param name="subjectTag">
		/// A <see cref="System.String"/> to display at the start of the subject line.
		/// </param>
		/// <param name="customMessage">
		/// A <see cref="System.String"/> with a custom message which will be included in the proof header.
		/// </param>
		public virtual void Proof(SubscriberList proofList, string subjectTag, string customMessage)
		{
			this.Proof(proofList.RecordId, subjectTag, customMessage);
		}

		/// <summary>
		/// Sends an approval request for an activity record to the list with the specified ID.
		/// </summary>
		/// <param name="approvalListId">
		/// A <see cref="System.String"/> indicating List ID of the approval list to which the approval request will be sent.
		/// </param>
		/// <param name="subjectTag">
		/// A <see cref="System.String"/> to display at the start of the subject line.
		/// </param>
		/// <param name="customMessage">
		/// A <see cref="System.String"/> with a custom message which will be included in the approval header.
		/// </param>
		public virtual void RequestApproval(string approvalListId, string subjectTag, string customMessage)
		{
			JArray data = new JArray();
			data.Add(new JObject());
			data[0]["id"] = new JValue(this.RecordId);
			data[0]["list_id"] = new JValue(approvalListId);
			data[0]["tag"] = new JValue(subjectTag);
			data[0]["message"] = new JValue(customMessage);

			context.MakeRequest(this.resourceType, "APPROVAL", this.backing["id"].ToString(), data.ToString(), null, null);
		}

		/// <summary>
		/// Sends an approval request for an activity record to a specific list.
		/// </summary>
		/// <param name="approvalList">
		/// The <see cref="SubscriberList"/> to which the approval request will be sent.
		/// </param>
		/// <param name="subjectTag">
		/// A <see cref="System.String"/> to display at the start of the subject line.
		/// </param>
		/// <param name="customMessage">
		/// A <see cref="System.String"/> with a custom message which will be included in the approval header.
		/// </param>
		public virtual void RequestApproval(SubscriberList approvalList, string subjectTag, string customMessage)
		{
			this.RequestApproval(approvalList.RecordId, subjectTag, customMessage);
		}

		/// <summary>
		/// Sets the approved flag for the activity with current settings.
		/// </summary>
		public virtual void Approve()
		{
			JArray data = new JArray();
			data.Add(new JObject());
			data[0]["revision.id"] = new JValue(this.LatestRevision.RecordId);
			data[0]["target_expression"] = new JValue(this.TargetExpression);
			data[0]["date"] = new JValue(this.DeployDateTime);

			context.MakeRequest(this.resourceType, "APPROVE", this.backing["id"].ToString(), data.ToString(), null, null);
		}

		/// <summary>
		/// Queues an activity for broadcast.
		/// </summary>
		/// <param name="throttle">
		/// A <see cref="System.Int32"/> indicating the maximum number of messages to send per minute.
		/// </param>
		public virtual void Queue(int throttle)
		{
			JArray data = new JArray();
			data.Add(new JObject());
			data[0]["throttle"] = new JValue(throttle);
			data[0]["method"] = new JValue("queue");

			context.MakeRequest(this.resourceType, "QUEUE", this.backing["id"].ToString(), data.ToString(), null, null);
		}

		/// <summary>
		/// Triggers the activity, causing it to be delivered to a specified list of subscribers.
		/// </summary>
		/// <param name="subscriberIds">
		/// A <see cref="System.String[]"/> containing subscriber IDs to whom the message should be delivered.
		/// </param>
		/// <param name="requestContent">
		/// A <see cref="System.String"/> of XML or JSON content for message customization. The requestContent document
		/// is available to the activity template's stylesheet, in addition to the revision's content. Should be
		/// null if unused.
		/// </param>
		/// <param name="test">
		/// A <see cref="System.Boolean"/> determining whether or not to treat this as a test send.
		/// </param>
		public virtual void Trigger(string[] subscriberIds, string requestContent, bool test)
		{
			JArray data = new JArray();
			data.Add(new JObject());
			data[0]["id"] = new JValue(this.RecordId);
			data[0]["test"] = new JValue(test ? 1 : 0);
			data[0]["request_content"] = new JValue(requestContent);
			data[0]["conditions"] = new JArray(subscriberIds);

			context.MakeRequest(this.resourceType, "TRIGGER", this.backing["id"].ToString(), data.ToString(), null, null);
		}

		/// <summary>
		/// Triggers the activity, causing it to be delivered to a specified list of subscribers.
		/// </summary>
		/// <param name="subscriberIds">
		/// A <see cref="Subscriber[]"/> containing subscribers to whom the message should be delivered.
		/// </param>
		/// <param name="requestContent">
		/// A <see cref="System.String"/> of XML or JSON content for message customization. The requestContent document
		/// is available to the activity template's stylesheet, in addition to the revision's content. Should be
		/// null if unused.
		/// </param>
		/// <param name="test">
		/// A <see cref="System.Boolean"/> determining whether or not to treat this as a test send.
		/// </param>
		public virtual void Trigger(Subscriber[] subscribers, string requestContent, bool test)
		{
			List<string> subscriberIds = new List<string>();
			foreach (Subscriber s in subscribers)
			{
				subscriberIds.Add(s.RecordId);
			}
			this.Trigger(subscriberIds.ToArray(), requestContent, test);
		}

		/// <summary>
		/// Triggers the activity, causing it to be delivered to all subscribers matching a target expression
		/// </summary>
		/// <param name="targetExpression">
		/// A target expression <see cref="System.String"/> identifying the
		/// </param>
		/// <param name="requestContent">
		/// A <see cref="System.String"/> of XML or JSON content for message customization. The requestContent document
		/// is available to the activity template's stylesheet, in addition to the revision's content. Should be
		/// null if unused.
		/// </param>
		/// <param name="test">
		/// A <see cref="System.Boolean"/> determining whether or not to treat this as a test send.
		/// </param>
		public virtual void Trigger(string targetExpression, string requestContent, bool test)
		{
			JArray data = new JArray();
			data.Add(new JObject());
			data[0]["id"] = new JValue(this.RecordId);
			data[0]["test"] = new JValue(test ? 1 : 0);
			data[0]["request_content"] = new JValue(requestContent);
			data[0]["conditions"] = new JObject();
			data[0]["conditions"]["expression"] = new JValue(targetExpression);

			context.MakeRequest(this.resourceType, "TRIGGER", this.backing["id"].ToString(), data.ToString(), null, null);
		}

		/// <summary>
		/// Retrieve a single Activity based on its TaguchiMail identifier.
		/// </summary>
		/// <param name="context">
		/// A <see cref="Context"/> object determining the TM instance and organization to query.
		/// </param>
		/// <param name="recordId">
		/// A <see cref="System.String"/> containing the list's unique TaguchiMail identifier.
		/// </param>
		/// <returns>
		/// The <see cref="Activity"/> with that ID.
		/// </returns>
		public static Activity Get(Context context, string recordId, Dictionary<string, string> parameters)
		{
			JArray results = JArray.Parse(context.MakeRequest("activity", "GET", recordId, null, parameters, null));
			Activity rec = new Activity(context);
			rec.backing = (JObject)results[0];
			rec.existingRevisions = (JArray)rec.backing["revisions"];
			// clear out existing revisions so they're not sent back to the server on update
			rec.backing["revisions"] = new JArray();
			return rec;
		}

		/// <summary>
		/// Retrieve a single Activity based on its TaguchiMail identifier, with its latest revision content.
		/// </summary>
		/// <param name="context">
		/// A <see cref="Context"/> object determining the TM instance and organization to query.
		/// </param>
		/// <param name="recordId">
		/// A <see cref="System.String"/> containing the activity's unique TaguchiMail identifier.
		/// </param>
		/// <returns>
		/// The <see cref="Activity"/> with that ID.
		/// </returns>
		public static Activity GetWithContent(Context context, string recordId, Dictionary<string, string> parameters)
		{
			Dictionary<string, string> newParams = new Dictionary<string, string>(parameters);
			newParams.Add("revisions", "latest");
			return Activity.Get(context, recordId, newParams);
		}

		/// <summary>
		/// Retrieve a list of Activities based on a query.
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
		/// A <see cref="Activity[]"/> matching the query.
		/// </returns>
		public static Activity[] Find(Context context, string sort, string order, int offset, int limit, string[] query)
		{
			Dictionary<string,string> parameters = new Dictionary<string, string>();
			parameters["revisions"] = "latest";
			parameters["sort"] = sort;
			parameters["order"] = order;
			parameters["offset"] = offset.ToString();
			parameters["limit"] = limit.ToString();

			JArray results = JArray.Parse(context.MakeRequest("activity", "GET", null, null, parameters, query));
			List<Activity> recordSet = new List<Activity>();
			foreach (var result in results)
			{
				Activity rec = new Activity(context);
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
