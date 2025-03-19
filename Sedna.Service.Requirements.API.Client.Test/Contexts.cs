using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sedna.Service.Requirements.API.Client.Test
{
    [TestClass]
    public class Contexts
    {
        [TestMethod]
        public void TestMethod1()
        {
            var contexts = API.Client.Contexts.GetAll();

            Assert.IsTrue(contexts != null);
            Assert.IsTrue(contexts.Count > 0);
        }
    }
}
