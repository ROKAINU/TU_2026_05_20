#nullable enable
using NUnit.Framework;
using UnityEngine;
using Game.Kernel;

namespace Game.Kernel.Utils.Unity.Tests
{
    [TestFixture]
    public class CoordinateConverterTests
    {
        [Test]
        public void ToUnity_Int2_ConvertsCorrectly()
        {
            var coord = new Int2(10, 20);
            var result = CoordinateConverter.ToUnity(coord);
            
            Assert.AreEqual(10, result.x);
            Assert.AreEqual(20, result.y);
        }
        
        [Test]
        public void FromUnity_Vector2Int_ConvertsCorrectly()
        {
            var v = new Vector2Int(30, 40);
            var result = CoordinateConverter.FromUnity(v);
            
            Assert.AreEqual(30, result.x);
            Assert.AreEqual(40, result.y);
        }
        
        [Test]
        public void RoundTrip_Int2_PreservesValue()
        {
            var original = new Int2(15, 25);
            var converted = CoordinateConverter.ToUnity(original);
            var back = CoordinateConverter.FromUnity(converted);
            
            Assert.AreEqual(original.x, back.x);
            Assert.AreEqual(original.y, back.y);
        }
        
        [Test]
        public void ToUnity_Coord2D_ConvertsCorrectly()
        {
            var coord = new Coord2D(1.5f, 2.5f);
            var result = CoordinateConverter.ToUnity(coord);
            
            Assert.AreEqual(1.5f, result.x);
            Assert.AreEqual(2.5f, result.y);
        }
        
        [Test]
        public void FromUnity_Vector2_ConvertsCorrectly()
        {
            var v = new Vector2(3.5f, 4.5f);
            var result = CoordinateConverter.FromUnity(v);
            
            Assert.AreEqual(3.5f, result.x);
            Assert.AreEqual(4.5f, result.y);
        }
    }
}