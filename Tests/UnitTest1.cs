using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Forces;

namespace Tests
{
    [TestClass]
    public class Matrices
    {
        [TestMethod]
        public void OneByOneIsOne()
        {
            var m1 = Matrix3D.One;
            var m2 = Matrix3D.One;
            var mm = m1 * m2;
            Assert.AreEqual(Matrix3D.One,mm);
        }

        [TestMethod]
        public void TwoByThreeIsSix()
        {
            var m1 = new Matrix3D(2,2,2);
            var m2 = new Matrix3D(3,3,3);
            var mm = m1 * m2;
            Assert.AreEqual(new Matrix3D(6,6,6), mm);

        }
    }
}
