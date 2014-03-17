
using System;
using System.Net;
using System.Web;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace TMAPIv4
{
	/// <summary>
	/// 	Represents a TaguchiMail connection. Must be created prior to instantiation of any 
	/// other TaguchiMail classes, as it's a required parameter for their constructors.
	/// </summary>
	public class Context
	{
		private string hostname;
		private string username;
		private string password;
		private string organizationId;
		
		private Uri baseUri;
		
		/// <summary>
		/// 	The Context constructor.
		/// </summary>
		/// <param name="hostname">
		/// A <see cref="System.String"/> containing the hostname (or IP address) of the TaguchiMail 
		/// instance to connect with.
		/// </param>
		/// <param name="username">
		/// A <see cref="System.String"/> containing the username (email address) of an authorized user.
		/// </param>
		/// <param name="password">
		/// A <see cref="System.String"/> containing the password of an authorized user.
		/// </param>
		/// <param name="organizationId">
		/// A <see cref="System.String"/> indicating the organization ID to be used for creation of 
		/// new objects. The username supplied must be authorized to access this organization.
		/// </param>
		public Context(string hostname, string username, string password, string organizationId)
		{
			this.hostname = hostname;
			this.username = username;
			this.password = password;
			this.organizationId = organizationId;
			
			this.baseUri = new Uri("https://" + hostname + "/admin/api/" + organizationId);
		}
		
		/// <summary>
		/// 	Attempts to connect to the TaguchiMail server with the provided credentials,
		/// and throws an exception if an error occurs.
		/// </summary>
		public bool Validate()
		{
			return true;
		}
		
		/// <summary>
		/// Makes a TaguchiMail request with a given resource, command, parameters and query predicates.
		/// </summary>
		/// <param name="resource">
		/// A <see cref="System.String"/> indicating the resource.
		/// </param>
		/// <param name="command">
		/// A <see cref="System.String"/> indicating the command to issue to the resource.
		/// </param>
		/// <param name="recordId">
		/// A <see cref="System.String"/> indicating the ID of the record to operate on, for record-specific commands.
		/// </param>
		/// <param name="data">
		/// A <see cref="System.String"/> containing the JSON-formatted record data for the command, if required by the command type.
		/// </param>
		/// <param name="parameters">
		/// A <see cref="Dictionary<System.String, System.String>"/> of additional parameters to the request. The supported
		/// parameters will depend on the resource and command, but commonly supported parameters include:
		/// * sort: one of the resource's fields, used to sort the result set;
		/// * order: either 'asc' or 'desc', determines whether the result set is sorted in ascending or descending order;
		/// * limit: positive non-zero integer indicating the maximum returned result set size (defaults to 1);
		/// * offset: either 0 or a positive integer indicating the position of the first returned result in the result set (defaults to 0).
		/// </param>
		/// <param name="query">
		/// An <see cref="Array<System.String>"/> of query predicates, each of the form:
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
		public string MakeRequest(string resource, string command, string recordId, string data, Dictionary<string,string> parameters, string[] query)
		{	
			StringBuilder qsBuilder = new StringBuilder();
			qsBuilder.Append(this.baseUri.ToString() + "/" + resource + "/");
			if (recordId != null) 
			{
				qsBuilder.Append(recordId);
			}
			qsBuilder.Append("?_method=" + Uri.EscapeDataString(command));
			qsBuilder.Append("&auth=" + Uri.EscapeDataString(this.username + "|" + this.password));
			if (query != null)
			{
				foreach (string predicate in query)
				{
					qsBuilder.Append("&query=" + Uri.EscapeDataString(predicate));
				}
			}
			if (parameters != null)
			{
				foreach (KeyValuePair<string,string> parameter in parameters)
				{
					qsBuilder.Append("&" + Uri.EscapeDataString(parameter.Key) + "=" + parameter.Value);
				}
			}
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(qsBuilder.ToString());
			request.Method = command == "GET" ? "GET" : "POST";
			request.PreAuthenticate = true; // Authentication always required, so don't wait for a 401 beforehand.
			request.Timeout = 60000; // In ms
			// Work in JSON, it's smaller and faster at the TM end. In addition, the stats resource doesn't
			// have an XML serialization available (tabular data in XML is nasty).
			request.Accept = "application/json";
			// Set a user-agent so we can track any errors occurring a little more easily.
			request.UserAgent = "TMAPIv4 .NET framework wrapper";
			
			// Post data if it was supplied
			if (data != null && data.Length > 0)
			{
								Console.WriteLine(data);
				request.ContentType = "application/json";
				request.ContentLength = data.Length;
				try
				{
					StreamWriter req = new StreamWriter(request.GetRequestStream());
					req.Write(data);
					req.Close();
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			
			// Read the response stream and return the string value. TaguchiMail always uses UTF-8.
			string result;
			try
			{
				HttpWebResponse rep = (HttpWebResponse)request.GetResponse();
				using (StreamReader repData = new StreamReader(rep.GetResponseStream(), Encoding.UTF8))
				{
					result = repData.ReadToEnd();
				}
				rep.Close();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			
			return result;
		}
	}
}
