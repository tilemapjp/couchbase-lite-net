using System;
using NUnit.Framework;
using Couchbase.Lite;
using System.Collections.Generic;
using Couchbase.Lite.Util;
using System.Linq;
using System.Text;
using System.IO;

namespace Couchbase.Lite.Tests
{
	[TestFixture]
	public class TestsSample
	{
		public const string Tag = "LiteTestCase";

		protected internal ObjectWriter mapper = new ObjectWriter();

		protected internal Manager manager = null;

		protected internal Database database = null;

		protected internal string DefaultTestDb = "cblitetest";

		[SetUp]
		public void Setup ()
		{
		}

		[TearDown]
		public void Tear ()
		{
		}

		[Test]
		public void Pass ()
		{
			Console.WriteLine ("test1");
			Assert.True (true);
		}

		[Test]
		public void TestCreateDocument()
		{
			var rootDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var rootDirectory = new DirectoryInfo(Path.Combine(rootDirectoryPath, "couchbase/tests/files"));

			var path = new DirectoryInfo(Path.Combine(rootDirectory.FullName, "API_SharedMapBlocks"));
			manager = new Manager(path, Manager.DefaultOptions);

			var properties = new Dictionary<String, Object>();
			properties["testName"] = "testCreateDocument";
			properties["tag"] = 1337L;

			var db = manager.GetExistingDatabase(DefaultTestDb);
			if (db == null) {
				db = manager.GetDatabase (DefaultTestDb);
			}
			var doc = CreateDocumentWithProperties(db, properties);
			var docID = doc.Id;
			Assert.IsTrue(docID.Length > 10, "Invalid doc ID: " + docID);

			var currentRevisionID = doc.CurrentRevisionId;
			Assert.IsTrue(currentRevisionID.Length > 10, "Invalid doc revision: " + docID);
			Assert.AreEqual(doc.UserProperties, properties);
			Assert.AreEqual(db.GetDocument(docID), doc);

			db.DocumentCache.EvictAll();

			// so we can load fresh copies
			var doc2 = db.GetExistingDocument(docID);
			Assert.AreEqual(doc2.Id, docID);
			Assert.AreEqual(doc2.CurrentRevisionId, currentRevisionID);
			Assert.IsNull(db.GetExistingDocument("b0gus"));
		}

		public static Document CreateDocumentWithProperties(Database db, IDictionary<String, Object> properties)
		{
			var doc = db.CreateDocument();

			Assert.IsNotNull(doc);
			Assert.IsNull(doc.CurrentRevisionId);
			Assert.IsNull(doc.CurrentRevision);
			Assert.IsNotNull("Document has no ID", doc.Id);

			// 'untitled' docs are no longer untitled (8/10/12)
			try
			{
				doc.PutProperties(properties);
			}
			catch (Exception e)
			{
				Log.E(Tag, "Error creating document", e);
				Assert.IsTrue( false, "can't create new document in db:" + db.Name +
					" with properties:" + properties.Aggregate(new StringBuilder(" >>> "), (str, kvp)=> { str.AppendFormat("'{0}:{1}' ", kvp.Key, kvp.Value); return str; }, str=>str.ToString()));
			}

			Assert.IsNotNull(doc.Id);
			Assert.IsNotNull(doc.CurrentRevisionId);
			Assert.IsNotNull(doc.UserProperties);
			Assert.AreEqual(db.GetDocument(doc.Id), doc);

			return doc;
		}

		[Test]
		public void Fail ()
		{
			Assert.False (true);
		}

		[Test]
		[Ignore ("another time")]
		public void Ignore ()
		{
			Assert.True (false);
		}

		[Test]
		public void Inconclusive ()
		{
			Assert.Inconclusive ("Inconclusive");
		}
	}
}

