using MongoDB.Driver;
using Moq;

namespace ChatApp.Test.Helpers
{
    public static class MongoDbTestHelper
    {
        public static Mock<IAsyncCursor<T>> CreateAsyncCursor<T>(List<T>? items)
        {
            items ??= new List<T>();

            var cursor = new Mock<IAsyncCursor<T>>();

            if (items.Count > 0)
            {
                cursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true)
                      .ReturnsAsync(false);
                cursor.SetupGet(c => c.Current).Returns(items);
            }
            else
            {
                cursor.Setup(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false);
            }

            return cursor;
        }

        public static void SetupCollectionFindAsync<TDocument>(Mock<IMongoCollection<TDocument>> collectionMock, List<TDocument>? items) where TDocument : class
        {
            var cursor = CreateAsyncCursor(items);

            collectionMock
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<TDocument>>(),
                    It.IsAny<FindOptions<TDocument, TDocument>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursor.Object);
        }
    }
}
