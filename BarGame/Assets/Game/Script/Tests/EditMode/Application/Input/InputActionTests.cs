#nullable enable
using System;
using NUnit.Framework;
using Game.Application;

namespace Game.Application.Tests
{
    [TestFixture]
    public class InputActionTests
    {
        // ========== Constructor Tests ==========

        [Test]
        public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
        {
            var action = new InputAction(GameCommandType.AddScore, 0.5f, 3);
            
            Assert.That(action.CommandType, Is.EqualTo(GameCommandType.AddScore));
            Assert.That(action.DelaySeconds, Is.EqualTo(0.5f));
            Assert.That(action.RepeatCount, Is.EqualTo(3));
        }

        [Test]
        public void Constructor_WithDefaultParameters_UsesDefaults()
        {
            var action = new InputAction(GameCommandType.Pause);
            
            Assert.That(action.CommandType, Is.EqualTo(GameCommandType.Pause));
            Assert.That(action.DelaySeconds, Is.EqualTo(0.1f));
            Assert.That(action.RepeatCount, Is.EqualTo(1));
        }

        [Test]
        public void Constructor_WithZeroDelaySeconds_AcceptsZero()
        {
            var action = new InputAction(GameCommandType.Resume, 0f, 1);
            
            Assert.That(action.DelaySeconds, Is.EqualTo(0f));
        }

        [Test]
        public void Constructor_WithNegativeDelaySeconds_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => 
                new InputAction(GameCommandType.AddScore, -0.1f, 1));
        }

        [Test]
        public void Constructor_WithZeroRepeatCount_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => 
                new InputAction(GameCommandType.AddScore, 0.1f, 0));
        }

        [Test]
        public void Constructor_WithNegativeRepeatCount_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => 
                new InputAction(GameCommandType.AddScore, 0.1f, -1));
        }

        // ========== GameCommandType Tests ==========

        [Test]
        public void Constructor_WithResumeCommandType_SetsCorrectly()
        {
            var action = new InputAction(GameCommandType.Resume);
            Assert.That(action.CommandType, Is.EqualTo(GameCommandType.Resume));
        }

        [Test]
        public void Constructor_WithPauseCommandType_SetsCorrectly()
        {
            var action = new InputAction(GameCommandType.Pause);
            Assert.That(action.CommandType, Is.EqualTo(GameCommandType.Pause));
        }

        [Test]
        public void Constructor_WithAddScoreCommandType_SetsCorrectly()
        {
            var action = new InputAction(GameCommandType.AddScore);
            Assert.That(action.CommandType, Is.EqualTo(GameCommandType.AddScore));
        }

        // ========== Property Tests ==========

        [Test]
        public void CommandType_IsReadOnly()
        {
            var action = new InputAction(GameCommandType.AddScore);
            // Struct is readonly, so properties are immutable
            Assert.That(action.CommandType, Is.EqualTo(GameCommandType.AddScore));
        }

        [Test]
        public void DelaySeconds_IsReadOnly()
        {
            var action = new InputAction(GameCommandType.AddScore, 1.5f);
            Assert.That(action.DelaySeconds, Is.EqualTo(1.5f));
        }

        [Test]
        public void RepeatCount_IsReadOnly()
        {
            var action = new InputAction(GameCommandType.AddScore, 0.1f, 5);
            Assert.That(action.RepeatCount, Is.EqualTo(5));
        }

        // ========== Edge Cases ==========

        [Test]
        public void Constructor_WithLargeDelaySeconds_AcceptsValue()
        {
            var action = new InputAction(GameCommandType.AddScore, 9999f, 1);
            Assert.That(action.DelaySeconds, Is.EqualTo(9999f));
        }

        [Test]
        public void Constructor_WithLargeRepeatCount_AcceptsValue()
        {
            var action = new InputAction(GameCommandType.AddScore, 0.1f, 10000);
            Assert.That(action.RepeatCount, Is.EqualTo(10000));
        }

        [Test]
        public void Constructor_WithVerySmallDelaySeconds_AcceptsValue()
        {
            var action = new InputAction(GameCommandType.AddScore, 0.001f, 1);
            Assert.That(action.DelaySeconds, Is.EqualTo(0.001f));
        }

        // ========== Struct Behavior Tests ==========

        [Test]
        public void Struct_IsValueType()
        {
            var action1 = new InputAction(GameCommandType.AddScore, 0.5f, 2);
            var action2 = action1; // Copy by value
            
            Assert.That(action2.CommandType, Is.EqualTo(action1.CommandType));
            Assert.That(action2.DelaySeconds, Is.EqualTo(action1.DelaySeconds));
            Assert.That(action2.RepeatCount, Is.EqualTo(action1.RepeatCount));
        }

        [Test]
        public void Equality_SameValues_AreEqual()
        {
            var action1 = new InputAction(GameCommandType.AddScore, 0.5f, 2);
            var action2 = new InputAction(GameCommandType.AddScore, 0.5f, 2);
            
            Assert.That(action1, Is.EqualTo(action2));
        }

        [Test]
        public void Equality_DifferentCommandType_AreNotEqual()
        {
            var action1 = new InputAction(GameCommandType.AddScore, 0.5f, 2);
            var action2 = new InputAction(GameCommandType.Pause, 0.5f, 2);
            
            Assert.That(action1, Is.Not.EqualTo(action2));
        }

        [Test]
        public void Equality_DifferentDelaySeconds_AreNotEqual()
        {
            var action1 = new InputAction(GameCommandType.AddScore, 0.5f, 2);
            var action2 = new InputAction(GameCommandType.AddScore, 0.6f, 2);
            
            Assert.That(action1, Is.Not.EqualTo(action2));
        }

        [Test]
        public void Equality_DifferentRepeatCount_AreNotEqual()
        {
            var action1 = new InputAction(GameCommandType.AddScore, 0.5f, 2);
            var action2 = new InputAction(GameCommandType.AddScore, 0.5f, 3);
            
            Assert.That(action1, Is.Not.EqualTo(action2));
        }
    }
}