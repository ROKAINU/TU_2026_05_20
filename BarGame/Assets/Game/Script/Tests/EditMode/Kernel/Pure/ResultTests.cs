#nullable enable
using NUnit.Framework;
using System;

namespace Game.Kernel.Tests
{
    [TestFixture]
    public class ResultTests
    {
        // ========== Result<T> ==========
        
        [Test]
        public void Ok_CreatesSuccessResult()
        {
            var result = Result<int>.Ok(42);
            
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(42, result.Value);
        }
        
        [Test]
        public void Err_CreatesErrorResult()
        {
            var result = Result<int>.Err("Something went wrong");
            
            Assert.IsTrue(result.IsErr);
            Assert.AreEqual("Something went wrong", result.Error);
        }
        
        [Test]
        public void Value_ThrowsWhenErr()
        {
            var result = Result<int>.Err("Error");
            
            Assert.Throws<InvalidOperationException>(() =>
            {
                _ = result.Value;
            });
        }
        
        [Test]
        public void Error_ThrowsWhenOk()
        {
            var result = Result<int>.Ok(42);
            
            Assert.Throws<InvalidOperationException>(() =>
            {
                _ = result.Error;
            });
        }
        
        [Test]
        public void Match_CallsOkFunc()
        {
            var result = Result<int>.Ok(42);
            
            string output = result.Match(
                ok: value => $"Success: {value}",
                err: error => $"Error: {error}"
            );
            
            Assert.AreEqual("Success: 42", output);
        }
        
        [Test]
        public void Match_CallsErrFunc()
        {
            var result = Result<int>.Err("Failed");
            
            string output = result.Match(
                ok: value => $"Success: {value}",
                err: error => $"Error: {error}"
            );
            
            Assert.AreEqual("Error: Failed", output);
        }
        
        [Test]
        public void Map_TransformsValue()
        {
            var result = Result<int>.Ok(10);
            var mapped = result.Map(x => x * 2);
            
            Assert.IsTrue(mapped.IsOk);
            Assert.AreEqual(20, mapped.Value);
        }
        
        [Test]
        public void Map_PropagatesError()
        {
            var result = Result<int>.Err("Error");
            var mapped = result.Map(x => x * 2);
            
            Assert.IsTrue(mapped.IsErr);
            Assert.AreEqual("Error", mapped.Error);
        }
        
        [Test]
        public void Bind_ChainsResults()
        {
            var result = Result<int>.Ok(10);
            var chained = result.Bind(x => Result<int>.Ok(x * 2));
            
            Assert.IsTrue(chained.IsOk);
            Assert.AreEqual(20, chained.Value);
        }
        
        [Test]
        public void Bind_StopsOnError()
        {
            var result = Result<int>.Err("Initial error");
            var chained = result.Bind(x => Result<int>.Ok(x * 2));
            
            Assert.IsTrue(chained.IsErr);
        }
        
        [Test]
        public void TryGetValue_ReturnsTrue()
        {
            var result = Result<int>.Ok(42);
            
            bool success = result.TryGetValue(out int value);
            
            Assert.IsTrue(success);
            Assert.AreEqual(42, value);
        }
        
        [Test]
        public void TryGetValue_ReturnsFalse()
        {
            var result = Result<int>.Err("Error");
            
            bool success = result.TryGetValue(out int value);
            
            Assert.IsFalse(success);
            Assert.AreEqual(0, value);
        }
        
        [Test]
        public void OnOk_ExecutesOnSuccess()
        {
            var result = Result<int>.Ok(42);
            int capturedValue = 0;
            
            result.OnOk(value => capturedValue = value);
            
            Assert.AreEqual(42, capturedValue);
        }
        
        [Test]
        public void OnOk_SkipsOnError()
        {
            var result = Result<int>.Err("Error");
            int capturedValue = -1;
            
            result.OnOk(value => capturedValue = value);
            
            Assert.AreEqual(-1, capturedValue);
        }
        
        [Test]
        public void ToString_FormatsOkCorrectly()
        {
            var result = Result<int>.Ok(42);
            Assert.AreEqual("Ok(42)", result.ToString());
        }
        
        [Test]
        public void ToString_FormatsErrCorrectly()
        {
            var result = Result<int>.Err("Error message");
            Assert.AreEqual("Err(Error message)", result.ToString());
        }
        
        // ========== Result<T, E> ==========
        
        public enum CustomError
        {
            InvalidInput,
            NotFound,
            Unknown
        }
        
        [Test]
        public void TypedResult_Ok_CreatesSuccessResult()
        {
            var result = Result<int, CustomError>.Ok(42);
            
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(42, result.Value);
        }
        
        [Test]
        public void TypedResult_Err_CreatesErrorResult()
        {
            var result = Result<int, CustomError>.Err(CustomError.InvalidInput);
            
            Assert.IsTrue(result.IsErr);
            Assert.AreEqual(CustomError.InvalidInput, result.Error);
        }
        
        [Test]
        public void TypedResult_MapError_TransformsError()
        {
            var result = Result<int, CustomError>.Err(CustomError.NotFound);
            var mapped = result.MapError(e => e.ToString());
            
            Assert.IsTrue(mapped.IsErr);
            Assert.AreEqual("NotFound", mapped.Error);
        }
    }
}