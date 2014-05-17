//
// ManagerTest.cs
//
// Author:
//	Zachary Gramana  <zack@xamarin.com>
//
// Copyright (c) 2013, 2014 Xamarin Inc (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
/**
* Original iOS version by Jens Alfke
* Ported to Android by Marty Schoch, Traun Leyden
*
* Copyright (c) 2012, 2013, 2014 Couchbase, Inc. All rights reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
* except in compliance with the License. You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software distributed under the
* License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
* either express or implied. See the License for the specific language governing permissions
* and limitations under the License.
*/

using System.Collections.Generic;
using System.IO;
using Couchbase.Lite;
using Sharpen;
using NUnit.Framework;
using System.Linq;

namespace Couchbase.Lite
{
	[TestFixture]
	public class ManagerTest : LiteTestCase
	{
		[SetUp]
		protected override void SetUp ()
		{
			base.SetUp ();
		}

		[TearDown]
		protected override void TearDown ()
		{
			base.TearDown ();
		}

		[Test]
		public virtual void TestServer()
		{
			//to ensure this test is easily repeatable we will explicitly remove
			//any stale foo.cblite
			bool mustExist = true;
			Database old = manager.GetDatabaseWithoutOpening("foo", mustExist);
			if (old != null)
			{
				old.Delete();
			}
			mustExist = false;
			Database db = manager.GetDatabaseWithoutOpening("foo", mustExist);
			NUnit.Framework.Assert.IsNotNull(db);
			NUnit.Framework.Assert.AreEqual("foo", db.Name);
			NUnit.Framework.Assert.IsTrue(db.Path.StartsWith(GetServerPath()));
			NUnit.Framework.Assert.IsFalse(db.Exists());
			// because foo doesn't exist yet
			IList<string> databaseNames = manager.AllDatabaseNames.ToList();
			NUnit.Framework.Assert.IsTrue(!databaseNames.Contains("foo"));
			NUnit.Framework.Assert.IsTrue(db.Open());
			NUnit.Framework.Assert.IsTrue(db.Exists());
			databaseNames = manager.AllDatabaseNames.ToList();
			NUnit.Framework.Assert.IsTrue(databaseNames.Contains("foo"));
			db.Close();
			db.Delete();
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
		public virtual void TestUpgradeOldDatabaseFiles()
		{
			string directoryName = "test-directory-" + Runtime.CurrentTimeMillis();
			string normalFilesDir = GetRootDirectory().FullName;
			string fakeFilesDir = string.Format("{0}/{1}", normalFilesDir, directoryName);
			FilePath directory = new FilePath(fakeFilesDir);
			if (!directory.Exists())
			{
				bool result = directory.Mkdir();
				if (!result)
				{
					throw new IOException("Unable to create directory " + directory);
				}
			}
			FilePath oldTouchDbFile = new FilePath(directory, string.Format("old{0}", Manager.
				DatabaseSuffixOld));
			oldTouchDbFile.CreateNewFile();
			FilePath newCbLiteFile = new FilePath(directory, string.Format("new{0}", Manager.DatabaseSuffix
				));
			newCbLiteFile.CreateNewFile();
			FilePath migratedOldFile = new FilePath(directory, string.Format("old{0}", Manager
				.DatabaseSuffix));
			migratedOldFile.CreateNewFile();
			base.StopCBLite();
			manager = new Manager(new DirectoryInfo(new FilePath(GetRootDirectory().FullName, directoryName).GetAbsolutePath()), Manager.DefaultOptions
				);
			NUnit.Framework.Assert.IsTrue(migratedOldFile.Exists());
			//cannot rename old.touchdb in old.cblite, old.cblite already exists
			NUnit.Framework.Assert.IsTrue(oldTouchDbFile.Exists());
			NUnit.Framework.Assert.IsTrue(newCbLiteFile.Exists());
			FilePath dir = new FilePath(GetRootDirectory().FullName, directoryName);
			NUnit.Framework.Assert.AreEqual(3, dir.ListFiles().Length);
			base.StopCBLite();
			migratedOldFile.Delete();
			manager = new Manager(new DirectoryInfo(new FilePath(GetRootDirectory().FullName, directoryName).GetAbsolutePath()), Manager.DefaultOptions
				);
			//rename old.touchdb in old.cblite, previous old.cblite already doesn't exist
			NUnit.Framework.Assert.IsTrue(migratedOldFile.Exists());
			NUnit.Framework.Assert.IsTrue(oldTouchDbFile.Exists() == false);
			NUnit.Framework.Assert.IsTrue(newCbLiteFile.Exists());
			dir = new FilePath(GetRootDirectory().FullName, directoryName);
			NUnit.Framework.Assert.AreEqual(2, dir.ListFiles().Length);
		}
	}
}
