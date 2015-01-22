using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Org.Xepher.Kazuma.Models;
using Org.Xepher.Kazuma.Utils;

namespace Org.Xepher.Kazuma.WindowsPhone.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            // Arrange
            string requestUrl = Constants.TEMPLATE_ALL_LINES;
            // Act
            List<Route> result = await SignatureUtil.WebRequestAsync<List<Route>>(requestUrl);
            // Assert
            Assert.AreEqual(277, result.Count);
        }

        [TestMethod]
        public void TestGenerateId()
        {
            // Arrange

            // Act
            string seqId = SignatureUtil.GenerateSeqId();
            // Assert
            Assert.IsNotNull(seqId);
        }

        [TestMethod]
        public void TestSHA1()
        {
            // Arrange
            string testData = "test";
            // Act
            string sha1 = SignatureUtil.GetHashedString(HashAlgorithmNames.Sha1, testData);
            // Assert
            Assert.AreEqual("a94a8fe5ccb19ba61c4c0873d391e987982fbbd3", sha1);
        }
    }
}
