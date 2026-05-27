using ChatApp.Domain.Entities.Command;

namespace ChatApp.Test.Domain
{
    public class ChatRoomEntityTest
    {
        [Fact]
        public void Constructor_WithValidName_SetsName()
        {
            var room = new ChatRoom("Test Room");

            Assert.Equal("Test Room", room.Name);
        }

        [Fact]
        public void Constructor_WithNullName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ChatRoom(null!));
        }

        [Fact]
        public void Constructor_InitializesEmptyMessages()
        {
            var room = new ChatRoom("Test Room");

            Assert.NotNull(room.Messages);
            Assert.Empty(room.Messages);
        }

        [Fact]
        public void AddMessage_AddsMessageToCollection()
        {
            var room = new ChatRoom("Test Room");

            room.AddMessage("Alice", "Hello");

            Assert.Single(room.Messages);
        }

        [Fact]
        public void AddMessage_SetsCorrectProperties()
        {
            var room = new ChatRoom("Test Room");

            room.AddMessage("Alice", "Hello");

            var message = room.Messages.First();
            Assert.Equal("Alice", message.UserName);
            Assert.Equal("Hello", message.Message);
        }

        [Fact]
        public void AddMessage_MultipleMessages_AllAdded()
        {
            var room = new ChatRoom("Test Room");

            room.AddMessage("Alice", "First");
            room.AddMessage("Bob", "Second");
            room.AddMessage("Charlie", "Third");

            Assert.Equal(3, room.Messages.Count);
        }
    }
}
