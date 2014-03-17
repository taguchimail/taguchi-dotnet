using System;
using System.Collections.Generic;
using TMAPIv4;

namespace TMAPISample
{
	class MainClass
	{
		/// <summary>
		/// Run a series of TaguchiMail API tests.
		/// </summary>
		public static void Main()
		{
			Dictionary<string, string>	connectionParams = new Dictionary<string, string>() {
				{"host", ""},
				{"organization_id", ""},
				{"username", ""},
				{"password", ""}
			};
			Console.WriteLine("Connecting to " + connectionParams["host"] + "...");

			Context ctx = new Context(connectionParams["host"], connectionParams["username"],
			                          connectionParams["password"], connectionParams["organization_id"]);

			// Look for an existing subscriber record, and make some changes
			Console.WriteLine("Running lookup test...");
			Subscriber s = Subscriber.Find(ctx, "id", "asc", 0, 1, new string[1] {"email-eq-" + connectionParams["username"]})[0];
			Console.WriteLine("Found subscriber ID " + s.RecordId + "; name " + s.FirstName + " " + s.LastName);

			string newLastName = s.LastName + "Lorem";
			Console.WriteLine("Changing subscriber ID " +s.RecordId + " last name from '" + s.LastName + "'");

			s.LastName = newLastName;
			s.Update();

			Console.WriteLine("Subscriber ID " + s.RecordId + " last name now '" + s.LastName + '"');

			Console.WriteLine("Toggling subscriber ID " +s.RecordId + " subscription list ID 123");
			if (s.IsSubscribedToList("123"))
			{
				Console.WriteLine("Unsubscribing...");
				s.UnsubscribeFromList("123");
			}
			else
			{
				Console.WriteLine("Subscribing...");
				s.SubscribeToList("123", "example");
			}

			s.Update();

			Console.WriteLine("Subscriber ID " + s.RecordId + " now subscribed to " + s.GetSubscribedLists().Length.ToString() + " lists");

			// Create some new subscriber records, and add to a list.
			Console.WriteLine("Running create test...");
			Random rand = new Random();
			for (int i = 0; i < 100; i++)
			{
				Subscriber s2 = new Subscriber(ctx);
				int idx = rand.Next(1000);
				s2.FirstName = "Subscriber" + idx.ToString();
				s2.LastName = "Test";
				s2.Email = "edmtest+" + idx.ToString() + "@mail.taguchimail.com";
				s2.SubscribeToList(idx % 2 == 0 ? "123" : "124", null);
				s2.CreateOrUpdate();
				Console.WriteLine("Created subscriber " + s2.Email + " with ID " + s2.RecordId);
			}

			// Retrieve subscribers from the lists
			SubscriberList[] allLists = SubscriberList.Find(ctx, "id", "asc", 0, 100, null);
			foreach (SubscriberList sl in allLists)
			{
				Console.WriteLine("Dumping list " + sl.Name + " (ID " + sl.RecordId + ")");
				Subscriber[] subscribers = sl.GetSubscribers(0, 1000);
				foreach (Subscriber sub in subscribers)
				{
					Console.WriteLine("    Subscriber " + sub.Email + " width ID " + sub.RecordId + " is " + (sub.IsSubscribedToList(sl) ? "subscribed" : "unsubscribed"));
				}
			}

			// Send an email to a subscriber
			string customContent = @"<purchase>
  <product href='http://example.com/1234'>
    <name>Example Product</name>
    <price>123.45</price>
    <sku>XYZ1234ABC</sku>
  </product>
  <transaction>
    <name>Ms Example User</name>
    <phone>03 9123 4567</phone>
    <date>Monday, 22 November 2010</date>
    <time>6:30 PM</time>
    <confirmation-number>ABC123</confirmation-number>
  </transaction>
</purchase>";
			Activity purchaseConfirmation = Activity.Get(ctx, "123", null);
			Console.WriteLine("Sending activity " + purchaseConfirmation.Name + " to " + s.Email);
			purchaseConfirmation.Trigger(s, customContent, false);
			Console.WriteLine("Sent");
		}
	}
}
